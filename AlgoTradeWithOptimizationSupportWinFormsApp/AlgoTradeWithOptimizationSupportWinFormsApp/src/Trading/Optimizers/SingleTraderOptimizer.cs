using System;
using System.Collections.Generic;
using System.Linq;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Optimizers
{
    /// <summary>
    /// Parameter range for optimization
    /// </summary>
    public class ParameterRange
    {
        public string Name { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }

        public ParameterRange(string name, double min, double max, double step)
        {
            Name = name;
            Min = min;
            Max = max;
            Step = step;
        }

        public List<double> GetValues()
        {
            var values = new List<double>();
            for (double v = Min; v <= Max; v += Step)
            {
                values.Add(v);
            }
            return values;
        }
    }

    /// <summary>
    /// Optimization result
    /// </summary>
    public class OptimizationResult
    {
        public Dictionary<string, object> Parameters { get; set; }
        public double NetProfit { get; set; }
        public double WinRate { get; set; }
        public double ProfitFactor { get; set; }
        public double MaxDrawdown { get; set; }
        public double SharpeRatio { get; set; }

        public OptimizationResult()
        {
            Parameters = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Single trader optimizer - finds best parameters for a strategy
    /// </summary>
    public class SingleTraderOptimizer
    {
        #region Properties

        public List<StockData> Data { get; private set; }
        public Type StrategyType { get; private set; }
        public List<ParameterRange> ParameterRanges { get; private set; }
        public List<OptimizationResult> Results { get; private set; }
        public bool IsInitialized { get; private set; }

        #endregion

        #region Constructor

        public SingleTraderOptimizer()
        {
            ParameterRanges = new List<ParameterRange>();
            Results = new List<OptimizationResult>();
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
        /// Set strategy type to optimize
        /// </summary>
        public void SetStrategy(Type strategyType)
        {
            if (!typeof(IStrategy).IsAssignableFrom(strategyType))
                throw new ArgumentException("Strategy type must implement IStrategy");

            StrategyType = strategyType;
        }

        /// <summary>
        /// Add parameter range for optimization
        /// </summary>
        public void AddParameterRange(string name, double min, double max, double step)
        {
            ParameterRanges.Add(new ParameterRange(name, min, max, step));
        }

        #endregion

        #region Optimization Methods

        /// <summary>
        /// Run optimization
        /// </summary>
        public OptimizationResult Optimize()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Optimizer not initialized");

            if (StrategyType == null)
                throw new InvalidOperationException("Strategy type not set");

            Results.Clear();

            // TODO: Implement optimization logic
            // 1. Generate all parameter combinations
            // 2. For each combination:
            //    - Create strategy instance
            //    - Set parameters
            //    - Run SingleTrader
            //    - Collect statistics
            // 3. Find best result

            return GetBestResult();
        }

        /// <summary>
        /// Get best optimization result
        /// </summary>
        public OptimizationResult GetBestResult()
        {
            if (Results.Count == 0)
                return null;

            // TODO: Sort by fitness function (e.g., net profit, sharpe ratio)
            return Results.OrderByDescending(r => r.NetProfit).FirstOrDefault();
        }

        /// <summary>
        /// Get trader with best parameters
        /// </summary>
        public SingleTrader GetBestTrader()
        {
            var bestResult = GetBestResult();
            if (bestResult == null)
                return null;

            // TODO: Create trader with best parameters
            return null;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Generate all parameter combinations
        /// </summary>
        private List<Dictionary<string, object>> GenerateParameterCombinations()
        {
            // TODO: Implement recursive parameter combination generator
            return new List<Dictionary<string, object>>();
        }

        #endregion
    }
}
