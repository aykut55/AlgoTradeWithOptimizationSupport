using System;
using System.Collections.Generic;
using System.Linq;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders
{
    /// <summary>
    /// Multiple trader - manages and runs multiple SingleTraders in parallel
    /// </summary>
    public class MultipleTrader
    {
        #region Properties

        public List<StockData> Data { get; private set; }
        public List<SingleTrader> Traders { get; private set; }
        public bool IsInitialized { get; private set; }
        public int CurrentIndex { get; private set; }

        #endregion

        #region Constructor

        public MultipleTrader()
        {
            Traders = new List<SingleTrader>();
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
            CurrentIndex = 0;
            IsInitialized = true;
        }

        /// <summary>
        /// Add a trader
        /// </summary>
        public void AddTrader(SingleTrader trader)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("MultipleTrader not initialized");

            Traders.Add(trader);

            // Initialize trader with same data
            if (!trader.IsInitialized)
            {
                trader.SetData(Data);
            }
        }

        /// <summary>
        /// Reset all traders
        /// </summary>
        public void Reset()
        {
            CurrentIndex = 0;
            foreach (var trader in Traders)
            {
                trader.Reset();
            }
        }

        #endregion

        #region Trading Methods

        /// <summary>
        /// Execute one step for all traders
        /// </summary>
        public void Step()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("MultipleTrader not initialized");

            if (CurrentIndex >= Data.Count)
                return;

            // TODO: Step all traders
            foreach (var trader in Traders)
            {
                trader.Step();
            }

            CurrentIndex++;
        }

        /// <summary>
        /// Run all traders
        /// </summary>
        public void Run()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("MultipleTrader not initialized");

            Reset();

            // TODO: Run all traders
            foreach (var trader in Traders)
            {
                trader.Run(0);
            }
        }

        #endregion

        #region Statistics Methods

        /// <summary>
        /// Get statistics from all traders
        /// </summary>
        public Dictionary<string, string> GetAllStatistics()
        {
            var stats = new Dictionary<string, string>();

            for (int i = 0; i < Traders.Count; i++)
            {
                stats[$"Trader_{i}"] = Traders[i].GetStatisticsSummary();
            }

            return stats;
        }

        /// <summary>
        /// Get best trader by net profit
        /// </summary>
        public SingleTrader GetBestTrader()
        {
            if (Traders.Count == 0)
                return null;

            // TODO: Find trader with highest net profit
            return Traders.OrderByDescending(t => t.Statistics.NetProfit).FirstOrDefault();
        }

        #endregion
    }
}
