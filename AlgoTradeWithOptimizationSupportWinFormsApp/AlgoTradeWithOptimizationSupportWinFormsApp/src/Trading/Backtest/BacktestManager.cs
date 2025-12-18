using System;
using System.Collections.Generic;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Backtest
{
    /// <summary>
    /// Backtest report
    /// </summary>
    public class BacktestReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalBars { get; set; }
        public string StrategyName { get; set; }
        public string StatisticsSummary { get; set; }
        public Dictionary<string, object> CustomMetrics { get; set; }

        public BacktestReport()
        {
            CustomMetrics = new Dictionary<string, object>();
        }

        public string GetFullReport()
        {
            return $@"
=== Backtest Report ===
Strategy:     {StrategyName}
Start Date:   {StartDate:yyyy-MM-dd HH:mm:ss}
End Date:     {EndDate:yyyy-MM-dd HH:mm:ss}
Total Bars:   {TotalBars}

{StatisticsSummary}
";
        }
    }

    /// <summary>
    /// Backtest manager - manages backtest execution and reporting
    /// </summary>
    public class BacktestManager
    {
        #region Properties

        public List<StockData> Data { get; private set; }
        public SingleTrader Trader { get; private set; }
        public BacktestReport Report { get; private set; }
        public bool IsInitialized { get; private set; }

        #endregion

        #region Constructor

        public BacktestManager()
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
        /// Set trader to backtest
        /// </summary>
        public void SetTrader(SingleTrader trader)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("BacktestManager not initialized");

            Trader = trader;
        }

        #endregion

        #region Backtest Methods

        /// <summary>
        /// Run backtest
        /// </summary>
        public BacktestReport RunBacktest()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("BacktestManager not initialized");

            if (Trader == null)
                throw new InvalidOperationException("Trader not set");

            // TODO: Run backtest and generate report
            // 1. Run trader
            // 2. Collect results
            // 3. Generate report

            Trader.Run(0);

            Report = new BacktestReport
            {
                StartDate = Data[0].DateTime,
                EndDate = Data[Data.Count - 1].DateTime,
                TotalBars = Data.Count,
                StrategyName = Trader.Strategy?.Name ?? "Unknown",
                StatisticsSummary = Trader.GetStatisticsSummary()
            };

            return Report;
        }

        /// <summary>
        /// Get report
        /// </summary>
        public BacktestReport GetReport()
        {
            return Report;
        }

        /// <summary>
        /// Save report to file
        /// </summary>
        public void SaveReport(string filePath)
        {
            // TODO: Implement file save
        }

        #endregion
    }
}
