using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Logging
{
    /// <summary>
    /// Ana Log Manager - Singleton, Thread-safe
    ///
    /// KULLANIM:
    ///
    /// // 1. Sink'leri register et
    /// LogManager.Instance.RegisterSink(new ConsoleSink());
    /// LogManager.Instance.RegisterSink(new FileSink("logs/app.log"));
    /// LogManager.Instance.RegisterSink(new RichTextBoxSink(richTextBox1));
    ///
    /// // 2. Log yaz (variadic)
    /// LogManager.Log("Application started");
    /// LogManager.Log("User clicked button", "param1", 123, true);
    /// LogManager.LogInfo("Info message");
    /// LogManager.LogError("Error occurred", exception);
    ///
    /// // 3. Hedef seçerek log (LogSinks enum ile)
    /// LogManager.Log("Debug info", sinks: LogSinks.Console | LogSinks.Gui);
    /// LogManager.Log("Sensitive data", sinks: LogSinks.File);
    ///
    /// // 4. Buffer yönetimi
    /// var logs = LogManager.Instance.GetBufferedLogs();
    /// LogManager.Instance.ClearBuffer();
    ///
    /// // 5. Sink yönetimi
    /// LogManager.Instance.EnableSink(LogSinks.Network, false);
    /// LogManager.Instance.ClearAllSinks();
    /// </summary>
    public class LogManager : IDisposable
    {
        private static LogManager? _instance;
        private static readonly object _instanceLock = new object();

        private readonly ConcurrentQueue<LogEntry> _buffer = new ConcurrentQueue<LogEntry>();
        private readonly List<ILogSink> _sinks = new List<ILogSink>();
        private readonly object _sinksLock = new object();
        private bool _isDisposed;

        /// <summary>
        /// Buffer max boyutu
        /// </summary>
        public int MaxBufferSize { get; set; } = 10000;

        /// <summary>
        /// Default log source
        /// </summary>
        public string DefaultSource { get; set; } = "App";

        private LogManager() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static LogManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LogManager();
                        }
                    }
                }
                return _instance;
            }
        }

        // ====================================================================
        // SINK YÖNETİMİ
        // ====================================================================

        /// <summary>
        /// Sink register et
        /// </summary>
        public void RegisterSink(ILogSink sink)
        {
            if (sink == null)
                throw new ArgumentNullException(nameof(sink));

            lock (_sinksLock)
            {
                // Aynı tip zaten varsa eski sini kaldır
                var existingSink = _sinks.FirstOrDefault(s => s.SinkType == sink.SinkType);
                if (existingSink != null)
                {
                    _sinks.Remove(existingSink);
                    existingSink.Dispose();
                }

                _sinks.Add(sink);
            }
        }

        /// <summary>
        /// Sink'i kaldır
        /// </summary>
        public void UnregisterSink(LogSinks sinkType)
        {
            lock (_sinksLock)
            {
                var sink = _sinks.FirstOrDefault(s => s.SinkType == sinkType);
                if (sink != null)
                {
                    _sinks.Remove(sink);
                    sink.Dispose();
                }
            }
        }

        /// <summary>
        /// Sink'i aktif/pasif yap
        /// </summary>
        public void EnableSink(LogSinks sinkType, bool enabled)
        {
            lock (_sinksLock)
            {
                var sink = _sinks.FirstOrDefault(s => s.SinkType == sinkType);
                if (sink != null)
                {
                    sink.IsEnabled = enabled;
                }
            }
        }

        /// <summary>
        /// Sink var mı kontrol et
        /// </summary>
        public bool HasSink(LogSinks sinkType)
        {
            lock (_sinksLock)
            {
                return _sinks.Any(s => s.SinkType == sinkType);
            }
        }

        /// <summary>
        /// Tüm sink'leri temizle
        /// </summary>
        public void ClearAllSinks()
        {
            lock (_sinksLock)
            {
                foreach (var sink in _sinks)
                {
                    try
                    {
                        sink.Clear();
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Tüm sink'leri flush et
        /// </summary>
        public void FlushAllSinks()
        {
            lock (_sinksLock)
            {
                foreach (var sink in _sinks)
                {
                    try
                    {
                        sink.Flush();
                    }
                    catch { }
                }
            }
        }

        // ====================================================================
        // LOG METODLARI (Variadic)
        // ====================================================================

        /// <summary>
        /// Genel log metodu (variadic)
        /// </summary>
        public static void Log(params object[] args)
        {
            Instance.LogInternal(LogLevel.Info, null, LogSinks.All, args);
        }

        /// <summary>
        /// Log seviyesi ve sink seçimiyle
        /// </summary>
        public static void Log(LogLevel level, LogSinks sinks, params object[] args)
        {
            Instance.LogInternal(level, null, sinks, args);
        }

        /// <summary>
        /// Log seviyesi, source ve sink seçimiyle
        /// </summary>
        public static void Log(LogLevel level, string source, LogSinks sinks, params object[] args)
        {
            Instance.LogInternal(level, source, sinks, args);
        }

        /// <summary>
        /// Trace level log
        /// </summary>
        public static void LogTrace(params object[] args)
        {
            Instance.LogInternal(LogLevel.Trace, null, LogSinks.All, args);
        }

        /// <summary>
        /// Debug level log
        /// </summary>
        public static void LogDebug(params object[] args)
        {
            Instance.LogInternal(LogLevel.Debug, null, LogSinks.All, args);
        }

        /// <summary>
        /// Info level log
        /// </summary>
        public static void LogInfo(params object[] args)
        {
            Instance.LogInternal(LogLevel.Info, null, LogSinks.All, args);
        }

        /// <summary>
        /// Warning level log
        /// </summary>
        public static void LogWarning(params object[] args)
        {
            Instance.LogInternal(LogLevel.Warning, null, LogSinks.All, args);
        }

        /// <summary>
        /// Error level log
        /// </summary>
        public static void LogError(params object[] args)
        {
            Instance.LogInternal(LogLevel.Error, null, LogSinks.All, args);
        }

        /// <summary>
        /// Fatal level log
        /// </summary>
        public static void LogFatal(params object[] args)
        {
            Instance.LogInternal(LogLevel.Fatal, null, LogSinks.All, args);
        }

        // ====================================================================
        // İÇ METOD - LOG İŞLEME
        // ====================================================================

        private void LogInternal(LogLevel level, string? source, LogSinks targetSinks, params object[] args)
        {
            if (_isDisposed || args == null || args.Length == 0)
                return;

            try
            {
                // Message ve exception'ı ayır
                string message = string.Empty;
                Exception? exception = null;
                var properties = new Dictionary<string, object>();

                // Args'ları işle
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == null)
                        continue;

                    if (args[i] is Exception ex)
                    {
                        exception = ex;
                    }
                    else if (i == 0)
                    {
                        // İlk parametre message
                        message = args[i].ToString() ?? string.Empty;
                    }
                    else
                    {
                        // Diğerleri properties
                        properties[$"arg{i}"] = args[i];
                    }
                }

                // LogEntry oluştur
                var entry = new LogEntry(
                    level: level,
                    message: message,
                    source: source ?? DefaultSource,
                    exception: exception,
                    properties: properties.Count > 0 ? properties : null,
                    targetSinks: targetSinks
                );

                // Buffer'a ekle
                AddToBuffer(entry);

                // Sink'lere gönder
                SendToSinks(entry);
            }
            catch (Exception ex)
            {
                // Log hatası - critical, ama uygulama durmasın
                System.Diagnostics.Debug.WriteLine($"LogManager error: {ex.Message}");
            }
        }

        private void AddToBuffer(LogEntry entry)
        {
            _buffer.Enqueue(entry);

            // Buffer boyut kontrolü
            while (_buffer.Count > MaxBufferSize)
            {
                _buffer.TryDequeue(out _);
            }
        }

        private void SendToSinks(LogEntry entry)
        {
            lock (_sinksLock)
            {
                foreach (var sink in _sinks)
                {
                    try
                    {
                        // Sink enabled mi?
                        if (!sink.IsEnabled)
                            continue;

                        // Bu sink hedeflenmiş mi?
                        if ((entry.TargetSinks & sink.SinkType) == 0)
                            continue;

                        // Gönder
                        sink.Write(entry);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Sink {sink.Name} error: {ex.Message}");
                    }
                }
            }
        }

        // ====================================================================
        // BUFFER YÖNETİMİ
        // ====================================================================

        /// <summary>
        /// Buffer'daki tüm logları al (copy)
        /// </summary>
        public List<LogEntry> GetBufferedLogs()
        {
            return _buffer.ToList();
        }

        /// <summary>
        /// Buffer'daki logları al ve temizle
        /// </summary>
        public List<LogEntry> GetAndClearBuffer()
        {
            var logs = new List<LogEntry>();
            while (_buffer.TryDequeue(out var entry))
            {
                logs.Add(entry);
            }
            return logs;
        }

        /// <summary>
        /// Buffer'ı temizle
        /// </summary>
        public void ClearBuffer()
        {
            _buffer.Clear();
        }

        /// <summary>
        /// Buffer boyutu
        /// </summary>
        public int BufferCount => _buffer.Count;

        // ====================================================================
        // DISPOSE
        // ====================================================================

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            // Tüm sink'leri flush ve dispose et
            lock (_sinksLock)
            {
                foreach (var sink in _sinks)
                {
                    try
                    {
                        sink.Flush();
                        sink.Dispose();
                    }
                    catch { }
                }
                _sinks.Clear();
            }

            _buffer.Clear();
        }
    }
}
