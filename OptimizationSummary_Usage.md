# OptimizationSummary - Kullanım Kılavuzu

## 📋 Genel Bakış

`OptimizationSummary`, strateji optimizasyonu sırasında her bir backtest sonucunu hızlıca CSV ve TXT dosyalarına yazmak için tasarlanmış bir struct'tır. Statistics sınıfına entegre edilmiştir ve optimizasyon döngülerinde yüksek performans sağlar.

---

## 🎯 Özellikler

- ✅ **Hafif ve Hızlı**: Struct olduğu için stack'te tutulur, heap allocation olmaz
- ✅ **CSV Desteği**: Excel/Google Sheets'te kolayca analiz edilebilir
- ✅ **TXT Desteği**: Tabular format, terminal/log viewer'da okunabilir
- ✅ **Kapsamlı Metrikler**: 40+ istatistiksel veri (getiri, kar/zarar, drawdown, etc.)
- ✅ **Type-Safe**: Compile-time kontrol, IntelliSense desteği

---

## 📊 İçerik

OptimizationSummary şu verileri içerir:

### **1. Kimlik Bilgileri**
- `TraderId`, `TraderName`
- `Symbol`, `Period`
- `StrategyId`, `StrategyName`

### **2. Çalıştırma Bilgileri**
- `ExecutionId`, `ExecutionTime`
- `ExecutionTimeMs` (performans ölçümü)

### **3. Bar/Tarih Bilgileri**
- `ToplamBarSayisi`
- `IlkBarTarihi`, `SonBarTarihi`

### **4. İşlem Sayıları**
- `IslemSayisi`, `AlisSayisi`, `SatisSayisi`
- `KazandiranIslemSayisi`, `KaybettirenIslemSayisi`
- `FlatSayisi`, `PassSayisi`

### **5. Bakiye ve Getiri (Brüt)**
- `IlkBakiyeFiyat`, `BakiyeFiyat`
- `GetiriFiyat`, `GetiriFiyatYuzde`

### **6. Komisyon**
- `KomisyonFiyat`, `KomisyonFiyatYuzde`

### **7. Bakiye ve Getiri (Net)**
- `BakiyeFiyatNet`, `GetiriFiyatNet`
- `GetiriFiyatYuzdeNet`

### **8. Min/Max Değerler**
- `MinBakiyeFiyat`, `MaxBakiyeFiyat`
- `MinBakiyeFiyatNet`, `MaxBakiyeFiyatNet`
- Yüzde değerleri

### **9. Performans Metrikleri**
- `ProfitFactor`
- `KarliIslemOrani` (%)
- `GetiriMaxDD` (Maximum Drawdown %)
- `GetiriMaxKayip` (Max Drawdown TL)
- `GetiriMaxDDTarih`

### **10. Pozisyon Bilgileri**
- `VarlikAdedSayisi`, `VarlikAdedSayisiMicro`
- `KomisyonCarpan`
- `MicroLotSizeEnabled`, `PyramidingEnabled`

---

## 🚀 Kullanım Örnekleri

### **YÖNTEM 1: Önerilen - Optimization Manager ile** ⭐⭐⭐

Bu yöntem SOLID prensiplerine uygun, test edilebilir ve esnek bir yaklaşımdır.

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics;

public class OptimizationManager
{
    private string csvFilePath;
    private string txtFilePath;
    private SingleTrader trader;

    public OptimizationManager(SingleTrader trader, string outputFolder = "logs")
    {
        this.trader = trader;

        // Çıktı klasörünü oluştur
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        // Dosya yollarını belirle
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        csvFilePath = Path.Combine(outputFolder, $"optimization_results_{timestamp}.csv");
        txtFilePath = Path.Combine(outputFolder, $"optimization_results_{timestamp}.txt");
    }

    /// <summary>
    /// Optimizasyon döngüsünü çalıştır
    /// </summary>
    public void RunOptimization(List<IStrategy> strategies)
    {
        Console.WriteLine($"Starting optimization with {strategies.Count} strategies...");
        Console.WriteLine($"Output files:");
        Console.WriteLine($"  CSV: {csvFilePath}");
        Console.WriteLine($"  TXT: {txtFilePath}");
        Console.WriteLine();

        // Header yaz
        InitializeOutputFiles();

        int strategyCounter = 1;
        var totalStopwatch = Stopwatch.StartNew();

        // Her strateji için backtest çalıştır
        foreach (var strategy in strategies)
        {
            Console.Write($"[{strategyCounter}/{strategies.Count}] Testing: {strategy.Name,-40}");

            // Trader'ı sıfırla ve hazırla
            trader.Reset();
            trader.Init();
            trader.SetStrategy(strategy);

            // Strategy ID'sini ayarla (optimizasyon parametreleri için)
            trader.StrategyId = $"OPT{strategyCounter:D4}";
            trader.StrategyName = strategy.Name;

            // Backtest çalıştır
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < trader.Data.Count; i++)
            {
                trader.Run(i);
            }

            stopwatch.Stop();

            // Execution bilgilerini kaydet
            trader.LastExecutionId = $"RUN{strategyCounter:D4}";
            trader.LastExecutionTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            trader.LastExecutionTimeInMSec = stopwatch.ElapsedMilliseconds.ToString();

            // İstatistikleri hesapla
            trader.Finalize();

            // Summary al ve dosyaya yaz
            var summary = trader.statistics.GetOptimizationSummary();
            AppendResults(summary);

            Console.WriteLine($" -> {stopwatch.ElapsedMilliseconds,6} ms | Return: {summary.GetiriFiyatYuzdeNet,6:F2}% | Trades: {summary.IslemSayisi,4}");

            strategyCounter++;
        }

        totalStopwatch.Stop();

        // Özet bilgi
        Console.WriteLine();
        Console.WriteLine($"Optimization complete!");
        Console.WriteLine($"Total time: {totalStopwatch.Elapsed.TotalSeconds:F2} seconds");
        Console.WriteLine($"Average time per strategy: {totalStopwatch.ElapsedMilliseconds / strategies.Count} ms");
        Console.WriteLine($"Results saved to:");
        Console.WriteLine($"  CSV: {csvFilePath}");
        Console.WriteLine($"  TXT: {txtFilePath}");
    }

    /// <summary>
    /// Dosya headerlarını yaz
    /// </summary>
    private void InitializeOutputFiles()
    {
        // CSV header
        File.WriteAllText(csvFilePath,
            Statistics.OptimizationSummary.GetCsvHeader() + Environment.NewLine,
            Encoding.UTF8);

        // TXT header
        var sb = new StringBuilder();
        sb.AppendLine($"OPTIMIZATION RESULTS - {DateTime.Now:yyyy.MM.dd HH:mm:ss}");
        sb.AppendLine($"Trader: {trader.Name}");
        sb.AppendLine($"Symbol: {trader.SymbolName} | Period: {trader.SymbolPeriod}");
        sb.AppendLine($"Initial Balance: {trader.status.IlkBakiyeFiyat:F2} TL");
        sb.AppendLine();
        sb.AppendLine(Statistics.OptimizationSummary.GetTxtSeparator());
        sb.AppendLine(Statistics.OptimizationSummary.GetTxtHeader());
        sb.AppendLine(Statistics.OptimizationSummary.GetTxtSeparator());
        File.WriteAllText(txtFilePath, sb.ToString(), Encoding.UTF8);
    }

    /// <summary>
    /// Sonuçları dosyalara ekle
    /// </summary>
    private void AppendResults(Statistics.OptimizationSummary summary)
    {
        // CSV'ye ekle
        File.AppendAllText(csvFilePath,
            summary.ToCsvRow() + Environment.NewLine,
            Encoding.UTF8);

        // TXT'ye ekle
        File.AppendAllText(txtFilePath,
            summary.ToTxtRow() + Environment.NewLine,
            Encoding.UTF8);
    }
}
```

#### **Kullanım:**

```csharp
// Test/Form kodunda:

// 1. Trader oluştur
var trader = new SingleTrader(
    id: 1,
    name: "OptTrader",
    data: historicalData,
    indicators: indicators,
    logger: logger);

trader.CreateModules();
trader.ConfigureUserFlagsOnce();

// Pozisyon büyüklüğü ayarları
trader.pozisyonBuyuklugu.VarlikAdedSayisi = 1.0;
trader.pozisyonBuyuklugu.IlkBakiyeFiyat = 100000.0;
trader.pozisyonBuyuklugu.KomisyonCarpan = 0.0001;

// 2. Optimize edilecek stratejileri oluştur
var strategies = new List<IStrategy>();

// Örnek: EMA Cross stratejisi farklı parametrelerle
for (int fastPeriod = 5; fastPeriod <= 20; fastPeriod += 5)
{
    for (int slowPeriod = 20; slowPeriod <= 50; slowPeriod += 10)
    {
        if (fastPeriod >= slowPeriod) continue;

        var strategy = new EMACrossStrategy(fastPeriod, slowPeriod);
        strategy.Name = $"EMA_{fastPeriod}_{slowPeriod}";
        strategies.Add(strategy);
    }
}

Console.WriteLine($"Total strategies to test: {strategies.Count}");

// 3. Optimizasyonu çalıştır
var optManager = new OptimizationManager(trader, outputFolder: "optimization_results");
optManager.RunOptimization(strategies);
```

---

### **YÖNTEM 2: Basit - Helper Methodları ile** ⭐⭐

Hızlı prototipleme için ideal.

```csharp
public void QuickOptimization()
{
    string csvPath = "logs\\quick_opt_results.csv";
    string txtPath = "logs\\quick_opt_results.txt";

    var strategies = GetStrategiesToTest();

    for (int i = 0; i < strategies.Count; i++)
    {
        // Backtest çalıştır
        trader.Reset();
        trader.Init();
        trader.SetStrategy(strategies[i]);

        var sw = Stopwatch.StartNew();
        for (int j = 0; j < trader.Data.Count; j++)
            trader.Run(j);
        sw.Stop();

        trader.LastExecutionTimeInMSec = sw.ElapsedMilliseconds.ToString();
        trader.Finalize();

        // Helper method kullan (ilk stratejide header yaz)
        bool writeHeader = (i == 0);
        trader.statistics.AppendToOptimizationCsv(csvPath, writeHeader);
        trader.statistics.AppendToOptimizationTxt(txtPath, writeHeader);
    }
}
```

---

### **YÖNTEM 3: Manuel - Tam Kontrol** ⭐

Özel gereksinimlerin olduğu durumlar için.

```csharp
public void ManualOptimization()
{
    var summaries = new List<Statistics.OptimizationSummary>();

    // Tüm stratejileri test et ve sonuçları topla
    foreach (var strategy in strategies)
    {
        trader.Reset();
        trader.Init();
        trader.SetStrategy(strategy);

        // Backtest...
        for (int i = 0; i < trader.Data.Count; i++)
            trader.Run(i);

        trader.Finalize();

        var summary = trader.statistics.GetOptimizationSummary();
        summaries.Add(summary);
    }

    // En iyi stratejileri filtrele
    var bestStrategies = summaries
        .Where(s => s.IslemSayisi >= 30)                    // En az 30 işlem
        .Where(s => s.GetiriFiyatYuzdeNet > 0)              // Pozitif getiri
        .Where(s => s.GetiriMaxDD < 20)                     // MaxDD < %20
        .Where(s => s.ProfitFactor > 1.5)                   // Profit Factor > 1.5
        .OrderByDescending(s => s.GetiriFiyatYuzdeNet)      // En yüksek getiri
        .Take(10)                                           // İlk 10
        .ToList();

    // Sadece en iyileri dosyaya yaz
    using (var writer = new StreamWriter("logs\\best_strategies.csv", false, Encoding.UTF8))
    {
        writer.WriteLine(Statistics.OptimizationSummary.GetCsvHeader());

        foreach (var summary in bestStrategies)
        {
            writer.WriteLine(summary.ToCsvRow());
        }
    }

    // Konsola yazdır
    Console.WriteLine("Top 10 Strategies:");
    Console.WriteLine(Statistics.OptimizationSummary.GetTxtHeader());
    Console.WriteLine(Statistics.OptimizationSummary.GetTxtSeparator());

    foreach (var summary in bestStrategies)
    {
        Console.WriteLine(summary.ToTxtRow());
    }
}
```

---

## 📂 Çıktı Dosyaları

### **CSV Dosyası (optimization_results.csv)**

Excel/Google Sheets'te açılabilir, pivot table, filtering, sorting yapılabilir.

```csv
TraderId;TraderName;Symbol;Period;StrategyId;StrategyName;ExecutionId;ExecutionTime;ExecutionTimeMs;ToplamBarSayisi;IlkBarTarihi;SonBarTarihi;IslemSayisi;AlisSayisi;SatisSayisi;FlatSayisi;PassSayisi;KazandiranIslemSayisi;KaybettirenIslemSayisi;NotrIslemSayisi;IlkBakiyeFiyat;BakiyeFiyat;GetiriFiyat;GetiriFiyatYuzde;KomisyonFiyat;KomisyonFiyatYuzde;BakiyeFiyatNet;GetiriFiyatNet;GetiriFiyatYuzdeNet;MinBakiyeFiyat;MaxBakiyeFiyat;MinBakiyeFiyatYuzde;MaxBakiyeFiyatYuzde;MinBakiyeFiyatNet;MaxBakiyeFiyatNet;MinBakiyeFiyatNetYuzde;MaxBakiyeFiyatNetYuzde;ProfitFactor;KarliIslemOrani;GetiriMaxDD;GetiriMaxKayip;GetiriMaxDDTarih;VarlikAdedSayisi;VarlikAdedSayisiMicro;KomisyonCarpan;MicroLotSizeEnabled;PyramidingEnabled;MaxPositionSizeEnabled
1;OptTrader;EURUSD;H1;OPT0001;EMA_5_20;RUN0001;2025.12.29 16:30:45;1234;5000;2024.01.01;2025.12.29;150;75;75;0;4850;85;65;0;100000.00;105234.50;5234.50;5.23;234.50;0.2345;105000.00;5000.00;5.00;98765.00;107890.00;-1.24;7.89;98500.00;107650.00;-1.50;7.65;1.85;56.67;12.34;12340.00;2025.11.15 10:30:00;1.00;0.0100;0.0001;False;False;False
1;OptTrader;EURUSD;H1;OPT0002;EMA_5_30;RUN0002;2025.12.29 16:30:47;987;5000;2024.01.01;2025.12.29;120;60;60;0;4880;70;50;0;100000.00;104123.00;4123.00;4.12;223.00;0.2230;103900.00;3900.00;3.90;97800.00;106500.00;-2.20;6.50;97550.00;106300.00;-2.45;6.30;1.65;58.33;15.20;15200.00;2025.10.20 14:15:00;1.00;0.0100;0.0001;False;False;False
```

### **TXT Dosyası (optimization_results.txt)**

Terminal/log viewer'da okunabilir, tabular format.

```
OPTIMIZATION RESULTS - 2025.12.29 16:30:45
Trader: OptTrader
Symbol: EURUSD | Period: H1
Initial Balance: 100000.00 TL

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   ID | Trader Name          | Symbol     | Period | Strategy Name                  | ExecMs     | Islem  | Kaz    | Kayb   | GetiriFiyat  | Getiri%    | GetiriNet    | GetiriNet% | Komisyon   | ProfitF  | MaxDD%     | KarliOran
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    1 | OptTrader            | EURUSD     | H1     | EMA_5_20                       | 1234       | 150    | 85     | 65     | 5234.50      | 5.23       | 5000.00      | 5.00       | 234.50     | 1.85     | 12.34      | 56.67
    1 | OptTrader            | EURUSD     | H1     | EMA_5_30                       | 987        | 120    | 70     | 50     | 4123.00      | 4.12       | 3900.00      | 3.90       | 223.00     | 1.65     | 15.20      | 58.33
    1 | OptTrader            | EURUSD     | H1     | EMA_10_40                      | 1156       | 135    | 78     | 57     | 4567.00      | 4.57       | 4350.00      | 4.35       | 217.00     | 1.72     | 13.50      | 57.78
```

---

## 🎨 Excel'de Analiz

CSV dosyasını Excel'de açtıktan sonra:

1. **Sıralama**: `GetiriFiyatYuzdeNet` kolonuna göre azalan sırada sırala
2. **Filtreleme**:
   - `IslemSayisi >= 30`
   - `ProfitFactor > 1.5`
   - `GetiriMaxDD < 20`
3. **Pivot Table**: Stratejileri karşılaştır
4. **Chart**: Getiri vs Drawdown scatter plot

---

## ⚡ Performans İpuçları

1. **Sadece gerekli field'ları kullan**: Eğer daha az veri yeterli ise struct'ı özelleştir
2. **Batch writing**: Çok sayıda strateji için StringBuilder kullan, tek seferde yaz
3. **Parallel optimization**: Thread-safe yap, paralel backtest çalıştır
4. **Memory**: Struct olduğu için zaten hafif, ama binlerce sonuç için List yerine dosyaya direkt yaz

---

## 🔧 Özelleştirme

### Özel Field Eklemek

Eğer strateji-spesifik parametreler eklemek istersen:

```csharp
// Statistics.cs -> OptimizationSummary
public struct OptimizationSummary
{
    // ... mevcut field'lar ...

    // Özel field'lar
    public double CustomParam1;
    public double CustomParam2;
    public string CustomNote;

    // ToCsvRow() ve GetCsvHeader() methodlarını güncelle!
}
```

### Filtreleme ve Sıralama

```csharp
var summaries = new List<Statistics.OptimizationSummary>();

// ... optimizasyon çalıştır ve summaries'e ekle ...

// En iyi 5 strateji (Sharpe Ratio benzeri metrik)
var best = summaries
    .Where(s => s.IslemSayisi >= 20)
    .Select(s => new {
        Summary = s,
        SharpeRatio = s.GetiriFiyatYuzdeNet / (s.GetiriMaxDD == 0 ? 1 : s.GetiriMaxDD)
    })
    .OrderByDescending(x => x.SharpeRatio)
    .Take(5)
    .Select(x => x.Summary)
    .ToList();
```

---

## 📌 Notlar

- ✅ `Hesapla()` methodunu çağırmadan önce `GetOptimizationSummary()` kullanırsan, otomatik olarak `AssignToMapMinimal()` çağrılır
- ✅ CSV ayracı olarak `;` (noktalı virgül) kullanılıyor (Türkçe Excel için uygun)
- ✅ TXT dosyası monospace font ile açılmalı (Notepad++, VS Code)
- ✅ Dosya encoding: UTF-8 (Türkçe karakter desteği)

---

## 🚀 Sonuç

OptimizationSummary, backtest optimizasyonu için hızlı, hafif ve esnek bir çözümdür. YÖNTEM 1 kullanarak temiz bir mimari elde edebilir, sonuçları kolayca analiz edebilirsin.

İyi optimizasyonlar! 🎯
