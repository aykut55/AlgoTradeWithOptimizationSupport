using System;
using System.Windows.Forms;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
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
            // Logger'ı temizle
            _singleTraderLogger = null;

            // AlgoTrader'ı temizle
            algoTrader?.Reset();
            algoTrader = null;

            _singleTraderLogger?.Log("=== AlgoTrader Objects Deleted ===");
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
        private class SingleTraderLogger : IAlgoTraderLogger
        {
            private readonly RichTextBox _richTextBox;
            private readonly object _lockObject = new object();

            public SingleTraderLogger(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
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
        /// AlgoTrader test butonu click event
        /// </summary>
        private void btnTestAlgoTrader_Click(object sender, EventArgs e)
        {
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

                // Run SingleTrader
                algoTrader.RunSingleTraderDemoSilinecek();
                algoTrader.RunMultipleTraderDemoSilinecek();
            }
            catch (Exception ex)
            {
                _singleTraderLogger?.LogError("AlgoTrader test hatası:", ex.Message, ex.StackTrace);
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // ====================================================================
        // ALGOTRADER - HELPER CLASSES (ÖRNEK STRATEJİLER)
        // ====================================================================

        /// <summary>
        /// Basit Moving Average Crossover Stratejisi
        /// Hızlı MA yavaş MA'yı yukarı keserse AL, aşağı keserse SAT
        /// </summary>
        private class SimpleMAStrategy : BaseStrategy
        {
            public override string Name => "Simple MA Crossover";

            private readonly int _fastPeriod;
            private readonly int _slowPeriod;
            private double[] _fastMA;
            private double[] _slowMA;

            public SimpleMAStrategy(int fastPeriod = 10, int slowPeriod = 20)
            {
                _fastPeriod = fastPeriod;
                _slowPeriod = slowPeriod;

                Parameters["FastPeriod"] = fastPeriod;
                Parameters["SlowPeriod"] = slowPeriod;
            }

            public override void OnInit()
            {
                if (!IsInitialized)
                    return;

                // Moving average'leri hesapla
                var closes = Indicators.GetClosePrices();
                _fastMA = Indicators.MA.SMA(closes, _fastPeriod);
                _slowMA = Indicators.MA.SMA(closes, _slowPeriod);

                LogManager.Log($"Strategy initialized: Fast={_fastPeriod}, Slow={_slowPeriod}");
            }

            public override TradeSignals OnStep(int currentIndex)
            {
                // İlk barlarda yeterli veri yok
                if (currentIndex < _slowPeriod)
                    return TradeSignals.None;

                // Geçerli ve önceki MA değerleri
                double currentFastMA = _fastMA[currentIndex];
                double currentSlowMA = _slowMA[currentIndex];
                double prevFastMA = _fastMA[currentIndex - 1];
                double prevSlowMA = _slowMA[currentIndex - 1];

                // Golden Cross (Hızlı MA yukarı kesiyor) - AL sinyali
                if (prevFastMA <= prevSlowMA && currentFastMA > currentSlowMA)
                {
                    return TradeSignals.Buy;
                }

                // Death Cross (Hızlı MA aşağı kesiyor) - SAT sinyali
                if (prevFastMA >= prevSlowMA && currentFastMA < currentSlowMA)
                {
                    return TradeSignals.Sell;
                }

                return TradeSignals.None;
            }
        }
    }
}
