using System;
using AlgoTradeWithOptimizationSupportWinFormsApp.ConsoleManagement;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks
{
    /// <summary>
    /// Console sink - ConsoleManager'a yazar
    /// </summary>
    public class ConsoleSink : ILogSink
    {
        private readonly object _lock = new object();
        private bool _isDisposed;
        private int _consoleIndex;

        public string Name => "Console";
        public LogSinks SinkType => LogSinks.Console;
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Constructor - varsayılan ana console (index 0)
        /// </summary>
        public ConsoleSink() : this(0)
        {
        }

        /// <summary>
        /// Constructor - belirli bir console index ile
        /// </summary>
        public ConsoleSink(int consoleIndex)
        {
            _consoleIndex = consoleIndex;
        }

        public void Write(LogEntry entry)
        {
            if (_isDisposed || !IsEnabled)
                return;

            lock (_lock)
            {
                try
                {
                    // ConsoleManager kullanarak yaz
                    var message = entry.ToString("medium");
                    var color = GetColor(entry.Level);
                    ConsoleManager.WriteLine(message, color, _consoleIndex);
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
                    ConsoleManager.Instance.ClearConsole(_consoleIndex);
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
