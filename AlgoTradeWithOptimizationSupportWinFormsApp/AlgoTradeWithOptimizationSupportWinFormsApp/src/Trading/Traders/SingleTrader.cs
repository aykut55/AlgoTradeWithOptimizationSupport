using System;
using System.Collections.Generic;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders
{
    /// <summary>
    /// Single trader - executes one strategy on market data
    /// </summary>
    public class SingleTrader
    {
        #region Properties

        public List<StockData> Data { get; private set; }
        public IndicatorManager Indicators { get; private set; }
        public IStrategy Strategy { get; private set; }
        public Position Position { get; private set; }
        public Bakiye Bakiye { get; private set; }
        public KarZarar KarZarar { get; private set; }
        public TradeStatistics Statistics { get; private set; }
        public int CurrentIndex { get; private set; }
        public bool IsInitialized { get; private set; }

        #endregion

        #region Constructor

        public SingleTrader()
        {
            Position = new Position();
            Bakiye = new Bakiye();
            KarZarar = new KarZarar();
            Statistics = new TradeStatistics();
            IsInitialized = false;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize with market data
        /// </summary>
        public void SetData(List<StockData> data)
        {
            if (data == null || data.Count == 0)
                throw new ArgumentException("Data cannot be null or empty");

            Data = data;
            CurrentIndex = 0;

            // Initialize indicators
            Indicators = new IndicatorManager();
            Indicators.SetData(data);

            IsInitialized = true;
        }

        /// <summary>
        /// Set strategy
        /// </summary>
        public void SetStrategy(IStrategy strategy)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized. Call Initialize() first.");

            Strategy = strategy;

            if (strategy is BaseStrategy baseStrategy)
            {
                baseStrategy.Initialize(Data, Indicators);
            }
            else
            {
                //strategy.OnInit();
            }
        }

        /// <summary>
        /// Reset to initial state
        /// </summary>


        #endregion

        #region Trading Methods

        /// <summary>
        /// Execute one step (process one bar)
        /// </summary>
        public void Step()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");

            if (Strategy == null)
                throw new InvalidOperationException("Strategy not set");

            if (CurrentIndex >= Data.Count)
                return;

            // TODO: Implement step logic
            // 1. Get signal from strategy
            // 2. Process signal (buy/sell/close)
            // 3. Update position
            // 4. Update balance
            // 5. Check kar/zarar
            // 6. Update statistics

            CurrentIndex++;
        }
        public void CreateModules()
        {
            // Create or reinitialize modules
            Position = new Position();
            Bakiye = new Bakiye();
            KarZarar = new KarZarar();
            Statistics = new TradeStatistics();
        }

        public void Reset()
        {
            CurrentIndex = 0;
            Position.Close();
            Bakiye.Reset();
            KarZarar.Reset();
            Statistics.Reset();
            Strategy?.Reset();
        }
        public void Init()
        {
            CurrentIndex = 0;
            Position.Close();
            Bakiye.Reset();
            KarZarar.Reset();
            Statistics.Reset();
            Strategy?.Reset();
        }

        public void Initialize(int i)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");
        }

        public void Run(int i)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");
        }
        public void Finalize(int i)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get current bar
        /// </summary>
        public StockData GetCurrentBar()
        {
            if (CurrentIndex < 0 || CurrentIndex >= Data.Count)
                throw new InvalidOperationException($"Invalid current index: {CurrentIndex}");

            return Data[CurrentIndex];
        }

        /// <summary>
        /// Get statistics summary
        /// </summary>
        public string GetStatisticsSummary()
        {
            return Statistics.GetSummary();
        }

        #endregion
    }
}
