using System;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Base;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Trend.Results;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Trend
{
    /// <summary>
    /// Trend Indicators - SuperTrend, MOST, ADX, Parabolic SAR, Ichimoku, etc.
    /// TODO: Implement remaining indicators
    /// </summary>
    public class TrendIndicators
    {
        private readonly IndicatorManager _manager;
        private readonly IndicatorConfig _config;

        public TrendIndicators(IndicatorManager manager, IndicatorConfig config)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// SuperTrend Indicator
        ///
        /// SuperTrend is an ATR-based trend-following indicator:
        /// - In uptrend: SuperTrend stays below price (support level)
        /// - In downtrend: SuperTrend stays above price (resistance level)
        ///
        /// Trading Logic:
        /// - Buy when direction changes from -1 to 1 (trend reversal to uptrend)
        /// - Sell when direction changes from 1 to -1 (trend reversal to downtrend)
        /// </summary>
        /// <param name="period">ATR period (default: 10)</param>
        /// <param name="multiplier">ATR multiplier for bands (default: 3.0)</param>
        /// <returns>SuperTrendResult containing supertrend values and direction</returns>
        public SuperTrendResult SuperTrend(int period = 10, double multiplier = 3.0)
        {
            int barCount = _manager.BarCount;
            if (barCount == 0)
                return new SuperTrendResult
                {
                    SuperTrend = new double[0],
                    Direction = new int[0]
                };

            // Get OHLC data
            double[] highs = _manager.GetHighPrices();
            double[] lows = _manager.GetLowPrices();
            double[] closes = _manager.GetClosePrices();

            // Calculate True Range
            double[] tr = new double[barCount];
            tr[0] = highs[0] - lows[0];

            for (int i = 1; i < barCount; i++)
            {
                double tr1 = highs[i] - lows[i];
                double tr2 = Math.Abs(highs[i] - closes[i - 1]);
                double tr3 = Math.Abs(lows[i] - closes[i - 1]);
                tr[i] = Math.Max(tr1, Math.Max(tr2, tr3));
            }

            // Calculate ATR (EMA of True Range)
            double[] atr = CalculateEMA(tr, period);

            // Calculate basic upper and lower bands
            double[] upperBand = new double[barCount];
            double[] lowerBand = new double[barCount];

            for (int i = 0; i < barCount; i++)
            {
                double hl2 = (highs[i] + lows[i]) / 2.0;
                upperBand[i] = hl2 + (multiplier * atr[i]);
                lowerBand[i] = hl2 - (multiplier * atr[i]);
            }

            // Calculate final upper and lower bands
            double[] finalUpperBand = new double[barCount];
            double[] finalLowerBand = new double[barCount];

            finalUpperBand[0] = upperBand[0];
            finalLowerBand[0] = lowerBand[0];

            for (int i = 1; i < barCount; i++)
            {
                // Final Upper Band
                if (closes[i - 1] <= finalUpperBand[i - 1])
                    finalUpperBand[i] = Math.Min(upperBand[i], finalUpperBand[i - 1]);
                else
                    finalUpperBand[i] = upperBand[i];

                // Final Lower Band
                if (closes[i - 1] >= finalLowerBand[i - 1])
                    finalLowerBand[i] = Math.Max(lowerBand[i], finalLowerBand[i - 1]);
                else
                    finalLowerBand[i] = lowerBand[i];
            }

            // Calculate SuperTrend and Direction
            double[] supertrend = new double[barCount];
            int[] direction = new int[barCount];

            supertrend[0] = finalLowerBand[0];
            direction[0] = 1;

            for (int i = 1; i < barCount; i++)
            {
                // Determine direction
                if (closes[i] > finalUpperBand[i - 1])
                    direction[i] = 1;
                else if (closes[i] < finalLowerBand[i - 1])
                    direction[i] = -1;
                else
                {
                    direction[i] = direction[i - 1];

                    // Adjust bands if direction continues
                    if (direction[i] == 1 && finalLowerBand[i] > finalLowerBand[i - 1])
                        finalLowerBand[i] = finalLowerBand[i - 1];

                    if (direction[i] == -1 && finalUpperBand[i] < finalUpperBand[i - 1])
                        finalUpperBand[i] = finalUpperBand[i - 1];
                }

                // Set SuperTrend value based on direction
                supertrend[i] = direction[i] == 1 ? finalLowerBand[i] : finalUpperBand[i];
            }

            return new SuperTrendResult
            {
                SuperTrend = supertrend,
                Direction = direction
            };
        }

        /// <summary>
        /// Helper method to calculate EMA (internal use)
        /// </summary>
        private double[] CalculateEMA(double[] source, int period)
        {
            if (source.Length == 0)
                return new double[0];

            double alpha = 2.0 / (period + 1);
            double[] ema = new double[source.Length];

            // First value is the source value itself
            ema[0] = source[0];

            // Calculate EMA
            for (int i = 1; i < source.Length; i++)
            {
                ema[i] = alpha * source[i] + (1 - alpha) * ema[i - 1];
            }

            return ema;
        }

        /// <summary>
        /// MOST (Moving Stop Loss) Indicator
        ///
        /// MOST is a trend-following indicator that acts as a trailing stop loss:
        /// - In uptrend: MOST stays below price (support)
        /// - In downtrend: MOST stays above price (resistance)
        ///
        /// Trading Logic:
        /// - Buy when price crosses MOST upward (trend changes from down to up)
        /// - Sell when price crosses MOST downward (trend changes from up to down)
        /// </summary>
        /// <param name="period">EMA period (default: 21)</param>
        /// <param name="percent">Band percentage (default: 1.0)</param>
        /// <returns>Tuple of (most, exmov) arrays</returns>
        public (double[] most, double[] exmov) MOST(int period = 21, double percent = 1.0)
        {
            int barCount = _manager.BarCount;
            if (barCount == 0)
                return (new double[0], new double[0]);

            // Get close prices
            double[] closes = _manager.GetClosePrices();

            // Calculate EMA (exmov)
            double[] exmov = _manager.MA.EMA(closes, period);

            // Calculate bands
            double[] fark = new double[barCount];
            double[] up = new double[barCount];
            double[] down = new double[barCount];

            for (int i = 0; i < barCount; i++)
            {
                fark[i] = exmov[i] * (percent / 100.0);
                up[i] = exmov[i] - fark[i];
                down[i] = exmov[i] + fark[i];
            }

            // Initialize arrays
            double[] trendUp = new double[barCount];
            double[] trendDown = new double[barCount];
            int[] trend = new int[barCount];
            double[] most = new double[barCount];

            // Initialize first values
            trendUp[0] = up[0];
            trendDown[0] = down[0];
            trend[0] = 1;
            most[0] = trendUp[0];

            // Calculate MOST for each bar
            for (int i = 1; i < barCount; i++)
            {
                // Pine Script: TrendUp = prev(ExMov,1)>prev(TrendUp,1) ? max(Up,prev(TrendUp,1)) : Up
                if (exmov[i - 1] > trendUp[i - 1])
                    trendUp[i] = Math.Max(up[i], trendUp[i - 1]);
                else
                    trendUp[i] = up[i];

                // Pine Script: TrendDown = prev(ExMov,1)<prev(TrendDown,1) ? min(Down,prev(TrendDown,1)) : Down
                if (exmov[i - 1] < trendDown[i - 1])
                    trendDown[i] = Math.Min(down[i], trendDown[i - 1]);
                else
                    trendDown[i] = down[i];

                // Pine Script: Trend = ExMov>prev(TrendDown,1) ? 1 : ExMov<prev(TrendUp,1) ? -1 : prev(Trend,1)
                if (exmov[i] > trendDown[i - 1])
                    trend[i] = 1;
                else if (exmov[i] < trendUp[i - 1])
                    trend[i] = -1;
                else
                    trend[i] = trend[i - 1];

                // Pine Script: MOST = Trend==1 ? TrendUp : TrendDown
                most[i] = trend[i] == 1 ? trendUp[i] : trendDown[i];
            }

            return (most, exmov);
        }

        /// <summary>
        /// Average Directional Index
        /// </summary>
        public double[] ADX(int period = 14)
        {
            // TODO: Implement ADX calculation
            throw new NotImplementedException("ADX not yet implemented - coming soon!");
        }

        /// <summary>
        /// Parabolic SAR
        /// </summary>
        public (double[] sar, bool[] trend) ParabolicSAR(double step = 0.02, double max = 0.2)
        {
            // TODO: Implement Parabolic SAR calculation
            throw new NotImplementedException("Parabolic SAR not yet implemented - coming soon!");
        }
    }
}
