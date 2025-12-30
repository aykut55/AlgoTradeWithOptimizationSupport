using System;
using System.Windows.Forms;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategies;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;

namespace AlgoTradeWithOptimizationSupportWinFormsApp
{
    /// <summary>
    /// Form1 - AlgoTrader Test ve Demo Metodları
    /// Bu dosya AlgoTrader ve alt bileşenleri ile ilgili tüm test kodlarını içerir
    /// </summary>
    public partial class Form1
    {
        // ====================================================================
        // ALGOTRADER - LOCAL LOGGER
        // ====================================================================

        private SingleTraderLogger? _singleTraderLogger = null;
        private SingleTraderOptLogger? _singleTraderOptLogger = null;
        private AlgoTrader? algoTrader = null;

        /// <summary>
        /// AlgoTrader objelerini oluştur
        /// Form constructor'dan çağrılır
        /// </summary>
        private void CreateObjects()
        {
            _singleTraderLogger = new SingleTraderLogger(richTextBoxSingleTrader);
            _singleTraderOptLogger = new SingleTraderOptLogger(richTextBoxSingleTraderOptimization);

            algoTrader = new AlgoTrader();
            algoTrader.RegisterLogger(_singleTraderLogger);

            // TODO 545 : Optimization results güncelleme callback'ini bağla
            algoTrader.OnOptimizationResultsUpdated = OnOptimizationResultsUpdated;

            _singleTraderLogger.Log("=== AlgoTrader Objects Created ===");
            _singleTraderOptLogger.Log("=== AlgoTrader Objects Created ===");
        }

        /// <summary>
        /// Optimization results dosyası güncellendiğinde çağrılır (TODO 545)
        /// Dosyayı okur, sort eder ve dataGridViewOptimizationResults'a yazar
        /// </summary>
        private void OnOptimizationResultsUpdated(string filePath, bool useCsv)
        {
            // Debug dosyasına yaz
            string debugFile = "logs\\gui_update_debug.txt";
            try
            {
                System.IO.Directory.CreateDirectory("logs");
                System.IO.File.AppendAllText(debugFile, $"\n=== {DateTime.Now:HH:mm:ss.fff} ===\n");
                System.IO.File.AppendAllText(debugFile, $"OnOptimizationResultsUpdated called\n");
                System.IO.File.AppendAllText(debugFile, $"File: {filePath}\n");
                System.IO.File.AppendAllText(debugFile, $"UseCsv: {useCsv}\n");
                System.IO.File.AppendAllText(debugFile, $"File exists: {System.IO.File.Exists(filePath)}\n");
            }
            catch { }

            _singleTraderOptLogger?.Log($"[DEBUG] OnOptimizationResultsUpdated called - File: {filePath}, UseCsv: {useCsv}");
            _singleTraderOptLogger?.Log($"[DEBUG] InvokeRequired: {InvokeRequired}");

            // Thread-safe UI update
            UpdateUIControl(() =>
            {
                try
                {
                    System.IO.File.AppendAllText(debugFile, $"Inside UpdateUIControl lambda\n");

                    _singleTraderOptLogger?.Log($"[DEBUG] Inside UpdateUIControl lambda");
                    _singleTraderOptLogger?.Log($"Optimization results updated: {filePath} (CSV: {useCsv})");

                    // Dosya var mı kontrol et
                    if (!System.IO.File.Exists(filePath))
                    {
                        System.IO.File.AppendAllText(debugFile, $"ERROR: File not found!\n");
                        _singleTraderOptLogger?.LogWarning($"Optimization results file not found: {filePath}");
                        return;
                    }

                    System.IO.File.AppendAllText(debugFile, $"File found, calling read method\n");
                    _singleTraderOptLogger?.Log($"[DEBUG] File exists, calling read method...");

                    // CSV dosyası ise oku
                    if (useCsv)
                    {
                        System.IO.File.AppendAllText(debugFile, $"Reading CSV...\n");
                        _singleTraderOptLogger?.Log($"[DEBUG] Reading CSV file...");
                        ReadAndDisplayCsvOptimizationResults(filePath);
                    }
                    else
                    {
                        System.IO.File.AppendAllText(debugFile, $"Reading TXT...\n");
                        _singleTraderOptLogger?.Log($"[DEBUG] Reading TXT file...");
                        // TXT dosyası oku
                        ReadAndDisplayTxtOptimizationResults(filePath);
                    }

                    System.IO.File.AppendAllText(debugFile, $"Read completed - Grid rows: {dataGridViewOptimizationResults.Rows.Count}\n");
                    _singleTraderOptLogger?.Log($"[DEBUG] Read method completed.");
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(debugFile, $"EXCEPTION: {ex.Message}\n");
                    System.IO.File.AppendAllText(debugFile, $"Stack: {ex.StackTrace}\n");
                    _singleTraderOptLogger?.LogError($"Error reading optimization results: {ex.Message}");
                    _singleTraderOptLogger?.LogError($"Stack trace: {ex.StackTrace}");
                }
            });

            System.IO.File.AppendAllText(debugFile, $"OnOptimizationResultsUpdated exiting\n");
            _singleTraderOptLogger?.Log($"[DEBUG] OnOptimizationResultsUpdated exiting");
        }

        /// <summary>
        /// CSV optimization results dosyasını okur ve dataGridViewOptimizationResults'a yazar
        /// Duplicate check yapar (CombNo'ya göre) ve NetProfit'e göre sort eder
        /// </summary>
        private void ReadAndDisplayCsvOptimizationResults(string filePath)
        {
            try
            {
                _singleTraderOptLogger?.Log($"[DEBUG] Reading CSV file: {filePath}");

                // CSV dosyasını oku
                var lines = System.IO.File.ReadAllLines(filePath);
                if (lines.Length == 0)
                {
                    _singleTraderOptLogger?.LogWarning("CSV file is empty.");
                    return;
                }

                // İlk satır header
                string headerLine = lines[0];
                var headers = headerLine.Split(',')
                                       .Select(h => h.Trim())
                                       .ToArray();

                _singleTraderOptLogger?.Log($"[DEBUG] Headers count: {headers.Length}");

                // CombNo ve NetProfit kolonlarının indexini bul
                int combNoIndex = Array.FindIndex(headers, h => h.Equals("CombNo", StringComparison.OrdinalIgnoreCase));
                int netProfitIndex = Array.FindIndex(headers, h => h.Contains("NetProf") || h.Contains("OR_NetProf"));

                _singleTraderOptLogger?.Log($"[DEBUG] CombNo index: {combNoIndex}, NetProfit index: {netProfitIndex}");

                // DataGridView'ı temizle ve kolonları oluştur (sadece ilk kez)
                if (dataGridViewOptimizationResults.Columns.Count == 0)
                {
                    _singleTraderOptLogger?.Log($"[DEBUG] Creating columns...");
                    dataGridViewOptimizationResults.Columns.Clear();

                    // İlk kolon: Rank
                    dataGridViewOptimizationResults.Columns.Add("Rank", "Rank");

                    // Geri kalan kolonlar
                    foreach (var header in headers)
                    {
                        dataGridViewOptimizationResults.Columns.Add(header, header);
                    }
                    _singleTraderOptLogger?.Log($"[DEBUG] Columns created: {dataGridViewOptimizationResults.Columns.Count}");
                }

                // Tüm veriyi oku ve Dictionary'de tut (CombNo -> values)
                var dataDict = new Dictionary<string, string[]>();
                int duplicateCount = 0;

                // Veri satırlarını oku (header hariç)
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    var values = lines[i].Split(',')
                                        .Select(v => v.Trim())
                                        .ToArray();

                    // Kolon sayısı eşleşiyorsa ekle
                    if (values.Length == headers.Length && combNoIndex >= 0)
                    {
                        string combNo = values[combNoIndex];

                        // Duplicate check - sadece en son eklenen değeri tut
                        if (dataDict.ContainsKey(combNo))
                        {
                            duplicateCount++;
                            _singleTraderOptLogger?.Log($"[DEBUG] Duplicate CombNo {combNo} - updating with latest data");
                        }

                        dataDict[combNo] = values;
                    }
                }

                _singleTraderOptLogger?.Log($"[DEBUG] Unique rows: {dataDict.Count}, Duplicates removed: {duplicateCount}");

                // Sort by NetProfit (azalan sırada)
                var sortedData = dataDict.Values.AsEnumerable();

                if (netProfitIndex >= 0)
                {
                    sortedData = sortedData.OrderByDescending(values =>
                    {
                        if (double.TryParse(values[netProfitIndex], out double netProfit))
                            return netProfit;
                        return double.MinValue;
                    });
                    _singleTraderOptLogger?.Log($"[DEBUG] Data sorted by NetProfit (descending)");
                }

                // DataGridView'ı temizle
                dataGridViewOptimizationResults.Rows.Clear();
                _singleTraderOptLogger?.Log($"[DEBUG] Rows cleared.");

                // Sıralı ve unique veriyi ekle (Rank ile birlikte)
                int rowsAdded = 0;
                int rank = 1;
                foreach (var values in sortedData)
                {
                    // Rank'ı integer olarak en başa ekle (numeric sorting için)
                    var valuesWithRank = new object[] { rank }.Concat(values.Cast<object>()).ToArray();
                    dataGridViewOptimizationResults.Rows.Add(valuesWithRank);
                    rowsAdded++;
                    rank++;
                }

                _singleTraderOptLogger?.Log($"[DEBUG] Total rows added to grid: {rowsAdded}");
                _singleTraderOptLogger?.Log($"Loaded {dataGridViewOptimizationResults.Rows.Count} optimization results from CSV (sorted by NetProfit).");
            }
            catch (Exception ex)
            {
                _singleTraderOptLogger?.LogError($"Error reading CSV file: {ex.Message}");
                _singleTraderOptLogger?.LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// TXT optimization results dosyasını okur ve dataGridViewOptimizationResults'a yazar
        /// TXT dosyası pipe (|) ile ayrılmış fixed-width format kullanıyor
        /// Duplicate check yapar (CombNo'ya göre) ve NetProfit'e göre sort eder
        /// </summary>
        private void ReadAndDisplayTxtOptimizationResults(string filePath)
        {
            string debugFile = "logs\\gui_update_debug.txt";
            try
            {
                System.IO.File.AppendAllText(debugFile, $"  ReadTXT: Starting\n");
                _singleTraderOptLogger?.Log($"[DEBUG] Reading TXT file: {filePath}");

                // TXT dosyasını oku
                var lines = System.IO.File.ReadAllLines(filePath);
                System.IO.File.AppendAllText(debugFile, $"  ReadTXT: Total lines = {lines.Length}\n");
                _singleTraderOptLogger?.Log($"[DEBUG] Total lines read: {lines.Length}");

                if (lines.Length == 0)
                {
                    _singleTraderOptLogger?.LogWarning("TXT file is empty.");
                    return;
                }

                // İlk birkaç satır başlık ve çizgi olabilir, header satırını bul
                int headerLineIndex = -1;
                System.IO.File.AppendAllText(debugFile, $"  ReadTXT: Searching for header in first 20 lines\n");

                for (int i = 0; i < Math.Min(20, lines.Length); i++)
                {
                    string preview = lines[i].Length > 100 ? lines[i].Substring(0, 100) + "..." : lines[i];
                    _singleTraderOptLogger?.Log($"[DEBUG] Line {i}: {preview}");
                    System.IO.File.AppendAllText(debugFile, $"  Line {i}: {preview}\n");

                    // Header'ı bul - pipe içeren ve "CombNo" içeren satır
                    if (lines[i].Contains("|") && lines[i].Contains("CombNo"))
                    {
                        headerLineIndex = i;
                        System.IO.File.AppendAllText(debugFile, $"  ReadTXT: Header found at line {i}\n");
                        _singleTraderOptLogger?.Log($"[DEBUG] Header found at line {i}");
                        break;
                    }
                }

                if (headerLineIndex == -1)
                {
                    System.IO.File.AppendAllText(debugFile, $"  ReadTXT: ERROR - Header not found!\n");
                    _singleTraderOptLogger?.LogWarning("TXT file header not found.");
                    _singleTraderOptLogger?.LogWarning("Check logs\\gui_update_debug.txt for details");
                    return;
                }

                // Header satırını parse et (pipe ile ayrılmış)
                string headerLine = lines[headerLineIndex];
                var headers = headerLine.Split('|', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(h => h.Trim())
                                       .ToArray();

                _singleTraderOptLogger?.Log($"[DEBUG] Headers count: {headers.Length}");
                _singleTraderOptLogger?.Log($"[DEBUG] First 5 headers: {string.Join(", ", headers.Take(5))}");

                // CombNo ve NetProfit kolonlarının indexini bul
                int combNoIndex = Array.FindIndex(headers, h => h.Equals("CombNo", StringComparison.OrdinalIgnoreCase));
                int netProfitIndex = Array.FindIndex(headers, h => h.Contains("NetProf") || h.Contains("OR_NetProf"));

                System.IO.File.AppendAllText(debugFile, $"  ReadTXT: CombNo index = {combNoIndex}, NetProfit index = {netProfitIndex}\n");
                _singleTraderOptLogger?.Log($"[DEBUG] CombNo index: {combNoIndex}, NetProfit index: {netProfitIndex}");

                // DataGridView'ı temizle ve kolonları oluştur (sadece ilk kez)
                if (dataGridViewOptimizationResults.Columns.Count == 0)
                {
                    _singleTraderOptLogger?.Log($"[DEBUG] Creating columns...");
                    dataGridViewOptimizationResults.Columns.Clear();

                    // İlk kolon: Rank
                    dataGridViewOptimizationResults.Columns.Add("Rank", "Rank");

                    // Geri kalan kolonlar
                    foreach (var header in headers)
                    {
                        dataGridViewOptimizationResults.Columns.Add(header, header);
                    }
                    _singleTraderOptLogger?.Log($"[DEBUG] Columns created: {dataGridViewOptimizationResults.Columns.Count}");
                }
                else
                {
                    _singleTraderOptLogger?.Log($"[DEBUG] Columns already exist: {dataGridViewOptimizationResults.Columns.Count}");
                }

                // Tüm veriyi oku ve Dictionary'de tut (CombNo -> values)
                var dataDict = new Dictionary<string, string[]>();
                int duplicateCount = 0;

                for (int i = headerLineIndex + 1; i < lines.Length; i++)
                {
                    // Boş satırları ve ayırıcı satırları atla
                    if (string.IsNullOrWhiteSpace(lines[i]) || lines[i].Contains("==="))
                        continue;

                    // Pipe ile ayrılmış değerleri parse et
                    var values = lines[i].Split('|', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(v => v.Trim())
                                        .ToArray();

                    // Kolon sayısı eşleşiyorsa ekle
                    if (values.Length == headers.Length && combNoIndex >= 0)
                    {
                        string combNo = values[combNoIndex];

                        // Duplicate check - sadece en son eklenen değeri tut
                        if (dataDict.ContainsKey(combNo))
                        {
                            duplicateCount++;
                            _singleTraderOptLogger?.Log($"[DEBUG] Duplicate CombNo {combNo} - updating with latest data");
                        }

                        dataDict[combNo] = values;
                    }
                }

                System.IO.File.AppendAllText(debugFile, $"  ReadTXT: Unique rows = {dataDict.Count}, Duplicates = {duplicateCount}\n");
                _singleTraderOptLogger?.Log($"[DEBUG] Unique rows: {dataDict.Count}, Duplicates removed: {duplicateCount}");

                // Sort by NetProfit (azalan sırada)
                var sortedData = dataDict.Values.AsEnumerable();

                if (netProfitIndex >= 0)
                {
                    sortedData = sortedData.OrderByDescending(values =>
                    {
                        if (double.TryParse(values[netProfitIndex], out double netProfit))
                            return netProfit;
                        return double.MinValue;
                    });
                    _singleTraderOptLogger?.Log($"[DEBUG] Data sorted by NetProfit (descending)");
                }

                // DataGridView'ı temizle
                dataGridViewOptimizationResults.Rows.Clear();
                _singleTraderOptLogger?.Log($"[DEBUG] Rows cleared.");

                // Sıralı ve unique veriyi ekle (Rank ile birlikte)
                int rowsAdded = 0;
                int rank = 1;
                foreach (var values in sortedData)
                {
                    // Rank'ı integer olarak en başa ekle (numeric sorting için)
                    var valuesWithRank = new object[] { rank }.Concat(values.Cast<object>()).ToArray();
                    dataGridViewOptimizationResults.Rows.Add(valuesWithRank);
                    rowsAdded++;
                    rank++;
                }

                System.IO.File.AppendAllText(debugFile, $"  ReadTXT: Rows added to grid = {rowsAdded}\n");
                _singleTraderOptLogger?.Log($"[DEBUG] Total rows added to grid: {rowsAdded}");
                _singleTraderOptLogger?.Log($"Loaded {dataGridViewOptimizationResults.Rows.Count} optimization results from TXT (sorted by NetProfit).");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(debugFile, $"  ReadTXT: EXCEPTION - {ex.Message}\n");
                _singleTraderOptLogger?.LogError($"Error reading TXT file: {ex.Message}");
                _singleTraderOptLogger?.LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// AlgoTrader objelerini temizle/sil
        /// Form kapatılırken veya reset gerektiğinde çağrılır
        /// </summary>
        private void DeleteObjects()
        {
            // Logger'ı dispose et ve temizle
            _singleTraderLogger?.Dispose();
            _singleTraderLogger = null;

            // AlgoTrader'ı temizle
            algoTrader?.Reset();
            algoTrader = null;
        }

        /// <summary>
        /// AlgoTrader objelerini sıfırla
        /// Objeleri silmeden sadece içlerini temizler
        /// </summary>
        private void ResetObjects()
        {
            // Logger'ı temizle
            _singleTraderLogger?.Clear();

            // AlgoTrader'ı sıfırla
            algoTrader?.Reset();

            _singleTraderLogger?.Log("=== AlgoTrader Objects Reset ===");
        }

        /// <summary>
        /// SingleTrader tab için local logger
        /// Implements IAlgoTraderLogger interface
        /// </summary>
        private class SingleTraderLogger : IAlgoTraderLogger, IDisposable
        {
            private readonly RichTextBox _richTextBox;
            private readonly FileSink _fileSink;
            private readonly object _lockObject = new object();
            private bool _isDisposed = false;

            public SingleTraderLogger(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
                _fileSink = new FileSink("logs", "singleTraderLog.txt", appendMode: false);
            }

            public void Log(params object[] args)
            {
                WriteLog("INFO", args);
            }

            public void LogWarning(params object[] args)
            {
                WriteLog("WARNING", args);
            }

            public void LogError(params object[] args)
            {
                WriteLog("ERROR", args);
            }

            public void Clear()
            {
                if (_richTextBox.InvokeRequired)
                {
                    _richTextBox.Invoke(() => _richTextBox.Clear());
                }
                else
                {
                    _richTextBox.Clear();
                }

                // Dosyayı da temizle
                _fileSink?.Clear();
            }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    _fileSink?.Dispose();
                }
            }

            private void WriteLog(string level, params object[] args)
            {
                if (args == null || args.Length == 0)
                    return;

                lock (_lockObject)
                {
                    var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                    var message = string.Join(" ", args);
                    var logLine = $"[{timestamp}] [{level}] {message}";

                    // RichTextBox'a yaz
                    if (_richTextBox.InvokeRequired)
                    {
                        _richTextBox.Invoke(() =>
                        {
                            _richTextBox.AppendText(logLine + Environment.NewLine);
                            _richTextBox.SelectionStart = _richTextBox.Text.Length;
                            _richTextBox.ScrollToCaret();
                        });
                    }
                    else
                    {
                        _richTextBox.AppendText(logLine + Environment.NewLine);
                        _richTextBox.SelectionStart = _richTextBox.Text.Length;
                        _richTextBox.ScrollToCaret();
                    }

                    // Dosyaya yaz
                    if (!_isDisposed && _fileSink != null)
                    {
                        var logLevel = level switch
                        {
                            "WARNING" => LogLevel.Warning,
                            "ERROR" => LogLevel.Error,
                            _ => LogLevel.Info
                        };

                        var logEntry = new LogEntry(logLevel, message, "SingleTrader");
                        _fileSink.Write(logEntry);
                    }
                }
            }
        }

        /// <summary>
        /// SingleTrader tab için local logger'ı başlat (sadece ilk kez)
        /// </summary>
        private void InitializeSingleTraderLogger()
        {
            if (_singleTraderLogger == null)
            {
                _singleTraderLogger = new SingleTraderLogger(richTextBoxSingleTrader);
                _singleTraderLogger.Log("=== SingleTrader Logger Initialized ===");
            }
            else
            {
                _singleTraderLogger.Clear();
                _singleTraderLogger.Log("=== SingleTrader Logger Cleared ===");
            }
        }

        /// <summary>
        /// SingleTraderOptimization tab için local logger
        /// Implements IAlgoTraderLogger interface
        /// </summary>
        private class SingleTraderOptLogger : IAlgoTraderLogger, IDisposable
        {
            private readonly RichTextBox _richTextBox;
            private readonly FileSink _fileSink;
            private readonly object _lockObject = new object();
            private bool _isDisposed = false;

            public SingleTraderOptLogger(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
                _fileSink = new FileSink("logs", "singleTraderOptDebug.txt", appendMode: false);
            }

            public void Log(params object[] args)
            {
                WriteLog("INFO", args);
            }

            public void LogWarning(params object[] args)
            {
                WriteLog("WARNING", args);
            }

            public void LogError(params object[] args)
            {
                WriteLog("ERROR", args);
            }

            public void Clear()
            {
                if (_richTextBox.InvokeRequired)
                {
                    _richTextBox.Invoke(() => _richTextBox.Clear());
                }
                else
                {
                    _richTextBox.Clear();
                }

                // Dosyayı da temizle
                _fileSink?.Clear();
            }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    _fileSink?.Dispose();
                }
            }

            private void WriteLog(string level, params object[] args)
            {
                if (args == null || args.Length == 0)
                    return;

                lock (_lockObject)
                {
                    var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                    var message = string.Join(" ", args);
                    var logLine = $"[{timestamp}] [{level}] {message}";

                    // RichTextBox'a yaz
                    if (_richTextBox.InvokeRequired)
                    {
                        _richTextBox.Invoke(() =>
                        {
                            _richTextBox.AppendText(logLine + Environment.NewLine);
                            _richTextBox.SelectionStart = _richTextBox.Text.Length;
                            _richTextBox.ScrollToCaret();
                        });
                    }
                    else
                    {
                        _richTextBox.AppendText(logLine + Environment.NewLine);
                        _richTextBox.SelectionStart = _richTextBox.Text.Length;
                        _richTextBox.ScrollToCaret();
                    }

                    // Dosyaya yaz
                    if (!_isDisposed && _fileSink != null)
                    {
                        var logLevel = level switch
                        {
                            "WARNING" => LogLevel.Warning,
                            "ERROR" => LogLevel.Error,
                            _ => LogLevel.Info
                        };

                        var logEntry = new LogEntry(logLevel, message, "SingleTraderOpt");
                        _fileSink.Write(logEntry);
                    }
                }
            }
        }

        /// <summary>
        /// SingleTraderOptimization tab için local logger'ı başlat (sadece ilk kez)
        /// </summary>
        private void InitializeSingleTraderOptLogger()
        {
            if (_singleTraderOptLogger == null)
            {
                _singleTraderOptLogger = new SingleTraderOptLogger(richTextBoxSingleTraderOptimization);
                _singleTraderOptLogger.Log("=== SingleTraderOpt Logger Initialized ===");
            }
            else
            {
                _singleTraderOptLogger.Clear();
                _singleTraderOptLogger.Log("=== SingleTraderOpt Logger Cleared ===");
            }
        }

        /// <summary>
        /// AlgoTrader ve logger'ı button click'te hazırla
        /// </summary>
        private void PrepareAlgoTraderForRun()
        {
            // Logger'ı temizle veya oluştur
            InitializeSingleTraderLogger();

            // AlgoTrader yoksa oluştur (normalde constructor'da oluşturulmuş olmalı)
            if (algoTrader == null)
            {
                algoTrader = new AlgoTrader();
                _singleTraderLogger?.Log("AlgoTrader instance created");
            }
        }

        // ====================================================================
        // ALGOTRADER - BUTTON EVENT HANDLERS
        // ====================================================================

        /// <summary>
        /// AlgoTrader test butonu click event (ASYNC VERSION)
        /// </summary>
        private async void btnTestAlgoTrader_Click(object sender, EventArgs e)
        {

        }

        private async void btnTestSingleTrader_Click(object sender, EventArgs e)
        {
            // Disable button during execution
            btnTestSingleTrader.Enabled = false;

            try
            {
                // Null check - objeler oluşturulmuş mu?
                if (_singleTraderLogger == null || algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader objeleri oluşturulamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Logger'ı temizle veya oluştur
                InitializeSingleTraderLogger();

                // AlgoTrader zaten initialize edilmişse reset et
                if (algoTrader.IsInitialized)
                {
                    _singleTraderLogger.Log("Resetting existing AlgoTrader...");
                    algoTrader.Reset();
                }

                // Logger'ı AlgoTrader'a tekrar kaydet (reset sonrası gerekli)
                algoTrader.RegisterLogger(_singleTraderLogger);

                _singleTraderLogger.Log("=== AlgoTrader Test Started ===");

                // Stock data kontrolü
                if (stockDataList == null || stockDataList.Count == 0)
                {
                    _singleTraderLogger.LogWarning("Stock data yüklü değil!");
                    MessageBox.Show("Önce stock data yükleyin!", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _singleTraderLogger.Log($"Data loaded: {stockDataList.Count} bars");

                // Initialize with stock data
                algoTrader.Initialize(stockDataList);

                if (algoTrader.IsInitialized)
                {
                    _singleTraderLogger.Log("AlgoTrader initialized with stock data.");
                    _singleTraderLogger.Log(algoTrader.GetDataInfo());
                    _singleTraderLogger.Log("=== AlgoTrader Initialized Successfully ===");
                }
                else
                {
                    _singleTraderLogger.LogError("AlgoTrader initialization failed!");
                    MessageBox.Show("AlgoTrader başlatılamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                // Progress reporter oluştur
                var progress = new Progress<BacktestProgressInfo>(progressInfo =>
                {
                    // UI kontrollerini güvenli şekilde güncelle
                    try
                    {
                        UpdateUIControl(() =>
                        {
                            if (progressBarSingleTrader != null)
                            {
                                progressBarSingleTrader.Value = (int)progressInfo.PercentComplete;
                            }

                            if (lblSingleTraderProgress != null)
                            {
                                // Programı kilitledigi icin simdilik kapalı
                                //lblSingleTraderProgress.Text = $"{progressInfo.CurrentBar}/{progressInfo.TotalBars} - {progressInfo.PercentComplete:F1}%";
                                //lblSingleTraderProgress.Refresh(); // Force immediate redraw for fast updates
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _singleTraderLogger?.LogWarning($"Progress update failed: {ex.Message}");
                    }
                });

                // Run SingleTrader with progress (ASYNC)
                await algoTrader.RunSingleTraderWithProgressAsync(progress);

                if (lblSingleTraderProgress != null)
                {
                    //lblSingleTraderProgress.Text = "Backtest completed!";
                }

                //MessageBox.Show("Backtest tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _singleTraderLogger?.LogError("AlgoTrader test hatası:", ex.Message, ex.StackTrace);
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (lblSingleTraderProgress != null)
                {
                    lblSingleTraderProgress.Text = "Error occurred";
                }
            }
            finally
            {
                // Re-enable button
                btnTestSingleTrader.Enabled = true;
            }
        }
        private async void btnTestMultipleTrader_Click(object sender, EventArgs e)
        {
            // Disable button during execution
            btnTestMultipleTrader.Enabled = false;

            try
            {
                // Null check - objeler oluşturulmuş mu?
                if (_singleTraderLogger == null || algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader objeleri oluşturulamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Logger'ı temizle veya oluştur
                InitializeSingleTraderLogger();

                // AlgoTrader zaten initialize edilmişse reset et
                if (algoTrader.IsInitialized)
                {
                    _singleTraderLogger.Log("Resetting existing AlgoTrader...");
                    algoTrader.Reset();
                }

                // Logger'ı AlgoTrader'a tekrar kaydet (reset sonrası gerekli)
                algoTrader.RegisterLogger(_singleTraderLogger);

                _singleTraderLogger.Log("=== AlgoTrader Test Started ===");

                // Stock data kontrolü
                if (stockDataList == null || stockDataList.Count == 0)
                {
                    _singleTraderLogger.LogWarning("Stock data yüklü değil!");
                    MessageBox.Show("Önce stock data yükleyin!", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _singleTraderLogger.Log($"Data loaded: {stockDataList.Count} bars");

                // Initialize with stock data
                algoTrader.Initialize(stockDataList);

                if (algoTrader.IsInitialized)
                {
                    _singleTraderLogger.Log("AlgoTrader initialized with stock data.");
                    _singleTraderLogger.Log(algoTrader.GetDataInfo());
                    _singleTraderLogger.Log("=== AlgoTrader Initialized Successfully ===");
                }
                else
                {
                    _singleTraderLogger.LogError("AlgoTrader initialization failed!");
                    MessageBox.Show("AlgoTrader başlatılamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Progress reporter oluştur
                var progress = new Progress<BacktestProgressInfo>(progressInfo =>
                {
                    // UI kontrollerini güvenli şekilde güncelle
                    try
                    {
                        UpdateUIControl(() =>
                        {
                            if (progressBarSingleTrader != null)
                            {
                                progressBarSingleTrader.Value = (int)progressInfo.PercentComplete;
                            }

                            if (lblSingleTraderProgress != null)
                            {
                                // Programı kilitledigi icin simdilik kapalı
                                //lblSingleTraderProgress.Text = $"{progressInfo.CurrentBar}/{progressInfo.TotalBars} - {progressInfo.PercentComplete:F1}%";
                                //lblSingleTraderProgress.Refresh(); // Force immediate redraw for fast updates
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _singleTraderLogger?.LogWarning($"Progress update failed: {ex.Message}");
                    }
                });

                // Run MultipleTrader with progress (ASYNC)
                await algoTrader.RunMultipleTraderWithProgressAsync(progress);

                if (lblSingleTraderProgress != null)
                {
                    //lblSingleTraderProgress.Text = "Backtest completed!";
                }

                //MessageBox.Show("Backtest tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _singleTraderLogger?.LogError("AlgoTrader test hatası:", ex.Message, ex.StackTrace);
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (lblSingleTraderProgress != null)
                {
                    lblSingleTraderProgress.Text = "Error occurred";
                }
            }
            finally
            {
                // Re-enable button
                btnTestMultipleTrader.Enabled = true;
            }
        }
        private async void btnStartSingleTraderOpt_Click(object sender, EventArgs e)
        {
            // Disable button during execution
            btnStartSingleTraderOpt.Enabled = false;
            btnStopSingleTraderOpt.Enabled = true;

            try
            {
                // Null check - objeler oluşturulmuş mu?
                if (_singleTraderOptLogger == null || algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader objeleri oluşturulamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Logger'ı temizle veya oluştur
                InitializeSingleTraderOptLogger();

                // AlgoTrader zaten initialize edilmişse reset et
                if (algoTrader.IsInitialized)
                {
                    _singleTraderOptLogger.Log("Resetting existing AlgoTrader...");
                    algoTrader.Reset();
                }

                // Logger'ı AlgoTrader'a tekrar kaydet (reset sonrası gerekli)
                algoTrader.RegisterLogger(_singleTraderOptLogger);

                _singleTraderOptLogger.Log("=== SingleTrader Optimization Started ===");

                // Stock data kontrolü
                if (stockDataList == null || stockDataList.Count == 0)
                {
                    _singleTraderOptLogger.LogWarning("Stock data yüklü değil!");
                    MessageBox.Show("Önce stock data yükleyin!", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _singleTraderOptLogger.Log($"Data loaded: {stockDataList.Count} bars");

                // Initialize with stock data
                algoTrader.Initialize(stockDataList);

                if (algoTrader.IsInitialized)
                {
                    _singleTraderOptLogger.Log("AlgoTrader initialized with stock data.");
                    _singleTraderOptLogger.Log(algoTrader.GetDataInfo());
                    _singleTraderOptLogger.Log("=== AlgoTrader Initialized Successfully ===");
                }
                else
                {
                    _singleTraderOptLogger.LogError("AlgoTrader initialization failed!");
                    MessageBox.Show("AlgoTrader başlatılamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Progress reporter oluştur - Optimization (kombinasyon ilerlemesi)
                var progressOptimization = new Progress<BacktestProgressInfo>(progressInfo =>
                {
                    // UI kontrollerini güvenli şekilde güncelle
                    try
                    {
                        UpdateUIControl(() =>
                        {
                            if (progressBarOptimizationProgress != null)
                            {
                                progressBarOptimizationProgress.Value = (int)progressInfo.PercentComplete;
                            }

                            if (lblOptimizationProgress != null)
                            {
                                lblOptimizationProgress.Text = $"{progressInfo.CurrentBar}/{progressInfo.TotalBars} - {progressInfo.PercentComplete:F1}%";
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _singleTraderOptLogger?.LogWarning($"Optimization progress update failed: {ex.Message}");
                    }
                });

                // Progress reporter oluştur - SingleTrader (bar ilerlemesi)
                var progressSingleTrader = new Progress<BacktestProgressInfo>(progressInfo =>
                {
                    // UI kontrollerini güvenli şekilde güncelle
                    try
                    {
                        UpdateUIControl(() =>
                        {
                            if (progressBarSingleTraderProgress != null)
                            {
                                progressBarSingleTraderProgress.Value = (int)progressInfo.PercentComplete;
                            }

                            if (lblSingleTraderProgress2 != null)
                            {
                                lblSingleTraderProgress2.Text = $"{progressInfo.CurrentBar}/{progressInfo.TotalBars} - {progressInfo.PercentComplete:F1}%";
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _singleTraderOptLogger?.LogWarning($"SingleTrader progress update failed: {ex.Message}");
                    }
                });

                // Run SingleTrader with progress (ASYNC)
                await algoTrader.RunSingleTraderOptWithProgressAsync(progressOptimization, progressSingleTrader);

                if (lblSingleTraderProgress != null)
                {
                    //lblSingleTraderProgress.Text = "Backtest completed!";
                }

                //MessageBox.Show("Backtest tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _singleTraderOptLogger?.LogError("SingleTrader Optimization hatası:", ex.Message, ex.StackTrace);
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (lblSingleTraderProgress != null)
                {
                    lblSingleTraderProgress.Text = "Error occurred";
                }
            }
            finally
            {
                // Re-enable button
                btnStartSingleTraderOpt.Enabled = true;
            }
        }
        private async void btnStopSingleTraderOpt_Click(object sender, EventArgs e)
        {
            btnStopSingleTraderOpt.Enabled = true;

            btnStartSingleTraderOpt.Enabled = true;
        }

        /// <summary>
        /// richTextBoxSingleTraderOptimization double click event handler
        /// Clears the log
        /// </summary>
        private void richTextBoxSingleTraderOptimization_DoubleClick(object sender, EventArgs e)
        {
            _singleTraderOptLogger?.Clear();
        }

        // ====================================================================
        // ALGOTRADER - TEST METODLARI
        // ====================================================================

            /// <summary>
            /// Basit bir strateji ile test
            /// </summary>
        private void TestSimpleStrategy(AlgoTrader algoTrader)
        {
            LogManager.Log("=== Testing Simple MA Strategy ===");

            // Basit bir MA stratejisi oluştur
            var strategy = new SimpleMAStrategy(fastPeriod: 10, slowPeriod: 20);

            // SingleTrader oluştur
            var trader = algoTrader.CreateSingleTrader(strategy);

            LogManager.Log($"Trader created with strategy: {strategy.Name}");
            LogManager.Log("Running backtest...");

            // Backtest çalıştır
            trader.Run(0);

            LogManager.Log("Backtest completed!");

            // Sonuçları göster
            ShowTraderResults(trader);
        }

        /// <summary>
        /// Trader sonuçlarını göster
        /// </summary>
        private void ShowTraderResults(SingleTrader trader)
        {
            var summary = trader.GetStatisticsSummary();

            LogManager.Log(summary);

            MessageBox.Show(summary, "Backtest Sonuçları",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ====================================================================
        // UI THREAD HELPER
        // ====================================================================

        /// <summary>
        /// UI kontrollerini güvenli bir şekilde güncellemek için helper method
        /// Progress callback'leri farklı thread'den geldiği için Invoke gerekiyor
        /// </summary>
        private void UpdateUIControl(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

    }
}
