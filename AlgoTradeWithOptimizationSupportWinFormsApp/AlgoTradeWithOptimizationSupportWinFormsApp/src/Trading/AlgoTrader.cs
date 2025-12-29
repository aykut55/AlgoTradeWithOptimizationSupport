using AlgoTradeWithOptimizationSupportWinFormsApp.DataProvider;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
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
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Tulip;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading
{
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
        
        public TimeManager timeManager { get; private set; }

        public string SymbolName { get; set; }
        public string SymbolPeriod { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string StrategyId { get; set; }
        public string StrategyName { get; set; }

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
                .SetKomisyonParams(komisyonCarpan: 3.0)
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

            indicators = new IndicatorManager(this.Data);
            if (indicators == null)
                return;

            // *****************************************************************************
            // *****************************************************************************
            // *****************************************************************************
            singleTrader = new SingleTrader(0, "singleTrader", this.Data, indicators, Logger);
            if (singleTrader == null) return;

            // Assign callbacks
            singleTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgress, OnApplyUserFlags);

            // Setup (order is important)
            singleTrader.CreateModules();

            // Tekrar Turlar(Optimizasyon için her parametre setinde)

            // Strategy Setup
/*
            var strategy1 = new SimpleMAStrategy(this.Data, indicators, fastPeriod: 10, slowPeriod: 20);
            if (strategy1 == null)
                return;
            strategy1.OnInit();
            singleTrader.SetStrategy(strategy1);
*/
            var strategy2 = new SimpleMostStrategy(this.Data, indicators, period: 21, percent: 1.0);
            if (strategy2 == null)
                return;
            strategy2.OnInit();
            singleTrader.SetStrategy(strategy2);
/*
            var strategy3 = new SimpleSuperTrendStrategy(this.Data, indicators, period: 21, multiplier: 3.0);
            if (strategy3 == null)
                return;
            strategy3.OnInit();
            singleTrader.SetStrategy(strategy3);
*/
            // Reset
            singleTrader.Reset();

            singleTrader.SymbolName        = this.SymbolName;
            singleTrader.SymbolPeriod      = this.SymbolPeriod;
            singleTrader.SystemId          = this.SystemId     = "0";
            singleTrader.SystemName        = this.SystemName   = "SystemName";
            singleTrader.StrategyId        = this.StrategyId   = "0";
            singleTrader.StrategyName      = this.StrategyName = "StrategyName";
            singleTrader.LastExecutionTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            singleTrader.LastExecutionTimeStart = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

            // Configure position sizing
            singleTrader.pozisyonBuyuklugu.Reset()
                .SetBakiyeParams(ilkBakiye: 100000.0)
                .SetKontratParamsFxParite(lotSayisi: 0.01)
                .SetKomisyonParams(komisyonCarpan: 3.0)
                .SetKaymaParams(kaymaMiktari: 0.5);

            singleTrader.pozisyonBuyuklugu.Reset()
                .SetBakiyeParams(ilkBakiye: 100000.0)
                .SetKontratParamsViopEndex(kontratSayisi: 1)
                .SetKomisyonParams(komisyonCarpan: 3.0)
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

            var startTime = DateTime.Now;
            await Task.Run(() =>
            {
                for (int i = 0; i < totalBars; i++)
                {
                    singleTrader.Run(i);

                    // Report progress every 10 bars or on last bar (more frequent updates)
                    if (progress != null && (i % 10 == 0 || i == totalBars - 1))
                    {
                        var elapsed = DateTime.Now - startTime;
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
            });

            if (singleTrader.OnProgress != null)
                singleTrader.OnProgress?.Invoke(singleTrader, totalBars, totalBars);

            Log("");

            this.timeManager.StopTimer("2");

            this.timeManager.StopTimer("0");
            var t0 = this.timeManager.GetElapsedTime("0");
            singleTrader.LastExecutionTimeStop = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            singleTrader.LastExecutionTimeInMSec = t0.ToString();

            // Finalize
            this.timeManager.ResetTimer("3");
            this.timeManager.StartTimer("3");
            Log("Single Trader - Finalize (~10 ms)");
            singleTrader.Finalize();
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

            // Tekrar Turlar(Optimizasyon için her parametre setinde)

            singleTrader.Dispose(); 
            singleTrader = null;

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

            indicators = new IndicatorManager(this.Data);
            if (indicators == null)
                return;


            // *****************************************************************************
            // *****************************************************************************
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
                .SetKomisyonParams(komisyonCarpan: 3.0)
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
                var strategy = new SimpleMAStrategy(this.Data, indicators, fastPeriod: 10, slowPeriod: 20);
                strategy.OnInit();

                var singleTrader = new SingleTrader(0, "singleTrader", this.Data, indicators, Logger);

                // Assign callbacks
                singleTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgress, OnApplyUserFlags);

                singleTrader.CreateModules();

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
                    .SetKomisyonParams(komisyonCarpan: 3.0)
                    .SetKaymaParams(kaymaMiktari: 0.5);

                singleTrader.Init();

                multipleTrader.AddTrader(singleTrader);
            }
            {
                var strategy = new SimpleMAStrategy(this.Data, indicators, fastPeriod: 10, slowPeriod: 20);
                strategy.OnInit();

                var singleTrader = new SingleTrader(1, "singleTrader", this.Data, indicators, Logger);

                // Assign callbacks
                singleTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgress, OnApplyUserFlags);

                singleTrader.CreateModules();

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
                    .SetKomisyonParams(komisyonCarpan: 3.0)
                    .SetKaymaParams(kaymaMiktari: 0.5);

                singleTrader.Init();

                multipleTrader.AddTrader(singleTrader);
            }
            {
                var strategy = new SimpleMAStrategy(this.Data, indicators, fastPeriod: 10, slowPeriod: 20);
                strategy.OnInit();

                var singleTrader = new SingleTrader(2, "singleTrader", this.Data, indicators, Logger);

                // Assign callbacks
                singleTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgress, OnApplyUserFlags);

                singleTrader.CreateModules();

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
                    .SetKomisyonParams(komisyonCarpan: 3.0)
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

            var startTime = DateTime.Now;

            this.timeManager.ResetTimer("2");
            this.timeManager.StartTimer("2");
            await Task.Run(() =>
            {
                for (int i = 0; i < totalBars; i++)
                {
                    multipleTrader.Run(i);

                    // Report progress every 10 bars or on last bar (more frequent updates)
                    if (progress != null && (i % 10 == 0 || i == totalBars - 1))
                    {
                        var elapsed = DateTime.Now - startTime;
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
            multipleTrader.Finalize();
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

            //multipleTrader.Dispose();
            multipleTrader = null;
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

            // Indicators oluştur
            if (indicators == null)
            {
                indicators = new IndicatorManager(Data);
                Log("IndicatorManager created");
            }

            // Optimizer oluştur
            singleTraderOptimizer = new SingleTraderOptimizer(0, this.Data, indicators, Logger);
            if (singleTraderOptimizer == null)
                return;

            // ============================================================
            // OPTİMİZASYON AYARLARI
            // ============================================================

            // --- Skip Iteration Ayarları ---
            // İlk N kombinasyonu atlayarak devam etmek için kullanılır

            // Örnek 1: İlk 500 kombinasyonu atla (501'den başla)
            // singleTraderOptimizer.SetSkipIterationSettings(enabled: true, skipCount: 500);

            // Örnek 2: Skip kullanma (baştan başla)
            singleTraderOptimizer.SetSkipIterationSettings(enabled: false, skipCount: 0);
            // veya
            // singleTraderOptimizer.DisableSkipIteration();

            // --- Max Iterations Ayarları ---
            // Kaç kombinasyon çalıştırıp duracağını belirler (skip hariç effective sayı)

            // Örnek 1: 3000 kombinasyon çalıştır ve dur
            // singleTraderOptimizer.SetMaxIterationsSettings(enabled: true, maxCount: 3000);

            // Örnek 2: Max iteration kullanma (sonuna kadar çalıştır)
            singleTraderOptimizer.SetMaxIterationsSettings(enabled: false, maxCount: 0);
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

            var startTime = DateTime.Now;

            if (progressOpt != null)
            {
                singleTraderOptimizer.OnOptimizationProgress = (currentCombination, totalCombinations) =>
                {
                    var elapsed = DateTime.Now - startTime;
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

            singleTraderOptimizer.Dispose();
            singleTraderOptimizer = null;
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
