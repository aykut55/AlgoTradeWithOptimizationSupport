using System;
using System.Collections.Generic;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Scripting
{
    /// <summary>
    /// Global objects and helpers exposed to C# scripts.
    /// Scripts can access these directly without any prefix.
    /// </summary>
    public class ScriptGlobals
    {
        /// <summary>
        /// Main AlgoTrader instance - provides access to traders, indicators, strategies
        /// </summary>
        public AlgoTrader algoTrader { get; set; }

        /// <summary>
        /// Stock data list loaded from file
        /// </summary>
        public List<StockData> stockData { get; set; }

        private readonly Action<string> _outputCallback;

        public ScriptGlobals(AlgoTrader trader, List<StockData> data, Action<string> outputCallback)
        {
            algoTrader = trader;
            stockData = data;
            _outputCallback = outputCallback;
        }

        /// <summary>
        /// Log a message to the script output
        /// </summary>
        public void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var logLine = $"[{timestamp}] {message}";
            _outputCallback?.Invoke(logLine);
        }

        /// <summary>
        /// Log with format string support
        /// </summary>
        public void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        /// <summary>
        /// Clear the output window
        /// </summary>
        public void ClearOutput()
        {
            _outputCallback?.Invoke("[CLEAR]");
        }
    }
}
