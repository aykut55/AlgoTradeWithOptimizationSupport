using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Debug
{
    /// <summary>
    /// Debug Logger - Visual Studio Output penceresine debug mesajları göndermek için yardımcı sınıf
    /// </summary>
    public static class DebugLogger
    {
        /// <summary>
        /// Debug mesajı gösterilsin mi (true: göster, false: gösterme)
        /// </summary>
        public static bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Timestamp gösterilsin mi
        /// </summary>
        public static bool ShowTimestamp { get; set; } = true;

        /// <summary>
        /// Caller method adı gösterilsin mi
        /// </summary>
        public static bool ShowCaller { get; set; } = false;

        /// <summary>
        /// Basit log mesajı
        /// </summary>
        public static void Log(string message)
        {
            if (!IsEnabled) return;

            string output = FormatMessage(message);
            System.Diagnostics.Debug.WriteLine(output);
        }

        /// <summary>
        /// Formatlı log mesajı (string interpolation destekler)
        /// </summary>
        public static void Log(string format, params object[] args)
        {
            if (!IsEnabled) return;

            string message = string.Format(format, args);
            Log(message);
        }

        /// <summary>
        /// Info seviyesinde log
        /// </summary>
        public static void Info(string message, [CallerMemberName] string callerName = "")
        {
            if (!IsEnabled) return;

            string output = FormatMessage($"[INFO] {message}", callerName);
            System.Diagnostics.Debug.WriteLine(output);
        }

        /// <summary>
        /// Warning seviyesinde log
        /// </summary>
        public static void Warning(string message, [CallerMemberName] string callerName = "")
        {
            if (!IsEnabled) return;

            string output = FormatMessage($"[WARNING] {message}", callerName);
            System.Diagnostics.Debug.WriteLine(output);
        }

        /// <summary>
        /// Error seviyesinde log
        /// </summary>
        public static void Error(string message, [CallerMemberName] string callerName = "")
        {
            if (!IsEnabled) return;

            string output = FormatMessage($"[ERROR] {message}", callerName);
            System.Diagnostics.Debug.WriteLine(output);
        }

        /// <summary>
        /// Exception log
        /// </summary>
        public static void Exception(Exception ex, string additionalMessage = "", [CallerMemberName] string callerName = "")
        {
            if (!IsEnabled) return;

            string message = string.IsNullOrEmpty(additionalMessage)
                ? $"[EXCEPTION] {ex.GetType().Name}: {ex.Message}"
                : $"[EXCEPTION] {additionalMessage} | {ex.GetType().Name}: {ex.Message}";

            string output = FormatMessage(message, callerName);
            System.Diagnostics.Debug.WriteLine(output);
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        }

        /// <summary>
        /// Performans ölçümü için Stopwatch başlat
        /// </summary>
        public static Stopwatch StartTimer(string operationName)
        {
            if (IsEnabled)
                Log($"[TIMER] Starting: {operationName}");

            return Stopwatch.StartNew();
        }

        /// <summary>
        /// Performans ölçümünü bitir ve sonucu logla
        /// </summary>
        public static void StopTimer(Stopwatch stopwatch, string operationName)
        {
            if (!IsEnabled) return;

            stopwatch.Stop();
            Log($"[TIMER] Completed: {operationName} - Elapsed: {stopwatch.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// Bar bilgisi ile log (Trading işlemleri için özel)
        /// </summary>
        public static void LogBar(int barIndex, string message)
        {
            if (!IsEnabled) return;

            Log($"[Bar {barIndex}] {message}");
        }

        /// <summary>
        /// Liste istatistiği log (Trading işlemleri için özel)
        /// </summary>
        public static void LogListStats(string listName, int countOfOnes, int totalCount)
        {
            if (!IsEnabled) return;

            double percentage = totalCount > 0 ? (countOfOnes * 100.0 / totalCount) : 0;
            Log($"[LIST] {listName}: {countOfOnes} adet '1' / {totalCount} toplam (% {percentage:F2})");
        }

        /// <summary>
        /// Ayırıcı çizgi ekle (görsel organizasyon için)
        /// </summary>
        public static void Separator(char character = '=', int length = 80)
        {
            if (!IsEnabled) return;

            System.Diagnostics.Debug.WriteLine(new string(character, length));
        }

        /// <summary>
        /// Tüm debug çıktılarını temizle
        /// </summary>
        public static void Clear()
        {
            // Not: Debug.WriteLine'ı temizleyemeyiz, ama ayırıcı ekleyebiliriz
            Separator('*', 100);
            Log("=== DEBUG OUTPUT CLEARED ===");
            Separator('*', 100);
        }

        #region Private Helper Methods

        private static string FormatMessage(string message, string callerName = "")
        {
            string output = "";

            if (ShowTimestamp)
                output += $"[{DateTime.Now:HH:mm:ss.fff}] ";

            if (ShowCaller && !string.IsNullOrEmpty(callerName))
                output += $"[{callerName}] ";

            output += message;

            return output;
        }

        #endregion
    }
}

/*
 * 
1. Basit Kullanım:
  DebugLogger.Log("Mesaj buraya");
  DebugLogger.Log("Bar {0} işleniyor", i);
  2. Seviye Bazlı Loglar:
  DebugLogger.Info("Bilgi mesajı");
  DebugLogger.Warning("Uyarı mesajı");
  DebugLogger.Error("Hata mesajı");
  DebugLogger.Exception(ex, "İşlem sırasında hata");
  3. Trading Özel Metodlar:
  DebugLogger.LogBar(i, "IsTradeEnabled=TRUE");
  DebugLogger.LogListStats("IsTradeEnabledList", 3851, 1854988);
  4. Performans Ölçümü:
  var sw = DebugLogger.StartTimer("Döngü işleme");
  // işlemler...
  DebugLogger.StopTimer(sw, "Döngü işleme");
  5. Ayarlar:
  DebugLogger.IsEnabled = true;      // Açık/Kapalı
  DebugLogger.ShowTimestamp = true;  // Zaman damgası göster
  DebugLogger.ShowCaller = false;    // Caller method adı göster

  💡 Örnek Kullanım:

  // Başlangıçta (Global.asax veya Program.cs)
  DebugLogger.IsEnabled = true;
  DebugLogger.ShowTimestamp = true;

  // SingleTrader.cs içinde
  DebugLogger.LogBar(i, $"IsTradeEnabled=TRUE -> liste[{i}]={this.lists.IsTradeEnabledList[i]}");

  // Statistics.cs içinde
  DebugLogger.LogListStats("IsTradeEnabledList", countIsTradeEnabled, Trader.Data.Count);
 */