using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks
{
    /// <summary>
    /// File sink - Dosyaya batch yazma ile log yazar
    /// </summary>
    public class FileSink : ILogSink
    {
        private readonly string _filePath;
        private readonly ConcurrentQueue<LogEntry> _writeQueue = new ConcurrentQueue<LogEntry>();
        private readonly System.Threading.Timer _flushTimer;
        private readonly object _fileLock = new object();
        private bool _isDisposed;

        public string Name => "File";
        public LogSinks SinkType => LogSinks.File;
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Flush intervali (ms) - Default 1000ms
        /// </summary>
        public int FlushIntervalMs { get; set; } = 1000;

        public FileSink(string filePath, bool appendMode = true)
        {
            _filePath = filePath;

            // Dizini oluştur
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Append mode değilse dosyayı sıfırla
            if (!appendMode && File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Periyodik flush timer
            _flushTimer = new System.Threading.Timer(_ => Flush(), null, FlushIntervalMs, FlushIntervalMs);
        }

        public void Write(LogEntry entry)
        {
            if (_isDisposed || !IsEnabled)
                return;

            _writeQueue.Enqueue(entry);

            // Queue çok doluysa hemen flush et
            if (_writeQueue.Count > 100)
            {
                Flush();
            }
        }

        public void Flush()
        {
            if (_isDisposed || _writeQueue.IsEmpty)
                return;

            lock (_fileLock)
            {
                try
                {
                    var entriesToWrite = new System.Collections.Generic.List<LogEntry>();
                    while (_writeQueue.TryDequeue(out var entry))
                    {
                        entriesToWrite.Add(entry);
                    }

                    if (entriesToWrite.Count > 0)
                    {
                        var lines = entriesToWrite.Select(e => e.ToString("long"));
                        File.AppendAllLines(_filePath, lines);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"FileSink error: {ex.Message}");
                }
            }
        }

        public void Clear()
        {
            lock (_fileLock)
            {
                try
                {
                    _writeQueue.Clear();
                    if (File.Exists(_filePath))
                    {
                        File.Delete(_filePath);
                    }
                }
                catch { }
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _flushTimer?.Dispose();
            Flush(); // Son kalan logları yaz
        }
    }
}
