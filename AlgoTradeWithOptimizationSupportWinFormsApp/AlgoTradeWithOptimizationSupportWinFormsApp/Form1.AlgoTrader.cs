using System;
using System.Windows.Forms;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategies;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Optimizers;

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
        private MultipleTraderLogger? _multipleTraderLogger = null;
        private SingleTraderOptLogger? _singleTraderOptLogger = null;
        private ConfirmingSingleTraderLogger? _confirmingSingleTraderLogger = null;
        private ConfirmingSingleTrader2Logger? _confirmingSingleTrader2Logger = null;
        private ConfirmingMultipleTraderLogger? _confirmingMultipleTraderLogger = null;
        private AlgoTrader? algoTrader = null;

        /// <summary>
        /// AlgoTrader objelerini oluştur
        /// Form constructor'dan çağrılır
        /// </summary>
        private void CreateObjects()
        {
            _singleTraderLogger = new SingleTraderLogger(richTextBoxSingleTrader);
            _multipleTraderLogger = new MultipleTraderLogger(richTextBoxMultipleTrader);
            _singleTraderOptLogger = new SingleTraderOptLogger(richTextBoxSingleTraderOptimization);
            _confirmingSingleTraderLogger = new ConfirmingSingleTraderLogger(richTextBoxConfirmingSingleTrader);

            algoTrader = new AlgoTrader();
            algoTrader.RegisterLogger(_singleTraderLogger);

            // TODO 545 : Optimization results güncelleme callback'ini bağla
            algoTrader.OnOptimizationResultsUpdated = OnOptimizationResultsUpdated;
            algoTrader.OnLastCombinationCompleted = OnLastCombinationCompleted;

            _singleTraderLogger.Log("=== AlgoTrader Objects Created ===");
            _multipleTraderLogger.Log("=== AlgoTrader Objects Created ===");
            _singleTraderOptLogger.Log("=== AlgoTrader Objects Created ===");
            _confirmingSingleTraderLogger.Log("=== AlgoTrader Objects Created ===");

            _confirmingSingleTrader2Logger = new ConfirmingSingleTrader2Logger(richTextBoxConfirmingSingleTrader2);
            _confirmingSingleTrader2Logger.Log("=== AlgoTrader Objects Created ===");

            _confirmingMultipleTraderLogger = new ConfirmingMultipleTraderLogger(richTextBoxConfirmingMultipleTrader);
            _confirmingMultipleTraderLogger.Log("=== AlgoTrader Objects Created ===");

            // ConfirmingSingleTrader Tab - Tetikleyici ComboBox default değer
            cmbTetikleyici.SelectedIndex = 0; // "Both"
            cmbTetikleyici2.SelectedIndex = 0; // "Both"
            cmbEsikTipi2.SelectedIndex = 0; // "Deger"
            cmbTetikleyiciMulti.SelectedIndex = 0; // "Both"
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
        /// Son tamamlanan kombinasyonun bilgilerini lblOptimizationResult'a yazar
        /// </summary>
        private void OnLastCombinationCompleted(int currentCombination, OptimizationResult result)
        {
            // Thread-safe UI update
            UpdateUIControl(() =>
            {
                try
                {
                    // Extract parameters
                    double period = result.Parameters.ContainsKey("period") ? Convert.ToDouble(result.Parameters["period"]) : 0;
                    double percent = result.Parameters.ContainsKey("percent") ? Convert.ToDouble(result.Parameters["percent"]) : 0;

                    // Format label text
                    string labelText = $"{currentCombination} ; " +
                                      $"({period:F0},{percent:F2}) ; " +
                                      $"{result.IlkBakiyeFiyat:F2} ; " +
                                      $"{result.BakiyeFiyat:F2} ; " +
                                      $"{result.GetiriFiyat:F2} ; " +
                                      $"{result.GetiriFiyatYuzde:F2} ; " +
                                      $"{result.KomisyonFiyat:F2} ; " +
                                      $"{result.BakiyeFiyatNet:F2} ; " +
                                      $"NetGetiri : {result.GetiriFiyatNet:F2} ; " +
                                      $"NetGetiri % : {result.GetiriFiyatYuzdeNet:F2} => " +
                                      $"IslemSayisi : {result.IslemSayisi:F0} ; " +
                                      $"ProfitFactor : {result.ProfitFactor:F2} ; " +
                                      $"KarliIslemOrani : {result.KarliIslemOrani:F0} ";

                    lblOptimizationResult.Text = labelText;
                }
                catch (Exception ex)
                {
                    _singleTraderOptLogger?.LogError($"Error updating lblOptimizationResult: {ex.Message}");
                }
            });
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

                    // İlk kolon: Rank (numeric)
                    var rankColumn = new DataGridViewTextBoxColumn();
                    rankColumn.Name = "Rank";
                    rankColumn.HeaderText = "Rank";
                    rankColumn.ValueType = typeof(int);
                    dataGridViewOptimizationResults.Columns.Add(rankColumn);

                    // Geri kalan kolonlar
                    foreach (var header in headers)
                    {
                        var column = new DataGridViewTextBoxColumn();
                        column.Name = header;
                        column.HeaderText = header;

                        // Numeric mi string mi belirle
                        if (IsNumericColumn(header))
                        {
                            column.ValueType = typeof(double);

                            // percent sütunu için özel format (2 ondalık basamak)
                            if (header.Equals("percent", StringComparison.OrdinalIgnoreCase))
                            {
                                column.DefaultCellStyle.Format = "F2";
                            }
                        }
                        else
                        {
                            column.ValueType = typeof(string);
                        }

                        dataGridViewOptimizationResults.Columns.Add(column);
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
                var sortedData = dataDict.Values.OrderByDescending(values =>
                {
                    if (netProfitIndex >= 0 && double.TryParse(values[netProfitIndex], out double netProfit))
                        return netProfit;
                    return double.MinValue;
                }).ToList();

                _singleTraderOptLogger?.Log($"[DEBUG] Data sorted by NetProfit (descending)");

                // ROW LIMIT UYGULA - GUI için
                int rowLimit = algoTrader?.GetOptimizationGuiRowLimit() ?? 5000;
                var limitedData = sortedData.Take(rowLimit);

                _singleTraderOptLogger?.Log($"[DEBUG] Row limit applied: {rowLimit}, Total rows: {sortedData.Count}, Limited to: {limitedData.Count()}");

                // SORTED DOSYAYA YAZ (tüm data)
                WriteSortedOptimizationResults(filePath, headers, sortedData);

                // DataGridView'ı temizle
                dataGridViewOptimizationResults.Rows.Clear();
                _singleTraderOptLogger?.Log($"[DEBUG] Rows cleared.");

                // GUI'ye sadece limit kadar yaz (Rank ile birlikte)
                int rowsAdded = 0;
                int rank = 1;
                foreach (var values in limitedData)
                {
                    // Parse values to correct types
                    var parsedValues = new object[values.Length + 1];
                    parsedValues[0] = rank; // Rank (int)

                    for (int i = 0; i < values.Length; i++)
                    {
                        string header = headers[i];
                        parsedValues[i + 1] = ParseCellValue(header, values[i]);
                    }

                    dataGridViewOptimizationResults.Rows.Add(parsedValues);
                    rowsAdded++;
                    rank++;
                }

                _singleTraderOptLogger?.Log($"[DEBUG] Total rows added to grid: {rowsAdded}");
                _singleTraderOptLogger?.Log($"Loaded {dataGridViewOptimizationResults.Rows.Count} optimization results from CSV (sorted by NetProfit, showing top {rowLimit}).");
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

                    // İlk kolon: Rank (numeric)
                    var rankColumn = new DataGridViewTextBoxColumn();
                    rankColumn.Name = "Rank";
                    rankColumn.HeaderText = "Rank";
                    rankColumn.ValueType = typeof(int);
                    dataGridViewOptimizationResults.Columns.Add(rankColumn);

                    // Geri kalan kolonlar
                    foreach (var header in headers)
                    {
                        var column = new DataGridViewTextBoxColumn();
                        column.Name = header;
                        column.HeaderText = header;

                        // Numeric mi string mi belirle
                        if (IsNumericColumn(header))
                        {
                            column.ValueType = typeof(double);

                            // percent sütunu için özel format (2 ondalık basamak)
                            if (header.Equals("percent", StringComparison.OrdinalIgnoreCase))
                            {
                                column.DefaultCellStyle.Format = "F2";
                            }
                        }
                        else
                        {
                            column.ValueType = typeof(string);
                        }

                        dataGridViewOptimizationResults.Columns.Add(column);
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
                var sortedData = dataDict.Values.OrderByDescending(values =>
                {
                    if (netProfitIndex >= 0 && double.TryParse(values[netProfitIndex], out double netProfit))
                        return netProfit;
                    return double.MinValue;
                }).ToList();

                _singleTraderOptLogger?.Log($"[DEBUG] Data sorted by NetProfit (descending)");

                // ROW LIMIT UYGULA - GUI için
                int rowLimit = algoTrader?.GetOptimizationGuiRowLimit() ?? 5000;
                var limitedData = sortedData.Take(rowLimit);

                _singleTraderOptLogger?.Log($"[DEBUG] Row limit applied: {rowLimit}, Total rows: {sortedData.Count}, Limited to: {limitedData.Count()}");

                // SORTED DOSYAYA YAZ (tüm data)
                WriteSortedOptimizationResultsTxt(filePath, headers, sortedData);

                // DataGridView'ı temizle
                dataGridViewOptimizationResults.Rows.Clear();
                _singleTraderOptLogger?.Log($"[DEBUG] Rows cleared.");

                // GUI'ye sadece limit kadar yaz (Rank ile birlikte)
                int rowsAdded = 0;
                int rank = 1;
                foreach (var values in limitedData)
                {
                    // Parse values to correct types
                    var parsedValues = new object[values.Length + 1];
                    parsedValues[0] = rank; // Rank (int)

                    for (int i = 0; i < values.Length; i++)
                    {
                        string header = headers[i];
                        parsedValues[i + 1] = ParseCellValue(header, values[i]);
                    }

                    dataGridViewOptimizationResults.Rows.Add(parsedValues);
                    rowsAdded++;
                    rank++;
                }

                System.IO.File.AppendAllText(debugFile, $"  ReadTXT: Rows added to grid = {rowsAdded}\n");
                _singleTraderOptLogger?.Log($"[DEBUG] Total rows added to grid: {rowsAdded}");
                _singleTraderOptLogger?.Log($"Loaded {dataGridViewOptimizationResults.Rows.Count} optimization results from TXT (sorted by NetProfit, showing top {rowLimit}).");
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

            _multipleTraderLogger?.Dispose();
            _multipleTraderLogger = null;

            _confirmingSingleTraderLogger?.Dispose();
            _confirmingSingleTraderLogger = null;

            _confirmingMultipleTraderLogger?.Dispose();
            _confirmingMultipleTraderLogger = null;

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
            _multipleTraderLogger?.Clear();
            _confirmingSingleTraderLogger?.Clear();
            _confirmingMultipleTraderLogger?.Clear();

            // AlgoTrader'ı sıfırla
            algoTrader?.Reset();

            _singleTraderLogger?.Log("=== AlgoTrader Objects Reset ===");
            _multipleTraderLogger?.Log("=== AlgoTrader Objects Reset ===");
            _confirmingSingleTraderLogger?.Log("=== AlgoTrader Objects Reset ===");
            _confirmingMultipleTraderLogger?.Log("=== AlgoTrader Objects Reset ===");
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
        /// MultipleTrader için local logger
        /// </summary>
        private class MultipleTraderLogger : IAlgoTraderLogger, IDisposable
        {
            private readonly RichTextBox _richTextBox;
            private readonly FileSink _fileSink;
            private readonly object _lockObject = new object();
            private bool _isDisposed = false;

            public MultipleTraderLogger(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
                _fileSink = new FileSink("logs", "multipleTraderLog.txt", appendMode: false);
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

                        var logEntry = new LogEntry(logLevel, message, "MultipleTrader");
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
        /// MultipleTrader tab için local logger'ı başlat (sadece ilk kez)
        /// </summary>
        private void InitializeMultipleTraderLogger()
        {
            if (_multipleTraderLogger == null)
            {
                _multipleTraderLogger = new MultipleTraderLogger(richTextBoxMultipleTrader);
                _multipleTraderLogger.Log("=== MultipleTrader Logger Initialized ===");
            }
            else
            {
                _multipleTraderLogger.Clear();
                _multipleTraderLogger.Log("=== MultipleTrader Logger Cleared ===");
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

        private async void btnStartSingleTrader_Click(object sender, EventArgs e)
        {
            // Disable button during execution
            btnStartSingleTrader.Enabled = false;
            btnStopSingleTrader.Enabled = true;

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

                // Configure strategy from GUI selection
                if (_selectedStrategyConfig == null)
                {
                    _singleTraderLogger.LogWarning("No strategy selected! Please select a strategy from the dropdown.");
                    MessageBox.Show("Lütfen bir strateji seçin!", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _singleTraderLogger.Log($"Selected Strategy: {_selectedStrategyConfig.StrategyName} ({_selectedStrategyConfig.Version})");
                _singleTraderLogger.Log($"Parameters: {_selectedStrategyConfig.GetParametersDisplayString()}");

                // Configure strategy before initialization
                algoTrader.ConfigureStrategy(
                    _selectedStrategyConfig.StrategyName,
                    _selectedStrategyConfig.GetParameterValues()
                );

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

                // Python ile grafik çizdirme (opsiyonel)
                try
                {
                    _singleTraderLogger.Log("Çizim için Python çağrılıyor...");

                    // Kullanıcıya grafik çizdirme seçeneği sun
                    string msg = "Backtest tamamlandı!\n\n" +
                                 "ImGui/ImPlot ile 5 panelli interaktif grafik çizdirmek ister misiniz?\n\n" +
                                 "Paneller:\n" +
                                 "  • Panel 0: OHLC Candlestick (Price Chart)\n" +
                                 "  • Panel 1: Trade Signals (-1/0/+1)\n" +
                                 "  • Panel 2: PnL (Kar/Zarar)\n" +
                                 "  • Panel 3: Balance (Brüt/Net Getiri)\n" +
                                 "  • Panel 4: Volume\n\n" +
                                 "NOT: pip install imgui-bundle gereklidir";
                    _singleTraderLogger.Log("\n" + msg + "\n");
                    /*
                    var result = MessageBox.Show(
                       msg,
                       "ImGui Grafik Çizdirme",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        // Task.Run ile UI bloğunu önle
                    }
                    */
                    // Task.Run ile UI bloğunu önle
                    await Task.Run(() =>
                    {
                        // ImGui/ImPlot ile 5 panelli grafik
                        algoTrader.PlotImGuiBundle(algoTrader.singleTrader);
                    });

                    _singleTraderLogger.Log("✓ ImGui grafik başarıyla çizdirildi!");
                }
                catch (Exception plotEx)
                {
                    _singleTraderLogger.LogWarning($"Grafik çizimi hatası: {plotEx.Message}");
                    MessageBox.Show(
                        $"Grafik çiziminde hata:\n{plotEx.Message}\n\n" +
                        "Python kurulumunu ve imgui-bundle paketini kontrol edin:\n\n" +
                        "pip install imgui-bundle\n\n" +
                        "Eğer imgui-bundle yüklüyse, Python DLL yolunu kontrol edin.",
                        "ImGui Plotting Hatası",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
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
                btnStartSingleTrader.Enabled = true;
                btnStopSingleTrader.Enabled = false;
            }
        }
        private void btnStopSingleTrader_Click(object sender, EventArgs e)
        {
            // Run stop logic in background to avoid logger deadlock
            Task.Run(() =>
            {
                _singleTraderLogger?.Log("Stop button clicked - requesting SingleTrader stop...");

                // Stop SingleTrader if running
                if (algoTrader?.singleTrader != null)
                {
                    if (algoTrader.singleTrader.IsRunning)
                    {
                        algoTrader.singleTrader.Stop();
                        _singleTraderLogger?.Log("Stop request sent to SingleTrader");
                    }
                    else
                    {
                        _singleTraderLogger?.LogWarning("SingleTrader is not running");
                    }
                }
                else
                {
                    _singleTraderLogger?.LogWarning("SingleTrader instance not found");
                }
            });

            // Disable stop button, enable start button (UI thread)
            btnStopSingleTrader.Enabled = false;
            btnStartSingleTrader.Enabled = true;
        }

        private async void btnStartMultipleTrader_Click(object sender, EventArgs e)
        {
            // Disable button during execution
            btnStartMultipleTrader.Enabled = false;
            btnStopMultipleTrader.Enabled = true;

            try
            {
                // Null check - objeler oluşturulmuş mu?
                if (_multipleTraderLogger == null || algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader objeleri oluşturulamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Logger'ı temizle veya oluştur
                InitializeMultipleTraderLogger();

                // AlgoTrader zaten initialize edilmişse reset et
                if (algoTrader.IsInitialized)
                {
                    _multipleTraderLogger.Log("Resetting existing AlgoTrader...");
                    algoTrader.Reset();
                }

                // Logger'ı AlgoTrader'a tekrar kaydet (reset sonrası gerekli)
                algoTrader.RegisterLogger(_multipleTraderLogger);

                _multipleTraderLogger.Log("=== AlgoTrader Test Started ===");

                // Stock data kontrolü
                if (stockDataList == null || stockDataList.Count == 0)
                {
                    _multipleTraderLogger.LogWarning("Stock data yüklü değil!");
                    MessageBox.Show("Önce stock data yükleyin!", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _multipleTraderLogger.Log($"Data loaded: {stockDataList.Count} bars");

                // Initialize with stock data
                algoTrader.Initialize(stockDataList);

                if (algoTrader.IsInitialized)
                {
                    _multipleTraderLogger.Log("AlgoTrader initialized with stock data.");
                    _multipleTraderLogger.Log(algoTrader.GetDataInfo());
                    _multipleTraderLogger.Log("=== AlgoTrader Initialized Successfully ===");
                }
                else
                {
                    _multipleTraderLogger.LogError("AlgoTrader initialization failed!");
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
                            if (progressBarMultipleTrader != null)
                            {
                                progressBarMultipleTrader.Value = (int)progressInfo.PercentComplete;
                            }

                            if (label5 != null)
                            {
                                label5.Text = $"{progressInfo.CurrentBar}/{progressInfo.TotalBars} - {progressInfo.PercentComplete:F1}%";
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _multipleTraderLogger?.LogWarning($"Progress update failed: {ex.Message}");
                    }
                });

                // Run MultipleTrader with progress (ASYNC)
                await algoTrader.RunMultipleTraderWithProgressAsync(progress);

                if (label5 != null)
                {
                    label5.Text = "Backtest completed!";
                }

                // Python ile grafik çizdirme (opsiyonel)
                try
                {
                    // Null check
                    if (algoTrader.multipleTrader == null)
                    {
                        _multipleTraderLogger.LogWarning("MultipleTrader is null - cannot plot");
                    }
                    else
                    {
                        _multipleTraderLogger.Log("Çizim için Python çağrılıyor...");

                        // Kullanıcıya grafik çizdirme seçeneği sun
                        string msg = "Backtest tamamlandı!\n\n" +
                                     "ImGui/ImPlot ile 5 panelli interaktif grafik çizdirmek ister misiniz?\n\n" +
                                     "Paneller:\n" +
                                     "  • Panel 0: OHLC Candlestick (Price Chart)\n" +
                                     "  • Panel 1: Trade Signals (-1/0/+1)\n" +
                                     "  • Panel 2: PnL (Kar/Zarar)\n" +
                                     "  • Panel 3: Balance (Brüt/Net Getiri)\n" +
                                     "NOT: pip install imgui-bundle gereklidir";
                        _multipleTraderLogger.Log("\n" + msg + "\n");

                        // Task.Run ile UI bloğunu önle
                        await Task.Run(() =>
                        {
                            // MultipleTrader'ın mainTrader'ını kullanarak çizdir
                            var mainTrader = algoTrader.multipleTrader.GetMainTrader();
                            algoTrader.PlotImGuiBundle(mainTrader);
                        });

                        _multipleTraderLogger.Log("✓ ImGui grafik başarıyla çizdirildi!");
                    }
                }
                catch (Exception plotEx)
                {
                    _multipleTraderLogger.LogWarning($"Grafik çizimi hatası: {plotEx.Message}");
                    MessageBox.Show(
                        $"Grafik çiziminde hata:\n{plotEx.Message}\n\n" +
                        "Python kurulumunu ve imgui-bundle paketini kontrol edin:\n\n" +
                        "pip install imgui-bundle\n\n" +
                        "Eğer imgui-bundle yüklüyse, Python DLL yolunu kontrol edin.",
                        "ImGui Plotting Hatası",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }

                //MessageBox.Show("Backtest tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _multipleTraderLogger?.LogError("AlgoTrader test hatası:", ex.Message, ex.StackTrace);
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (label5 != null)
                {
                    label5.Text = "Error occurred";
                }
            }
            finally
            {
                // Re-enable button
                btnStartMultipleTrader.Enabled = true;
                btnStopMultipleTrader.Enabled = false;
            }
        }
        private void btnStopMultipleTrader_Click(object sender, EventArgs e)
        {
            // Run stop logic in background to avoid logger deadlock
            Task.Run(() =>
            {
                _multipleTraderLogger?.Log("Stop button clicked - requesting MultipleTrader stop...");

                // Stop MultipleTrader if running
                if (algoTrader?.multipleTrader != null)
                {
                    if (algoTrader.multipleTrader.IsRunning)
                    {
                        algoTrader.multipleTrader.Stop();
                        _multipleTraderLogger?.Log("Stop request sent to MultipleTrader");
                    }
                    else
                    {
                        _multipleTraderLogger?.LogWarning("MultipleTrader is not running");
                    }
                }
                else
                {
                    _multipleTraderLogger?.LogWarning("MultipleTrader instance not found");
                }
            });

            // Disable stop button, enable start button (UI thread)
            btnStopMultipleTrader.Enabled = false;
            btnStartMultipleTrader.Enabled = true;
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

                // Grid'i temizle
                dataGridViewOptimizationResults.Rows.Clear();
                _singleTraderOptLogger?.Log("Optimization results grid cleared");

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

                // ============================================================
                // PERFORMANS OPTİMİZASYONU AYARLARI
                // ============================================================

                // Callback throttle - Her kaç kombinasyonda bir GUI güncellensin?
                // Düşük değer: Daha sık güncelleme, daha yavaş (örn: 100)
                // Yüksek değer: Daha az güncelleme, daha hızlı (örn: 5000)
                algoTrader.SetOptimizationCallbackThrottle(1);  // Her 100 kombinasyonda bir güncelle

                // GUI row limit - GUI'de max kaç satır gösterilsin?
                // Yüksek değer daha fazla satır gösterir ama performans düşer
                algoTrader.SetOptimizationGuiRowLimit(5000);  // Max 5000 satır göster

                _singleTraderOptLogger.Log($"Throttle: {algoTrader.GetOptimizationCallbackThrottle()}, Row limit: {algoTrader.GetOptimizationGuiRowLimit()}");

                // ============================================================
                // SKIP ITERATION VE MAX ITERATION AYARLARI
                // ============================================================

                // TextBox'lardan değerleri oku
                int skipIteration = -1;
                int maxIteration = -1;

                if (int.TryParse(txtSkipIteration.Text, out int skipValue))
                {
                    skipIteration = skipValue;
                }

                if (int.TryParse(txtMaxIteration.Text, out int maxValue))
                {
                    maxIteration = maxValue;
                }

                // AlgoTrader'a değerleri set et (RunSingleTraderOptWithProgressAsync içinde kullanılacak)
                algoTrader.SetSkipIterationValue(skipIteration);
                algoTrader.SetMaxIterationValue(maxIteration);

                _singleTraderOptLogger.Log($"Skip Iteration set to: {skipIteration} (GUI input)");
                _singleTraderOptLogger.Log($"Max Iteration set to: {maxIteration} (GUI input)");

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
        private void btnStopSingleTraderOpt_Click(object sender, EventArgs e)
        {
            _singleTraderOptLogger?.Log("Stop button clicked - requesting optimization stop...");

            // Stop optimization if running
            if (algoTrader?.singleTraderOptimizer != null)
            {
                if (algoTrader.singleTraderOptimizer.IsRunning)
                {
                    algoTrader.singleTraderOptimizer.Stop();
                    _singleTraderOptLogger?.Log("Stop request sent to optimizer");
                }
                else
                {
                    _singleTraderOptLogger?.LogWarning("Optimizer is not running");
                }
            }
            else
            {
                _singleTraderOptLogger?.LogWarning("Optimizer instance not found");
            }

            // Disable stop button, enable start button
            btnStopSingleTraderOpt.Enabled = false;
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

        // ====================================================================
        // OPTIMIZATION RESULTS - HELPER METHODS
        // ====================================================================

        /// <summary>
        /// Get sorted file path from original file path
        /// Adds "_sorted" suffix before extension
        /// Example: "logs\\singleTraderOptLog.txt" -> "logs\\singleTraderOptLog_sorted.txt"
        /// </summary>
        private string GetSortedFilePath(string originalFilePath)
        {
            string directory = System.IO.Path.GetDirectoryName(originalFilePath) ?? string.Empty;
            string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(originalFilePath);
            string extension = System.IO.Path.GetExtension(originalFilePath);

            return System.IO.Path.Combine(directory, $"{fileNameWithoutExt}_sorted{extension}");
        }

        /// <summary>
        /// Get column width for tabular TXT format based on header name
        /// Follows the same width rules as AppendSingleOptSummaryToTxt
        /// </summary>
        private int GetColumnWidth(string headerName)
        {
            // Special cases
            if (headerName == "CombNo") return 10;
            if (headerName == "OptResult" || headerName == "OptSummary") return 20;

            // Name columns
            if (headerName.Contains("Name") || headerName.Contains("Strat")) return 20;

            // DateTime columns
            if (headerName.Contains("DT") || headerName.Contains("Time") || headerName.Contains("Date")) return 20;

            // Financial/numeric columns (larger numbers)
            if (headerName.Contains("Fiyat") || headerName.Contains("Fyt") ||
                headerName.Contains("Puan") || headerName.Contains("Pua") ||
                headerName.Contains("Kar") || headerName.Contains("Zar") ||
                headerName.Contains("Profit") || headerName.Contains("Loss") ||
                headerName.Contains("Bak") || headerName.Contains("Get") ||
                headerName.Contains("Win") || headerName.Contains("Avg")) return 15;

            // Default width
            return 10;
        }

        /// <summary>
        /// Determine if a column should be numeric based on header name
        /// Returns true if the column contains numeric data (for proper sorting)
        /// </summary>
        private bool IsNumericColumn(string headerName)
        {
            // Explicit numeric columns
            if (headerName == "Rank") return true;
            if (headerName == "CombNo") return true;

            // ID columns
            if (headerName.Contains("Id") || headerName.Contains("ID")) return true;

            // Count/Sayı columns
            if (headerName.Contains("Sayi") || headerName.Contains("Count") || headerName.Contains("Toplam")) return true;

            // Financial/numeric columns
            if (headerName.Contains("Fiyat") || headerName.Contains("Fyt") ||
                headerName.Contains("Puan") || headerName.Contains("Pua") ||
                headerName.Contains("Kar") || headerName.Contains("Zar") ||
                headerName.Contains("Profit") || headerName.Contains("Loss") ||
                headerName.Contains("Bak") || headerName.Contains("Get") ||
                headerName.Contains("Komisyon") || headerName.Contains("Net")) return true;

            // Trading metrics
            if (headerName.Contains("Win") || headerName.Contains("Kazanc") ||
                headerName.Contains("Kayip") || headerName.Contains("Avg") ||
                headerName.Contains("Max") || headerName.Contains("Min") ||
                headerName.Contains("PF") || headerName.Contains("Factor") ||
                headerName.Contains("Ratio") || headerName.Contains("Rate") ||
                headerName.Contains("Percent") || headerName.Contains("Yuzde")) return true;

            // Drawdown columns
            if (headerName.Contains("DD") || headerName.Contains("Drawdown")) return true;

            // Period/multiplier parameters
            if (headerName.Contains("Period") || headerName.Contains("Multiplier") ||
                headerName.Contains("period") || headerName.Contains("multiplier")) return true;

            // Default: string
            return false;
        }

        /// <summary>
        /// Parse cell value to correct type based on column header
        /// Returns numeric value if column is numeric, otherwise returns string
        /// </summary>
        private object ParseCellValue(string headerName, string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Rank özel durum - integer
            if (headerName == "Rank")
            {
                if (int.TryParse(value, out int intVal))
                    return intVal;
                return value;
            }

            // Numeric sütunlar - double
            if (IsNumericColumn(headerName))
            {
                if (double.TryParse(value, out double doubleVal))
                {
                    // percent sütunu için floating point precision sorununu çöz
                    if (headerName.Equals("percent", StringComparison.OrdinalIgnoreCase))
                    {
                        return Math.Round(doubleVal, 2);
                    }
                    return doubleVal;
                }
                return value; // Parse edilemezse string olarak dön
            }

            // String sütunlar
            return value;
        }

        /// <summary>
        /// Write sorted optimization results to CSV file
        /// Tüm veri sorted dosyaya yazılır (row limit uygulanmaz)
        /// </summary>
        private void WriteSortedOptimizationResults(string originalFilePath, string[] headers, IEnumerable<string[]> sortedData)
        {
            try
            {
                // Get sorted file path
                string sortedFilePath = GetSortedFilePath(originalFilePath);

                _singleTraderOptLogger?.Log($"[DEBUG] Writing sorted CSV to: {sortedFilePath}");

                using (var sw = new System.IO.StreamWriter(sortedFilePath, false, System.Text.Encoding.UTF8))
                {
                    // Header yaz
                    sw.WriteLine(string.Join(",", headers));

                    // Sorted data yaz
                    foreach (var values in sortedData)
                    {
                        sw.WriteLine(string.Join(",", values));
                    }
                }

                _singleTraderOptLogger?.Log($"Sorted CSV file written: {sortedFilePath}");
            }
            catch (Exception ex)
            {
                // Hata logla ama UI'yi bloke etme
                _singleTraderOptLogger?.LogWarning($"Sorted CSV file write error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Sorted file write error: {ex.Message}");
            }
        }

        /// <summary>
        /// Write sorted optimization results to TXT file (tabular format with fixed-width columns)
        /// Tüm veri sorted dosyaya yazılır (row limit uygulanmaz)
        /// </summary>
        private void WriteSortedOptimizationResultsTxt(string originalFilePath, string[] headerValues, IEnumerable<string[]> sortedData)
        {
            try
            {
                // Get sorted file path
                string sortedFilePath = GetSortedFilePath(originalFilePath);

                _singleTraderOptLogger?.Log($"[DEBUG] Writing sorted TXT to: {sortedFilePath}");

                using (var sw = new System.IO.StreamWriter(sortedFilePath, false, System.Text.Encoding.UTF8))
                {
                    // Title satırı
                    sw.WriteLine($"OPTIMIZATION RESULTS (SORTED BY NETPROFIT) - {DateTime.Now:yyyy.MM.dd HH:mm:ss}");
                    sw.WriteLine("".PadRight(3000, '='));

                    // Header satırını tabular format ile yaz
                    var headerBuilder = new System.Text.StringBuilder();

                    foreach (var header in headerValues)
                    {
                        int width = GetColumnWidth(header);
                        string paddedHeader = header.PadLeft(width);
                        headerBuilder.Append(paddedHeader + " | ");
                    }

                    sw.WriteLine(headerBuilder.ToString().TrimEnd(' ', '|'));
                    sw.WriteLine("".PadRight(3000, '-'));

                    // Sorted data satırlarını tabular format ile yaz
                    foreach (var values in sortedData)
                    {
                        var rowBuilder = new System.Text.StringBuilder();

                        for (int i = 0; i < values.Length && i < headerValues.Length; i++)
                        {
                            int width = GetColumnWidth(headerValues[i]);
                            string paddedValue = values[i].PadLeft(width);
                            rowBuilder.Append(paddedValue + " | ");
                        }

                        sw.WriteLine(rowBuilder.ToString().TrimEnd(' ', '|'));
                    }
                }

                _singleTraderOptLogger?.Log($"Sorted TXT file written: {sortedFilePath}");
            }
            catch (Exception ex)
            {
                // Hata logla ama UI'yi bloke etme
                _singleTraderOptLogger?.LogWarning($"Sorted TXT file write error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Sorted file write error: {ex.Message}");
            }
        }

        // ====================================================================
        // PLOT BUTTONS - Re-plot last run data using Python ImGui
        // ====================================================================

        /// <summary>
        /// Plot SingleTrader data button click event
        /// Re-plots the last SingleTrader run using Python ImGui/ImPlot
        /// </summary>
        private async void btnPlotSingleTraderData_Click(object sender, EventArgs e)
        {
            try
            {
                // Null check - AlgoTrader ve singleTrader oluşturulmuş mu?
                if (algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader instance yok!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (algoTrader.singleTrader == null)
                {
                    MessageBox.Show("SingleTrader verisi yok!\n\nÖnce SingleTrader'ı çalıştırın.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _singleTraderLogger?.Log("");
                _singleTraderLogger?.Log("Re-plotting SingleTrader data with Python ImGui...");

                // Task.Run ile UI bloğunu önle
                await Task.Run(() =>
                {
                    // ImGui/ImPlot ile 5 panelli grafik
                    algoTrader.PlotImGuiBundle(algoTrader.singleTrader);
                });

                _singleTraderLogger?.Log("✓ SingleTrader grafik başarıyla çizdirildi!");
            }
            catch (Exception ex)
            {
                _singleTraderLogger?.LogError($"Grafik çizimi hatası: {ex.Message}");
                MessageBox.Show(
                    $"Grafik çiziminde hata:\n{ex.Message}\n\n" +
                    "Python kurulumunu ve imgui-bundle paketini kontrol edin:\n\n" +
                    "pip install imgui-bundle",
                    "ImGui Plotting Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // ====================================================================
        // CONFIRMING SINGLE TRADER - BUTTON EVENT HANDLERS
        // ====================================================================

        private async void btnStartConfirmingSingleTrader_Click(object sender, EventArgs e)
        {
            // Disable button during execution
            btnStartConfirmingSingleTrader.Enabled = false;
            btnStopConfirmingSingleTrader.Enabled = true;

            try
            {
                // Null check - objeler oluşturulmuş mu?
                if (_confirmingSingleTraderLogger == null || algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader objeleri oluşturulamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Logger'ı temizle veya oluştur
                InitializeConfirmingSingleTraderLogger();

                // AlgoTrader zaten initialize edilmişse reset et
                if (algoTrader.IsInitialized)
                {
                    _confirmingSingleTraderLogger.Log("Resetting existing AlgoTrader...");
                    algoTrader.Reset();
                }

                // Logger'ı AlgoTrader'a tekrar kaydet (reset sonrası gerekli)
                algoTrader.RegisterLogger(_confirmingSingleTraderLogger);

                _confirmingSingleTraderLogger.Log("=== ConfirmingSingleTrader Test Started ===");

                // Stock data kontrolü
                if (stockDataList == null || stockDataList.Count == 0)
                {
                    _confirmingSingleTraderLogger.LogWarning("Stock data yüklü değil!");
                    MessageBox.Show("Önce stock data yükleyin!", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _confirmingSingleTraderLogger.Log($"Data loaded: {stockDataList.Count} bars");

                // Initialize with stock data
                algoTrader.Initialize(stockDataList);

                if (algoTrader.IsInitialized)
                {
                    _confirmingSingleTraderLogger.Log("AlgoTrader initialized with stock data.");
                    _confirmingSingleTraderLogger.Log(algoTrader.GetDataInfo());
                    _confirmingSingleTraderLogger.Log("=== AlgoTrader Initialized Successfully ===");
                }
                else
                {
                    _confirmingSingleTraderLogger.LogError("AlgoTrader initialization failed!");
                    MessageBox.Show("AlgoTrader başlatılamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ═══════════════════════════════════════════════════════════════
                // CONFIRMATION MODE AYARLARINI OKU
                // ═══════════════════════════════════════════════════════════════
                bool confirmationEnabled = chkConfirmationModeEnabled.Checked;
                double karEsigi = 10.0;
                double zararEsigi = 5.0;
                string tetikleyici = cmbTetikleyici.SelectedItem?.ToString() ?? "Both";

                if (double.TryParse(txtKarEsigi.Text, out double karVal))
                    karEsigi = karVal;

                if (double.TryParse(txtZararEsigi.Text, out double zararVal))
                    zararEsigi = zararVal;

                _confirmingSingleTraderLogger.Log($"Confirmation Mode: {confirmationEnabled}");
                _confirmingSingleTraderLogger.Log($"Kar Esigi: {karEsigi}, Zarar Esigi: {zararEsigi}, Tetikleyici: {tetikleyici}");

                // Confirmation Mode ayarlarını parse et
                ConfirmationTrigger trigger = tetikleyici switch
                {
                    "KarOnly" => ConfirmationTrigger.KarOnly,
                    "ZararOnly" => ConfirmationTrigger.ZararOnly,
                    "Both" => ConfirmationTrigger.Both,
                    _ => ConfirmationTrigger.Both
                };

                _confirmingSingleTraderLogger.Log($"Parsed Trigger: {trigger}");

                // AlgoTrader'a Confirmation Mode ayarlarını gönder
                algoTrader.ConfigureConfirmationMode(confirmationEnabled, karEsigi, zararEsigi, trigger);

                // Progress reporter oluştur
                var progress = new Progress<BacktestProgressInfo>(progressInfo =>
                {
                    // UI kontrollerini güvenli şekilde güncelle
                    try
                    {
                        UpdateUIControl(() =>
                        {
                            if (progressBarConfirmingSingleTrader != null)
                            {
                                progressBarConfirmingSingleTrader.Value = (int)progressInfo.PercentComplete;
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _confirmingSingleTraderLogger?.LogWarning($"Progress update failed: {ex.Message}");
                    }
                });

                // Run SingleTrader with progress (ASYNC)
                await algoTrader.RunSingleTraderWithProgressAsync(progress);

                // Python ile grafik çizdirme (opsiyonel)
                try
                {
                    _confirmingSingleTraderLogger.Log("Çizim için Python çağrılıyor...");

                    // Task.Run ile UI bloğunu önle
                    await Task.Run(() =>
                    {
                        // ImGui/ImPlot ile 5 panelli grafik
                        algoTrader.PlotImGuiBundle(algoTrader.singleTrader);
                    });

                    _confirmingSingleTraderLogger.Log("✓ ImGui grafik başarıyla çizdirildi!");
                }
                catch (Exception plotEx)
                {
                    _confirmingSingleTraderLogger.LogWarning($"Grafik çizimi hatası: {plotEx.Message}");
                }
            }
            catch (Exception ex)
            {
                _confirmingSingleTraderLogger?.LogError("ConfirmingSingleTrader test hatası:", ex.Message, ex.StackTrace);
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable button
                btnStartConfirmingSingleTrader.Enabled = true;
                btnStopConfirmingSingleTrader.Enabled = false;
            }
        }

        private void btnStopConfirmingSingleTrader_Click(object sender, EventArgs e)
        {
            // Run stop logic in background to avoid logger deadlock
            Task.Run(() =>
            {
                _confirmingSingleTraderLogger?.Log("Stop button clicked - requesting ConfirmingSingleTrader stop...");

                // Stop SingleTrader if running
                if (algoTrader?.singleTrader != null)
                {
                    if (algoTrader.singleTrader.IsRunning)
                    {
                        algoTrader.singleTrader.Stop();
                        _confirmingSingleTraderLogger?.Log("Stop request sent to SingleTrader");
                    }
                    else
                    {
                        _confirmingSingleTraderLogger?.LogWarning("SingleTrader is not running");
                    }
                }
                else
                {
                    _confirmingSingleTraderLogger?.LogWarning("SingleTrader instance not found");
                }
            });

            // Disable stop button, enable start button (UI thread)
            btnStopConfirmingSingleTrader.Enabled = false;
            btnStartConfirmingSingleTrader.Enabled = true;
        }

        private async void btnPlotConfirmingSingleTraderData_Click(object sender, EventArgs e)
        {
            try
            {
                // Null check - AlgoTrader ve singleTrader oluşturulmuş mu?
                if (algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader instance yok!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (algoTrader.singleTrader == null)
                {
                    MessageBox.Show("SingleTrader verisi yok!\n\nÖnce ConfirmingSingleTrader'ı çalıştırın.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _confirmingSingleTraderLogger?.Log("");
                _confirmingSingleTraderLogger?.Log("Re-plotting ConfirmingSingleTrader data with Python ImGui...");

                // Task.Run ile UI bloğunu önle
                await Task.Run(() =>
                {
                    // ImGui/ImPlot ile 5 panelli grafik
                    algoTrader.PlotImGuiBundle(algoTrader.singleTrader);
                });

                _confirmingSingleTraderLogger?.Log("✓ ConfirmingSingleTrader grafik başarıyla çizdirildi!");
            }
            catch (Exception ex)
            {
                _confirmingSingleTraderLogger?.LogError($"Grafik çizimi hatası: {ex.Message}");
                MessageBox.Show(
                    $"Grafik çiziminde hata:\n{ex.Message}\n\n" +
                    "Python kurulumunu ve imgui-bundle paketini kontrol edin:\n\n" +
                    "pip install imgui-bundle",
                    "ImGui Plotting Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// ConfirmingSingleTrader tab için local logger'ı başlat (sadece ilk kez)
        /// </summary>
        private void InitializeConfirmingSingleTraderLogger()
        {
            if (_confirmingSingleTraderLogger == null)
            {
                _confirmingSingleTraderLogger = new ConfirmingSingleTraderLogger(richTextBoxConfirmingSingleTrader);
                _confirmingSingleTraderLogger.Log("=== ConfirmingSingleTrader Logger Initialized ===");
            }
            else
            {
                _confirmingSingleTraderLogger.Clear();
                _confirmingSingleTraderLogger.Log("=== ConfirmingSingleTrader Logger Cleared ===");
            }
        }

        /// <summary>
        /// ConfirmingSingleTrader tab için local logger
        /// Implements IAlgoTraderLogger interface
        /// </summary>
        private class ConfirmingSingleTraderLogger : IAlgoTraderLogger, IDisposable
        {
            private readonly RichTextBox _richTextBox;
            private readonly FileSink _fileSink;
            private readonly object _lockObject = new object();
            private bool _isDisposed = false;

            public ConfirmingSingleTraderLogger(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
                _fileSink = new FileSink("logs", "confirmingSingleTraderLog.txt", appendMode: false);
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

                        var logEntry = new LogEntry(logLevel, message, "ConfirmingSingleTrader");
                        _fileSink.Write(logEntry);
                    }
                }
            }
        }

        // ====================================================================
        // CONFIRMING MULTIPLE TRADER - BUTTON EVENT HANDLERS
        // ====================================================================

        private async void btnStartConfirmingMultipleTrader_Click(object sender, EventArgs e)
        {
            // Disable button during execution
            btnStartConfirmingMultipleTrader.Enabled = false;
            btnStopConfirmingMultipleTrader.Enabled = true;

            try
            {
                // Null check - objeler oluşturulmuş mu?
                if (_confirmingMultipleTraderLogger == null || algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader objeleri oluşturulamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Logger'ı temizle veya oluştur
                InitializeConfirmingMultipleTraderLogger();

                // AlgoTrader zaten initialize edilmişse reset et
                if (algoTrader.IsInitialized)
                {
                    _confirmingMultipleTraderLogger.Log("Resetting existing AlgoTrader...");
                    algoTrader.Reset();
                }

                // Logger'ı AlgoTrader'a tekrar kaydet (reset sonrası gerekli)
                algoTrader.RegisterLogger(_confirmingMultipleTraderLogger);

                _confirmingMultipleTraderLogger.Log("=== ConfirmingMultipleTrader Test Started ===");

                // Stock data kontrolü
                if (stockDataList == null || stockDataList.Count == 0)
                {
                    _confirmingMultipleTraderLogger.LogWarning("Stock data yüklü değil!");
                    MessageBox.Show("Önce stock data yükleyin!", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _confirmingMultipleTraderLogger.Log($"Data loaded: {stockDataList.Count} bars");

                // Initialize with stock data
                algoTrader.Initialize(stockDataList);

                if (algoTrader.IsInitialized)
                {
                    _confirmingMultipleTraderLogger.Log("AlgoTrader initialized with stock data.");
                    _confirmingMultipleTraderLogger.Log(algoTrader.GetDataInfo());
                    _confirmingMultipleTraderLogger.Log("=== AlgoTrader Initialized Successfully ===");
                }
                else
                {
                    _confirmingMultipleTraderLogger.LogError("AlgoTrader initialization failed!");
                    MessageBox.Show("AlgoTrader başlatılamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ═══════════════════════════════════════════════════════════════
                // CONFIRMATION MODE AYARLARINI OKU
                // ═══════════════════════════════════════════════════════════════
                bool confirmationEnabled = chkConfirmationModeEnabledMulti.Checked;
                double karEsigi = 10.0;
                double zararEsigi = 5.0;
                string tetikleyici = cmbTetikleyiciMulti.SelectedItem?.ToString() ?? "Both";

                if (double.TryParse(txtKarEsigiMulti.Text, out double karVal))
                    karEsigi = karVal;

                if (double.TryParse(txtZararEsigiMulti.Text, out double zararVal))
                    zararEsigi = zararVal;

                _confirmingMultipleTraderLogger.Log($"Confirmation Mode: {confirmationEnabled}");
                _confirmingMultipleTraderLogger.Log($"Kar Esigi: {karEsigi}, Zarar Esigi: {zararEsigi}, Tetikleyici: {tetikleyici}");

                // Confirmation Mode ayarlarını parse et
                ConfirmationTrigger trigger = tetikleyici switch
                {
                    "KarOnly" => ConfirmationTrigger.KarOnly,
                    "ZararOnly" => ConfirmationTrigger.ZararOnly,
                    "Both" => ConfirmationTrigger.Both,
                    _ => ConfirmationTrigger.Both
                };

                _confirmingMultipleTraderLogger.Log($"Parsed Trigger: {trigger}");

                // AlgoTrader'a Confirmation Mode ayarlarını gönder (her SingleTrader'a uygulanacak)
                algoTrader.ConfigureConfirmationMode(confirmationEnabled, karEsigi, zararEsigi, trigger);

                // Progress reporter oluştur
                var progress = new Progress<BacktestProgressInfo>(progressInfo =>
                {
                    // UI kontrollerini güvenli şekilde güncelle
                    try
                    {
                        UpdateUIControl(() =>
                        {
                            if (progressBarConfirmingMultipleTrader != null)
                            {
                                progressBarConfirmingMultipleTrader.Value = (int)progressInfo.PercentComplete;
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _confirmingMultipleTraderLogger?.LogWarning($"Progress update failed: {ex.Message}");
                    }
                });

                // Run MultipleTrader with progress (ASYNC)
                await algoTrader.RunMultipleTraderWithProgressAsync(progress);

                // Python ile grafik çizdirme (opsiyonel)
                try
                {
                    _confirmingMultipleTraderLogger.Log("Çizim için Python çağrılıyor...");

                    // Task.Run ile UI bloğunu önle
                    await Task.Run(() =>
                    {
                        // ImGui/ImPlot ile 5 panelli grafik (mainTrader)
                        if (algoTrader.multipleTrader != null)
                        {
                            algoTrader.PlotImGuiBundle(algoTrader.multipleTrader.GetMainTrader());
                        }
                    });

                    _confirmingMultipleTraderLogger.Log("✓ ImGui grafik başarıyla çizdirildi!");
                }
                catch (Exception plotEx)
                {
                    _confirmingMultipleTraderLogger.LogWarning($"Grafik çizimi hatası: {plotEx.Message}");
                }
            }
            catch (Exception ex)
            {
                _confirmingMultipleTraderLogger?.LogError("ConfirmingMultipleTrader test hatası:", ex.Message, ex.StackTrace);
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable button
                btnStartConfirmingMultipleTrader.Enabled = true;
                btnStopConfirmingMultipleTrader.Enabled = false;
            }
        }

        private void btnStopConfirmingMultipleTrader_Click(object sender, EventArgs e)
        {
            // Run stop logic in background to avoid logger deadlock
            Task.Run(() =>
            {
                _confirmingMultipleTraderLogger?.Log("Stop button clicked - requesting ConfirmingMultipleTrader stop...");

                // Stop MultipleTrader if running
                if (algoTrader?.multipleTrader != null)
                {
                    if (algoTrader.multipleTrader.IsRunning)
                    {
                        algoTrader.multipleTrader.Stop();
                        _confirmingMultipleTraderLogger?.Log("Stop request sent to MultipleTrader");
                    }
                    else
                    {
                        _confirmingMultipleTraderLogger?.LogWarning("MultipleTrader is not running");
                    }
                }
                else
                {
                    _confirmingMultipleTraderLogger?.LogWarning("MultipleTrader instance not found");
                }
            });

            // Disable stop button, enable start button (UI thread)
            btnStopConfirmingMultipleTrader.Enabled = false;
            btnStartConfirmingMultipleTrader.Enabled = true;
        }

        private async void btnPlotConfirmingMultipleTraderData_Click(object sender, EventArgs e)
        {
            try
            {
                // Null check - AlgoTrader ve multipleTrader oluşturulmuş mu?
                if (algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader instance yok!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (algoTrader.multipleTrader == null)
                {
                    MessageBox.Show("MultipleTrader verisi yok!\n\nÖnce ConfirmingMultipleTrader'ı çalıştırın.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _confirmingMultipleTraderLogger?.Log("");
                _confirmingMultipleTraderLogger?.Log("Re-plotting ConfirmingMultipleTrader data with Python ImGui...");

                // Task.Run ile UI bloğunu önle
                await Task.Run(() =>
                {
                    // ImGui/ImPlot ile 5 panelli grafik (mainTrader)
                    algoTrader.PlotImGuiBundle(algoTrader.multipleTrader.GetMainTrader());
                });

                _confirmingMultipleTraderLogger?.Log("✓ ConfirmingMultipleTrader grafik başarıyla çizdirildi!");
            }
            catch (Exception ex)
            {
                _confirmingMultipleTraderLogger?.LogError($"Grafik çizimi hatası: {ex.Message}");
                MessageBox.Show(
                    $"Grafik çiziminde hata:\n{ex.Message}\n\n" +
                    "Python kurulumunu ve imgui-bundle paketini kontrol edin:\n\n" +
                    "pip install imgui-bundle",
                    "ImGui Plotting Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// ConfirmingMultipleTrader tab için local logger'ı başlat (sadece ilk kez)
        /// </summary>
        private void InitializeConfirmingMultipleTraderLogger()
        {
            if (_confirmingMultipleTraderLogger == null)
            {
                _confirmingMultipleTraderLogger = new ConfirmingMultipleTraderLogger(richTextBoxConfirmingMultipleTrader);
                _confirmingMultipleTraderLogger.Log("=== ConfirmingMultipleTraderLogger Initialized ===");
            }
            else
            {
                _confirmingMultipleTraderLogger.Clear();
                _confirmingMultipleTraderLogger.Log("=== ConfirmingMultipleTraderLogger Cleared ===");
            }
        }

        /// <summary>
        /// ConfirmingMultipleTrader tab için local logger
        /// Implements IAlgoTraderLogger interface
        /// </summary>
        private class ConfirmingMultipleTraderLogger : IAlgoTraderLogger, IDisposable
        {
            private readonly RichTextBox _richTextBox;
            private readonly FileSink _fileSink;
            private readonly object _lockObject = new object();
            private bool _isDisposed = false;

            public ConfirmingMultipleTraderLogger(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
                _fileSink = new FileSink("logs", "confirmingMultipleTraderLog.txt", appendMode: false);
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

                        var logEntry = new LogEntry(logLevel, message, "ConfirmingMultipleTrader");
                        _fileSink.Write(logEntry);
                    }
                }
            }
        }

        // ====================================================================
        // CONFIRMINGSINGLETRADER2 - EVENT HANDLERS & LOGGER
        // ====================================================================

        /// <summary>
        /// ConfirmingSingleTrader2 logger'ı initialize et veya temizle
        /// </summary>
        private void InitializeConfirmingSingleTrader2Logger()
        {
            if (_confirmingSingleTrader2Logger == null)
            {
                _confirmingSingleTrader2Logger = new ConfirmingSingleTrader2Logger(richTextBoxConfirmingSingleTrader2);
                _confirmingSingleTrader2Logger.Log("=== ConfirmingSingleTrader2Logger Initialized ===");
            }
            else
            {
                _confirmingSingleTrader2Logger.Clear();
                _confirmingSingleTrader2Logger.Log("=== ConfirmingSingleTrader2Logger Cleared ===");
            }
        }

        /// <summary>
        /// ConfirmingSingleTrader2 tab için local logger
        /// Implements IAlgoTraderLogger interface
        /// </summary>
        private class ConfirmingSingleTrader2Logger : IAlgoTraderLogger, IDisposable
        {
            private readonly RichTextBox _richTextBox;
            private readonly FileSink _fileSink;
            private readonly object _lockObject = new object();
            private bool _isDisposed = false;

            public ConfirmingSingleTrader2Logger(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
                _fileSink = new FileSink("logs", "confirmingSingleTrader2Log.txt", appendMode: false);
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

                        var logEntry = new LogEntry(logLevel, message, "ConfirmingSingleTrader2");
                        _fileSink.Write(logEntry);
                    }
                }
            }
        }

        private async void btnStartConfirmingSingleTrader2_Click(object sender, EventArgs e)
        {
            // Disable button during execution
            btnStartConfirmingSingleTrader2.Enabled = false;
            btnStopConfirmingSingleTrader2.Enabled = true;

            try
            {
                // Null check - objeler oluşturulmuş mu?
                if (_confirmingSingleTrader2Logger == null || algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader objeleri oluşturulamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Logger'ı temizle veya oluştur
                InitializeConfirmingSingleTrader2Logger();

                // AlgoTrader zaten initialize edilmişse reset et
                if (algoTrader.IsInitialized)
                {
                    _confirmingSingleTrader2Logger.Log("Resetting existing AlgoTrader...");
                    algoTrader.Reset();
                }

                // Logger'ı AlgoTrader'a tekrar kaydet (reset sonrası gerekli)
                algoTrader.RegisterLogger(_confirmingSingleTrader2Logger);

                _confirmingSingleTrader2Logger.Log("=== ConfirmingSingleTrader2 Test Started ===");

                // Stock data kontrolü
                if (stockDataList == null || stockDataList.Count == 0)
                {
                    _confirmingSingleTrader2Logger.LogWarning("Stock data yüklü değil!");
                    MessageBox.Show("Önce stock data yükleyin!", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _confirmingSingleTrader2Logger.Log($"Data loaded: {stockDataList.Count} bars");

                // Initialize with stock data
                algoTrader.Initialize(stockDataList);

                if (algoTrader.IsInitialized)
                {
                    _confirmingSingleTrader2Logger.Log("AlgoTrader initialized with stock data.");
                    _confirmingSingleTrader2Logger.Log(algoTrader.GetDataInfo());
                    _confirmingSingleTrader2Logger.Log("=== AlgoTrader Initialized Successfully ===");
                }
                else
                {
                    _confirmingSingleTrader2Logger.LogError("AlgoTrader initialization failed!");
                    MessageBox.Show("AlgoTrader başlatılamadı!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // GUI'den Confirmation Mode ayarlarını oku
                bool confirmationEnabled = chkConfirmationModeEnabled2.Checked;
                double karEsigi = double.TryParse(txtKarEsigi2.Text, out var k) ? k : 5000.0;
                double zararEsigi = double.TryParse(txtZararEsigi2.Text, out var z) ? z : -3000.0;
                bool esikTipiYuzde = cmbEsikTipi2.SelectedItem?.ToString() == "Yuzde";

                // Tetikleyici'yi parse et
                ConfirmationTrigger tetikleyici = ConfirmationTrigger.Both;
                string selectedTrigger = cmbTetikleyici2.SelectedItem?.ToString() ?? "Both";
                if (selectedTrigger == "KarOnly") tetikleyici = ConfirmationTrigger.KarOnly;
                else if (selectedTrigger == "ZararOnly") tetikleyici = ConfirmationTrigger.ZararOnly;

                // Confirmation Mode'u yapılandır
                algoTrader.ConfigureConfirmationMode(confirmationEnabled, karEsigi, zararEsigi, tetikleyici, esikTipiYuzde);

                _confirmingSingleTrader2Logger.Log($"Confirmation Mode: Enabled={confirmationEnabled}");
                _confirmingSingleTrader2Logger.Log($"  Eşik Tipi: {(esikTipiYuzde ? "Yüzde" : "Değer")}");
                _confirmingSingleTrader2Logger.Log($"  Kar Eşiği: {karEsigi}, Zarar Eşiği: {zararEsigi}");
                _confirmingSingleTrader2Logger.Log($"  Tetikleyici: {tetikleyici}");

                // Progress reporter oluştur
                var progress = new Progress<BacktestProgressInfo>(progressInfo =>
                {
                    // UI kontrollerini güvenli şekilde güncelle
                    try
                    {
                        UpdateUIControl(() =>
                        {
                            if (progressBarConfirmingSingleTrader2 != null)
                            {
                                progressBarConfirmingSingleTrader2.Value = (int)progressInfo.PercentComplete;
                            }

                            if (lblConfirmingSingleTrader2Progress != null)
                            {
                                lblConfirmingSingleTrader2Progress.Text = $"{progressInfo.CurrentBar}/{progressInfo.TotalBars} - {progressInfo.PercentComplete:F1}%";
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _confirmingSingleTrader2Logger?.LogWarning($"Progress update failed: {ex.Message}");
                    }
                });

                // Run MultipleTrader with progress (ASYNC) - MultipleTraders tabındaki gibi
                await algoTrader.RunConfirmingSingleTraderWithProgressAsync(progress);

                if (lblConfirmingSingleTrader2Progress != null)
                {
                    lblConfirmingSingleTrader2Progress.Text = "Backtest completed!";
                }

                _confirmingSingleTrader2Logger.Log("=== ConfirmingSingleTrader2 Test Completed ===");

                // Plot sonuçlarını otomatik olarak çizdir
                if (algoTrader?.confirmingSingleTrader != null)
                {
                    _confirmingSingleTrader2Logger.Log("Plotting results...");
                    await Task.Run(() =>
                    {
                        var mainTrader = algoTrader.confirmingSingleTrader.GetMainTrader();
                        algoTrader.PlotImGuiBundle(mainTrader);
                    });
                    _confirmingSingleTrader2Logger.Log("Plot completed.");
                }

                // Test başarıyla tamamlandı - butonları aktif hale getir
                btnStartConfirmingSingleTrader2.Enabled = true;
                btnStopConfirmingSingleTrader2.Enabled = false;
            }
            catch (Exception ex)
            {
                _confirmingSingleTrader2Logger?.LogError("ConfirmingSingleTrader2 test hatası:", ex.Message, ex.StackTrace);
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (lblConfirmingSingleTrader2Progress != null)
                {
                    lblConfirmingSingleTrader2Progress.Text = "Error occurred";
                }

                // Hata durumunda da butonları aktif hale getir
                btnStartConfirmingSingleTrader2.Enabled = true;
                btnStopConfirmingSingleTrader2.Enabled = false;
            }
        }

        private void btnStopConfirmingSingleTrader2_Click(object sender, EventArgs e)
        {
            // Run stop logic in background to avoid logger deadlock
            Task.Run(() =>
            {
                _confirmingSingleTrader2Logger?.Log("Stop button clicked - requesting ConfirmingSingleTrader2 stop...");

                // Stop MultipleTrader if running
                if (algoTrader?.multipleTrader != null)
                {
                    if (algoTrader.multipleTrader.IsRunning)
                    {
                        algoTrader.multipleTrader.Stop();
                        _confirmingSingleTrader2Logger?.Log("Stop request sent to MultipleTrader");
                    }
                    else
                    {
                        _confirmingSingleTrader2Logger?.LogWarning("MultipleTrader is not running");
                    }
                }
                else
                {
                    _confirmingSingleTrader2Logger?.LogWarning("MultipleTrader instance not found");
                }
            });

            // Disable stop button, enable start button (UI thread)
            btnStopConfirmingSingleTrader2.Enabled = false;
            btnStartConfirmingSingleTrader2.Enabled = true;
        }

        private async void btnPlotConfirmingSingleTrader2Data_Click(object sender, EventArgs e)
        {
            try
            {
                // Null check - AlgoTrader ve multipleTrader oluşturulmuş mu?
                if (algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader instance yok!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (algoTrader.multipleTrader == null)
                {
                    MessageBox.Show("MultipleTrader verisi yok!\n\nÖnce ConfirmingSingleTrader2'yi çalıştırın.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _confirmingSingleTrader2Logger?.Log("");
                _confirmingSingleTrader2Logger?.Log("Re-plotting ConfirmingSingleTrader2 data with Python ImGui...");

                // Task.Run ile UI bloğunu önle
                await Task.Run(() =>
                {
                    // MultipleTrader'ın mainTrader'ını kullanarak çizdir
                    var mainTrader = algoTrader.multipleTrader.GetMainTrader();
                    algoTrader.PlotImGuiBundle(mainTrader);
                });

                _confirmingSingleTrader2Logger?.Log("✓ ConfirmingSingleTrader2 grafik başarıyla çizdirildi!");
            }
            catch (Exception ex)
            {
                _confirmingSingleTrader2Logger?.LogError($"Grafik çizimi hatası: {ex.Message}");
                MessageBox.Show(
                    $"Grafik çiziminde hata:\n{ex.Message}\n\n" +
                    "Python kurulumunu ve imgui-bundle paketini kontrol edin:\n\n" +
                    "pip install imgui-bundle",
                    "ImGui Plotting Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        /// <summary>
        /// Plot MultipleTrader data button click event
        /// Re-plots the last MultipleTrader run using Python ImGui/ImPlot
        /// </summary>
        private async void btnPlotMultipleTraderData_Click(object sender, EventArgs e)
        {
            try
            {
                // Null check - AlgoTrader ve multipleTrader oluşturulmuş mu?
                if (algoTrader == null)
                {
                    MessageBox.Show("AlgoTrader instance yok!", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (algoTrader.multipleTrader == null)
                {
                    MessageBox.Show("MultipleTrader verisi yok!\n\nÖnce MultipleTrader'ı çalıştırın.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _multipleTraderLogger?.Log("");
                _multipleTraderLogger?.Log("Re-plotting MultipleTrader data with Python ImGui...");

                // Task.Run ile UI bloğunu önle
                await Task.Run(() =>
                {
                    // MultipleTrader'ın mainTrader'ını kullanarak çizdir
                    var mainTrader = algoTrader.multipleTrader.GetMainTrader();
                    algoTrader.PlotImGuiBundle(mainTrader);
                });

                _multipleTraderLogger?.Log("✓ MultipleTrader grafik başarıyla çizdirildi!");
            }
            catch (Exception ex)
            {
                _multipleTraderLogger?.LogError($"Grafik çizimi hatası: {ex.Message}");
                MessageBox.Show(
                    $"Grafik çiziminde hata:\n{ex.Message}\n\n" +
                    "Python kurulumunu ve imgui-bundle paketini kontrol edin:\n\n" +
                    "pip install imgui-bundle",
                    "ImGui Plotting Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

    }
}
