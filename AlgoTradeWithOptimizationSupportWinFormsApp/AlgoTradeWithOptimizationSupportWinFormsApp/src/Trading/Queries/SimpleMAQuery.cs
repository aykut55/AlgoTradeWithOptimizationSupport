using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Query;
using System;
using System.Collections.Generic;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Queries
{
    /// <summary>
    /// Simple Moving Average Query
    /// Returns MA values, cross signals, and distance metrics for the last bar
    /// Example query results:
    /// [0] = Close
    /// [1] = MA8
    /// [2] = MA200
    /// [3] = CrossSignal (1 if MA8 > MA200, -1 otherwise)
    /// [4] = Distance% = (Close - MA200) / MA200 * 100
    /// </summary>
    public class SimpleMAQuery : BaseQuery
    {
        public override string Name => "Simple MA Query";

        private readonly int _ma8Period;
        private readonly int _ma200Period;
        private double[]? _ma8;
        private double[]? _ma200;

        // Parametresiz constructor (eski kullanımlar için)
        public SimpleMAQuery(int ma8Period = 8, int ma200Period = 200)
        {
            _ma8Period = ma8Period;
            _ma200Period = ma200Period;

            Parameters["MA8Period"] = ma8Period;
            Parameters["MA200Period"] = ma200Period;
        }

        // Parametreli constructor (yeni kullanım)
        public SimpleMAQuery(List<StockData> data, IndicatorManager indicators,
                            int ma8Period = 8, int ma200Period = 200)
        {
            _ma8Period = ma8Period;
            _ma200Period = ma200Period;

            Parameters["MA8Period"] = ma8Period;
            Parameters["MA200Period"] = ma200Period;

            // Initialize base query
            Initialize(data, indicators);
        }

        public override void OnInit()
        {
            if (!IsInitialized)
                return;

            // Calculate moving averages
            var closes = Indicators.GetClosePrices();
            _ma8 = Indicators.MA.SMA(closes, _ma8Period);
            _ma200 = Indicators.MA.SMA(closes, _ma200Period);

            LogManager.Log($"Query initialized: MA8={_ma8Period}, MA200={_ma200Period}");
        }

        public override List<object> OnExecute(int lastBarIndex)
        {
            var results = new List<object>();

            // Check if we have enough data
            if (lastBarIndex < _ma200Period)
            {
                LogManager.LogWarning($"Not enough data for query. Need {_ma200Period} bars, have {lastBarIndex + 1}");
                return results;
            }

            // Results[0] = Close[LastBar]
            double lastClose = Data[lastBarIndex].Close;
            results.Add(lastClose);

            // Results[1] = MA8[LastBar]
            double lastMA8 = _ma8[lastBarIndex];
            results.Add(lastMA8);

            // Results[2] = MA200[LastBar]
            double lastMA200 = _ma200[lastBarIndex];
            results.Add(lastMA200);

            // Results[3] = CrossSignal: (MA8[LastBar] > MA200[LastBar]) ? 1 : -1
            int crossSignal = lastMA8 > lastMA200 ? 1 : -1;
            results.Add(crossSignal);

            // Results[4] = Distance percentage: (Close - MA200) / MA200 * 100
            double distancePercent = (lastClose - lastMA200) / lastMA200 * 100.0;
            results.Add(distancePercent);

            // Optional: Add strategy signal if trader has a strategy
            if (Trader?.Strategy != null)
            {
                // Results[5] = Strategy Signal
                var signal = Trader.StrategySignal;
                results.Add(signal.ToString());
            }

            // Optional: Add position info if trader has open position
            if (Trader != null)
            {
                // Results[6] = Last Signal Direction (A/S/F)
                results.Add(Trader.SonYon ?? "F");

                // Results[7] = Bars since last signal
                results.Add(Trader.SonSinyaldenBeriBarSayisi);

                // Results[8] = Current P&L
                results.Add(Trader.SonKarZararFiyat);

                // Results[9] = Current P&L %
                results.Add(Trader.SonKarZararYuzde);
            }

            return results;
        }
    }
}
