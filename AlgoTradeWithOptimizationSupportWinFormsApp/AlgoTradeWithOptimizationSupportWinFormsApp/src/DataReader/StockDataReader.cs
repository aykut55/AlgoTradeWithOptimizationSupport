using AlgoTradeWithOptimizationSupportWinFormsApp.Config;
using AlgoTradeWithOptimizationSupportWinFormsApp.DataReader;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.src.timer;
using OoplesFinance.StockIndicators.Models;
using ScottPlot.Colormaps;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
// Merkezi tanımlardan StockData kullanımı (OoplesFinance ile karışmaması için açıkça belirtildi)
using StockData = AlgoTradeWithOptimizationSupportWinFormsApp.Definitions.StockData;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.DataReader
{
    /// <summary>
    /// Stock (Hisse Senedi) veri okuyucu
    /// - Senkron/Paralel/Async okuma desteği
    /// - Streaming (Producer-Consumer pattern) desteği
    /// - Filtreleme özellikleri (7 farklı mod)
    /// - LogManager, TimeManager, ConfigManager entegrasyonu
    /// - Metadata okuma desteği
    /// </summary>
    public class StockDataReader : IDisposable
    {
        // ====================================================================
        // EXISTING FIELDS (File1 - Original)
        // ====================================================================
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private int _readCount;

        // ====================================================================
        // NEW FIELDS (File2 - Extended Features)
        // ====================================================================
        private BlockingCollection<StockData>? _dataQueue;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _readTask;
        private bool _isDisposed;

        // Metadata storage
        private readonly ConcurrentDictionary<string, string> _metadata = new ConcurrentDictionary<string, string>();

        // Manager integrations
        private readonly string _timerIdPrefix = "StockDataReader";
        private bool _useLogManager = false;
        private bool _useTimeManager = false;
        private bool _useConfigManager = false;

        // ====================================================================
        // PROPERTIES
        // ====================================================================
        public int ReadCount => _readCount;

        public void StartTimer()
        {
            _stopwatch.Start();
        }

        public void StopTimer()
        {
            _stopwatch.Stop();
        }

        public TimeSpan GetElapsedTime()
        {
            return _stopwatch.Elapsed;
        }

        public long GetElapsedTimeMsec()
        {
            return _stopwatch.ElapsedMilliseconds;
        }

        public void Clear()
        {
            _stopwatch.Reset();
            _readCount = 0;
        }

        public List<StockData> ReadData(string filePath)
        {
            var data = new List<StockData>();
            var culture = new CultureInfo("tr-TR"); // For parsing numbers with comma as decimal separator

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified data file was not found.", filePath);
            }

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                // Skip header, comments, or empty lines
                if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#") || line.Trim().StartsWith("Id"))
                {
                    continue;
                }

                var parts = line.Split(';').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToArray();

                // Expected format: Id ; Date Time ; Open ; High ; Low ; Close ; Volume ; Lot
                if (parts.Length < 8)
                {
                    // Log or ignore malformed lines
                    Console.WriteLine($"Skipping malformed line: {line}");
                    continue;
                }

                try
                {
                    var dateTimePart = parts[1];
                    
                    // Check if date and time are in separate fields
                    if (parts.Length >= 9 && parts[1].Length == 10 && parts[2].Contains(":"))
                    {
                        // Format: parts[1] = "2013.07.12", parts[2] = "09:30:00"
                        dateTimePart = $"{parts[1]} {parts[2]}";
                        // Shift other parts indices by 1
                        var newParts = new string[parts.Length - 1];
                        newParts[0] = parts[0]; // Id
                        newParts[1] = dateTimePart; // Combined datetime
                        Array.Copy(parts, 3, newParts, 2, parts.Length - 3); // Rest of the data
                        parts = newParts;
                    }
                    else if (dateTimePart.Contains(";"))
                    {
                        // Format: "2007.06.11;13:15:00"
                        dateTimePart = dateTimePart.Replace(";", " ");
                    }
                    
                    var dateTime = DateTime.ParseExact(dateTimePart, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
                    
                    var stockData = new StockData
                    {
                        Id = int.Parse(parts[0]),
                        DateTime = dateTime,
                        Date = dateTime.Date,
                        Time = dateTime.TimeOfDay,
                        Open = double.Parse(parts[2], culture),
                        High = double.Parse(parts[3], culture),
                        Low = double.Parse(parts[4], culture),
                        Close = double.Parse(parts[5], culture),
                        Volume = long.Parse(parts[6], NumberStyles.Any, culture), // Changed to long.Parse
                        Size = int.Parse(parts[7], NumberStyles.Any, culture)
                    };
                    data.Add(stockData);
                }
                catch (FormatException ex)
                {
                    // Log or handle parsing errors
                    Console.WriteLine($"Could not parse line: '{line}'. Error: {ex.Message}");
                }
            }

            _readCount = data.Count;
            return data;
        }

        public List<StockData> _rdFstFl(string filePath)
        {
            var culture = new CultureInfo("tr-TR");
            var bag = new System.Collections.Concurrent.ConcurrentBag<StockData>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified data file was not found.", filePath);
            }

            File.ReadLines(filePath)
                .AsParallel()
                .Where(line => !(string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#") || line.Trim().StartsWith("Id")))
                .ForAll(line =>
                {
                    var parts = line.Split(';').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToArray();

                    if (parts.Length < 8)
                    {
                        return; // Skip malformed lines
                    }

                    try
                    {
                        var dateTimePart = parts[1];

                        // Check if date and time are in separate fields
                        if (parts.Length >= 9 && parts[1].Length == 10 && parts[2].Contains(":"))
                        {
                            // Format: parts[1] = "2013.07.12", parts[2] = "09:30:00"
                            dateTimePart = $"{parts[1]} {parts[2]}";
                            // Shift other parts indices by 1
                            var newParts = new string[parts.Length - 1];
                            newParts[0] = parts[0]; // Id
                            newParts[1] = dateTimePart; // Combined datetime
                            Array.Copy(parts, 3, newParts, 2, parts.Length - 3); // Rest of the data
                            parts = newParts;
                        }
                        else if (dateTimePart.Contains(";"))
                        {
                            // Format: "2007.06.11;13:15:00"
                            dateTimePart = dateTimePart.Replace(";", " ");
                        }

                        var dateTime = DateTime.ParseExact(dateTimePart, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);

                        var stockData = new StockData
                        {
                            Id = int.Parse(parts[0]),
                            DateTime = dateTime,
                            Date = dateTime.Date,
                            Time = dateTime.TimeOfDay,
                            Open = double.Parse(parts[2], culture),
                            High = double.Parse(parts[3], culture),
                            Low = double.Parse(parts[4], culture),
                            Close = double.Parse(parts[5], culture),
                            Volume = long.Parse(parts[6], NumberStyles.Any, culture),
                            Size = long.Parse(parts[7], NumberStyles.Any, culture)
                        };
                        bag.Add(stockData);
                    }
                    catch (FormatException ex)
                    {
                        // This might be noisy in parallel. Consider a different logging strategy for production.
                        Console.WriteLine($"Could not parse line: '{line}'. Error: {ex.Message}");
                    }
                });

            // Convert to list and sort to maintain original order, as parallel processing is non-deterministic.
            var sortedList = bag.ToList();
            sortedList.Sort((x, y) => x.Id.CompareTo(y.Id));
            _readCount = sortedList.Count;
            var stockData = sortedList[sortedList.Count - 1];
            return sortedList;
        }

        private static string ParseDateTimePart(string[] parts)
        {
            var dateTimePart = parts[1];
            
            // Check if date and time are in separate fields
            if (parts.Length >= 9 && parts[1].Length == 10 && parts[2].Contains(":"))
            {
                // Format: parts[1] = "2013.07.12", parts[2] = "09:30:00"
                dateTimePart = $"{parts[1]} {parts[2]}";
            }
            else if (dateTimePart.Contains(";"))
            {
                // Format: "2007.06.11;13:15:00"
                dateTimePart = dateTimePart.Replace(";", " ");
            }
            
            return dateTimePart;
        }

        private static StockData CreateStockData(string[] parts, CultureInfo culture)
        {
            var normalizedParts = NormalizeParts(parts);
            var dateTimePart = ParseDateTimePart(parts);
            var dateTime = DateTime.ParseExact(dateTimePart, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);

            return new StockData
            {
                Id = int.Parse(normalizedParts[0]),
                DateTime = dateTime,
                Date = dateTime.Date,
                Time = dateTime.TimeOfDay,
                Open = double.Parse(normalizedParts[2], culture),
                High = double.Parse(normalizedParts[3], culture),
                Low = double.Parse(normalizedParts[4], culture),
                Close = double.Parse(normalizedParts[5], culture),
                Volume = long.Parse(normalizedParts[6], NumberStyles.Any, culture),
                Size = long.Parse(normalizedParts[7], NumberStyles.Any, culture)
            };
        }

        private static string[] NormalizeParts(string[] parts)
        {
            // Check if date and time are in separate fields
            if (parts.Length >= 9 && parts[1].Length == 10 && parts[2].Contains(":"))
            {
                // Format: parts[1] = "2013.07.12", parts[2] = "09:30:00"
                var dateTimePart = $"{parts[1]} {parts[2]}";
                // Shift other parts indices by 1
                var newParts = new string[parts.Length - 1];
                newParts[0] = parts[0]; // Id
                newParts[1] = dateTimePart; // Combined datetime
                Array.Copy(parts, 3, newParts, 2, parts.Length - 3); // Rest of the data
                return newParts;
            }
            return parts;
        }

        public enum FilterMode
        {
            All,
            LastN,
            FirstN,
            IndexRange,
            AfterDateTime,
            BeforeDateTime,
            DateTimeRange
        }

        public List<StockData> ReadDataFast(string filePath, FilterMode mode = FilterMode.All, int n1 = 0, int n2 = 0, DateTime? dt1 = null, DateTime? dt2 = null)
        {
            var culture = new CultureInfo("tr-TR");
            var bag = new System.Collections.Concurrent.ConcurrentBag<StockData>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified data file was not found.", filePath);
            }

            File.ReadLines(filePath)
                .AsParallel()
                .Where(line => !(string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#") || line.Trim().StartsWith("Id")))
                .ForAll(line =>
                {
                    var parts = line.Split(';').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToArray();

                    if (parts.Length < 8)
                    {
                        return; // Skip malformed lines
                    }

                    try
                    {
                        var stockData = CreateStockData(parts, culture);
                        bag.Add(stockData);
                    }
                    catch (FormatException ex)
                    {
                        // This might be noisy in parallel. Consider a different logging strategy for production.
                        Console.WriteLine($"Could not parse line: '{line}'. Error: {ex.Message}");
                    }
                });

            // Convert to list and sort to maintain original order, as parallel processing is non-deterministic.
            var allData = bag.ToList();
            allData.Sort((x, y) => x.Id.CompareTo(y.Id));
            
            // Apply filtering based on mode
            var filteredData = ApplyFilter(allData, mode, n1, n2, dt1, dt2);
            
            _readCount = filteredData.Count;
            return filteredData;
        }

        private static List<StockData> ApplyFilter(List<StockData> data, FilterMode mode, int n1, int n2, DateTime? dt1, DateTime? dt2)
        {
            switch (mode)
            {
                case FilterMode.All:
                    return data;

                case FilterMode.LastN:
                    return data.TakeLast(n1).ToList();

                case FilterMode.FirstN:
                    return data.Take(n1).ToList();

                case FilterMode.IndexRange:
                    if (n1 < 0 || n2 < 0 || n1 > n2 || n1 >= data.Count)
                        return new List<StockData>();
                    var endIndex = Math.Min(n2, data.Count - 1);
                    return data.Skip(n1).Take(endIndex - n1 + 1).ToList();

                case FilterMode.AfterDateTime:
                    if (!dt1.HasValue) return data;
                    return data.Where(x => x.DateTime >= dt1.Value).ToList();

                case FilterMode.BeforeDateTime:
                    if (!dt1.HasValue) return data;
                    return data.Where(x => x.DateTime <= dt1.Value).ToList();

                case FilterMode.DateTimeRange:
                    if (!dt1.HasValue || !dt2.HasValue) return data;
                    return data.Where(x => x.DateTime >= dt1.Value && x.DateTime <= dt2.Value).ToList();

                default:
                    return data;
            }
        }

        // ====================================================================
        // MANAGER INTEGRATION - Configuration
        // ====================================================================

        /// <summary>
        /// LogManager kullanımını aktifleştir/devre dışı bırak
        /// </summary>
        public void EnableLogManager(bool enable = true)
        {
            _useLogManager = enable;
            if (_useLogManager)
            {
                LogManager.LogInfo("StockStockDataReader: LogManager enabled");
            }
        }

        /// <summary>
        /// TimeManager kullanımını aktifleştir/devre dışı bırak
        /// </summary>
        public void EnableTimeManager(bool enable = true)
        {
            _useTimeManager = enable;
        }

        /// <summary>
        /// ConfigManager kullanımını aktifleştir/devre dışı bırak
        /// </summary>
        public void EnableConfigManager(bool enable = true)
        {
            _useConfigManager = enable;
        }

        /// <summary>
        /// Tüm manager'ları aktifleştir
        /// </summary>
        public void EnableAllManagers(bool enable = true)
        {
            EnableLogManager(enable);
            EnableTimeManager(enable);
            EnableConfigManager(enable);
        }

        // ====================================================================
        // METADATA OPERATIONS (File2)
        // ====================================================================

        /// <summary>
        /// Metadata dictionary'si (# ile başlayan satırlardan okunan bilgiler)
        /// </summary>
        public ConcurrentDictionary<string, string> Metadata => _metadata;

        /// <summary>
        /// Metadata satırını parse et
        /// Format: # Key: Value
        /// </summary>
        private void ProcessMetadataLine(string line)
        {
            if (!line.TrimStart().StartsWith("#"))
                return;

            var content = line.TrimStart('#').Trim();
            var separatorIndex = content.IndexOf(':');

            if (separatorIndex > 0)
            {
                var key = content.Substring(0, separatorIndex).Trim();
                var value = content.Substring(separatorIndex + 1).Trim();
                _metadata[key] = value;

                if (_useLogManager)
                {
                    LogManager.LogTrace($"StockDataReader: Metadata loaded - {key}: {value}");
                }
            }
        }

        // ====================================================================
        // ASYNC OPERATIONS (File2)
        // ====================================================================

        /// <summary>
        /// Async senkron okuma (UI-friendly)
        /// </summary>
        public async Task<List<StockData>> ReadDataAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var timerId = $"{_timerIdPrefix}_ReadDataAsync";

            if (_useTimeManager)
            {
                TimeManager.Instance.StartTimer(timerId);
            }

            if (_useLogManager)
            {
                LogManager.LogInfo($"StockDataReader: Starting async read - {filePath}");
            }

            return await Task.Run(() =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = ReadData(filePath);

                    if (_useLogManager)
                    {
                        LogManager.LogInfo($"StockDataReader: Async read completed - {result.Count} records");
                    }

                    return result;
                }
                finally
                {
                    if (_useTimeManager)
                    {
                        TimeManager.Instance.StopTimer(timerId);
                        var elapsed = TimeManager.Instance.GetElapsedTime(timerId);
                        if (_useLogManager)
                        {
                            LogManager.LogInfo($"StockDataReader: Async read took {elapsed}ms");
                        }
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Async paralel okuma + filtreleme (UI-friendly)
        /// </summary>
        public async Task<List<StockData>> ReadDataFastAsync(
            string filePath,
            FilterMode mode = FilterMode.All,
            int n1 = 0,
            int n2 = 0,
            DateTime? dt1 = null,
            DateTime? dt2 = null,
            CancellationToken cancellationToken = default)
        {
            var timerId = $"{_timerIdPrefix}_ReadDataFastAsync";

            if (_useTimeManager)
            {
                TimeManager.Instance.StartTimer(timerId);
            }

            if (_useLogManager)
            {
                LogManager.LogInfo($"StockDataReader: Starting fast async read - {filePath}, Mode: {mode}");
            }

            return await Task.Run(() =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = ReadDataFast(filePath, mode, n1, n2, dt1, dt2);

                    if (_useLogManager)
                    {
                        LogManager.LogInfo($"StockDataReader: Fast async read completed - {result.Count} records");
                    }

                    return result;
                }
                finally
                {
                    if (_useTimeManager)
                    {
                        TimeManager.Instance.StopTimer(timerId);
                        var elapsed = TimeManager.Instance.GetElapsedTime(timerId);
                        if (_useLogManager)
                        {
                            LogManager.LogInfo($"StockDataReader: Fast async read took {elapsed}ms");
                        }
                    }
                }
            }, cancellationToken);
        }

        // ====================================================================
        // STREAMING OPERATIONS (File2 - Producer-Consumer Pattern)
        // ====================================================================

        /// <summary>
        /// Streaming okumayı başlat (memory-efficient, büyük dosyalar için)
        /// </summary>
        public async Task StartStreamingAsync(string filePath, int queueCapacity = 1000)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(StockDataReader));

            if (_dataQueue != null)
                throw new InvalidOperationException("Streaming already started. Call StopStreaming first.");

            var timerId = $"{_timerIdPrefix}_Streaming";

            if (_useTimeManager)
            {
                TimeManager.Instance.StartTimer(timerId);
            }

            if (_useLogManager)
            {
                LogManager.LogInfo($"StockDataReader: Starting streaming read - {filePath}");
            }

            _dataQueue = new BlockingCollection<StockData>(queueCapacity);
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            _readTask = Task.Run(async () =>
            {
                var culture = new CultureInfo("tr-TR");
                int id = 0;
                int processedLines = 0;

                try
                {
                    using var fileStream = new FileStream(
                        filePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 65536,
                        FileOptions.SequentialScan);

                    using var reader = new StreamReader(fileStream);

                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        // Process metadata
                        if (line.TrimStart().StartsWith("#"))
                        {
                            ProcessMetadataLine(line);
                            continue;
                        }

                        // Skip headers and empty lines
                        if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("Id"))
                        {
                            continue;
                        }

                        // Parse data line
                        var stockData = ParseDataLine(line, ++id, culture);
                        if (stockData.HasValue)
                        {
                            _dataQueue.Add(stockData.Value, cancellationToken);
                            processedLines++;
                        }
                    }

                    _readCount = processedLines;

                    if (_useLogManager)
                    {
                        LogManager.LogInfo($"StockDataReader: Streaming read completed - {processedLines} records");
                    }
                }
                catch (OperationCanceledException)
                {
                    if (_useLogManager)
                    {
                        LogManager.LogWarning("StockDataReader: Streaming read cancelled");
                    }
                }
                catch (Exception ex)
                {
                    if (_useLogManager)
                    {
                        LogManager.LogError($"StockDataReader: Streaming error - {ex.Message}", ex);
                    }
                    throw;
                }
                finally
                {
                    _dataQueue.CompleteAdding();

                    if (_useTimeManager)
                    {
                        TimeManager.Instance.StopTimer(timerId);
                        var elapsed = TimeManager.Instance.GetElapsedTime(timerId);
                        if (_useLogManager)
                        {
                            LogManager.LogInfo($"StockDataReader: Streaming took {elapsed}ms");
                        }
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Sıradaki veriyi al (blocking, timeout yok)
        /// </summary>
        public StockData? GetNextData()
        {
            if (_dataQueue == null)
                throw new InvalidOperationException("Streaming not started. Call StartStreamingAsync first.");

            try
            {
                if (_dataQueue.TryTake(out var data, Timeout.Infinite))
                {
                    return data;
                }
            }
            catch (InvalidOperationException)
            {
                // Queue completed
            }

            return null;
        }

        /// <summary>
        /// Sıradaki veriyi al (timeout destekli)
        /// </summary>
        public StockData? GetNextData(int timeoutMs)
        {
            if (_dataQueue == null)
                throw new InvalidOperationException("Streaming not started. Call StartStreamingAsync first.");

            try
            {
                if (_dataQueue.TryTake(out var data, timeoutMs))
                {
                    return data;
                }
            }
            catch (InvalidOperationException)
            {
                // Queue completed
            }

            return null;
        }

        /// <summary>
        /// Batch olarak veri al (N adet)
        /// </summary>
        public List<StockData> GetNextBatch(int batchSize, int timeoutMs = 1000)
        {
            var batch = new List<StockData>();

            for (int i = 0; i < batchSize; i++)
            {
                var data = GetNextData(timeoutMs);
                if (data.HasValue)
                {
                    batch.Add(data.Value);
                }
                else
                {
                    break;
                }
            }

            return batch;
        }

        /// <summary>
        /// Streaming'de daha fazla veri var mı?
        /// </summary>
        public bool HasMoreData => _dataQueue != null && !_dataQueue.IsCompleted;

        /// <summary>
        /// Streaming'i iptal et
        /// </summary>
        public void StopStreaming()
        {
            _cancellationTokenSource?.Cancel();
            _dataQueue?.CompleteAdding();

            if (_useLogManager)
            {
                LogManager.LogInfo("StockDataReader: Streaming stopped");
            }
        }

        // ====================================================================
        // HELPER METHODS FOR STREAMING
        // ====================================================================

        /// <summary>
        /// Streaming için veri satırını parse et
        /// </summary>
        private StockData? ParseDataLine(string line, int id, CultureInfo culture)
        {
            try
            {
                var parts = line.Split(';').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToArray();

                if (parts.Length < 8)
                {
                    if (_useLogManager)
                    {
                        LogManager.LogWarning($"StockDataReader: Malformed line (length {parts.Length})");
                    }
                    return null;
                }

                var stockData = CreateStockData(parts, culture);
                return stockData;
            }
            catch (Exception ex)
            {
                if (_useLogManager)
                {
                    LogManager.LogWarning($"StockDataReader: Parse error - {ex.Message}");
                }
                return null;
            }
        }

        // ====================================================================
        // SAFE PARSING HELPERS (File2)
        // ====================================================================

        /// <summary>
        /// Güvenli double parsing (virgül/nokta desteği)
        /// </summary>
        private static double ParseDoubleSafe(string value, CultureInfo culture)
        {
            if (double.TryParse(value, NumberStyles.Any, culture, out var result))
                return result;

            // Try with invariant culture
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result;

            return 0.0;
        }

        /// <summary>
        /// Güvenli long parsing
        /// </summary>
        private static long ParseLongSafe(string value, CultureInfo culture)
        {
            if (long.TryParse(value, NumberStyles.Any, culture, out var result))
                return result;

            if (long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result;

            return 0L;
        }

        // ====================================================================
        // CONFIG MANAGER INTEGRATION
        // ====================================================================

        /// <summary>
        /// Config dosyasından StockDataReader ayarlarını yükle
        /// </summary>
        public void LoadConfig(string configFilePath)
        {
            if (!_useConfigManager)
            {
                if (_useLogManager)
                {
                    LogManager.LogWarning("StockStockDataReader: ConfigManager not enabled. Call EnableConfigManager first.");
                }
                return;
            }

            try
            {
                var config = ConfigManager.Instance.LoadConfig<StockDataReaderConfig>(configFilePath);
                if (config != null)
                {
                    _useLogManager = config.UseLogManager;
                    _useTimeManager = config.UseTimeManager;

                    if (_useLogManager)
                    {
                        LogManager.LogInfo($"StockStockDataReader: Config loaded from {configFilePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                if (_useLogManager)
                {
                    LogManager.LogError($"StockStockDataReader: Config load error - {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Config dosyasına StockDataReader ayarlarını kaydet
        /// </summary>
        public void SaveConfig(string configFilePath)
        {
            if (!_useConfigManager)
            {
                if (_useLogManager)
                {
                    LogManager.LogWarning("StockStockDataReader: ConfigManager not enabled. Call EnableConfigManager first.");
                }
                return;
            }

            try
            {
                var config = new StockDataReaderConfig
                {
                    UseLogManager = _useLogManager,
                    UseTimeManager = _useTimeManager
                };

                ConfigManager.Instance.SaveConfig(configFilePath, config);

                if (_useLogManager)
                {
                    LogManager.LogInfo($"StockStockDataReader: Config saved to {configFilePath}");
                }
            }
            catch (Exception ex)
            {
                if (_useLogManager)
                {
                    LogManager.LogError($"StockStockDataReader: Config save error - {ex.Message}", ex);
                }
            }
        }

        // ====================================================================
        // IDISPOSABLE PATTERN (File2)
        // ====================================================================

        public void Dispose()
        {
            if (_isDisposed)
                return;

            if (_useLogManager)
            {
                LogManager.LogInfo("StockDataReader: Disposing resources");
            }

            _isDisposed = true;

            // Cancel streaming
            StopStreaming();

            // Wait for read task
            try
            {
                _readTask?.Wait(TimeSpan.FromSeconds(5));
            }
            catch { }

            // Dispose resources
            _cancellationTokenSource?.Dispose();
            _dataQueue?.Dispose();

            _metadata.Clear();
        }
    }

    // ====================================================================
    // CONFIG CLASS FOR STOCKDATAREADER
    // ====================================================================

    /// <summary>
    /// StockDataReader konfigürasyon sınıfı
    /// </summary>
    public class StockDataReaderConfig
    {
        public bool UseLogManager { get; set; } = false;
        public bool UseTimeManager { get; set; } = false;
        public bool UseConfigManager { get; set; } = false;
        public int StreamingQueueCapacity { get; set; } = 1000;
        public int StreamingBatchSize { get; set; } = 100;
    }

    // ====================================================================
    // USAGE EXAMPLES - Kapsamlı Kullanım Örnekleri
    // ====================================================================

    /*
    /// <summary>
    /// StockDataReader Usage Examples - Tüm kullanım senaryoları
    ///
    /// ====================================================================
    /// STOCKDATAREADER GENİŞLETİLMİŞ ÖZELLİKLER
    /// ====================================================================
    ///
    /// ✅ EKLENEN ÖZELLİKLER:
    ///
    /// 1. MANAGER ENTEGRASYONLARI
    ///    - EnableLogManager() - Loglama aktif/pasif
    ///    - EnableTimeManager() - Timing aktif/pasif
    ///    - EnableConfigManager() - Config yönetimi aktif/pasif
    ///    - EnableAllManagers() - Hepsini birden aç/kapat
    ///
    /// 2. METADATA OPERATIONS (File2)
    ///    - Metadata property - # ile başlayan satırları okur
    ///    - ProcessMetadataLine() - Metadata parsing
    ///    - Format: # Key: Value
    ///
    /// 3. ASYNC OPERATIONS (File2)
    ///    - ReadDataAsync() - UI-friendly async okuma
    ///    - ReadDataFastAsync() - Async paralel okuma + filtreleme
    ///    - CancellationToken desteği
    ///    - LogManager ve TimeManager entegreli
    ///
    /// 4. STREAMING OPERATIONS (File2 - Producer-Consumer Pattern)
    ///    - StartStreamingAsync() - Büyük dosyalar için memory-efficient
    ///    - GetNextData() - Tek veri al (blocking)
    ///    - GetNextData(timeoutMs) - Timeout destekli
    ///    - GetNextBatch(batchSize) - Toplu veri al
    ///    - HasMoreData - Daha veri var mı?
    ///    - StopStreaming() - İptal et
    ///
    /// 5. SAFE PARSING (File2)
    ///    - ParseDoubleSafe() - Güvenli double parsing
    ///    - ParseLongSafe() - Güvenli long parsing
    ///
    /// 6. CONFIG MANAGEMENT
    ///    - LoadConfig() - JSON config dosyasından ayarları yükle
    ///    - SaveConfig() - JSON config dosyasına ayarları kaydet
    ///    - DataReaderConfig class - Config veri yapısı
    ///
    /// 7. IDISPOSABLE PATTERN (File2)
    ///    - Dispose() - Proper resource cleanup
    ///    - using statement desteği
    ///
    /// ====================================================================
    /// 🔒 KORUNAN KODLAR (MEVCUT ÖZELLİKLER - HİÇ DEĞİŞMEDİ)
    /// ====================================================================
    ///
    ///    - ReadData() - Senkron okuma
    ///    - _rdFstFl() - Paralel hızlı okuma
    ///    - ReadDataFast() - Paralel okuma + filtreleme
    ///    - FilterMode enum - 7 farklı filtreleme modu
    ///    - Tüm helper metodlar (ParseDateTimePart, CreateStockData, vb.)
    ///    - StartTimer/StopTimer/GetElapsedTime metodları
    ///
    /// ====================================================================
    /// KULLANIM KARŞILAŞTIRMASI
    /// ====================================================================
    ///
    /// ESKİ KULLANIM (Hala çalışır):
    ///   var reader = new StockDataReader();
    ///   var data = reader.ReadDataFast("data.txt", StockStockDataReader.FilterMode.LastN, 1000);
    ///
    /// YENİ KULLANIM (LogManager + TimeManager):
    ///   var reader = new StockDataReader();
    ///   reader.EnableAllManagers(true);
    ///   var data = await reader.ReadDataFastAsync("data.txt", StockStockDataReader.FilterMode.LastN, 1000);
    ///
    /// YENİ KULLANIM (Streaming - büyük dosyalar):
    ///   using var reader = new StockDataReader();
    ///   await reader.StartStreamingAsync("large.txt");
    ///   while (reader.HasMoreData)
    ///   {
    ///       var stockData = reader.GetNextData(1000);
    ///       // Process...
    ///   }
    ///
    /// ====================================================================
    /// </summary>
    public static class StockDataReaderUsageExamples
    {
        // ====================================================================
        // ÖRNEK 1: Basit Hızlı Okuma (Mevcut - File1)
        // ====================================================================
        public static void Example1_BasicFastRead()
        {
            var reader = new StockDataReader();

            reader.StartTimer();
            var data = reader.ReadData("data/AAPL_1min.txt");
            reader.StopTimer();

            Console.WriteLine($"Okunan: {reader.ReadCount} kayıt");
            Console.WriteLine($"Süre: {reader.GetElapsedTimeMsec()}ms");
        }

        // ====================================================================
        // ÖRNEK 2: Paralel Hızlı Okuma (Mevcut - File1)
        // ====================================================================
        public static void Example2_ParallelFastRead()
        {
            var reader = new StockDataReader();

            reader.StartTimer();
            var data = reader._rdFstFl("data/AAPL_1min.txt");
            reader.StopTimer();

            Console.WriteLine($"Paralel okuma - {reader.ReadCount} kayıt, {reader.GetElapsedTimeMsec()}ms");
        }

        // ====================================================================
        // ÖRNEK 3: Filtrelenmiş Okuma - Son 1000 Kayıt (Mevcut - File1)
        // ====================================================================
        public static void Example3_FilteredRead_LastN()
        {
            var reader = new StockDataReader();

            var data = reader.ReadDataFast(
                "data/AAPL_1min.txt",
                StockDataReader.FilterMode.LastN,
                n1: 1000);

            Console.WriteLine($"Son 1000 kayıt okundu - Toplam: {data.Count}");
        }

        // ====================================================================
        // ÖRNEK 4: Tarih Aralığı Filtresi (Mevcut - File1)
        // ====================================================================
        public static void Example4_FilteredRead_DateRange()
        {
            var reader = new StockDataReader();

            var data = reader.ReadDataFast(
                "data/AAPL_1min.txt",
                StockDataReader.FilterMode.DateTimeRange,
                dt1: new DateTime(2023, 1, 1),
                dt2: new DateTime(2023, 12, 31));

            Console.WriteLine($"2023 yılı verileri - {data.Count} kayıt");
        }

        // ====================================================================
        // ÖRNEK 5: Async Okuma - UI Responsive (Yeni - File2)
        // ====================================================================
        public static async Task Example5_AsyncRead()
        {
            var reader = new StockDataReader();
            reader.EnableLogManager(true);
            reader.EnableTimeManager(true);

            var cts = new CancellationTokenSource();

            try
            {
                var data = await reader.ReadDataAsync("data/AAPL_1min.txt", cts.Token);
                Console.WriteLine($"Async okuma - {data.Count} kayıt");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Okuma iptal edildi");
            }
        }

        // ====================================================================
        // ÖRNEK 6: Async Hızlı Okuma + Filtreleme (Yeni - File2)
        // ====================================================================
        public static async Task Example6_AsyncFastReadFiltered()
        {
            var reader = new StockDataReader();
            reader.EnableAllManagers(true);

            var data = await reader.ReadDataFastAsync(
                "data/AAPL_1min.txt",
                StockDataReader.FilterMode.LastN,
                n1: 5000);

            Console.WriteLine($"Async + Filter - {data.Count} kayıt");
        }

        // ====================================================================
        // ÖRNEK 7: Streaming Okuma - Büyük Dosyalar (Yeni - File2)
        // ====================================================================
        public static async Task Example7_StreamingRead()
        {
            using var reader = new StockDataReader();
            reader.EnableLogManager(true);

            await reader.StartStreamingAsync("data/LARGE_FILE.txt", queueCapacity: 1000);

            int count = 0;
            while (reader.HasMoreData)
            {
                var data = reader.GetNextData(timeoutMs: 1000);
                if (data.HasValue)
                {
                    // Process data
                    count++;

                    if (count % 10000 == 0)
                    {
                        Console.WriteLine($"Processed {count} records...");
                    }
                }
            }

            Console.WriteLine($"Streaming completed - {count} kayıt");
        }

        // ====================================================================
        // ÖRNEK 8: Streaming + Batch Processing (Yeni - File2)
        // ====================================================================
        public static async Task Example8_StreamingBatchRead()
        {
            using var reader = new StockDataReader();

            await reader.StartStreamingAsync("data/LARGE_FILE.txt");

            while (reader.HasMoreData)
            {
                var batch = reader.GetNextBatch(batchSize: 100, timeoutMs: 1000);

                if (batch.Count > 0)
                {
                    // Process batch
                    Console.WriteLine($"Processing batch of {batch.Count} records");
                    ProcessBatch(batch);
                }
            }
        }

        // ====================================================================
        // ÖRNEK 9: Metadata Okuma (Yeni - File2)
        // ====================================================================
        public static async Task Example9_ReadWithMetadata()
        {
            using var reader = new StockDataReader();
            reader.EnableLogManager(true);

            await reader.StartStreamingAsync("data/AAPL_with_metadata.txt");

            // Metadata oku
            var symbol = reader.Metadata.GetValueOrDefault("GrafikSembol", "N/A");
            var barCount = reader.Metadata.GetValueOrDefault("BarCount", "N/A");
            var period = reader.Metadata.GetValueOrDefault("Periyot", "N/A");

            Console.WriteLine($"Symbol: {symbol}");
            Console.WriteLine($"Bar Count: {barCount}");
            Console.WriteLine($"Period: {period}");

            // Veriyi oku
            var allData = new List<StockData>();
            while (reader.HasMoreData)
            {
                var data = reader.GetNextData();
                if (data.HasValue)
                {
                    allData.Add(data.Value);
                }
            }

            Console.WriteLine($"Total records: {allData.Count}");
        }

        // ====================================================================
        // ÖRNEK 10: Config Yönetimi (Yeni - ConfigManager)
        // ====================================================================
        public static void Example10_ConfigManagement()
        {
            var reader = new StockDataReader();
            reader.EnableConfigManager(true);

            // Config kaydet
            reader.EnableLogManager(true);
            reader.EnableTimeManager(true);
            reader.SaveConfig("config/datareader.json");

            // Yeni reader ile config yükle
            var reader2 = new StockDataReader();
            reader2.EnableConfigManager(true);
            reader2.LoadConfig("config/datareader.json");

            Console.WriteLine("Config loaded successfully");
        }

        // ====================================================================
        // ÖRNEK 11: WinForms UI Integration - Progress Reporting
        // ====================================================================
        public static async Task Example11_WinFormsIntegration(IProgress<int> progress)
        {
            using var reader = new StockDataReader();
            reader.EnableAllManagers(true);

            await reader.StartStreamingAsync("data/AAPL_1min.txt");

            int count = 0;
            while (reader.HasMoreData)
            {
                var data = reader.GetNextData(100);
                if (data.HasValue)
                {
                    count++;

                    // Her 100 kayıtta bir progress güncelle
                    if (count % 100 == 0)
                    {
                        progress.Report(count);
                    }
                }
            }

            progress.Report(count); // Final count
        }

        // ====================================================================
        // ÖRNEK 12: Karşılaştırmalı Performans Testi
        // ====================================================================
        public static void Example12_PerformanceComparison()
        {
            var filePath = "data/LARGE_FILE.txt";

            // Test 1: Senkron okuma
            var reader1 = new StockDataReader();
            reader1.StartTimer();
            var data1 = reader1.ReadData(filePath);
            reader1.StopTimer();
            Console.WriteLine($"Senkron: {reader1.ReadCount} kayıt, {reader1.GetElapsedTimeMsec()}ms");

            // Test 2: Paralel okuma
            var reader2 = new StockDataReader();
            reader2.StartTimer();
            var data2 = reader2._rdFstFl(filePath);
            reader2.StopTimer();
            Console.WriteLine($"Paralel: {reader2.ReadCount} kayıt, {reader2.GetElapsedTimeMsec()}ms");

            // Test 3: Paralel + Filter
            var reader3 = new StockDataReader();
            reader3.StartTimer();
            var data3 = reader3.ReadDataFast(filePath, StockDataReader.FilterMode.LastN, 10000);
            reader3.StopTimer();
            Console.WriteLine($"Paralel+Filter: {reader3.ReadCount} kayıt, {reader3.GetElapsedTimeMsec()}ms");
        }

        // ====================================================================
        // Helper Methods
        // ====================================================================

        private static void ProcessBatch(List<StockData> batch)
        {
            // Batch işleme mantığı
            foreach (var data in batch)
            {
                // Process individual record
            }
        }
    }
    */
}









/*
● Çok iyi plan! 👍

  Önerdiğin yapı:
  src / DataReader /
    ├── StockDataReader.cs           // Text okuma (mevcut)
    ├── StockDataWriter.cs           // Text yazma (CSV export)
    ├── StockDataBinaryReader.cs     // Binary okuma (ReadList, GetRecord, GetRecordRange, vs.)
    └── StockDataBinaryWriter.cs     // Binary yazma (WriteList, AppendToFile, vs.)

  Artıları:
  - ✅ .NET naming convention'larına uygun (BinaryReader/BinaryWriter gibi)
  - ✅ Single Responsibility - her class tek bir iş yapar
  - ✅ Okuma/Yazma açıkça ayrı
  - ✅ Text/Binary açıkça ayrı
  - ✅ Scalable - ileride StockDataJsonReader, StockDataParquetWriter eklenebilir
  - ✅ Unit test yazmak kolay

  Opsiyonel iyileştirme:
  // Her birinden interface implement edebilirsin
  public interface IStockDataReader
{
    List<StockData> ReadList(string filePath);
    StockData GetRecord(string filePath, int index);
}

public class StockDataBinaryReader : IStockDataReader { ... }
public class StockDataTextReader : IStockDataReader { ... }

İleride dependency injection ile kullanabilirsin. Ama şimdilik basit versiyonu yeterli.

  Güzel refactoring planı, mantıklı. 👌
*/


/*
DataReader klasörünün adı ne olsun?

● Birkaç seçenek:

  1. Data (Önerim ⭐)
  src/Data/
    ├── StockDataReader.cs
    ├── StockDataWriter.cs
    ├── StockDataBinaryReader.cs
    └── StockDataBinaryWriter.cs
  - ✅ Kısa ve öz
  - ✅ .NET convention'a uygun
  - ✅ Hem okuma hem yazma kapsıyor

*/
