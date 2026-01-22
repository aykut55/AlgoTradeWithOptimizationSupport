using AlgoTradeWithOptimizationSupportWinFormsApp.DataProvider;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Plotting;
using AlgoTradeWithOptimizationSupportWinFormsApp.Timer;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Backtest;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Optimizers;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategies;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;
using MathNet.Numerics.Distributions;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Tulip;
using static ScottPlot.Generate;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading
{
    /// <summary>
    /// Delegate for creating strategy instances with parameters
    /// </summary>
    public delegate IStrategy StrategyFactory(List<StockData> data, IndicatorManager indicators, Dictionary<string, object> parameters);

    /// <summary>
    /// Progress information for backtest execution
    /// </summary>
    public class BacktestProgressInfo
    {
        public int CurrentBar { get; set; }
        public int TotalBars { get; set; }
        public double PercentComplete { get; set; }
        public string StatusMessage { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan EstimatedTimeRemaining { get; set; }
    }

    /// <summary>
    /// Simple logger interface for AlgoTrader
    /// </summary>
    public interface IAlgoTraderLogger
    {
        void Log(params object[] args);
        void LogWarning(params object[] args);
        void LogError(params object[] args);
    }

    /// <summary>
    /// Main AlgoTrader class - orchestrates all trading components
    /// Entry point for algorithmic trading system
    /// </summary>
    public class AlgoTrader : MarketDataProvider
    {
        #region Properties

        private List<StockData> _data = new List<StockData>();
        public override List<StockData> Data => _data;

        public override bool IsInitialized { get => _isInitialized; }
        private bool _isInitialized = false;

        private IAlgoTraderLogger? Logger { get; set; }
        public SingleTrader singleTrader { get; private set; }
        public MultipleTrader multipleTrader { get; private set; }
        public IndicatorManager indicators { get; private set; }
        public BaseStrategy strategy { get; private set; }
        public SingleTraderOptimizer? singleTraderOptimizer { get; private set; }
        public StrategyFactory StrategyFactoryMethod { get; private set; }

        public TimeManager timeManager { get; private set; }

        public string SymbolName { get; set; }
        public string SymbolPeriod { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string StrategyId { get; set; }
        public string StrategyName { get; set; }

        // Optimization log file paths (for TODO 544)
        public Action<string, bool>? OnOptimizationResultsUpdated { get; set; }  // (filePath, useCsv)
        public Action<int, OptimizationResult>? OnLastCombinationCompleted { get; set; }  // (currentCombination, result)

        // Optimization file type preference for reading (CSV or TXT)
        private bool _preferCsvForReading = false;  // Default: TXT

        // Performance optimization parameters
        private int _optimizationCallbackThrottle = 5000;  // Her kaç kombinasyonda callback tetiklensin
        private int _optimizationGuiRowLimit = 5000;        // GUI'de max kaç satır gösterilsin

        // Skip and Max iteration settings
        private int _skipIterationValue = -1;  // -1 = disabled
        private int _maxIterationValue = -1;   // -1 = disabled

        #endregion

        #region Constructor

        public AlgoTrader()
        {
            _isInitialized = false;

            timeManager = TimeManager.GetNewInstance();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize with market data
        /// </summary>
        public void Initialize(List<StockData> data)
        {
            if (data == null || data.Count == 0)
                throw new ArgumentException("Data cannot be null or empty");

            _data = data;
            _isInitialized = true;
        }

        /// <summary>
        /// Reset AlgoTrader to uninitialized state
        /// Clears all data and resets state
        /// </summary>
        public void Reset()
        {
            _data = new List<StockData>();
            _isInitialized = false;
        }

        /// <summary>
        /// Register a logger for AlgoTrader
        /// </summary>
        public void RegisterLogger(IAlgoTraderLogger logger)
        {
            Logger = logger;
            Logger?.Log("Logger registered to AlgoTrader");
        }

        /// <summary>
        /// Set strategy factory method for creating strategy instances with parameters
        /// </summary>
        public void SetStrategyFactory(StrategyFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            StrategyFactoryMethod = factory;
        }

        /// <summary>
        /// Configure Confirmation Mode settings for SingleTrader
        /// Must be called before RunSingleTraderWithProgressAsync
        /// </summary>
        public void ConfigureConfirmationMode(bool enabled, double karEsigi, double zararEsigi, ConfirmationTrigger trigger)
        {
            _confirmationModeEnabled = enabled;
            _karKonfirmasyonEsigi = karEsigi;
            _zararKonfirmasyonEsigi = zararEsigi;
            _konfirmasyonTetikleyici = trigger;

            Log($"Confirmation Mode configured: Enabled={enabled}, KarEsigi={karEsigi}, ZararEsigi={zararEsigi}, Trigger={trigger}");
        }

        // Confirmation Mode private fields
        private bool _confirmationModeEnabled = false;
        private double _karKonfirmasyonEsigi = 10.0;
        private double _zararKonfirmasyonEsigi = 5.0;
        private ConfirmationTrigger _konfirmasyonTetikleyici = ConfirmationTrigger.Both;

        #endregion

        #region Logging

        private void Log(params object[] args) => Logger?.Log(args);
        private void LogWarning(params object[] args) => Logger?.LogWarning(args);
        private void LogError(params object[] args) => Logger?.LogError(args);

        #endregion

        #region Single Trader Methods

        /// <summary>
        /// Create and configure a single trader using AlgoTrader's data
        /// </summary>
        public SingleTrader CreateSingleTrader(IStrategy strategy)
        {
            return CreateSingleTrader(Data, strategy);
        }

        /// <summary>
        /// Create and configure a single trader
        /// </summary>
        public SingleTrader CreateSingleTrader(List<StockData> data, IStrategy strategy)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AlgoTrader not initialized");

            var trader = new SingleTrader();
            trader.SetData(data);
            trader.SetStrategy(strategy);
            return trader;
        }

        /// <summary>
        /// Run single trader backtest
        /// </summary>
        public BacktestReport RunSingleTrader(List<StockData> data, IStrategy strategy)
        {
            var trader = CreateSingleTrader(data, strategy);
            var backtestMgr = new BacktestManager();
            backtestMgr.Initialize(Data);
            backtestMgr.SetTrader(trader);
            return backtestMgr.RunBacktest();
        }

        #endregion

        #region Multiple Trader Methods

        /// <summary>
        /// Create multiple trader
        /// </summary>
        public MultipleTrader CreateMultipleTrader()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AlgoTrader not initialized");

            var multiTrader = new MultipleTrader();
            multiTrader.Initialize(Data);
            return multiTrader;
        }

        /// <summary>
        /// Run multiple traders
        /// </summary>
        public Dictionary<string, string> RunMultipleTraders(List<IStrategy> strategies)
        {
            var multiTrader = CreateMultipleTrader();

            foreach (var strategy in strategies)
            {
                var trader = new SingleTrader();
                trader.SetData(Data);
                trader.SetStrategy(strategy);
                multiTrader.AddTrader(trader);
            }

            multiTrader.Run();
            return multiTrader.GetAllStatistics();
        }

        #endregion

        #region Optimization Methods

        /// <summary>
        /// Create optimizer
        /// </summary>
        public SingleTraderOptimizer CreateOptimizer(Type strategyType)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AlgoTrader not initialized");

            var optimizer = new SingleTraderOptimizer();
            optimizer.Initialize(Data);
            optimizer.SetStrategy(strategyType);
            return optimizer;
        }

        /// <summary>
        /// Run optimization
        /// </summary>
        public OptimizationResult RunOptimization(Type strategyType, List<ParameterRange> parameterRanges)
        {
            var optimizer = CreateOptimizer(strategyType);

            foreach (var range in parameterRanges)
            {
                optimizer.AddParameterRange(range.Name, range.Min, range.Max, range.Step);
            }

            return optimizer.Run();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get data info
        /// </summary>
        public string GetDataInfo()
        {
            if (!IsInitialized)
                return "Not initialized";

            return $@"

=== Data Info ===
Total Bars:   {Data.Count}
Start Date:   {Data[0].DateTime:yyyy-MM-dd HH:mm:ss}
End Date:    {Data[Data.Count - 1].DateTime:yyyy-MM-dd HH:mm:ss}
";
        }

        public void RunSingleTraderDemoSilinecek()
        {
            if (!IsInitialized)
            {
                LogError("AlgoTrader not initialized!");
                throw new InvalidOperationException("AlgoTrader not initialized");
            }

            Log("=== Running Single Trader Demo ===");
            Log($"Processing {Data.Count} bars...");

            // Dispose old indicators before creating new one
            if (indicators != null)
            {
                Log("Disposing previous indicators instance...");
                indicators.Dispose();
                indicators = null;
            }

            indicators = new IndicatorManager(this.Data);
            if (indicators == null)
                return;

            var strategy = new SimpleMAStrategy(this.Data, indicators, fastPeriod: 10, slowPeriod: 20);
            if (strategy == null)
                return;
            strategy.OnInit();

            singleTrader = new SingleTrader(0, "singleTrader", this.Data, indicators, Logger);
            if (singleTrader == null)
                return;

            // Assign callback to receive per-bar notification after emirleri_uygula(i)
            singleTrader.OnBeforeOrdersCallback = OnSingleTraderBeforeOrder; 
            singleTrader.OnAfterOrdersCallback = OnSingleTraderAfterOrder;

            // --------------------------------------------------------------
            singleTrader.CreateModules();               // Bir kez cagrilir
            singleTrader.Reset();                       // Bir kez cagrilir

            singleTrader.signals.AlEnabled = false;
            singleTrader.signals.SatEnabled = false;
            singleTrader.signals.FlatOlEnabled = false;
            singleTrader.signals.PasGecEnabled = false;
            singleTrader.signals.KarAlEnabled = false;
            singleTrader.signals.ZararKesEnabled = false;
            singleTrader.signals.Alindi = false;
            singleTrader.signals.Satildi = false;
            singleTrader.signals.FlatOlundu = false;
            singleTrader.signals.PasGecildi = false;
            singleTrader.signals.KarAlindi = false;
            singleTrader.signals.ZararKesildi = false;
            singleTrader.signals.PozAcilabilir = false;
            singleTrader.signals.PozAcildi = false;
            singleTrader.signals.PozKapatilabilir = false;
            singleTrader.signals.PozKapatildi = false;
            singleTrader.signals.PozAcilabilirAlis = false;
            singleTrader.signals.PozAcilabilirSatis = false;
            singleTrader.signals.PozAcildiAlis = false;
            singleTrader.signals.PozAcildiSatis = false;
            singleTrader.signals.GunSonuPozKapatEnabled = false;
            singleTrader.signals.GunSonuPozKapatildi = false;
            singleTrader.signals.TimeFilteringEnabled = false;
            singleTrader.signals.IsTradeEnabled = false;
            singleTrader.signals.IsPozKapatEnabled = false;

            singleTrader.pozisyonBuyuklugu.Reset()
                .SetBakiyeParams(ilkBakiye: 100000.0)
                .SetKontratParamsFxParite(lotSayisi: 0.01)
                .SetKomisyonParams(komisyonCarpan: 3.0)
                .SetKaymaParams(kaymaMiktari: 0.5);

            singleTrader.pozisyonBuyuklugu.Reset()
                .SetBakiyeParams(ilkBakiye: 100000.0)
                .SetKontratParamsViopEndex(kontratSayisi: 1)
                .SetKomisyonParams(komisyonCarpan: 20.0)
                .SetKaymaParams(kaymaMiktari: 0.5);

            // --------------------------------------------------------------
            singleTrader.Init();                        // Bir kez cagrilir
            singleTrader.InitModules();                 // Bir kez cagrilir

            // --------------------------------------------------------------
            this.timeManager.ResetTimer("1");
            this.timeManager.StartTimer("1");
            Log("Single Trader - Initialize (~10 ms)");
            singleTrader.Initialize();
            this.timeManager.StopTimer("1");

            this.timeManager.ResetTimer("2");
            this.timeManager.StartTimer("2");
            Log("Single Trader - Run (~100 ms)");
            for (int i = 0; i < Data.Count; i++)
            {
                singleTrader.Run(i);
            }
            this.timeManager.StopTimer("2");

            this.timeManager.ResetTimer("3");
            this.timeManager.StartTimer("3");
            Log("Single Trader - Finalize (~10 ms)");
            singleTrader.Finalize();
            this.timeManager.StopTimer("3");

            var t1 = this.timeManager.GetElapsedTime("1");
            var t2 = this.timeManager.GetElapsedTime("2");
            var t3 = this.timeManager.GetElapsedTime("3");

            Log($"t1 = {t1} msec...");
            Log($"t2 = {t2} msec...");
            Log($"t3 = {t3} msec...");

            // TODO: Bu method içine gerçek trading logic gelecek
            // Şimdilik sadece demo log yazıyoruz

            Log("Single Trader demo completed");
        }

        private void OnSingleTraderReset(SingleTrader trader,int mode)
        {

        }

        private void OnSingleTraderInit(SingleTrader trader, int mode)
        {

        }

        private void OnSingleTraderRun(SingleTrader trader, int mode)
        {

        }

        private void OnSingleTraderFinal(SingleTrader trader, int mode)
        {

        }

        // Callback function to be assigned to SingleTrader.Callback
        // Runs right after emirleri_uygula(i) for each bar
        private void OnSingleTraderBeforeOrder(SingleTrader trader, int barIndex)
        {
            // Example: you can inspect last signal/direction here
            // Logger?.Log($"CB | Bar={barIndex} Yon={trader.signals.SonYon} EmirStatus={trader.signals.EmirStatus}");
            // No-op by default
        }

        // Notification when a concrete A/S/F sinyali gerçekleştiğinde tetiklenir
        private void OnSingleTraderNotifySignal(SingleTrader trader, string signal, int barIndex)
        {
            // Example: Logger?.Log($"SIG | Yon={trader.signals.SonYon} Sinyal={signal}");
            // No-op by default
        }

        // Callback function to be assigned to SingleTrader.Callback
        // Runs right after emirleri_uygula(i) for each bar
        private void OnSingleTraderAfterOrder(SingleTrader trader, int barIndex)
        {
            // Example: you can inspect last signal/direction here
            // Logger?.Log($"CB | Bar={barIndex} Yon={trader.signals.SonYon} EmirStatus={trader.signals.EmirStatus}");
            // No-op by default
        }

        private void OnSingleTraderProgress(SingleTrader trader, int currentBar, int totalBars)
        {
            // Example: you can inspect last signal/direction here
            // Logger?.Log($"CB | Bar={barIndex} Yon={trader.signals.SonYon} EmirStatus={trader.signals.EmirStatus}");
            // No-op by default
        }

        private void OnApplyUserFlags(SingleTrader trader)
        {
            // InitializeUserControlledFlags
            trader.ConfigureUserFlagsOnce();

            int traderId = trader.GetId();
            if (traderId == 0)
            {
                // 0 id'li trader icin
            }
            else if (traderId == 1)
            {
                // 1 id'li trader icin
            }

            var dateTimes = new string[] { "2025.05.25 09:35:00", "2025.06.02 17:55:00" };

            trader.StartDateTimeStr = dateTimes[0];
            trader.StopDateTimeStr  = dateTimes[1];

            var startDateTime = System.DateTime.ParseExact(dateTimes[0], "yyyy.MM.dd HH:mm:ss", null);
            trader.StartDateStr = startDateTime.ToString("yyyy.MM.dd");  // "2025.05.25"
            trader.StartTimeStr = startDateTime.ToString("HH:mm:ss");    // "14:30:00"

            var stopDateTime = System.DateTime.ParseExact(dateTimes[1], "yyyy.MM.dd HH:mm:ss", null);
            trader.StopDateStr = stopDateTime.ToString("yyyy.MM.dd");    // "2025.06.02"
            trader.StopTimeStr = stopDateTime.ToString("HH:mm:ss");      // "14:00:00"
        }

        private void OnReadOptimizationResultsFile(SingleTraderOptimizer traderOptimizer, SingleTrader trader, int currentCombination)
        {
            // DONE TODO 545 : Optimization sırasında her bir kosum sonrasında callback ile buraya gelinir.
            // Burada OptimizationLogFile dosyası okunacak ve içi sort edilip  dataGridViewOptimizationResults e yazılacak...
            // Form1.cs ile bir sekilde iletişim kurulmalı... cunku dataGridViewOptimizationResults gui elemanı...
            // IMPLEMENTATION: OnOptimizationResultsUpdated callback kullanılarak Form1'e bildirim yapılıyor
            // Form1'de bu callback'i handle ederek dosyayı okuyup, sort edip dataGridViewOptimizationResults'a yazacak

            // Throttling check - her N kombinasyonda bir tetikle (performance optimization)
            // İlk kombinasyonda (1) ve her N'inci kombinasyonda tetikle
            if (currentCombination > 1 && currentCombination % _optimizationCallbackThrottle != 0)
                return;

            // Get the optimization log file path (using user preference)
            string optimizationLogFilePath = GetOptimizationLogFilePath(preferCsv: _preferCsvForReading);

            if (string.IsNullOrEmpty(optimizationLogFilePath))
            {
                LogWarning("Optimization log file path is not set.");
                return;
            }

            if (!System.IO.File.Exists(optimizationLogFilePath))
            {
                LogWarning($"Optimization log file does not exist: {optimizationLogFilePath}");
                return;
            }

            // Trigger callback to Form1 to update dataGridViewOptimizationResults
            // Form1 will read the file, sort it, and update the DataGridView
            bool useCsv = optimizationLogFilePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
            OnOptimizationResultsUpdated?.Invoke(optimizationLogFilePath, useCsv);

            // Trigger callback to update lblOptimizationResult with last combination info
            if (traderOptimizer != null && traderOptimizer.Results != null && traderOptimizer.Results.Count > 0)
            {
                var lastResult = traderOptimizer.Results[traderOptimizer.Results.Count - 1];
                OnLastCombinationCompleted?.Invoke(currentCombination, lastResult);
            }
        }

        /// <summary>
        /// Get optimization log file path (CSV or TXT)
        /// </summary>
        /// <param name="preferCsv">If true, returns CSV path if available, otherwise TXT path</param>
        /// <returns>File path or empty string if not available</returns>
        public string GetOptimizationLogFilePath(bool preferCsv = true)
        {
            if (singleTraderOptimizer == null)
                return string.Empty;

            if (preferCsv && singleTraderOptimizer.CsvFileLoggingEnabled && !string.IsNullOrEmpty(singleTraderOptimizer.CsvFilePath))
            {
                return singleTraderOptimizer.CsvFilePath;
            }
            else if (singleTraderOptimizer.TxtFileLoggingEnabled && !string.IsNullOrEmpty(singleTraderOptimizer.TxtFilePath))
            {
                return singleTraderOptimizer.TxtFilePath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get optimization CSV log file path
        /// </summary>
        public string GetOptimizationCsvFilePath()
        {
            if (singleTraderOptimizer == null || !singleTraderOptimizer.CsvFileLoggingEnabled)
                return string.Empty;

            return singleTraderOptimizer.CsvFilePath ?? string.Empty;
        }

        /// <summary>
        /// Get optimization TXT log file path
        /// </summary>
        public string GetOptimizationTxtFilePath()
        {
            if (singleTraderOptimizer == null || !singleTraderOptimizer.TxtFileLoggingEnabled)
                return string.Empty;

            return singleTraderOptimizer.TxtFilePath ?? string.Empty;
        }

        /// <summary>
        /// Set which optimization log file type to prefer for reading (CSV or TXT)
        /// Both files can be written simultaneously, but this determines which one to read for GUI updates
        /// </summary>
        /// <param name="preferCsv">True: CSV, False: TXT</param>
        public void SetOptimizationFileTypePreference(bool preferCsv)
        {
            _preferCsvForReading = preferCsv;
            Log($"Optimization file type preference set to: {(preferCsv ? "CSV" : "TXT")}");
        }

        /// <summary>
        /// Get current optimization file type preference
        /// </summary>
        /// <returns>True if CSV is preferred, False if TXT is preferred</returns>
        public bool GetOptimizationFileTypePreference()
        {
            return _preferCsvForReading;
        }

        /// <summary>
        /// Set optimization callback throttle
        /// Callback her kaç kombinasyonda bir tetiklenecek (performance için)
        /// </summary>
        /// <param name="throttle">Her kaç kombinasyonda bir callback tetiklensin (default: 5000)</param>
        public void SetOptimizationCallbackThrottle(int throttle)
        {
            _optimizationCallbackThrottle = throttle;
            Log($"Optimization callback throttle set to: {throttle}");
        }

        /// <summary>
        /// Get optimization callback throttle value
        /// </summary>
        /// <returns>Current throttle value</returns>
        public int GetOptimizationCallbackThrottle()
        {
            return _optimizationCallbackThrottle;
        }

        /// <summary>
        /// Set optimization GUI row limit
        /// GUI'de max kaç satır gösterilecek (performance için)
        /// </summary>
        /// <param name="rowLimit">Max kaç satır gösterilsin (default: 5000)</param>
        public void SetOptimizationGuiRowLimit(int rowLimit)
        {
            _optimizationGuiRowLimit = rowLimit;
            Log($"Optimization GUI row limit set to: {rowLimit}");
        }

        /// <summary>
        /// Get optimization GUI row limit
        /// </summary>
        /// <returns>Current row limit</returns>
        public int GetOptimizationGuiRowLimit()
        {
            return _optimizationGuiRowLimit;
        }

        /// <summary>
        /// Set skip iteration value
        /// -1 = disabled, >=0 = enabled
        /// </summary>
        public void SetSkipIterationValue(int skipValue)
        {
            _skipIterationValue = skipValue;
            Log($"Skip iteration value set to: {skipValue}");
        }

        /// <summary>
        /// Get skip iteration value
        /// </summary>
        public int GetSkipIterationValue()
        {
            return _skipIterationValue;
        }

        /// <summary>
        /// Set max iteration value
        /// -1 = disabled, >=0 = enabled
        /// </summary>
        public void SetMaxIterationValue(int maxValue)
        {
            _maxIterationValue = maxValue;
            Log($"Max iteration value set to: {maxValue}");
        }

        /// <summary>
        /// Get max iteration value
        /// </summary>
        public int GetMaxIterationValue()
        {
            return _maxIterationValue;
        }

        /// <summary>
        /// Run single trader with progress reporting (async version)
        /// </summary>
        public async Task RunSingleTraderWithProgressAsync(IProgress<BacktestProgressInfo> progress = null)
        {
            if (!IsInitialized)
            {
                LogError("AlgoTrader not initialized!");
                throw new InvalidOperationException("AlgoTrader not initialized");
            }

            int totalBars = Data.Count;

            Log("");
            Log("=== Running Single Trader Demo (Async) ===");
            Log($"Processing {totalBars} bars total...");

            // Dispose old indicators before creating new one
            if (indicators != null)
            {
                Log("Disposing previous indicators instance...");
                indicators.Dispose();
                indicators = null;
            }

            indicators = new IndicatorManager(this.Data);
            if (indicators == null)
                return;

            // ============================================================
            // STRATEGY CONFIGURATION - Change this section for different strategies
            // ============================================================

            StrategyFactoryMethod = null;

            // Define parameter combination for SingleTrader (single set, not optimization)
            var StrategyParams = new Dictionary<string, object>
            {
                { "period", 21 },
                { "percent", 0.5 }
            };

            //SimpleMostStrategy(this.Data, indicators, period: 21, percent: 1.0);
            this.SetStrategyFactory((data, indicators, parameters) =>
            {
                int period = Convert.ToInt32(parameters["period"]);
                double percent = Convert.ToDouble(parameters["percent"]);
                return new SimpleMostStrategy(data, indicators, period, percent);
            });

            // ============================================================
            // END STRATEGY CONFIGURATION
            // ============================================================

            // *****************************************************************************
            // CLEANUP PREVIOUS RUN (if exists) - Dispose old singleTrader before creating new one
            // This allows plotting after run completes, while preventing memory leaks on subsequent runs
            // *****************************************************************************
            if (singleTrader != null)
            {
                Log("Disposing previous singleTrader instance...");
                singleTrader.Dispose();
                singleTrader = null;
            }

            // *****************************************************************************
            // CREATE NEW SINGLETRADER
            // *****************************************************************************
            singleTrader = new SingleTrader(0, "singleTrader", this.Data, indicators, Logger);
            if (singleTrader == null) return;

            // Assign callbacks
            singleTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgress, OnApplyUserFlags);

            // Setup (order is important)
            singleTrader.CreateModules();

            // Tekrar Turlar(Optimizasyon için her parametre setinde)

            // Validate StrategyFactory is set
            if (StrategyFactoryMethod == null)
                throw new InvalidOperationException("StrategyFactory must be set before running. Use SetStrategyFactory().");

            // Create strategy instance using factory (generic!)
            var strategy = StrategyFactoryMethod(this.Data, indicators, StrategyParams);
            strategy.OnInit();

            // Assign strategy
            singleTrader.SetStrategy(strategy);

            // Reset
            singleTrader.Reset();

            // Apply Confirmation Mode settings (if configured)
            singleTrader.ConfirmationModeEnabled = _confirmationModeEnabled;
            singleTrader.KarKonfirmasyonEsigi = _karKonfirmasyonEsigi;
            singleTrader.ZararKonfirmasyonEsigi = _zararKonfirmasyonEsigi;
            singleTrader.KonfirmasyonTetikleyici = _konfirmasyonTetikleyici;

            if (_confirmationModeEnabled)
            {
                Log($"✓ Confirmation Mode aktif - Kar: {_karKonfirmasyonEsigi}, Zarar: {_zararKonfirmasyonEsigi}, Trigger: {_konfirmasyonTetikleyici}");
            }

            singleTrader.SymbolName             = this.SymbolName;
            singleTrader.SymbolPeriod           = this.SymbolPeriod;
            singleTrader.SystemId               = this.SystemId     = "0";
            singleTrader.SystemName             = this.SystemName   = "SystemName";
            singleTrader.StrategyId             = this.StrategyId   = "0";
            singleTrader.StrategyName           = this.StrategyName = "StrategyName";
            singleTrader.LastExecutionTime      = System.DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            singleTrader.LastExecutionTimeStart = System.DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

            // Configure position sizing
            singleTrader.pozisyonBuyuklugu.Reset()
                .SetBakiyeParams(ilkBakiye: 100000.0)
                .SetKontratParamsFxParite(lotSayisi: 0.01)
                .SetKomisyonParams(komisyonCarpan: 3.0)
                .SetKaymaParams(kaymaMiktari: 0.5);

            singleTrader.pozisyonBuyuklugu.Reset()
                .SetBakiyeParams(ilkBakiye: 100000.0)
                .SetKontratParamsViopEndex(kontratSayisi: 1)
                .SetKomisyonParams(komisyonCarpan: 20.0)
                .SetKaymaParams(kaymaMiktari: 0.5);

            singleTrader.Init();
            // *****************************************************************************
            // *****************************************************************************
            // *****************************************************************************

            this.timeManager.ResetTimer("0");
            this.timeManager.StartTimer("0");

            // Initialize
            this.timeManager.ResetTimer("1");
            this.timeManager.StartTimer("1");
            Log("Single Trader - Initialize (~10 ms)");
            singleTrader.Initialize();
            this.timeManager.StopTimer("1");

            Log("");

            // Run with progress reporting
            this.timeManager.ResetTimer("2");
            this.timeManager.StartTimer("2");
            Log("Single Trader - Run (~100 ms)");

            var startTime = System.DateTime.Now;
            await Task.Run(() =>
            {
                // Set state flags
                singleTrader.IsStarted = true;
                singleTrader.IsRunning = true;
                singleTrader.IsStopped = false;
                singleTrader.IsStopRequested = false;

                for (int i = 0; i < totalBars; i++)
                {
                    // Check if stop is requested
                    if (singleTrader.IsStopRequested)
                    {
                        Log($"SingleTrader stopped by user request at bar {i}/{totalBars}");
                        break;
                    }

                    singleTrader.Run(i);

                    // Report progress every 10 bars or on last bar (more frequent updates)
                    if (progress != null && (i % 10 == 0 || i == totalBars - 1))
                    {
                        var elapsed = System.DateTime.Now - startTime;
                        double percentComplete = (double)(i + 1) / totalBars * 100.0;
                        double barsPerSecond = (i + 1) / elapsed.TotalSeconds;
                        int remainingBars = totalBars - (i + 1);
                        TimeSpan estimatedRemaining = barsPerSecond > 0
                            ? TimeSpan.FromSeconds(remainingBars / barsPerSecond)
                            : TimeSpan.Zero;

                        var progressInfo = new BacktestProgressInfo
                        {
                            CurrentBar = i + 1,
                            TotalBars = totalBars,
                            PercentComplete = percentComplete,
                            StatusMessage = $"Processing bar {i + 1}/{totalBars}",
                            ElapsedTime = elapsed,
                            EstimatedTimeRemaining = estimatedRemaining
                        };

                        progress.Report(progressInfo);
                    }

                    if (singleTrader.OnProgress != null && (i % 10 == 0 || i == totalBars - 1))
                        singleTrader.OnProgress?.Invoke(singleTrader, i, totalBars);
                }

                if (singleTrader.OnProgress != null)
                    singleTrader.OnProgress?.Invoke(singleTrader, totalBars, totalBars);

                Log("");

                this.timeManager.StopTimer("2");

                this.timeManager.StopTimer("0");
                var t0 = this.timeManager.GetElapsedTime("0");
                singleTrader.LastExecutionTimeStop = System.DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                singleTrader.LastExecutionTimeInMSec = t0.ToString();

                // Finalize
                this.timeManager.ResetTimer("3");
                this.timeManager.StartTimer("3");
                Log("Single Trader - Finalize (~10 ms)");

                if (singleTrader.IsStopRequested)
                    singleTrader.Finalize(false);
                else
                    singleTrader.Finalize(true);
                this.timeManager.StopTimer("3");

                Log("");

                var t1 = this.timeManager.GetElapsedTime("1");
                var t2 = this.timeManager.GetElapsedTime("2");
                var t3 = this.timeManager.GetElapsedTime("3");

                Log($"t0 = {t0} msec...");
                Log($"t1 = {t1} msec...");
                Log($"t2 = {t2} msec...");
                Log($"t3 = {t3} msec...");

                Log("Single Trader demo completed (Async)");

                // Update state flags
                singleTrader.IsRunning = false;
                singleTrader.IsStopped = true;
                Log($"SingleTrader finished - IsRunning: {singleTrader.IsRunning}, IsStopped: {singleTrader.IsStopped}");

                // NOTE: Do NOT dispose here - singleTrader data is needed for plotting
                // Disposal will happen at the start of next run (see cleanup section above)
                // This allows user to re-plot the results anytime before next run
            });

            // NOTE: singleTrader object remains alive for plotting purposes
            // It will be disposed automatically when next run starts
        }

        public async Task RunMultipleTraderWithProgressAsync(IProgress<BacktestProgressInfo> progress = null)
        {
            if (!IsInitialized)
            {
                LogError("AlgoTrader not initialized!");
                throw new InvalidOperationException("AlgoTrader not initialized");
            }

            int totalBars = Data.Count;

            Log("");
            Log("=== Running Multiple Trader Demo (Async) ===");
            Log($"Processing {totalBars} bars total...");

            // Dispose old indicators before creating new one
            if (indicators != null)
            {
                Log("Disposing previous indicators instance...");
                indicators.Dispose();
                indicators = null;
            }

            indicators = new IndicatorManager(this.Data);
            if (indicators == null)
                return;

            // ============================================================
            // STRATEGY CONFIGURATION - Change this section for different strategies
            // ============================================================


            // ============================================================
            // END STRATEGY CONFIGURATION
            // ============================================================

            // *****************************************************************************
            // CLEANUP PREVIOUS RUN (if exists) - Dispose old multipleTrader before creating new one
            // This allows plotting after run completes, while preventing memory leaks on subsequent runs
            // *****************************************************************************
            if (multipleTrader != null)
            {
                Log("Disposing previous multipleTrader instance...");
                multipleTrader.Dispose();
                multipleTrader = null;
            }

            // *****************************************************************************
            // CREATE NEW MULTIPLETRADER
            // *****************************************************************************
            multipleTrader = new MultipleTrader(0, this.Data, indicators, Logger);

            multipleTrader.Reset();

            var mainTrader = multipleTrader.GetMainTrader();
            // Assign callbacks
            mainTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgress, OnApplyUserFlags);
            mainTrader.CreateModules();
            mainTrader.Reset();
            mainTrader.pozisyonBuyuklugu.Reset()
                .SetBakiyeParams(ilkBakiye: 100000.0)
                .SetKontratParamsFxParite(lotSayisi: 0.01)
                .SetKomisyonParams(komisyonCarpan: 3.0)
                .SetKaymaParams(kaymaMiktari: 0.5);
            mainTrader.pozisyonBuyuklugu.Reset()
                .SetBakiyeParams(ilkBakiye: 100000.0)
                .SetKontratParamsViopEndex(kontratSayisi: 1)
                .SetKomisyonParams(komisyonCarpan: 20.0)
                .SetKaymaParams(kaymaMiktari: 0.5);
            multipleTrader.DynamicPositionSizeEnabled = true;                   // (default false) : mainTrader her pozisyonda aynı lot büyüklüğünü kullanır
                                                                                //          true   : mainTrader consensus'ten gelen lot büyüklüğünü kullanır (her pozisyon farklı olabilir)
            mainTrader.pozisyonBuyuklugu.PyramidingEnabled = true;              // false -> Varsayılan
            if (mainTrader.pozisyonBuyuklugu.PyramidingEnabled) { 
                mainTrader.pozisyonBuyuklugu.MaxPositionSizeEnabled = true;     // false -> Sınırsız
                mainTrader.pozisyonBuyuklugu.MaxPositionSize = 10.0;            // Max 10 lot
            }
            mainTrader.Init();

            {
                // ============================================================
                // STRATEGY CONFIGURATION - Change this section for different strategies
                // ============================================================

                StrategyFactoryMethod = null;

                // Define parameter combination for SingleTrader (single set, not optimization)
                var StrategyParams = new Dictionary<string, object>
                {
                    { "fastPeriod", 10 },
                    { "slowPeriod", 20 }
                };

                //SimpleMostStrategy(this.Data, indicators, period: 21, percent: 1.0);
                this.SetStrategyFactory((data, indicators, parameters) =>
                {
                    int fastPeriod = Convert.ToInt32(parameters["fastPeriod"]);
                    int slowPeriod = Convert.ToInt32(parameters["slowPeriod"]);
                    return new SimpleMAStrategy(data, indicators, fastPeriod, slowPeriod);
                });

                //var strategy = new SimpleMAStrategy(this.Data, indicators, fastPeriod: 10, slowPeriod: 20);
                //strategy.OnInit();

                // ============================================================
                // END STRATEGY CONFIGURATION
                // ============================================================

                var singleTrader = new SingleTrader(0, "singleTrader", this.Data, indicators, Logger);

                // Assign callbacks
                singleTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgress, OnApplyUserFlags);

                singleTrader.CreateModules();

                // Validate StrategyFactory is set
                if (StrategyFactoryMethod == null)
                    throw new InvalidOperationException("StrategyFactory must be set before running. Use SetStrategyFactory().");

                // Create strategy instance using factory (generic!)
                var strategy = StrategyFactoryMethod(this.Data, indicators, StrategyParams);
                strategy.OnInit();

                // Assign strategy
                singleTrader.SetStrategy(strategy);

                singleTrader.Reset();

                singleTrader.pozisyonBuyuklugu.Reset()
                    .SetBakiyeParams(ilkBakiye: 100000.0)
                    .SetKontratParamsFxParite(lotSayisi: 0.01)
                    .SetKomisyonParams(komisyonCarpan: 3.0)
                    .SetKaymaParams(kaymaMiktari: 0.5);

                singleTrader.pozisyonBuyuklugu.Reset()
                    .SetBakiyeParams(ilkBakiye: 100000.0)
                    .SetKontratParamsViopEndex(kontratSayisi: 1)
                    .SetKomisyonParams(komisyonCarpan: 20.0)
                    .SetKaymaParams(kaymaMiktari: 0.5);

                singleTrader.Init();

                multipleTrader.AddTrader(singleTrader);
            }
            {
                // ============================================================
                // STRATEGY CONFIGURATION - Change this section for different strategies
                // ============================================================

                StrategyFactoryMethod = null;

                // Define parameter combination for SingleTrader (single set, not optimization)
                var StrategyParams = new Dictionary<string, object>
                {
                    { "fastPeriod", 10 },
                    { "slowPeriod", 20 }
                };

                //SimpleMostStrategy(this.Data, indicators, period: 21, percent: 1.0);
                this.SetStrategyFactory((data, indicators, parameters) =>
                {
                    int fastPeriod = Convert.ToInt32(parameters["fastPeriod"]);
                    int slowPeriod = Convert.ToInt32(parameters["slowPeriod"]);
                    return new SimpleMAStrategy(data, indicators, fastPeriod, slowPeriod);
                });

                //var strategy = new SimpleMAStrategy(this.Data, indicators, fastPeriod: 10, slowPeriod: 20);
                //strategy.OnInit();

                // ============================================================
                // END STRATEGY CONFIGURATION

                var singleTrader = new SingleTrader(1, "singleTrader", this.Data, indicators, Logger);

                // Assign callbacks
                singleTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgress, OnApplyUserFlags);

                singleTrader.CreateModules();

                // Validate StrategyFactory is set
                if (StrategyFactoryMethod == null)
                    throw new InvalidOperationException("StrategyFactory must be set before running. Use SetStrategyFactory().");

                // Create strategy instance using factory (generic!)
                var strategy = StrategyFactoryMethod(this.Data, indicators, StrategyParams);
                strategy.OnInit();

                // Assign strategy
                singleTrader.SetStrategy(strategy);

                singleTrader.Reset();

                singleTrader.pozisyonBuyuklugu.Reset()
                    .SetBakiyeParams(ilkBakiye: 100000.0)
                    .SetKontratParamsFxParite(lotSayisi: 0.01)
                    .SetKomisyonParams(komisyonCarpan: 3.0)
                    .SetKaymaParams(kaymaMiktari: 0.5);

                singleTrader.pozisyonBuyuklugu.Reset()
                    .SetBakiyeParams(ilkBakiye: 100000.0)
                    .SetKontratParamsViopEndex(kontratSayisi: 1)
                    .SetKomisyonParams(komisyonCarpan: 20.0)
                    .SetKaymaParams(kaymaMiktari: 0.5);

                singleTrader.Init();

                multipleTrader.AddTrader(singleTrader);
            }
            {
                // ============================================================
                // STRATEGY CONFIGURATION - Change this section for different strategies
                // ============================================================

                StrategyFactoryMethod = null;

                // Define parameter combination for SingleTrader (single set, not optimization)
                var StrategyParams = new Dictionary<string, object>
                {
                    { "fastPeriod", 10 },
                    { "slowPeriod", 20 }
                };

                //SimpleMostStrategy(this.Data, indicators, period: 21, percent: 1.0);
                this.SetStrategyFactory((data, indicators, parameters) =>
                {
                    int fastPeriod = Convert.ToInt32(parameters["fastPeriod"]);
                    int slowPeriod = Convert.ToInt32(parameters["slowPeriod"]);
                    return new SimpleMAStrategy(data, indicators, fastPeriod, slowPeriod);
                });

                //var strategy = new SimpleMAStrategy(this.Data, indicators, fastPeriod: 10, slowPeriod: 20);
                //strategy.OnInit();

                // ============================================================
                // END STRATEGY CONFIGURATION

                var singleTrader = new SingleTrader(2, "singleTrader", this.Data, indicators, Logger);

                // Assign callbacks
                singleTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgress, OnApplyUserFlags);

                singleTrader.CreateModules();

                // Validate StrategyFactory is set
                if (StrategyFactoryMethod == null)
                    throw new InvalidOperationException("StrategyFactory must be set before running. Use SetStrategyFactory().");

                // Create strategy instance using factory (generic!)
                var strategy = StrategyFactoryMethod(this.Data, indicators, StrategyParams);
                strategy.OnInit();

                // Assign strategy
                singleTrader.SetStrategy(strategy);

                singleTrader.Reset();

                singleTrader.pozisyonBuyuklugu.Reset()
                    .SetBakiyeParams(ilkBakiye: 100000.0)
                    .SetKontratParamsFxParite(lotSayisi: 0.01)
                    .SetKomisyonParams(komisyonCarpan: 3.0)
                    .SetKaymaParams(kaymaMiktari: 0.5);

                singleTrader.pozisyonBuyuklugu.Reset()
                    .SetBakiyeParams(ilkBakiye: 100000.0)
                    .SetKontratParamsViopEndex(kontratSayisi: 1)
                    .SetKomisyonParams(komisyonCarpan: 20.0)
                    .SetKaymaParams(kaymaMiktari: 0.5);

                singleTrader.Init();

                multipleTrader.AddTrader(singleTrader);
            }

            multipleTrader.Init();
            // *****************************************************************************
            // *****************************************************************************
            // *****************************************************************************

            // Initialize
            this.timeManager.ResetTimer("1");
            this.timeManager.StartTimer("1");
            multipleTrader.Initialize();
            this.timeManager.StopTimer("1");

            var startTime = System.DateTime.Now;

            this.timeManager.ResetTimer("2");
            this.timeManager.StartTimer("2");

            await Task.Run(() =>
            {
                // Set state flags
                multipleTrader.IsStarted = true;
                multipleTrader.IsRunning = true;
                multipleTrader.IsStopped = false;
                multipleTrader.IsStopRequested = false;

                for (int i = 0; i < totalBars; i++)
                {
                    // Check if stop is requested
                    if (multipleTrader.IsStopRequested)
                    {
                        Log($"MultipleTrader stopped by user request at bar {i}/{totalBars}");
                        break;
                    }

                    multipleTrader.Run(i);

                    // Report progress every 10 bars or on last bar (more frequent updates)
                    if (progress != null && (i % 10 == 0 || i == totalBars - 1))
                    {
                        var elapsed = System.DateTime.Now - startTime;
                        double percentComplete = (double)(i + 1) / totalBars * 100.0;
                        double barsPerSecond = (i + 1) / elapsed.TotalSeconds;
                        int remainingBars = totalBars - (i + 1);
                        TimeSpan estimatedRemaining = barsPerSecond > 0
                            ? TimeSpan.FromSeconds(remainingBars / barsPerSecond)
                            : TimeSpan.Zero;

                        var progressInfo = new BacktestProgressInfo
                        {
                            CurrentBar = i + 1,
                            TotalBars = totalBars,
                            PercentComplete = percentComplete,
                            StatusMessage = $"Processing bar {i + 1}/{totalBars}",
                            ElapsedTime = elapsed,
                            EstimatedTimeRemaining = estimatedRemaining
                        };

                        progress.Report(progressInfo);
                    }

                    if (multipleTrader.OnProgress != null && (i % 10 == 0 || i == totalBars - 1))
                        multipleTrader.OnProgress?.Invoke(multipleTrader, i, totalBars);
                }
            });
            this.timeManager.StopTimer("2");

            if (multipleTrader.OnProgress != null)
                multipleTrader.OnProgress?.Invoke(multipleTrader, totalBars, totalBars);

            this.timeManager.ResetTimer("3");
            this.timeManager.StartTimer("3");

            if (multipleTrader.IsStopRequested)
                multipleTrader.Finalize(false);
            else
                multipleTrader.Finalize(true);

            this.timeManager.StopTimer("3");

            Log("");

            var t1 = this.timeManager.GetElapsedTime("1");
            var t2 = this.timeManager.GetElapsedTime("2");
            var t3 = this.timeManager.GetElapsedTime("3");

            Log($"t1 = {t1} msec...");
            Log($"t2 = {t2} msec...");
            Log($"t3 = {t3} msec...");

            Log("");

            Log("Multiple Trader demo completed (Async)");

            // Update state flags
            multipleTrader.IsRunning = false;
            multipleTrader.IsStopped = true;
            Log($"MultipleTrader finished - IsRunning: {multipleTrader.IsRunning}, IsStopped: {multipleTrader.IsStopped}");

            // NOTE: Do NOT dispose here - multipleTrader data is needed for plotting
            // Disposal will happen at the start of next run (see cleanup section above)
            // This allows user to re-plot the results anytime before next run
        }

        public async Task RunSingleTraderOptWithProgressAsync(IProgress<BacktestProgressInfo> progressOpt = null, IProgress<BacktestProgressInfo> progressTrader = null)
        {
            if (!IsInitialized)
            {
                LogError("AlgoTrader not initialized!");
                throw new InvalidOperationException("AlgoTrader not initialized");
            }

            int totalBars = Data.Count;

            Log("");
            Log("=== Running Single Trader Optimization (Async) ===");
            Log($"Processing {totalBars} bars total...");

            // Dispose old indicators before creating new one
            if (indicators != null)
            {
                Log("Disposing previous indicators instance...");
                indicators.Dispose();
                indicators = null;
            }

            indicators = new IndicatorManager(Data);
            Log("IndicatorManager created");

            // *****************************************************************************
            // CLEANUP PREVIOUS RUN (if exists) - Dispose old optimizer before creating new one
            // This allows plotting after run completes, while preventing memory leaks on subsequent runs
            // *****************************************************************************
            if (singleTraderOptimizer != null)
            {
                Log("Disposing previous singleTraderOptimizer instance...");
                singleTraderOptimizer.Dispose();
                singleTraderOptimizer = null;
            }

            // *****************************************************************************
            // CREATE NEW OPTIMIZER
            // *****************************************************************************
            singleTraderOptimizer = new SingleTraderOptimizer(0, this.Data, indicators, Logger);
            if (singleTraderOptimizer == null)
                return;

            // ============================================================
            // OPTİMİZASYON AYARLARI
            // ============================================================

            bool csvFileLoggingEnabled = true;   // CSV enabled - for fast processing later
            bool txtFileLoggingEnabled = true;   // TXT enabled - for human readable logs
            bool appendEnabled = true;
            singleTraderOptimizer.SetOptimizationLogFileParams( csvFileLoggingEnabled, "logs\\singleTraderOptLog.csv",
                                                                txtFileLoggingEnabled, "logs\\singleTraderOptLog.txt",
                                                                appendEnabled);

            // Set preference to read TXT file for GUI updates (can be changed to CSV later)
            SetOptimizationFileTypePreference(preferCsv: false);  // false = TXT, true = CSV

            // DONE TODO 544 : SetOptimizationLogFileParams kullanmıştım ama bunu daha smart hale getirmek gerekecek cunku simdilik
            // dataGridViewOptimizationResults ı txt dosyasını okuyarak güncelleyeceğim ama daha sonrasında hızlı olması bakımından
            // csv dosyasını okuyarak yapmayı düşünüyorum...
            // DONE TODO 545 ile bağlantısını kurmayı düşünüyorum...
            // DONE - Sanıyorum kullanılacak olan OptimizationLogFile'ı bir setter'la singleTraderOptimizer e tanıtmak gerekecek ki TODO 545 yapılıyorken
            // bu dosyayı okuyup dataGridViewOptimizationResults ı güncelleyebileyim...
            // IMPLEMENTATION: GetOptimizationLogFilePath() methodları eklendi (GetOptimizationCsvFilePath, GetOptimizationTxtFilePath)
            // IMPLEMENTATION: OnOptimizationResultsUpdated callback eklendi - Form1'e dosya yolunu ve dosya tipini (csv/txt) bildirir
            // TODO: Form1.cs'te InitializeOptimizationResultsDataGridView() methodu oluşturulmalı (InitializeStockDataGridView gibi)
            // TODO: dataGridViewOptimizationResults için kolonlar AppendSingleOptSummaryToFiles methodundaki sütun sıralamasına göre oluşturulmalı

            singleTraderOptimizer.OnReadOptimizationResultsFile = OnReadOptimizationResultsFile;

            // --- Skip Iteration Ayarları ---
            // İlk N kombinasyonu atlayarak devam etmek için kullanılır

            // Örnek 1: İlk 500 kombinasyonu atla (501'den başla)
            // singleTraderOptimizer.SetSkipIterationSettings(enabled: true, skipCount: 500);

            // Örnek 2: Skip kullanma (baştan başla)
            // singleTraderOptimizer.SetSkipIterationSettings(enabled: false, skipCount: 0);
            // veya
            // singleTraderOptimizer.DisableSkipIteration();

            // --- Max Iterations Ayarları ---
            // Kaç kombinasyon çalıştırıp duracağını belirler (skip hariç effective sayı)

            // Örnek 1: 3000 kombinasyon çalıştır ve dur
            // singleTraderOptimizer.SetMaxIterationsSettings(enabled: true, maxCount: 3000);

            // Örnek 2: Max iteration kullanma (sonuna kadar çalıştır)
            // singleTraderOptimizer.SetMaxIterationsSettings(enabled: false, maxCount: 0);
            // veya
            // singleTraderOptimizer.DisableMaxIterations();

            // --- Parça Parça Optimizasyon Örneği ---
            // Toplam 10,000 kombinasyonu 3 parçada çalıştır:

            // 1. Çalıştırma (1-3000):
            // singleTraderOptimizer.SetSkipIterationSettings(enabled: false, skipCount: 0);
            // singleTraderOptimizer.SetMaxIterationsSettings(enabled: true, maxCount: 3000);

            // 2. Çalıştırma (3001-6000):
            // singleTraderOptimizer.SetSkipIterationSettings(enabled: true, skipCount: 3000);
            // singleTraderOptimizer.SetMaxIterationsSettings(enabled: true, maxCount: 3000);

            // 3. Çalıştırma (6001-sonuna kadar):
            // singleTraderOptimizer.SetSkipIterationSettings(enabled: true, skipCount: 6000);
            // singleTraderOptimizer.SetMaxIterationsSettings(enabled: false, maxCount: 0);

            // --- Skip Iteration Ayarları (Form'dan alınan değerlerle) ---
            // _skipIterationValue: -1 = disabled, >=0 = enabled
            if (_skipIterationValue >= 0)
            {
                singleTraderOptimizer.SetSkipIterationSettings(enabled: true, skipCount: _skipIterationValue);
                Log($"Skip Iteration: ENABLED - Skip count: {_skipIterationValue}");
            }
            else
            {
                singleTraderOptimizer.SetSkipIterationSettings(enabled: false, skipCount: 0);
                Log($"Skip Iteration: DISABLED");
            }

            // --- Max Iterations Ayarları (Form'dan alınan değerlerle) ---
            // _maxIterationValue: -1 = disabled, >=0 = enabled
            if (_maxIterationValue >= 0)
            {
                singleTraderOptimizer.SetMaxIterationsSettings(enabled: true, maxCount: _maxIterationValue);
                Log($"Max Iteration: ENABLED - Max count: {_maxIterationValue}");
            }
            else
            {
                singleTraderOptimizer.SetMaxIterationsSettings(enabled: false, maxCount: 0);
                Log($"Max Iteration: DISABLED");
            }

            // --- Ara Sonuç Kaydetme Ayarları ---
            // Her N kombinasyonda bir ara sonuçları kaydetmek için

            // Örnek: Her 500 kombinasyonda bir kaydet
            // singleTraderOptimizer.SetIntermediateSaveSettings(saveEveryN: 500);

            // Ara sonuç kaydetme callback'i (kullanıcı kendi kayıt methodunu bağlayabilir)
            // singleTraderOptimizer.OnSaveResults = (results, currentCombination) =>
            // {
            //     // Kendi kayıt logiğinizi buraya yazın
            //     // Örneğin: CSV, JSON, Database vb.
            //     var filename = $"optimization_results_{currentCombination}.json";
            //     var json = System.Text.Json.JsonSerializer.Serialize(results, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            //     System.IO.File.WriteAllText(filename, json);
            //     Log($"Results saved to {filename}");
            // };

            // Progress callbacks'leri bağla
            // progressOpt: Kombinasyon ilerlemesi (kaç kombinasyon test edildi)
            // progressTrader: Her kombinasyon için bar ilerlemesi

            var startTime = System.DateTime.Now;

            if (progressOpt != null)
            {
                singleTraderOptimizer.OnOptimizationProgress = (currentCombination, totalCombinations) =>
                {
                    var elapsed = System.DateTime.Now - startTime;
                    var percentComplete = (double)currentCombination / totalCombinations * 100.0;

                    // Estimated time remaining
                    var estimatedTotal = elapsed.TotalSeconds / percentComplete * 100.0;
                    var estimatedRemaining = TimeSpan.FromSeconds(estimatedTotal - elapsed.TotalSeconds);

                    var progressInfo = new BacktestProgressInfo
                    {
                        CurrentBar = currentCombination,
                        TotalBars = totalCombinations,
                        PercentComplete = percentComplete,
                        StatusMessage = $"Testing combination {currentCombination}/{totalCombinations}",
                        ElapsedTime = elapsed,
                        EstimatedTimeRemaining = estimatedRemaining
                    };

                    progressOpt.Report(progressInfo);
                };
            }

            if (progressTrader != null)
            {
                singleTraderOptimizer.OnSingleTraderProgressCallback = (currentBar, totalBarsInner) =>
                {
                    var percentComplete = (double)currentBar / totalBarsInner * 100.0;

                    var progressInfo = new BacktestProgressInfo
                    {
                        CurrentBar = currentBar,
                        TotalBars = totalBarsInner,
                        PercentComplete = percentComplete,
                        StatusMessage = $"Processing bar {currentBar}/{totalBarsInner}",
                        ElapsedTime = TimeSpan.Zero,
                        EstimatedTimeRemaining = TimeSpan.Zero
                    };

                    progressTrader.Report(progressInfo);
                };
            }

            // Run optimization in background task
            await Task.Run(() =>
            {
                singleTraderOptimizer.Reset();
                singleTraderOptimizer.Init();
                singleTraderOptimizer.Run();
            });

            Log("Optimization completed!");
            Log($"Total combinations tested: {singleTraderOptimizer.Results.Count}");

            // NOTE: Do NOT dispose here - optimizer data is needed for plotting and analysis
            // Disposal will happen at the start of next run (see cleanup section above)
            // This allows user to access optimization results anytime before next run
        }

        /// <summary>
        /// Set optimization skip iteration settings
        /// Optimizasyonu parça parça yapmak için kullanılır
        /// </summary>
        /// <param name="enabled">Skip özelliği aktif mi?</param>
        /// <param name="skipCount">İlk kaç kombinasyon atlanacak?</param>
        public void SetOptimizationSkipSettings(bool enabled, int skipCount)
        {
            if (singleTraderOptimizer != null)
            {
                singleTraderOptimizer.SetSkipIterationSettings(enabled, skipCount);
            }
            else
            {
                LogWarning("Optimizer not initialized. Call RunSingleTraderOptWithProgressAsync first.");
            }
        }

        /// <summary>
        /// Disable optimization skip iteration
        /// Optimizasyonu baştan başlatmak için kullanılır
        /// </summary>
        public void DisableOptimizationSkip()
        {
            if (singleTraderOptimizer != null)
            {
                singleTraderOptimizer.DisableSkipIteration();
            }
        }

        /// <summary>
        /// Set optimization max iterations settings
        /// Optimizasyonu parçalara bölmek için kullanılır
        /// </summary>
        /// <param name="enabled">Max iterations özelliği aktif mi?</param>
        /// <param name="maxCount">Kaç kombinasyon çalıştırılacak? (effective - skip hariç)</param>
        public void SetOptimizationMaxIterations(bool enabled, int maxCount)
        {
            if (singleTraderOptimizer != null)
            {
                singleTraderOptimizer.SetMaxIterationsSettings(enabled, maxCount);
            }
            else
            {
                LogWarning("Optimizer not initialized. Call RunSingleTraderOptWithProgressAsync first.");
            }
        }

        /// <summary>
        /// Disable optimization max iterations
        /// Optimizasyonu sonuna kadar çalıştırmak için kullanılır
        /// </summary>
        public void DisableOptimizationMaxIterations()
        {
            if (singleTraderOptimizer != null)
            {
                singleTraderOptimizer.DisableMaxIterations();
            }
        }

        /// <summary>
        /// Set optimization intermediate save settings
        /// Ara sonuçları kaydetmek için kullanılır
        /// </summary>
        /// <param name="saveEveryN">Her kaç kombinasyonda bir kaydet (0 = disable)</param>
        public void SetOptimizationIntermediateSave(int saveEveryN)
        {
            if (singleTraderOptimizer != null)
            {
                singleTraderOptimizer.SetIntermediateSaveSettings(saveEveryN);
            }
            else
            {
                LogWarning("Optimizer not initialized. Call RunSingleTraderOptWithProgressAsync first.");
            }
        }

        /// <summary>
        /// SingleTrader sonuçlarını Python matplotlib ile çizdirir
        /// </summary>
        /// <param name="savePath">Grafiği kaydetmek için dosya yolu (opsiyonel)</param>
        public void PlotSingleTraderResults(string? savePath = null)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AlgoTrader not initialized");

            if (singleTrader == null)
                throw new InvalidOperationException("SingleTrader henüz çalıştırılmadı. RunSingleTrader() veya RunSingleTraderWithProgressAsync() çalıştırın.");

            try
            {
                Log("Plotting SingleTrader results with Python...");
/*
                using (var plotter = new PythonPlotter())
                {
                    // Verileri topla
                    var dates = Data.Select(d => d.DateTime).ToList();
                    var prices = Data.Select(d => d.Close).ToList();

                    // Al/Sat sinyallerini topla
                    var buySignals = new List<(int, double)>();
                    var sellSignals = new List<(int, double)>();

                    foreach (var trade in singleTrader.Trades)
                    {
                        if (trade.Type == TradeType.Buy)
                            buySignals.Add((trade.BarIndex, trade.Price));
                        else if (trade.Type == TradeType.Sell)
                            sellSignals.Add((trade.BarIndex, trade.Price));
                    }

                    // Bakiye ve PnL verilerini al
                    var balance = singleTrader.BalanceHistory.ToList();
                    var pnl = singleTrader.CumulativePnLHistory.ToList();

                    // EMA verilerini al (strategy'den)
                    List<double>? emaFast = null;
                    List<double>? emaSlow = null;
                    int fastPeriod = 10;
                    int slowPeriod = 20;

                    // Strategy'nin EMA'larına erişim (eğer varsa)
                    if (strategy != null)
                    {
                        // Strategy parametrelerinden EMA periyotlarını almayı dene
                        var strategyParams = strategy.GetParameters();
                        if (strategyParams.ContainsKey("FastPeriod"))
                            fastPeriod = Convert.ToInt32(strategyParams["FastPeriod"]);
                        if (strategyParams.ContainsKey("SlowPeriod"))
                            slowPeriod = Convert.ToInt32(strategyParams["SlowPeriod"]);

                        // IndicatorManager'dan EMA'ları almayı dene
                        if (indicators != null)
                        {
                            try
                            {
                                var emaFastResult = indicators.GetEma(Data, fastPeriod);
                                var emaSlowResult = indicators.GetEma(Data, slowPeriod);

                                if (emaFastResult != null)
                                    emaFast = emaFastResult.Select(x => (double)x.Ema).ToList();

                                if (emaSlowResult != null)
                                    emaSlow = emaSlowResult.Select(x => (double)x.Ema).ToList();
                            }
                            catch (Exception ex)
                            {
                                LogWarning($"EMA'lar alınamadı: {ex.Message}");
                            }
                        }
                    }

                    // Python plotter'ı çağır
                    plotter.PlotTradingResults(
                        dates: dates,
                        prices: prices,
                        buySignals: buySignals,
                        sellSignals: sellSignals,
                        balance: balance,
                        pnl: pnl,
                        emaFast: emaFast,
                        emaSlow: emaSlow,
                        fastPeriod: fastPeriod,
                        slowPeriod: slowPeriod,
                        savePath: savePath
                    );

                    Log("✓ Plotting completed successfully!");
                }
*/
            
            }
            catch (Exception ex)
            {
                LogError($"Plotting error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Equity curve ve drawdown grafiğini çizdirir
        /// </summary>
        public void PlotEquityCurve()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AlgoTrader not initialized");

            if (singleTrader == null)
                throw new InvalidOperationException("SingleTrader henüz çalıştırılmadı");

            try
            {
                Log("Plotting equity curve with drawdown...");
/*
                using (var plotter = new PythonPlotter())
                {
                    var dates = Data.Select(d => d.DateTime).ToList();
                    var balance = singleTrader.BalanceHistory.ToList();

                    plotter.PlotEquityCurveWithDrawdown(balance, dates);

                    Log("✓ Equity curve plotting completed!");
                }
*/
            }
            catch (Exception ex)
            {
                LogError($"Equity curve plotting error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Dinamik panel sistemi ile grafik çizimi
        /// Örnek kullanım:
        ///   plotter.AddPanel(0, dates, closes, "Close", "blue");
        ///   plotter.AddPanel(0, dates, ma50, "MA50", "red");
        ///   plotter.AddPanel(1, dates, volumes, "Volume");
        ///   plotter.Plot();
        /// </summary>
        public void PlotDynamic()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AlgoTrader not initialized");

            if (this.singleTrader == null)
            {
                LogError("singleTrader is NULL!");
                throw new InvalidOperationException("SingleTrader henüz çalıştırılmadı - önce RunSingleTrader() çağırın");
            }

            if (Data == null || Data.Count == 0)
                throw new InvalidOperationException("Data yok - Initialize() çalıştırın");

            try
            {
                Log("Plotting with dynamic panel system...");

                using (var plotter = new DynamicPlotter())
                {
                    // Ortak veriler
                    var dates = Data.Select(d => d.DateTime).ToList();
                    var opens = Data.Select(d => d.Open).ToList();
                    var highs = Data.Select(d => d.High).ToList();
                    var lows = Data.Select(d => d.Low).ToList();
                    var closes = Data.Select(d => d.Close).ToList();
                    var volumes = Data.Select(d => (double)d.Volume).ToList(); // long -> double cast
                    var lots = Data.Select(d => d.Size).ToList();

                    // SingleTrader.lists verileri
                    var sinyalList = singleTrader.lists.SinyalList;
                    var karZararFiyatList = singleTrader.lists.KarZararFiyatList;
                    var karZararFiyatYuzdeList = singleTrader.lists.KarZararFiyatYuzdeList;
                    var bakiyeFiyatList = singleTrader.lists.BakiyeFiyatList;
                    var getiriFiyatList = singleTrader.lists.GetiriFiyatList;
                    var getiriFiyatYuzdeList = singleTrader.lists.GetiriFiyatYuzdeList;
                    var komisyonFiyatList = singleTrader.lists.KomisyonFiyatList;
                    var getiriFiyatNetList = singleTrader.lists.GetiriFiyatNetList;
                    var bakiyeFiyatNetList = singleTrader.lists.BakiyeFiyatNetList;
                    var getiriFiyatYuzdeNetList = singleTrader.lists.GetiriFiyatYuzdeNetList;

                    Log($"Panel sistemi ile çiziliyor - {closes.Count} bar");

                    // PANEL 0: OHLC Candlestick + Close Line
                    // plotter.AddOHLCPanel(0, dates, opens, highs, lows, closes, "OHLC", "black");
                    // Close çizgisini de ekle (overlay)
                    plotter.AddPanel(0, dates, closes, "Close", "blue", "-", 1.0);

                    // PANEL 1: Volume
                    plotter.AddPanel(1, dates, volumes, "Volume", "gray", "-", 1.0);

                    // PANEL 2: Sinyal
                    plotter.AddPanel(2, dates, sinyalList, "Sinyal", "green", "-", 1.5);

                    // PANEL 3: Kar/Zarar Fiyat
                    plotter.AddPanel(3, dates, karZararFiyatList, "Kar/Zarar Fiyat", "purple", "-", 1.5);

                    // PANEL 4: Kar/Zarar Yüzde
                    plotter.AddPanel(4, dates, karZararFiyatYuzdeList, "Kar/Zarar %", "orange", "-", 1.5);

                    // PANEL 5: Bakiye Fiyat
                    plotter.AddPanel(5, dates, bakiyeFiyatList, "Bakiye", "darkgreen", "-", 2.0);

                    // PANEL 6: Getiri Fiyat
                    plotter.AddPanel(6, dates, getiriFiyatList, "Getiri", "steelblue", "-", 1.5);

                    // PANEL 7: Getiri Yüzde
                    plotter.AddPanel(7, dates, getiriFiyatYuzdeList, "Getiri %", "teal", "-", 1.5);

                    // PANEL 8: Komisyon
                    plotter.AddPanel(8, dates, komisyonFiyatList, "Komisyon", "red", "-", 1.0);

                    // PANEL 9: Getiri Net
                    plotter.AddPanel(9, dates, getiriFiyatNetList, "Getiri Net", "navy", "-", 1.5);

                    // ÇİZDİR!
                    bool result = plotter.Plot(title: "AlgoTrade - Tüm Veriler", savePath: null);

                    if (result)
                        Log("✓ Dynamic panel plotting completed!");
                    else
                        LogWarning("Dynamic panel plotting returned false");
                }
            }
            catch (Exception ex)
            {
                LogError($"Dynamic panel plotting error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// ImGui/ImPlot ile 5 panelli grafik çizdirir
        /// AlgoTradeWithPythonWithGemini projesindeki plotDataImgBundle yapısına benzer
        /// REQUIREMENTS: pip install imgui-bundle
        /// </summary>
        /// <summary>
        /// Plot ImGui bundle (for singleTrader - backward compatibility)
        /// </summary>
        public void PlotImGuiBundle()
        {
            if (this.singleTrader == null)
            {
                LogError("singleTrader is NULL!");
                throw new InvalidOperationException("SingleTrader henüz çalıştırılmadı - önce RunSingleTrader() çağırın");
            }

            PlotImGuiBundle(this.singleTrader);
        }

        /// <summary>
        /// Plot ImGui bundle (generic version - accepts SingleTrader)
        /// </summary>
        public void PlotImGuiBundle(SingleTrader trader)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AlgoTrader not initialized");

            if (trader == null)
            {
                LogError("trader is NULL!");
                throw new InvalidOperationException("Trader henüz çalıştırılmadı");
            }

            if (Data == null || Data.Count == 0)
                throw new InvalidOperationException("Data yok - Initialize() çalıştırın");

            try
            {
                Log("Plotting with ImGui/ImPlot...");

                using (var plotter = new ImGuiPlotter())
                {
                    // Ortak veriler
                    var dates = Data.Select(d => d.DateTime).ToList();
                    var opens = Data.Select(d => d.Open).ToList();
                    var highs = Data.Select(d => d.High).ToList();
                    var lows = Data.Select(d => d.Low).ToList();
                    var closes = Data.Select(d => d.Close).ToList();
                    var volumes = Data.Select(d => d.Volume).ToList(); // long (no cast needed)
                    var lots = Data.Select(d => d.Size).ToList();      // long (no cast needed)

                    // Trader.lists verileri
                    var sinyalList = trader.lists.SinyalList;
                    var karZararFiyatList = trader.lists.KarZararFiyatList;
                    var bakiyeFiyatList = trader.lists.BakiyeFiyatList;
                    var getiriFiyatList = trader.lists.GetiriFiyatList;
                    var getiriFiyatNetList = trader.lists.GetiriFiyatNetList;

                    // Strategy'den MOST ve EXMOV verilerini al (SimpleMostStrategy için)
                    List<double>? mostList = null;
                    List<double>? exmovList = null;

                    if (trader.Strategy is SimpleMostStrategy mostStrategy)
                    {
                        var mostArray = mostStrategy.GetMOST();
                        var exmovArray = mostStrategy.GetEXMOV();

                        if (mostArray != null && mostArray.Length > 0)
                            mostList = mostArray.ToList();

                        if (exmovArray != null && exmovArray.Length > 0)
                            exmovList = exmovArray.ToList();

                        Log($"MOST ve EXMOV verileri alındı: MOST={mostList?.Count ?? 0}, EXMOV={exmovList?.Count ?? 0}");
                    }

                    Log($"ImGui bundle ile çiziliyor - {closes.Count} bar");

                    // ImGui/ImPlot ile çizdir
                    bool result = plotter.PlotDataBundle(
                        dates, opens, highs, lows, closes, volumes, lots,
                        sinyalList, karZararFiyatList, bakiyeFiyatList,
                        getiriFiyatList, getiriFiyatNetList,
                        mostList, exmovList,
                        title: SymbolName ?? "AlgoTrade",
                        periyot: SymbolPeriod ?? "1H"
                    );

                    if (result)
                        Log("✓ ImGui plotting completed!");
                    else
                        LogWarning("ImGui plotting returned false");
                }
            }
            catch (Exception ex)
            {
                LogError($"ImGui plotting error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Basit test - Sadece Close değerlerini çizdirir
        /// Adım adım ilerleme için minimal plot
        /// </summary>
        public void PlotSimpleClose()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AlgoTrader not initialized");

            if (Data == null || Data.Count == 0)
                throw new InvalidOperationException("Data yok - Initialize() çalıştırın");

            try
            {
                Log("Plotting simple close values...");

                using (var pythonHelper = new PythonHelper())
                {
                    // Data'dan Close değerlerini ve tarihleri topla
                    var dates = Data.Select(d => d.DateTime).ToList();
                    var closes = Data.Select(d => d.Close).ToList();

                    Log($"Çizdirilecek data: {closes.Count} bar");

                    // Python ile çizdir
                    bool result = pythonHelper.TestPlotSimpleClose(dates, closes);

                    if (result)
                        Log("✓ Simple close plotting completed!");
                    else
                        LogWarning("Simple close plotting returned false");
                }
            }
            catch (Exception ex)
            {
                LogError($"Simple close plotting error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 6 panelli grafik - Close, Volume, Sinyal, KarZarar, KarZarar%, GetiriNet
        /// </summary>
        public void Plot6Panels()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("AlgoTrader not initialized");

            if (this.singleTrader == null)
            {
                LogError("singleTrader is NULL!");
                throw new InvalidOperationException("SingleTrader henüz çalıştırılmadı - önce RunSingleTrader() çağırın");
            }

            if (Data == null || Data.Count == 0)
                throw new InvalidOperationException("Data yok - Initialize() çalıştırın");

            try
            {
                Log("Plotting 6 panels...");
                Log($"singleTrader null check passed: {this.singleTrader != null}");

                using (var pythonHelper = new PythonHelper())
                {
                    // Data'dan verileri topla
                    var dates = Data.Select(d => d.DateTime).ToList();
                    var opens = Data.Select(d => d.Open).ToList();
                    var highs = Data.Select(d => d.High).ToList();
                    var lows = Data.Select(d => d.Low).ToList();
                    var closes = Data.Select(d => d.Close).ToList();
                    var volumes = Data.Select(d => (double)d.Volume).ToList(); // long -> double cast
                    var lots = Data.Select(d => d.Size).ToList();


                    // SingleTrader.lists'den TÜM verileri topla
                    var sinyalList = singleTrader.lists.SinyalList;
                    var karZararFiyatList = singleTrader.lists.KarZararFiyatList;
                    var karZararFiyatYuzdeList = singleTrader.lists.KarZararFiyatYuzdeList;
                    var bakiyeFiyatList = singleTrader.lists.BakiyeFiyatList;
                    var getiriFiyatList = singleTrader.lists.GetiriFiyatList;
                    var getiriFiyatYuzdeList = singleTrader.lists.GetiriFiyatYuzdeList;
                    var komisyonFiyatList = singleTrader.lists.KomisyonFiyatList;
                    var getiriFiyatNetList = singleTrader.lists.GetiriFiyatNetList;
                    var bakiyeFiyatNetList = singleTrader.lists.BakiyeFiyatNetList;
                    var getiriFiyatYuzdeNetList = singleTrader.lists.GetiriFiyatYuzdeNetList;

                    Log($"Çizdirilecek data: {closes.Count} bar");
                    Log($"Sinyal count: {sinyalList.Count}");
                    Log($"KarZararFiyat count: {karZararFiyatList.Count}");
                    Log($"KarZararFiyatYuzde count: {karZararFiyatYuzdeList.Count}");
                    Log($"BakiyeFiyat count: {bakiyeFiyatList.Count}");
                    Log($"GetiriFiyat count: {getiriFiyatList.Count}");
                    Log($"GetiriFiyatYuzde count: {getiriFiyatYuzdeList.Count}");
                    Log($"KomisyonFiyat count: {komisyonFiyatList.Count}");
                    Log($"GetiriFiyatNet count: {getiriFiyatNetList.Count}");
                    Log($"BakiyeFiyatNet count: {bakiyeFiyatNetList.Count}");
                    Log($"GetiriFiyatYuzdeNet count: {getiriFiyatYuzdeNetList.Count}");

                    // Python ile çizdir - TÜM listeleri gönder
                    bool result = pythonHelper.TestPlot6Panels(
                        dates,
                        closes,
                        volumes,
                        sinyalList,
                        karZararFiyatList,
                        karZararFiyatYuzdeList,
                        bakiyeFiyatList,
                        getiriFiyatList,
                        getiriFiyatYuzdeList,
                        komisyonFiyatList,
                        getiriFiyatNetList,
                        bakiyeFiyatNetList,
                        getiriFiyatYuzdeNetList
                    );

                    if (result)
                        Log("✓ 6 panel plotting completed!");
                    else
                        LogWarning("6 panel plotting returned false");
                }
            }
            catch (Exception ex)
            {
                LogError($"6 panel plotting error: {ex.Message}");
                throw;
            }
        }

        public void RunMultipleTraderDemoSilinecek()
        {
            if (!IsInitialized)
            {
                LogError("AlgoTrader not initialized!");
                throw new InvalidOperationException("AlgoTrader not initialized");
            }

            Log("=== Running Single Trader Demo ===");
            Log($"Processing {Data.Count} bars...");

            // TODO: Bu method içine gerçek trading logic gelecek
            // Şimdilik sadece demo log yazıyoruz

            Log("Multiple Trader demo completed");
        }

        #endregion
    }
}
