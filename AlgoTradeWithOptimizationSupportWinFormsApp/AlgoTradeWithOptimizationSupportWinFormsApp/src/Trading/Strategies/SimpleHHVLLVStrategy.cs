using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using System;
using System.Collections.Generic;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategies
{
    /// <summary>
    /// HHV/LLV (Highest High Value / Lowest Low Value) Breakout Stratejisi
    ///
    /// HHV/LLV Mantığı:
    /// - Belirli periyottaki en yüksek ve en düşük seviyeleri izler
    /// - Kırılım stratejisi: seviyeler aşıldığında sinyal üretir
    ///
    /// Trading Logic:
    /// - AL: Fiyat HHV seviyesini yukarı kırıyor (breakout)
    /// - SAT: Fiyat LLV seviyesini aşağı kırıyor (breakdown)
    ///
    /// Parametreler:
    /// - period: HHV/LLV lookback periyodu (varsayılan 20)
    /// </summary>
    public class SimpleHHVLLVStrategy : BaseStrategy
    {
        public override string Name => "Simple HHV/LLV Strategy";

        private readonly int _period;
        private double[]? _hhv;
        private double[]? _llv;

        public SimpleHHVLLVStrategy(int period = 20)
        {
            _period = period;

            Parameters["Period"] = period;
        }

        public SimpleHHVLLVStrategy(List<StockData> data, IndicatorManager indicators, int period = 20)
        {
            _period = period;

            Parameters["Period"] = period;

            Initialize(data, indicators);
        }

        public override void OnInit()
        {
            if (!IsInitialized)
                return;

            var highs = Indicators.GetHighPrices();
            var lows = Indicators.GetLowPrices();

            _hhv = Indicators.Utils.HHV(highs, _period);
            _llv = Indicators.Utils.LLV(lows, _period);

            LogManager.Log($"SimpleHHVLLVStrategy initialized: Period={_period}");
        }

        public override TradeSignals OnStep(int currentIndex)
        {
            bool buy = false;
            bool sell = false;
            bool takeProfit = false;
            bool stopLoss = false;
            bool flat = false;
            bool skip = false;

            if (currentIndex < _period + 1)
                return TradeSignals.None;

            if (_hhv == null || _llv == null || _hhv.Length == 0 || _llv.Length == 0)
                return TradeSignals.None;

            double currentClose = Data[currentIndex].Close;
            double prevClose = Data[currentIndex - 1].Close;

            // Previous period's HHV/LLV (shift 1)
            double prevHHV = _hhv[currentIndex - 1];
            double prevLLV = _llv[currentIndex - 1];

            if (double.IsNaN(prevHHV) || double.IsNaN(prevLLV))
                return TradeSignals.None;

            // AL: Fiyat önceki periyodun HHV'sini yukarı kırıyor
            if (prevClose <= prevHHV && currentClose > prevHHV)
            {
                buy = true;
            }

            // SAT: Fiyat önceki periyodun LLV'sini aşağı kırıyor
            if (prevClose >= prevLLV && currentClose < prevLLV)
            {
                sell = true;
            }

            if (Trader != null)
            {
                takeProfit = Trader.karAlZararKes.SonFiyataGoreKarAlSeviyeHesaplaSeviyeli(currentIndex, 5, 50, 1000) != 0;
            }

            if (Trader != null)
            {
                stopLoss = Trader.karAlZararKes.SonFiyataGoreZararKesSeviyeHesaplaSeviyeli(currentIndex, -1, -10, 1000) != 0;
            }

            if (skip) return TradeSignals.Skip;
            else if (flat) return TradeSignals.Flat;
            else if (takeProfit) return TradeSignals.TakeProfit;
            else if (stopLoss) return TradeSignals.StopLoss;
            else if (buy) return TradeSignals.Buy;
            else if (sell) return TradeSignals.Sell;

            return TradeSignals.None;
        }

        public double[]? GetHHV() => _hhv;
        public double[]? GetLLV() => _llv;

        public override Dictionary<string, double[]>? GetPlotIndicators()
        {
            var indicators = new Dictionary<string, double[]>();

            var closes = Indicators.GetClosePrices();
            if (closes != null && closes.Length > 0)
                indicators["Close"] = closes;

            if (_hhv != null && _hhv.Length > 0)
                indicators["HHV"] = _hhv;

            if (_llv != null && _llv.Length > 0)
                indicators["LLV"] = _llv;

            return indicators.Count > 0 ? indicators : null;
        }
    }
}
