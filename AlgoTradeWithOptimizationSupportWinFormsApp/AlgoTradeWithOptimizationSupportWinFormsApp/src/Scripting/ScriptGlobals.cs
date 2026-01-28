using System;
using System.Collections.Generic;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;
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
        private readonly Action<string, object>? _resultCallback;

        // Store subscribed handlers for cleanup
        private Action<SingleTrader, int, int>? _progressHandler;
        private Action<SingleTrader, string, int>? _signalHandler;

        public ScriptGlobals(
            AlgoTrader trader,
            List<StockData> data,
            Action<string> outputCallback,
            Action<string, object>? resultCallback = null)
        {
            algoTrader = trader;
            stockData = data;
            _outputCallback = outputCallback;
            _resultCallback = resultCallback;
        }

        #region Logging

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

        #endregion

        #region Script → Form Communication (Callbacks)

        /// <summary>
        /// Send a result/data from script to the main form.
        /// Form can handle this via OnScriptResult event.
        /// </summary>
        /// <param name="key">Identifier for the result (e.g., "OptResult", "BestParams")</param>
        /// <param name="value">The data to send (can be any object)</param>
        public void SendResult(string key, object value)
        {
            _resultCallback?.Invoke(key, value);
            Log($"[RESULT] {key}: {value}");
        }

        /// <summary>
        /// Send a simple string message to the form
        /// </summary>
        public void SendMessage(string message)
        {
            SendResult("Message", message);
        }

        #endregion

        #region Form → Script Communication (Event Subscriptions)

        /// <summary>
        /// Subscribe to SingleTrader progress events.
        /// Callback receives: (currentBar, totalBars)
        /// </summary>
        /// <param name="callback">Handler to call on progress update</param>
        /// <param name="intervalBars">Call handler every N bars (default: 1000)</param>
        public void OnProgress(Action<int, int> callback, int intervalBars = 1000)
        {
            if (algoTrader?.singleTrader == null)
            {
                Log("[WARNING] SingleTrader not available for OnProgress subscription");
                return;
            }

            // Wrap user callback with interval check
            int lastReported = -intervalBars;
            _progressHandler = (trader, current, total) =>
            {
                if (current - lastReported >= intervalBars || current == total - 1)
                {
                    lastReported = current;
                    callback(current, total);
                }
            };

            algoTrader.singleTrader.OnProgress += _progressHandler;
            Log($"[SUBSCRIBED] OnProgress (every {intervalBars} bars)");
        }

        /// <summary>
        /// Subscribe to strategy signal events.
        /// Callback receives: (signalType, barIndex) where signalType is "A", "S", "F", etc.
        /// </summary>
        public void OnSignal(Action<string, int> callback)
        {
            if (algoTrader?.singleTrader == null)
            {
                Log("[WARNING] SingleTrader not available for OnSignal subscription");
                return;
            }

            _signalHandler = (trader, signal, barIndex) =>
            {
                callback(signal, barIndex);
            };

            algoTrader.singleTrader.OnNotifyStrategySignal += _signalHandler;
            Log("[SUBSCRIBED] OnSignal");
        }

        /// <summary>
        /// Subscribe to trade execution events (when position opens/closes).
        /// Callback receives: (tradeType, price, barIndex, profitLoss)
        /// </summary>
        public void OnTrade(Action<string, double, int, double> callback)
        {
            if (algoTrader?.singleTrader == null)
            {
                Log("[WARNING] SingleTrader not available for OnTrade subscription");
                return;
            }

            // Use OnAfterOrdersCallback to detect trades
            algoTrader.singleTrader.OnAfterOrdersCallback += (trader, barIndex) =>
            {
                var sonYon = trader.SonYon;
                var fiyat = trader.SonSinyalFiyati;
                var kz = trader.SonKarZararFiyat;

                // Only call if there's an active position change
                if (trader.SonSinyalBarIndex == barIndex)
                {
                    callback(sonYon, fiyat, barIndex, kz);
                }
            };
            Log("[SUBSCRIBED] OnTrade");
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Unsubscribe from all events. Call this when script execution ends.
        /// </summary>
        public void Cleanup()
        {
            if (algoTrader?.singleTrader != null)
            {
                if (_progressHandler != null)
                {
                    algoTrader.singleTrader.OnProgress -= _progressHandler;
                    _progressHandler = null;
                }

                if (_signalHandler != null)
                {
                    algoTrader.singleTrader.OnNotifyStrategySignal -= _signalHandler;
                    _signalHandler = null;
                }
            }
        }

        #endregion

        #region Helper Properties

        /// <summary>
        /// Quick access to SingleTrader
        /// </summary>
        public SingleTrader? Trader => algoTrader?.singleTrader;

        /// <summary>
        /// Quick access to current equity (if available)
        /// </summary>
        public double Equity => algoTrader?.singleTrader?.Bakiye?.Equity ?? 0;

        /// <summary>
        /// Total number of bars in data
        /// </summary>
        public int TotalBars => stockData?.Count ?? 0;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Run SingleTrader for all bars (convenience method).
        /// Equivalent to: for(int i=0; i &lt; Data.Count; i++) singleTrader.Run(i);
        /// </summary>
        /// <param name="progressInterval">Log progress every N bars (0 = no logging)</param>
        public void RunAll(int progressInterval = 0)
        {
            if (algoTrader?.singleTrader == null)
            {
                Log("[ERROR] SingleTrader not available");
                return;
            }

            if (stockData == null || stockData.Count == 0)
            {
                Log("[ERROR] No stock data loaded");
                return;
            }

            var total = stockData.Count;
            for (int i = 0; i < total; i++)
            {
                algoTrader.singleTrader.Run(i);

                // Progress logging
                if (progressInterval > 0 && (i % progressInterval == 0 || i == total - 1))
                {
                    Log($"Progress: {i + 1}/{total} ({100.0 * (i + 1) / total:F1}%)");
                }

                // Fire OnProgress event
                _progressHandler?.Invoke(algoTrader.singleTrader, i, total);
            }
        }

        /// <summary>
        /// Initialize AlgoTrader with current stockData, configure a strategy, and prepare SingleTrader.
        /// This is the main setup method for script execution.
        /// </summary>
        public void Setup(string strategyName, Dictionary<string, object>? parameters = null)
        {
            if (algoTrader == null)
            {
                Log("[ERROR] AlgoTrader not available");
                return;
            }

            if (stockData == null || stockData.Count == 0)
            {
                Log("[ERROR] No stock data loaded");
                return;
            }

            // 1. Initialize with data
            algoTrader.Initialize(stockData);
            Log($"AlgoTrader initialized with {stockData.Count} bars");

            // 2. Configure strategy
            if (!string.IsNullOrEmpty(strategyName))
            {
                algoTrader.ConfigureStrategy(strategyName, parameters ?? new Dictionary<string, object>());
                Log($"Strategy configured: {strategyName}");
            }

            // 3. Prepare SingleTrader (creates indicators, singleTrader, strategy instance)
            algoTrader.PrepareSingleTrader();
            Log("SingleTrader ready");
        }

        #endregion
    }
}
