using System;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Logging
{
    /// <summary>
    /// Log sink hedefleri - Bitwise flags ile kullanılır
    ///
    /// KULLANIM ÖRNEKLERİ:
    ///
    /// // 1. Tüm GUI'lere gönder (merkezi!)
    /// LogManager.Log("User clicked button", sinks: LogSinks.Gui);
    ///
    /// // 2. Sadece Console ve GUI
    /// LogManager.Log("Debug info", sinks: LogSinks.Console | LogSinks.Gui);
    ///
    /// // 3. Sadece File (GUI'ye gösterme)
    /// LogManager.Log("Sensitive data", sinks: LogSinks.File);
    ///
    /// // 4. Granular kontrol (nadir kullanım)
    /// LogManager.Log("Only RichTextBox", sinks: LogSinks.RichTextBox);
    ///
    /// // 5. Her yere gönder (default)
    /// LogManager.Log("Important event");  // sinks parametresi verilmezse All
    ///
    /// // 6. Network hariç her yere
    /// LogManager.Log("Local only", sinks: LogSinks.AllButNetwork);
    ///
    /// // 7. Sadece lokal (Console + GUI)
    /// LogManager.Log("Dev debug", sinks: LogSinks.Local);
    ///
    /// // 8. Storage (File + ileride Database)
    /// LogManager.Log("Persistent data", sinks: LogSinks.Storage);
    /// </summary>
    [Flags]
    public enum LogSinks
    {
        /// <summary>
        /// Hiçbir sink'e gönderme
        /// </summary>
        None = 0,

        // ============================================================
        // BİREYSEL SINK'LER (Granular kontrol için)
        // ============================================================

        /// <summary>
        /// Console sink (Debug konsolu)
        /// </summary>
        Console = 1 << 0,  // 1

        /// <summary>
        /// File sink (Dosyaya log yazma)
        /// </summary>
        File = 1 << 1,  // 2

        /// <summary>
        /// Network sink (UDP/TCP ile log gönderme)
        /// </summary>
        Network = 1 << 2,  // 4

        /// <summary>
        /// RichTextBox sink (Renkli, formatlı GUI log)
        /// </summary>
        RichTextBox = 1 << 3,  // 8

        /// <summary>
        /// TextBox sink (Basit text GUI log)
        /// </summary>
        TextBox = 1 << 4,  // 16

        /// <summary>
        /// ListBox sink (Liste halinde GUI log)
        /// </summary>
        ListBox = 1 << 5,  // 32

        // İleride eklenebilecekler:
        // Database = 1 << 6,     // 64
        // Cloud = 1 << 7,        // 128
        // Email = 1 << 8,        // 256
        // DataGridView = 1 << 9, // 512

        // ============================================================
        // KATEGORİK GRUPLAR (Merkezi yönetim için)
        // ============================================================

        /// <summary>
        /// Tüm GUI sink'leri (RichTextBox + TextBox + ListBox)
        /// İleride yeni GUI sink eklenirse otomatik dahil olur
        /// </summary>
        Gui = RichTextBox | TextBox | ListBox,

        /// <summary>
        /// Persistent storage sink'leri (File + ileride Database)
        /// </summary>
        Storage = File,

        /// <summary>
        /// Remote sink'ler (Network + ileride Cloud + Email)
        /// </summary>
        Remote = Network,

        // ============================================================
        // KULLANIM KOMBİNASYONLARI (Sık kullanılanlar)
        // ============================================================

        /// <summary>
        /// Sadece lokal sink'ler (Console + Gui)
        /// Network'e gönderme, sadece ekranda göster
        /// </summary>
        Local = Console | Gui,

        /// <summary>
        /// Network hariç tümü (Console + File + Gui)
        /// Lokal development için ideal
        /// </summary>
        AllButNetwork = Console | File | Gui,

        /// <summary>
        /// Tüm sink'ler (Default değer)
        /// Her yere gönder
        /// </summary>
        All = Console | File | Network | Gui
    }
}
