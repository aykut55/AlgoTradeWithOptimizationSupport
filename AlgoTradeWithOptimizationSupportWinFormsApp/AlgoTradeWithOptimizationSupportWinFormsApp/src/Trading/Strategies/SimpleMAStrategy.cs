using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using ScottPlot.TickGenerators.Financial;
using System;
using System.Collections.Generic;
using static Nessos.LinqOptimizer.Core.QueryExpr;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategies
{
    /// <summary>
    /// Basit Moving Average Crossover Stratejisi
    /// Hızlı MA yavaş MA'yı yukarı keserse AL, aşağı keserse SAT
    /// </summary>
    public class SimpleMAStrategy : BaseStrategy
    {
        public override string Name => "Simple MA Crossover";

        private readonly int _fastPeriod;
        private readonly int _slowPeriod;
        private double[]? _fastMA;
        private double[]? _slowMA;

        // Parametresiz constructor (eski kullanımlar için)
        public SimpleMAStrategy(int fastPeriod = 10, int slowPeriod = 20)
        {
            _fastPeriod = fastPeriod;
            _slowPeriod = slowPeriod;

            Parameters["FastPeriod"] = fastPeriod;
            Parameters["SlowPeriod"] = slowPeriod;
        }

        // Parametreli constructor (yeni kullanım)
        public SimpleMAStrategy(List<StockData> data, IndicatorManager indicators, int fastPeriod = 10, int slowPeriod = 20)
        {
            _fastPeriod = fastPeriod;
            _slowPeriod = slowPeriod;

            Parameters["FastPeriod"] = fastPeriod;
            Parameters["SlowPeriod"] = slowPeriod;

            // Initialize base strategy
            Initialize(data, indicators);
        }

        public override void OnInit()
        {
            if (!IsInitialized)
                return;

            // Moving average'leri hesapla
            var closes = Indicators.GetClosePrices();
            _fastMA = Indicators.MA.SMA(closes, _fastPeriod);
            _slowMA = Indicators.MA.SMA(closes, _slowPeriod);

            LogManager.Log($"Strategy initialized: Fast={_fastPeriod}, Slow={_slowPeriod}");
        }

        public override TradeSignals OnStep(int currentIndex)
        {
            bool buy = false;
            bool sell = false;
            bool takeProfit = false;
            bool stopLoss = false;
            bool flat = false;
            bool skip = false;

            // İlk barlarda yeterli veri yok
            if (currentIndex < _slowPeriod)
                return TradeSignals.None;

            // Geçerli ve önceki MA değerleri
            double currentFastMA = _fastMA[currentIndex];
            double currentSlowMA = _slowMA[currentIndex];
            double prevFastMA = _fastMA[currentIndex - 1];
            double prevSlowMA = _slowMA[currentIndex - 1];

            // Golden Cross (Hızlı MA yukarı kesiyor) - AL sinyali
            if (prevFastMA <= prevSlowMA && currentFastMA > currentSlowMA)
            {
                buy = true;
            }

            // Death Cross (Hızlı MA aşağı kesiyor) - SAT sinyali
            if (prevFastMA >= prevSlowMA && currentFastMA < currentSlowMA)
            {
                sell = true;
            }

            if (1 == 2)
            {
                takeProfit = true;
            }

            if (2 == 3)
            {
                stopLoss = true;
            }

            if (3 == 4)
            {
                flat = true;
            }

            if (4 == 5)
            {
                skip = true;
            }

            if (skip)
            {
                return TradeSignals.None;
            }
            else if (flat)
            {
                return TradeSignals.Flat;
            }
            else if (takeProfit)
            {
                return TradeSignals.TakeProfit;
            }
            else if (stopLoss)
            {
                return TradeSignals.StopLoss;
            }
            else if (buy)
            {
                return TradeSignals.Buy;
            }
            else if (sell)
            {
                return TradeSignals.Sell;
            }

            return TradeSignals.None;
        }
    }
}
