# Main Control Loop - Hızlı Başlangıç

## 📋 Ne Yaptık?

Sürekli çalışan, 8 adımlı bir ana kontrol döngüsü oluşturduk:

```
┌─────────────────────────────────────────┐
│         MAIN CONTROL LOOP               │
│  (Background Thread - Sürekli Döner)    │
├─────────────────────────────────────────┤
│ 1. ReadGuiItems()      ← GUI            │
│ 2. ReadNetwork()       ← UDP/TCP        │
│ 3. ReadConfig()        ← Config File    │
│ 4. ExecuteBusinessLogic() (Strategy)    │
│ 5. UpdateConfig()      → Config File    │
│ 6. SendNetwork()       → UDP/TCP        │
│ 7. WriteDataToFiles()  → Log Files      │
│ 8. UpdateGui()         → GUI            │
└─────────────────────────────────────────┘
```

## 🗂️ Dosyalar

```
src/
├── MainControlLoop.cs        → Ana döngü implementasyonu
├── LoopDataStructures.cs     → Veri yapıları (MarketData, Order, vb.)
├── DataReader.cs             → Mevcut (değişmedi)
└── AlgoTrader.cs             → Mevcut (değişmedi)

Form1.cs                      → Main loop entegrasyonu
MAINLOOP_USAGE.md            → Detaylı kullanım kılavuzu
```

## ⚡ Hızlı Test

### 1. Uygulamayı Çalıştır

```bash
dotnet run
```

### 2. Main Loop'u Başlat

- View → Toolbars → "Main ToolStrip 2" ✓ (göster)
- Toolbar'da "▶️ Run" butonuna tıkla
- StatusBar'da "Main loop started" yazısını gör

### 3. Console'u İzle

```
[12:34:56.789] [Info] [MainLoop] Starting main control loop...
[12:34:56.890] [Debug] [Metrics] Iterations: 100, Success: 100, Failed: 0, Avg: 10.50ms, Rate: 95.2/sec
[12:34:57.990] [Debug] [Metrics] Iterations: 200, Success: 200, Failed: 0, Avg: 10.48ms, Rate: 95.4/sec
```

### 4. Durdur

- "⏹️ Stop" butonuna tıkla
- StatusBar'da "Main loop stopped" yazısını gör

## 📊 Metrikler

```csharp
var metrics = _mainLoop.GetMetrics();
// TotalIterations: Toplam döngü sayısı
// SuccessfulIterations: Başarılı döngüler
// FailedIterations: Hatalı döngüler
// AverageIterationTime: Ortalama süre
// IterationsPerSecond: Loop hızı (loops/saniye)
```

## 🔧 Konfigürasyon

```csharp
// Loop hızını değiştir (1-1000ms)
_mainLoop.UpdateLoopDelay(50);  // 50ms = ~20 loop/saniye

// Networking aç/kapat
_mainLoop.SetNetworkingEnabled(true);

// File logging aç/kapat
_mainLoop.SetFileLoggingEnabled(true);

// Stratejiyi başlat/durdur
_mainLoop.SetStrategyRunning(true);
```

## 🧪 Test Senaryosu

```csharp
// Form1.cs içinde bir test butonu ekle:

private void TestMainLoop()
{
    // 1. Loop başlat
    _mainLoop.Start();
    _mainLoop.SetFileLoggingEnabled(true);

    // 2. Fake market data ekle
    Task.Run(() =>
    {
        for (int i = 0; i < 100; i++)
        {
            var data = new MarketData
            {
                Symbol = "EURUSD",
                Timestamp = DateTime.Now,
                Price = 1.0850m + (decimal)(i * 0.0001),
                Volume = 10000
            };

            _mainLoop.EnqueueMarketData(data);
            Thread.Sleep(100);  // Her 100ms'de bir
        }
    });

    // 3. 10 saniye sonra durdur
    Task.Delay(10000).ContinueWith(_ =>
    {
        _mainLoop.Stop();

        // Metrikleri göster
        var m = _mainLoop.GetMetrics();
        MessageBox.Show(
            $"Total: {m.TotalIterations}\n" +
            $"Success: {m.SuccessfulIterations}\n" +
            $"Rate: {m.IterationsPerSecond:F1}/sec\n" +
            $"Avg: {m.AverageIterationTime.TotalMilliseconds:F2}ms",
            "Loop Metrics"
        );
    });
}
```

## 🎯 Sonraki Adımlar

1. **Trading Strategy Ekle**
   - `ExecuteBusinessLogic()` içine gerçek strateji kodu
   - MA, RSI, MACD hesaplamaları

2. **UDP Network Listener**
   - Market data alan UDP listener thread
   - `EnqueueMarketData()` ile loop'a besle

3. **GUI Güncellemeleri**
   - Chart çizimi
   - Position grid
   - Real-time metrikler

4. **Database Entegrasyonu**
   - Historical data okuma
   - Trade log'larını kaydetme

## ❓ SSS

**S: Loop ne kadar hızlı çalışmalı?**
A: Trading için 10-100ms uygun. Default 10ms (~100 loop/saniye).

**S: Queue'lar dolup taşar mı?**
A: Hayır, backpressure var. Her iterasyonda max 50-100 item işlenir.

**S: GUI donma riski var mı?**
A: Hayır, `BeginInvoke()` kullanılıyor (async, non-blocking).

**S: Exception olursa ne olur?**
A: Loop durmuyor, log'lanıp devam ediyor.

**S: Thread-safe mi?**
A: Evet, tüm public metodlar thread-safe.

## 📚 Daha Fazla Bilgi

- **Detaylı Kullanım**: `MAINLOOP_USAGE.md`
- **Kod**: `src/MainControlLoop.cs`
- **Veri Yapıları**: `src/LoopDataStructures.cs`

## ✅ Durum

- [x] Ana loop yapısı
- [x] 8 adımlı döngü
- [x] Thread-safe queue'lar
- [x] Performans metrikleri
- [x] File logging
- [x] GUI entegrasyonu
- [x] Run/Stop butonları
- [ ] Gerçek trading strategy
- [ ] UDP network listener
- [ ] Chart güncelleme
- [ ] Database entegrasyonu

---

**Created**: 2025-12-16
**Version**: 1.0
**Status**: Production Ready (Temel Yapı)
