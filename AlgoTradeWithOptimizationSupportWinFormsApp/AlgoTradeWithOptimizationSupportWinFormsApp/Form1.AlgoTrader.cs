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
        private AlgoTrader? algoTrader = null;

        /// <summary>
        /// AlgoTrader objelerini oluştur
        /// Form constructor'dan çağrılır
        /// </summary>
        private void CreateObjects()
        {
            _singleTraderLogger = new SingleTraderLogger(richTextBoxSingleTrader);
            algoTrader = new AlgoTrader();
            algoTrader.RegisterLogger(_singleTraderLogger);
            _singleTraderLogger.Log("=== AlgoTrader Objects Created ===");
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
            // Disable button during execution
            btnTestAlgoTrader.Enabled = false;

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
                    // UI thread'de otomatik çalışır
                    //_singleTraderLogger?.Log($"Progress: {progressInfo.PercentComplete:F1}% - Bar {progressInfo.CurrentBar}/{progressInfo.TotalBars}");
                    //lblSingleTraderProgress.Text = $"{progressInfo.CurrentBar}/{progressInfo.TotalBars} ({progressInfo.PercentComplete:F1}%)";

                    // ProgressBar varsa güncelle
                    try
                    {
                        if (progressBarSingleTrader != null)
                        {
                            progressBarSingleTrader.Value = (int)progressInfo.PercentComplete;
                        }
                    }
                    catch (Exception ex)
                    {
                        _singleTraderLogger?.LogWarning($"ProgressBar update failed: {ex.Message}");
                    }

                    // Label varsa güncelle
                    try
                    {
                        /*
                        if (lblSingleTraderProgress != null)
                        {
                            lblSingleTraderProgress.Text =
                                $"Bar: {progressInfo.CurrentBar}/{progressInfo.TotalBars} " +
                                $"({progressInfo.PercentComplete:F1}%) - " +
                                $"Elapsed: {progressInfo.ElapsedTime:mm\\:ss} - " +
                                $"ETA: {progressInfo.EstimatedTimeRemaining:mm\\:ss}";
                        }
                        */
                    }
                    catch (Exception ex)
                    {
                        _singleTraderLogger?.LogWarning($"Label update failed: {ex.Message}");
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
                btnTestAlgoTrader.Enabled = true;
            }
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

    }
}
