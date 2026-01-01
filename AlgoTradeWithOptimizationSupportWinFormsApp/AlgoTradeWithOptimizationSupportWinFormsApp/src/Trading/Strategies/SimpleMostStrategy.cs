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
    /// MOST (Moving Stop Loss) İndikatörü Stratejisi
    ///
    /// MOST Mantığı:
    /// - Yükseliş trendinde: MOST fiyatın altında stop loss görevi görür
    /// - Düşüş trendinde: MOST fiyatın üstünde direnç görevi görür
    /// - Fiyat MOST'u yukarı kırınca AL (trend değişimi)
    /// - Fiyat MOST'u aşağı kırınca SAT (trend değişimi)
    ///
    /// Parametreler:
    /// - period: MOST periyodu (varsayılan 21)
    /// - percent: MOST yüzde sapması (varsayılan 1.0)
    /// </summary>
    public class SimpleMostStrategy : BaseStrategy
    {
        public override string Name => "Simple MOST Strategy";

        private readonly int _period;
        private readonly double _percent;
        private double[]? _most;
        private double[]? _exmov;

        // Parametresiz constructor (eski kullanımlar için)
        public SimpleMostStrategy(int period = 21, double percent = 1.0)
        {
            _period = period;
            _percent = percent;

            Parameters["Period"] = period;
            Parameters["Percent"] = percent;
        }

        // Parametreli constructor (yeni kullanım)
        public SimpleMostStrategy(List<StockData> data, IndicatorManager indicators, int period = 21, double percent = 1.0)
        {
            _period = period;
            _percent = percent;

            Parameters["Period"] = period;
            Parameters["Percent"] = percent;

            // Initialize base strategy
            Initialize(data, indicators);
        }

        public override void OnInit()
        {
            if (!IsInitialized)
                return;

            try
            {
                // MOST indicator'ı hesapla
                // NOT: MOST henüz TrendIndicators.cs içinde implement edilmemiş
                // Implement edildikten sonra bu satır çalışacak
                (_most, _exmov) = Indicators.Trend.MOST(_period, _percent);

                LogManager.Log($"SimpleMostStrategy initialized: Period={_period}, Percent={_percent}");
            }
            catch (NotImplementedException)
            {
                LogManager.LogWarning($"MOST indicator not yet implemented! Strategy will not generate signals.");
                LogManager.LogWarning($"To implement MOST, edit src/Trading/Indicators/Trend/TrendIndicators.cs");

                // MOST henüz implement edilmemiş, boş arrayler oluştur
                int barCount = Indicators.BarCount;
                _most = new double[barCount];
                _exmov = new double[barCount];
            }
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
            if (currentIndex < _period)
                return TradeSignals.None;

            // MOST implement edilmemişse sinyal üretme
            if (_most == null || _most.Length == 0)
                return TradeSignals.None;

            // Geçerli ve önceki değerler
            double currentPrice = Data[currentIndex].Close;
            double prevPrice = Data[currentIndex - 1].Close;
            double currentMost = _most[currentIndex];
            double prevMost = _most[currentIndex - 1];

            // MOST AL Sinyali: Fiyat MOST'u yukarı kırıyor (trend değişimi: düşüşten yükselişe)
            // Önceki bar: fiyat <= MOST
            // Şimdiki bar: fiyat > MOST
            if (prevPrice <= prevMost && currentPrice > currentMost)
            {
                buy = true;
            }

            // MOST SAT Sinyali: Fiyat MOST'u aşağı kırıyor (trend değişimi: yükselişten düşüşe)
            // Önceki bar: fiyat >= MOST
            // Şimdiki bar: fiyat < MOST
            if (prevPrice >= prevMost && currentPrice < currentMost)
            {
                sell = true;
            }

            // Opsiyonel: Take Profit / Stop Loss mantığı eklenebilir
            // Şu an için placeholder'lar
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

            // Sinyal önceliklendirmesi
            if (skip)
            {
                return TradeSignals.Skip;
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

        /// <summary>
        /// MOST değerlerini al (plotting veya analiz için)
        /// </summary>
        public double[]? GetMOST() => _most;

        /// <summary>
        /// EXMOV değerlerini al (plotting veya analiz için)
        /// </summary>
        public double[]? GetEXMOV() => _exmov;

        /// <summary>
        /// Period parametresini al
        /// </summary>
        public int Period => _period;

        /// <summary>
        /// Percent parametresini al
        /// </summary>
        public double Percent => _percent;
    }
}
