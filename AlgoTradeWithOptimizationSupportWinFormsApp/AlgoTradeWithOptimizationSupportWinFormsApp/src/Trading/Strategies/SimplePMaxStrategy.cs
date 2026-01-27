using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Trend.Results;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using System;
using System.Collections.Generic;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategies
{
    /// <summary>
    /// PMax (Profit Maximizer) İndikatörü Stratejisi
    ///
    /// PMax Mantığı:
    /// - MOST + SuperTrend hibrit yapısı
    /// - ATR tabanlı trailing stop ile hareketli ortalama kombinasyonu
    ///
    /// Trading Logic:
    /// - AL: Direction -1'den 1'e değişirse (trend dönüşü)
    /// - SAT: Direction 1'den -1'e değişirse (trend dönüşü)
    ///
    /// Parametreler:
    /// - atrPeriod: ATR periyodu (varsayılan 10)
    /// - multiplier: ATR çarpanı (varsayılan 3.0)
    /// - maPeriod: MA periyodu (varsayılan 10)
    /// </summary>
    public class SimplePMaxStrategy : BaseStrategy
    {
        public override string Name => "Simple PMax Strategy";

        private readonly int _atrPeriod;
        private readonly double _multiplier;
        private readonly int _maPeriod;
        private PMaxResult? _pmaxResult;

        public SimplePMaxStrategy(int atrPeriod = 10, double multiplier = 3.0, int maPeriod = 10)
        {
            _atrPeriod = atrPeriod;
            _multiplier = multiplier;
            _maPeriod = maPeriod;

            Parameters["ATRPeriod"] = atrPeriod;
            Parameters["Multiplier"] = multiplier;
            Parameters["MAPeriod"] = maPeriod;
        }

        public SimplePMaxStrategy(List<StockData> data, IndicatorManager indicators, int atrPeriod = 10, double multiplier = 3.0, int maPeriod = 10)
        {
            _atrPeriod = atrPeriod;
            _multiplier = multiplier;
            _maPeriod = maPeriod;

            Parameters["ATRPeriod"] = atrPeriod;
            Parameters["Multiplier"] = multiplier;
            Parameters["MAPeriod"] = maPeriod;

            Initialize(data, indicators);
        }

        public override void OnInit()
        {
            if (!IsInitialized)
                return;

            _pmaxResult = Indicators.Trend.PMax(_atrPeriod, _multiplier, _maPeriod);

            LogManager.Log($"SimplePMaxStrategy initialized: ATRPeriod={_atrPeriod}, Multiplier={_multiplier}, MAPeriod={_maPeriod}");
        }

        public override TradeSignals OnStep(int currentIndex)
        {
            bool buy = false;
            bool sell = false;
            bool takeProfit = false;
            bool stopLoss = false;
            bool flat = false;
            bool skip = false;

            if (currentIndex < Math.Max(_atrPeriod, _maPeriod) + 1)
                return TradeSignals.None;

            if (_pmaxResult == null || _pmaxResult.Direction.Length == 0)
                return TradeSignals.None;

            int currentDirection = _pmaxResult.Direction[currentIndex];
            int prevDirection = _pmaxResult.Direction[currentIndex - 1];

            // AL: Direction -1'den 1'e değişiyor
            if (prevDirection == -1 && currentDirection == 1)
            {
                buy = true;
            }

            // SAT: Direction 1'den -1'e değişiyor
            if (prevDirection == 1 && currentDirection == -1)
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

        public double[]? GetPMax() => _pmaxResult?.PMax;
        public double[]? GetMA() => _pmaxResult?.PMaxMA;
        public int[]? GetDirection() => _pmaxResult?.Direction;

        public override Dictionary<string, double[]>? GetPlotIndicators()
        {
            var indicators = new Dictionary<string, double[]>();

            var closes = Indicators.GetClosePrices();
            if (closes != null && closes.Length > 0)
                indicators["Close"] = closes;

            if (_pmaxResult?.PMax != null && _pmaxResult.PMax.Length > 0)
                indicators["PMax"] = _pmaxResult.PMax;

            if (_pmaxResult?.PMaxMA != null && _pmaxResult.PMaxMA.Length > 0)
                indicators["MA"] = _pmaxResult.PMaxMA;

            return indicators.Count > 0 ? indicators : null;
        }
    }
}
