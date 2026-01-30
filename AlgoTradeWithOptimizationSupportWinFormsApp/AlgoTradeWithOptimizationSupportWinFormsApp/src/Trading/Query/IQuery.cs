using System.Collections.Generic;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Query
{
    /// <summary>
    /// Query interface
    /// All market queries must implement this interface
    /// Similar to IStrategy but for querying market state instead of trading
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Query name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Query parameters
        /// </summary>
        Dictionary<string, object> Parameters { get; }

        /// <summary>
        /// Initialize query
        /// </summary>
        void OnInit();

        /// <summary>
        /// Execute query for the last bar
        /// Returns list of results (dynamic columns)
        /// Example: [Close, MA8, MA200, CrossSignal, DistancePercent, ...]
        /// </summary>
        List<object> OnExecute(int lastBarIndex);

        /// <summary>
        /// Reset query to initial state
        /// </summary>
        void Reset();
    }
}
