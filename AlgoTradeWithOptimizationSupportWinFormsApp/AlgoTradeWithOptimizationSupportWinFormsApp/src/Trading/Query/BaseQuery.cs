using System.Collections.Generic;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Query
{
    /// <summary>
    /// Base query class
    /// Provides common functionality for all queries
    /// Similar to BaseStrategy but for querying market state
    /// </summary>
    public abstract class BaseQuery : IQuery
    {
        #region Properties

        public abstract string Name { get; }
        public Dictionary<string, object> Parameters { get; protected set; }

        /// <summary>Market data</summary>
        protected List<StockData> Data { get; set; }

        /// <summary>Indicator manager</summary>
        protected IndicatorManager Indicators { get; set; }

        /// <summary>Trader instance - allows query to access trader's state and results</summary>
        protected SingleTrader Trader { get; private set; }

        /// <summary>Is initialized?</summary>
        protected bool IsInitialized { get; set; }

        #endregion

        #region Constructor

        protected BaseQuery()
        {
            Parameters = new Dictionary<string, object>();
            IsInitialized = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize query with data and indicators
        /// </summary>
        public void Initialize(List<StockData> data, IndicatorManager indicators)
        {
            Data = data;
            Indicators = indicators;
            IsInitialized = true;
            OnInit();
        }

        /// <summary>
        /// Initialize query (override in derived classes)
        /// </summary>
        public virtual void OnInit()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Execute query for the last bar
        /// Returns list of results (must be implemented in derived classes)
        /// </summary>
        public abstract List<object> OnExecute(int lastBarIndex);

        /// <summary>
        /// Reset query
        /// </summary>
        public virtual void Reset()
        {
            IsInitialized = false;
        }

        /// <summary>
        /// Set parameter
        /// </summary>
        public void SetParameter(string key, object value)
        {
            Parameters[key] = value;
        }

        /// <summary>
        /// Get parameter
        /// </summary>
        public T GetParameter<T>(string key, T defaultValue = default)
        {
            if (Parameters.TryGetValue(key, out var value))
                return (T)value;
            return defaultValue;
        }

        /// <summary>
        /// Set trader instance
        /// Allows query to access trader's state, strategy results, positions, etc.
        /// </summary>
        public void SetTrader(SingleTrader trader)
        {
            Trader = trader;
        }

        #endregion
    }
}
