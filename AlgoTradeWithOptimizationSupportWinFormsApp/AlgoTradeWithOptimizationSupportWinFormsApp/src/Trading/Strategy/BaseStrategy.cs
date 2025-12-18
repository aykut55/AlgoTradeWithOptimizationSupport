using System.Collections.Generic;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy
{
    /// <summary>
    /// Base strategy class
    /// Provides common functionality for all strategies
    /// </summary>
    public abstract class BaseStrategy : IStrategy
    {
        #region Properties

        public abstract string Name { get; }
        public Dictionary<string, object> Parameters { get; protected set; }

        /// <summary>Market data</summary>
        protected List<StockData> Data { get; set; }

        /// <summary>Indicator manager</summary>
        protected IndicatorManager Indicators { get; set; }

        /// <summary>Is initialized?</summary>
        protected bool IsInitialized { get; set; }

        #endregion

        #region Constructor

        protected BaseStrategy()
        {
            Parameters = new Dictionary<string, object>();
            IsInitialized = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize strategy with data and indicators
        /// </summary>
        public void Initialize(List<StockData> data, IndicatorManager indicators)
        {
            Data = data;
            Indicators = indicators;
            IsInitialized = true;
            OnInit();
        }

        /// <summary>
        /// Initialize strategy (override in derived classes)
        /// </summary>
        public virtual void OnInit()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called on each bar/step
        /// </summary>
        public abstract TradeSignals OnStep(int currentIndex);

        /// <summary>
        /// Reset strategy
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

        #endregion
    }
}
