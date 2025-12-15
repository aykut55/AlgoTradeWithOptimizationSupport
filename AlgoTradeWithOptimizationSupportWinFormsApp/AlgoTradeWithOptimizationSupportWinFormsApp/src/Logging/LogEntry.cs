using System;
using System.Collections.Generic;
using System.Threading;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Logging
{
    /// <summary>
    /// Log entry - Tek bir log kaydı
    /// Thread-safe, immutable after creation
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Log zamanı (UTC değil, local time)
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Log seviyesi
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Thread ID
        /// </summary>
        public int ThreadId { get; }

        /// <summary>
        /// Thread adı (varsa)
        /// </summary>
        public string? ThreadName { get; }

        /// <summary>
        /// Kaynak (Logger adı, sınıf adı, vb.)
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Log mesajı
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Exception (varsa)
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Ek veriler (key-value pairs)
        /// </summary>
        public IReadOnlyDictionary<string, object>? Properties { get; }

        /// <summary>
        /// Hangi sink'lere gönderilecek
        /// </summary>
        public LogSinks TargetSinks { get; }

        public LogEntry(
            LogLevel level,
            string message,
            string source = "",
            Exception? exception = null,
            Dictionary<string, object>? properties = null,
            LogSinks targetSinks = LogSinks.All)
        {
            Timestamp = DateTime.Now;
            Level = level;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            ThreadName = Thread.CurrentThread.Name;
            Source = source;
            Message = message ?? string.Empty;
            Exception = exception;
            Properties = properties;
            TargetSinks = targetSinks;
        }

        /// <summary>
        /// Varsayılan string formatı
        /// </summary>
        public override string ToString()
        {
            return ToString("default");
        }

        /// <summary>
        /// Özelleştirilmiş format
        /// </summary>
        public string ToString(string format)
        {
            var threadInfo = string.IsNullOrEmpty(ThreadName)
                ? $"T{ThreadId}"
                : $"{ThreadName}({ThreadId})";

            var sourceText = string.IsNullOrEmpty(Source) ? "" : $"[{Source}] ";
            var exceptionText = Exception != null
                ? $"\n  Exception: {Exception.GetType().Name}: {Exception.Message}\n  StackTrace: {Exception.StackTrace}"
                : string.Empty;

            return format switch
            {
                "short" => $"[{Timestamp:HH:mm:ss}] [{Level}] {Message}",
                "medium" => $"[{Timestamp:HH:mm:ss.fff}] [{Level}] {sourceText}{Message}",
                "default" => $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level,-7}] [{threadInfo,-12}] {sourceText}{Message}{exceptionText}",
                "long" => $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level,-7}] [{threadInfo,-12}] {sourceText}{Message}{exceptionText}{GetPropertiesText()}",
                "json" => ToJson(),
                _ => ToString("default")
            };
        }

        private string GetPropertiesText()
        {
            if (Properties == null || Properties.Count == 0)
                return string.Empty;

            var props = string.Join(", ", Properties.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            return $"\n  Properties: {props}";
        }

        /// <summary>
        /// JSON formatında (basit)
        /// </summary>
        private string ToJson()
        {
            var escapedMessage = Message.Replace("\"", "\\\"").Replace("\n", "\\n");
            var exceptionJson = Exception != null
                ? $",\"exception\":{{\"type\":\"{Exception.GetType().Name}\",\"message\":\"{Exception.Message.Replace("\"", "\\\"")}\"}}"
                : string.Empty;

            return $"{{\"timestamp\":\"{Timestamp:O}\",\"level\":\"{Level}\",\"thread\":{ThreadId},\"source\":\"{Source}\",\"message\":\"{escapedMessage}\"{exceptionJson}}}";
        }
    }
}
