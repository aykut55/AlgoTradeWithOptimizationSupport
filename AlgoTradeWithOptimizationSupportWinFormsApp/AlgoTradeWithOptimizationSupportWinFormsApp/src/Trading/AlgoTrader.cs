using System;
using System.Collections.Generic;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Optimizers;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Backtest;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading
{
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
    public class AlgoTrader
    {
        #region Properties

        public List<StockData> Data { get; private set; }
        public bool IsInitialized { get; private set; }
        private IAlgoTraderLogger? Logger { get; set; }

        public SingleTrader singleTrader { get; private set; }
        
        #endregion

        #region Constructor

        public AlgoTrader()
        {
            IsInitialized = false;
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

            Data = data;
            IsInitialized = true;
        }

        /// <summary>
        /// Reset AlgoTrader to uninitialized state
        /// Clears all data and resets state
        /// </summary>
        public void Reset()
        {
            Data = null;
            IsInitialized = false;
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
End Date:     {Data[Data.Count - 1].DateTime:yyyy-MM-dd HH:mm:ss}
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

            singleTrader = CreateSingleTrader(this.Data, null);

            if (singleTrader != null)
            {  
                singleTrader.Reset();   // Bir kez cagrilir

                singleTrader.Init();   // Bir kez cagrilir

                // Setterlar cagrilacak

                Log("Single Trader - Initialize");
                for (int i = 0; i < Data.Count; i++)
                {
                    singleTrader.Initialize(i);
                }

                Log("Single Trader - Run");
                for (int i = 0; i < Data.Count; i++)
                {
                    singleTrader.Run(i);
                }

                Log("Single Trader - Finalize");
                for (int i = 0; i < Data.Count; i++)
                {
                    singleTrader.Finalize(i);
                }

                // TODO: Bu method içine gerçek trading logic gelecek
                // Şimdilik sadece demo log yazıyoruz
            }


            Log("Single Trader demo completed");
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
