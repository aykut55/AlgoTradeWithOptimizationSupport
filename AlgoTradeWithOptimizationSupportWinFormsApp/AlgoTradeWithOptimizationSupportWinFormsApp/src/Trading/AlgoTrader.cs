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
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
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
        public IndicatorManager indicators { get; private set; }
        public BaseStrategy strategy { get; private set; }

        public TimeManager timeManager { get; private set; }

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

            return optimizer.Optimize();
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

            singleTrader = new SingleTrader(this.Data, indicators, strategy);
            if (singleTrader == null)
                return;

            if (Logger != null)
                singleTrader.SetLogger(Logger);

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

            singleTrader.IlkBakiyeFiyat = 100000.0;
            singleTrader.SonBakiyeFiyat = 0.0;
            singleTrader.NetBakiyeFiyat = 0.0;

            singleTrader.MarketType = MarketTypes.ViopEndex;            
            if (singleTrader.MarketType == MarketTypes.ViopEndex)       // VIP-X030-T
            {
                singleTrader.KontratSayisi = 1;                         // 1 Kontrat 
                singleTrader.VarlikAdedCarpani = 10;
                singleTrader.VarlikAdedSayisi = singleTrader.KontratSayisi * singleTrader.VarlikAdedCarpani;
                singleTrader.KomisyonVarlikAdedSayisi = singleTrader.KontratSayisi;
                singleTrader.KomisyonCarpan = 0.0;
            }
            if (singleTrader.MarketType == MarketTypes.ViopHisse)       // VIP-THYAO
            {
                singleTrader.KontratSayisi = 1;                         // 1 Kontrat 
                singleTrader.VarlikAdedCarpani = 100;
                singleTrader.VarlikAdedSayisi = singleTrader.KontratSayisi * singleTrader.VarlikAdedCarpani;
                singleTrader.KomisyonVarlikAdedSayisi = singleTrader.KontratSayisi;
                singleTrader.KomisyonCarpan = 0.0;
            }
            if (singleTrader.MarketType == MarketTypes.ViopParite)      // VIP-USDTRY
            {
                singleTrader.KontratSayisi = 1;                         // 1 Kontrat 
                singleTrader.VarlikAdedCarpani = 1000;
                singleTrader.VarlikAdedSayisi = singleTrader.KontratSayisi * singleTrader.VarlikAdedCarpani;
                singleTrader.KomisyonVarlikAdedSayisi = singleTrader.KontratSayisi;
                singleTrader.KomisyonCarpan = 0.0;
            }
            if (singleTrader.MarketType == MarketTypes.BistHisse)       // THYAO
            {
                singleTrader.HisseSayisi = 1000;                        // 1000 Hisse 
                singleTrader.VarlikAdedCarpani = 1;
                singleTrader.VarlikAdedSayisi = singleTrader.HisseSayisi * singleTrader.VarlikAdedCarpani;
                singleTrader.KomisyonVarlikAdedSayisi = singleTrader.HisseSayisi;
                singleTrader.KomisyonCarpan = 0.0;
            }

            // --------------------------------------------------------------
            singleTrader.Init();                        // Bir kez cagrilir
            singleTrader.InitModules();                 // Bir kez cagrilir

            // --------------------------------------------------------------
            this.timeManager.ResetTimer("1");
            this.timeManager.StartTimer("1");
            Log("Single Trader - Initialize (~10 ms)");
            singleTrader.Initialize(0);
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
            singleTrader.Finalize(0);
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

            var strategy = new SimpleMAStrategy(this.Data, indicators, fastPeriod: 10, slowPeriod: 20);
            if (strategy == null)
                return;
            strategy.OnInit();

            singleTrader = new SingleTrader(this.Data, indicators, strategy);
            if (singleTrader == null)
                return;

            if (Logger != null)
                singleTrader.SetLogger(Logger);

            // Setup
            singleTrader.CreateModules();
            singleTrader.Reset();

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

            singleTrader.IlkBakiyeFiyat = 100000.0;
            singleTrader.SonBakiyeFiyat = 0.0;
            singleTrader.NetBakiyeFiyat = 0.0;

            singleTrader.MarketType = MarketTypes.ViopEndex;
            if (singleTrader.MarketType == MarketTypes.ViopEndex)
            {
                singleTrader.KontratSayisi = 1;
                singleTrader.VarlikAdedCarpani = 10;
                singleTrader.VarlikAdedSayisi = singleTrader.KontratSayisi * singleTrader.VarlikAdedCarpani;
                singleTrader.KomisyonVarlikAdedSayisi = singleTrader.KontratSayisi;
                singleTrader.KomisyonCarpan = 0.0;
            }
            if (singleTrader.MarketType == MarketTypes.ViopHisse)
            {
                singleTrader.KontratSayisi = 1;
                singleTrader.VarlikAdedCarpani = 100;
                singleTrader.VarlikAdedSayisi = singleTrader.KontratSayisi * singleTrader.VarlikAdedCarpani;
                singleTrader.KomisyonVarlikAdedSayisi = singleTrader.KontratSayisi;
                singleTrader.KomisyonCarpan = 0.0;
            }
            if (singleTrader.MarketType == MarketTypes.ViopParite)
            {
                singleTrader.KontratSayisi = 1;
                singleTrader.VarlikAdedCarpani = 1000;
                singleTrader.VarlikAdedSayisi = singleTrader.KontratSayisi * singleTrader.VarlikAdedCarpani;
                singleTrader.KomisyonVarlikAdedSayisi = singleTrader.KontratSayisi;
                singleTrader.KomisyonCarpan = 0.0;
            }
            if (singleTrader.MarketType == MarketTypes.BistHisse)
            {
                singleTrader.HisseSayisi = 1000;
                singleTrader.VarlikAdedCarpani = 1;
                singleTrader.VarlikAdedSayisi = singleTrader.HisseSayisi * singleTrader.VarlikAdedCarpani;
                singleTrader.KomisyonVarlikAdedSayisi = singleTrader.HisseSayisi;
                singleTrader.KomisyonCarpan = 0.0;
            }

            singleTrader.Init();
            singleTrader.InitModules();

            // Initialize
            this.timeManager.ResetTimer("1");
            this.timeManager.StartTimer("1");
            Log("Single Trader - Initialize (~10 ms)");
            singleTrader.Initialize(0);
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
                }
            });

            Log("");

            this.timeManager.StopTimer("2");

            // Finalize
            this.timeManager.ResetTimer("3");
            this.timeManager.StartTimer("3");
            Log("Single Trader - Finalize (~10 ms)");
            singleTrader.Finalize(0);
            this.timeManager.StopTimer("3");

            Log("");

            var t1 = this.timeManager.GetElapsedTime("1");
            var t2 = this.timeManager.GetElapsedTime("2");
            var t3 = this.timeManager.GetElapsedTime("3");

            Log($"t1 = {t1} msec...");
            Log($"t2 = {t2} msec...");
            Log($"t3 = {t3} msec...");

            Log("Single Trader demo completed (Async)");
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
