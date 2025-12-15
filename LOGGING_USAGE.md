# LogManager Kullanım Kılavuzu

## 📋 Genel Bakış

Profesyonel, thread-safe, variadic log sistemi:

- ✅ **Singleton** pattern
- ✅ **Thread-safe** (ConcurrentQueue + locks)
- ✅ **Variadic** log metodları (`params object[]`)
- ✅ **Buffer** sistemi (max 10000 entry)
- ✅ **Multiple sinks** (Console, File, Network, RichTextBox, TextBox, ListBox)
- ✅ **Selective routing** (LogSinks flags)
- ✅ **Thread bilgisi** (ID + Name)
- ✅ **Exception handling**

## 🚀 Hızlı Başlangıç

### 1. Sink'leri Register Et

```csharp
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks;

// Form1 constructor'da
public Form1()
{
    InitializeComponent();

    // Sink'leri register et
    LogManager.Instance.RegisterSink(new ConsoleSink());
    LogManager.Instance.RegisterSink(new FileSink("logs/app.log"));
    LogManager.Instance.RegisterSink(new RichTextBoxSink(richTextBox1));
}
```

### 2. Log Yaz (Basit)

```csharp
// En basit kullanım
LogManager.Log("Application started");

// Seviye belirt
LogManager.LogInfo("User logged in");
LogManager.LogWarning("Low memory");
LogManager.LogError("Connection failed");

// Variadic (çoklu parametre)
LogManager.Log("User clicked", "buttonName", 123, true);
```

### 3. Hedef Seçerek Log (Önemli!)

```csharp
// Sadece GUI'ye gönder (merkezi!)
LogManager.Log("Button clicked", sinks: LogSinks.Gui);

// Sadece Console ve GUI
LogManager.Log("Debug info", sinks: LogSinks.Console | LogSinks.Gui);

// Sadece File (GUI'ye gösterme)
LogManager.Log("Sensitive data", sinks: LogSinks.File);

// Network hariç her yere
LogManager.Log("Local event", sinks: LogSinks.AllButNetwork);

// Granular kontrol (nadir)
LogManager.Log("RichTextBox only", sinks: LogSinks.RichTextBox);
```

## 📊 LogSinks Flags (Merkezi Sistem)

```csharp
// Bireysel sink'ler
LogSinks.Console
LogSinks.File
LogSinks.Network
LogSinks.RichTextBox
LogSinks.TextBox
LogSinks.ListBox

// Kategorik gruplar (MERKEZI!)
LogSinks.Gui           // = RichTextBox | TextBox | ListBox
LogSinks.Storage       // = File (+ ileride Database)
LogSinks.Remote        // = Network (+ ileride Cloud)

// Kombinasyonlar
LogSinks.Local         // = Console | Gui
LogSinks.AllButNetwork // = Console | File | Gui
LogSinks.All           // = Hepsi (default)
```

## 🎯 Kullanım Senaryoları

### Senaryo 1: Genel Uygulama Logu

```csharp
// Tüm sink'lere gider (default)
LogManager.LogInfo("Application initialized");
LogManager.LogInfo("MainLoop started");
```

### Senaryo 2: Debug Bilgisi (Sadece Lokal)

```csharp
// Sadece Console ve GUI'ye, File/Network'e gitmesin
LogManager.Log("Variable x =", x, "y =", y, sinks: LogSinks.Local);
```

### Senaryo 3: Hassas Veri (Sadece File)

```csharp
// Sadece dosyaya, ekranda gösterme
LogManager.Log("API Key: xxx", sinks: LogSinks.File);
LogManager.Log("User password hash", sinks: LogSinks.Storage);
```

### Senaryo 4: GUI Feedback (Sadece GUI)

```csharp
// Kullanıcıya göster, dosyaya yazma
LogManager.Log("Order placed successfully", sinks: LogSinks.Gui);
LogManager.LogWarning("Low balance", sinks: LogSinks.Gui);
```

### Senaryo 5: Remote Monitoring (Network)

```csharp
// Sadece network'e gönder
LogManager.Log("Critical system event", sinks: LogSinks.Remote);
```

### Senaryo 6: Exception Logging

```csharp
try
{
    // Risky operation
}
catch (Exception ex)
{
    // Exception otomatik yakalanır
    LogManager.LogError("Operation failed", ex);

    // Sadece File'a exception yaz (GUI'de gösterme)
    LogManager.LogError("Exception details", ex, sinks: LogSinks.File);
}
```

## 🔧 Sink Yönetimi

### Sink Ekleme/Çıkarma

```csharp
// Ekle
LogManager.Instance.RegisterSink(new ConsoleSink());
LogManager.Instance.RegisterSink(new FileSink("logs/app.log"));

// Çıkar
LogManager.Instance.UnregisterSink(LogSinks.Console);

// Enable/Disable
LogManager.Instance.EnableSink(LogSinks.Network, false);  // Network'ü kapat
LogManager.Instance.EnableSink(LogSinks.Network, true);   // Aç

// Var mı kontrol et
if (LogManager.Instance.HasSink(LogSinks.File))
{
    // File sink var
}
```

### Tüm Sink'leri Yönet

```csharp
// Tüm sink'leri temizle
LogManager.Instance.ClearAllSinks();

// Tüm sink'leri flush et (dosyaya yazdır)
LogManager.Instance.FlushAllSinks();
```

## 📦 Buffer Yönetimi

```csharp
// Buffer'daki logları al (copy)
var logs = LogManager.Instance.GetBufferedLogs();
Console.WriteLine($"Buffer'da {logs.Count} log var");

// Buffer'ı al ve temizle
var logs = LogManager.Instance.GetAndClearBuffer();

// Sadece temizle
LogManager.Instance.ClearBuffer();

// Buffer boyutu
int count = LogManager.Instance.BufferCount;
```

## 🎨 GUI Sink Kurulumu

### RichTextBox (Renkli)

```csharp
// Form1.Designer.cs'de RichTextBox ekle
// Name: richTextBox1

// Form1.cs constructor'da
LogManager.Instance.RegisterSink(new RichTextBoxSink(richTextBox1));

// Test
LogManager.LogInfo("Info mesajı - Mavi");
LogManager.LogWarning("Uyarı - Turuncu");
LogManager.LogError("Hata - Kırmızı");
```

### TextBox (Basit)

```csharp
// Form1.Designer.cs'de TextBox ekle
// Name: textBox1
// Multiline: true
// ScrollBars: Vertical

// Form1.cs constructor'da
LogManager.Instance.RegisterSink(new TextBoxSink(textBox1));
```

### ListBox (Liste)

```csharp
// Form1.Designer.cs'de ListBox ekle
// Name: listBox1

// Form1.cs constructor'da
LogManager.Instance.RegisterSink(new ListBoxSink(listBox1));
```

## 🌐 Network Sink Kurulumu

```csharp
// UDP listener'a log gönder
LogManager.Instance.RegisterSink(new NetworkSink("127.0.0.1", 514));

// Log format değiştir (json, short, medium, long)
var networkSink = new NetworkSink("192.168.1.100", 9000);
networkSink.LogFormat = "json";
LogManager.Instance.RegisterSink(networkSink);

// Test
LogManager.Log("Network test", sinks: LogSinks.Network);
```

## 📁 File Sink Kurulumu

```csharp
// Append mode (default)
LogManager.Instance.RegisterSink(new FileSink("logs/app.log"));

// Overwrite mode (her başlatmada sıfırla)
LogManager.Instance.RegisterSink(new FileSink("logs/app.log", appendMode: false));

// Flush interval değiştir
var fileSink = new FileSink("logs/app.log");
fileSink.FlushIntervalMs = 5000;  // 5 saniyede bir flush
LogManager.Instance.RegisterSink(fileSink);
```

## 🧪 Tam Örnek (Form1)

```csharp
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        InitializeLogging();
    }

    private void InitializeLogging()
    {
        // 1. Sink'leri register et
        LogManager.Instance.RegisterSink(new ConsoleSink());
        LogManager.Instance.RegisterSink(new FileSink("logs/app.log"));
        LogManager.Instance.RegisterSink(new RichTextBoxSink(richTextBox1));

        // 2. İlk log
        LogManager.LogInfo("Application started");

        // 3. Network sink (opsiyonel)
        // LogManager.Instance.RegisterSink(new NetworkSink("127.0.0.1", 514));
    }

    private void button1_Click(object sender, EventArgs e)
    {
        // Kullanıcıya göster
        LogManager.Log("Button clicked!", sinks: LogSinks.Gui);

        // Dosyaya kaydet
        LogManager.Log("User action logged", sinks: LogSinks.File);

        // Her yere
        LogManager.LogInfo("Important event");
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        LogManager.LogInfo("Application closing");
        LogManager.Instance.FlushAllSinks();
        LogManager.Instance.Dispose();
        base.OnFormClosing(e);
    }
}
```

## 📈 Performans

- **Buffer**: ConcurrentQueue (lock-free)
- **Sink gönderimi**: Try-catch ile korumalı
- **GUI güncellemesi**: BeginInvoke (async, non-blocking)
- **File yazma**: Batch write (1 saniyede bir flush)
- **Network gönderimi**: Async Task.Run
- **Max buffer**: 10,000 entry (configurable)

## ⚙️ Konfigürasyon

```csharp
// Buffer boyutu
LogManager.Instance.MaxBufferSize = 50000;

// Default source
LogManager.Instance.DefaultSource = "MyApp";

// Sink max satır/item
var richTextBoxSink = new RichTextBoxSink(richTextBox1);
richTextBoxSink.MaxLines = 5000;
LogManager.Instance.RegisterSink(richTextBoxSink);
```

## 🎓 Best Practices

1. **Sink'leri başlangıçta register et**
   ```csharp
   // Form constructor veya Program.Main
   InitializeLogging();
   ```

2. **Hedef seçimi akıllıca kullan**
   ```csharp
   // Hassas veri → Sadece File
   LogManager.Log("API Key", sinks: LogSinks.File);

   // Debug → Sadece Console
   LogManager.LogDebug("x=", x, sinks: LogSinks.Console);

   // User feedback → Sadece GUI
   LogManager.Log("Success!", sinks: LogSinks.Gui);
   ```

3. **Exception'ları logla**
   ```csharp
   catch (Exception ex)
   {
       LogManager.LogError("Failed", ex);
   }
   ```

4. **Form kapanırken flush et**
   ```csharp
   protected override void OnFormClosing(...)
   {
       LogManager.Instance.FlushAllSinks();
       LogManager.Instance.Dispose();
   }
   ```

5. **Production'da Network sink'i disable et**
   ```csharp
   #if DEBUG
       LogManager.Instance.RegisterSink(new NetworkSink(...));
   #endif
   ```

## ✅ Özet

```csharp
// Setup (bir kez)
LogManager.Instance.RegisterSink(new ConsoleSink());
LogManager.Instance.RegisterSink(new FileSink("app.log"));
LogManager.Instance.RegisterSink(new RichTextBoxSink(richTextBox1));

// Kullanım (her yerde)
LogManager.Log("Message");                                    // → Tümüne
LogManager.Log("Debug", sinks: LogSinks.Console | LogSinks.Gui);  // → Seçici
LogManager.LogError("Error", exception);                      // → Exception ile
LogManager.Log("Data", x, y, z);                             // → Variadic

// Cleanup (form kapanırken)
LogManager.Instance.Dispose();
```

---

**Created**: 2025-12-16
**Version**: 1.0
**Status**: Production Ready
