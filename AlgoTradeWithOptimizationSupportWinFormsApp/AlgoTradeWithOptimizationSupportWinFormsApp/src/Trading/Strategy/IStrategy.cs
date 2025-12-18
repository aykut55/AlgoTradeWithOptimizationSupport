using System.Collections.Generic;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy
{
    /// <summary>
    /// Strategy interface
    /// All trading strategies must implement this interface
    /// </summary>
    public interface IStrategy
    {
        /// <summary>
        /// Strategy name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Strategy parameters
        /// </summary>
        Dictionary<string, object> Parameters { get; }

        /// <summary>
        /// Initialize strategy
        /// </summary>
        void OnInit();

        /// <summary>
        /// Called on each bar/step
        /// Returns trading signal
        /// </summary>
        TradeSignals OnStep(int currentIndex);

        /// <summary>
        /// Reset strategy to initial state
        /// </summary>
        void Reset();
    }
}
