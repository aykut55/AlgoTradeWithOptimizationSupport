using System;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks
{
    /// <summary>
    /// Console sink - Debug console'a yazar
    /// </summary>
    public class ConsoleSink : ILogSink
    {
        private readonly object _lock = new object();
        private bool _isDisposed;

        public string Name => "Console";
        public LogSinks SinkType => LogSinks.Console;
        public bool IsEnabled { get; set; } = true;

        public void Write(LogEntry entry)
        {
            if (_isDisposed || !IsEnabled)
                return;

            lock (_lock)
            {
                try
                {
                    // Renkli console output
                    var originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = GetColor(entry.Level);
                    Console.WriteLine(entry.ToString("medium"));
                    Console.ForegroundColor = originalColor;
                }
                catch (Exception ex)
                {
                    // Console yazma hatası - sessizce yut
                    System.Diagnostics.Debug.WriteLine($"ConsoleSink error: {ex.Message}");
                }
            }
        }

        private ConsoleColor GetColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => ConsoleColor.Gray,
                LogLevel.Debug => ConsoleColor.DarkGray,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Fatal => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };
        }

        public void Clear()
        {
            lock (_lock)
            {
                try
                {
                    Console.Clear();
                }
                catch { }
            }
        }

        public void Flush()
        {
            // Console otomatik flush eder
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}
