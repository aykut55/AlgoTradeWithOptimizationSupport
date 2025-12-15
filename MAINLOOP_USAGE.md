# Main Control Loop Kullanım Kılavuzu

## Genel Bakış

Main Control Loop, uygulamanın kalbi olan sürekli çalışan döngüdür. Her iterasyonda 8 temel adımı sırayla çalıştırır:

```
1. ReadGuiItems()      → GUI'den veri oku
2. ReadNetwork()       → Network'ten market data oku
3. ReadConfig()        → Config ayarlarını oku
4. ExecuteBusinessLogic() → Trading stratejisini çalıştır
5. UpdateConfig()      → Config'i güncelle
6. SendNetwork()       → Emirleri network'e gönder
7. WriteDataToFiles()  → Log dosyalarına yaz
8. UpdateGui()         → GUI'yi güncelle
```

## Mimari

### Thread Modeli

- **Main UI Thread**: GUI okuma/yazma
- **Background Loop Thread**: 8 adımlı ana döngü
- **External Threads**: UDP listener, file watcher, vb.

### Veri Akışı (Queue-Based)

```
External Thread          Main Loop              Main UI Thread
----------------         ---------              --------------
UDP Listener ──→ MarketDataQueue ──→ ReadNetwork()
Strategy     ──→ OrderQueue      ──→ SendNetwork()
Loop         ──→ LogQueue        ──→ WriteDataToFiles()
Loop         ──→ GuiUpdateQueue  ──→ UpdateGui() ──→ Form1
```

## Kullanım

### 1. Başlatma

```csharp
// Form1.cs constructor'da
_mainLoop = new MainControlLoop(this);

// Run butonuna basınca
_mainLoop.Start();
_mainLoop.SetStrategyRunning(true);
```

### 2. Market Data Ekleme

```csharp
// UDP listener thread'den
var marketData = new MarketData
{
    Symbol = "EURUSD",
    Timestamp = DateTime.Now,
    Price = 1.0850m,
    Volume = 10000,
    Bid = 1.0849m,
    Ask = 1.0851m
};

_mainLoop.EnqueueMarketData(marketData);
```

### 3. Emir Gönderme

```csharp
// Strategy'den veya GUI'den
var order = new Order
{
    Symbol = "EURUSD",
    Side = OrderSide.Buy,
    Price = 1.0850m,
    Quantity = 10000
};

_mainLoop.EnqueueOrder(order);
```

### 4. GUI Güncelleme

```csharp
// Background thread'den
var update = new GuiUpdate
{
    TargetControl = "ChartPanel",
    Data = new Dictionary<string, object>
    {
        { "Symbol", "EURUSD" },
        { "Price", 1.0850m }
    }
};

_mainLoop.EnqueueGuiUpdate(update);
```

### 5. Durdurma

```csharp
// Stop butonuna basınca
_mainLoop.SetStrategyRunning(false);
_mainLoop.Stop();
```

## Performans

### Loop Hızı

```csharp
// Default: 10ms = ~100 iterasyon/saniye
_mainLoop.UpdateLoopDelay(10);

// Hızlı: 1ms = ~1000 iterasyon/saniye
_mainLoop.UpdateLoopDelay(1);

// Yavaş: 100ms = ~10 iterasyon/saniye
_mainLoop.UpdateLoopDelay(100);
```

### Metrikler

```csharp
var metrics = _mainLoop.GetMetrics();

Console.WriteLine($"Total Iterations: {metrics.TotalIterations}");
Console.WriteLine($"Success Rate: {metrics.SuccessfulIterations}/{metrics.TotalIterations}");
Console.WriteLine($"Average Time: {metrics.AverageIterationTime.TotalMilliseconds:F2}ms");
Console.WriteLine($"Rate: {metrics.IterationsPerSecond:F1} loops/sec");
```

## Backpressure (Aşırı Yük Koruması)

Her iterasyonda işlenen maximum veri sayısı:

- **ReadNetwork**: Max 100 market data/iteration
- **SendNetwork**: Max 50 order/iteration
- **WriteDataToFiles**: Max 100 log entry/iteration
- **UpdateGui**: Max 20 güncelleme/iteration

Bu sayede loop bloke olmaz, queue'lar kontrollü şekilde tüketilir.

## Log Seviyeleri

```csharp
LogLevel.Debug   → Detaylı debug bilgisi
LogLevel.Info    → Genel bilgi
LogLevel.Warning → Uyarılar
LogLevel.Error   → Hatalar
LogLevel.Fatal   → Kritik hatalar
```

## Örnek Senaryo

```csharp
// 1. Loop'u başlat
_mainLoop.Start();

// 2. File logging'i aç
_mainLoop.SetFileLoggingEnabled(true);

// 3. Stratejiyi çalıştır
_mainLoop.SetStrategyRunning(true);

// 4. Market data gelmeye başlasın (UDP thread)
udpListener.OnDataReceived += (data) =>
{
    _mainLoop.EnqueueMarketData(ParseMarketData(data));
};

// 5. Loop otomatik olarak:
//    - Market data'yı işler (ReadNetwork)
//    - Strateji sinyalleri üretir (ExecuteBusinessLogic)
//    - Emirleri gönderir (SendNetwork)
//    - Log'ları dosyaya yazar (WriteDataToFiles)
//    - GUI'yi günceller (UpdateGui)

// 6. Durdur
_mainLoop.SetStrategyRunning(false);
_mainLoop.Stop();
```

## Thread Safety

Tüm public metodlar thread-safe'tir:

- `ConcurrentQueue<T>` → Lock-free queue'lar
- `lock` statements → Config ve metrik güncellemeleri
- `Invoke/BeginInvoke` → GUI marshaling

## Best Practices

1. **GUI işlemleri sadece main thread'de**
   - ReadGuiItems() → `Invoke()` kullanır
   - UpdateGui() → `BeginInvoke()` kullanır

2. **Blocking işlem yapma**
   - Network I/O → Async veya queue-based
   - Disk I/O → Batch write (toplu yazma)

3. **Exception handling**
   - Loop exception'da durmaz, devam eder
   - Hatalar log'lanır

4. **Performans**
   - Loop delay'i ihtiyaca göre ayarla
   - Queue boyutlarını monitör et
   - Metrikleri kontrol et

## Gelecek Geliştirmeler

- [ ] Config dosyasından okuma/yazma
- [ ] UDP network listener entegrasyonu
- [ ] Gerçek trading strategy implementasyonu
- [ ] GUI'de canlı metrik gösterimi
- [ ] Chart/grafik güncelleme
- [ ] Position management UI
