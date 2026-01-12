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
        /// <returns>MOSTResult containing most and exmov arrays</returns>
        public MOSTResult MOST(int period = 21, double percent = 1.0)
        {
            int barCount = _manager.BarCount;
            if (barCount == 0)
                return new MOSTResult(new double[0], new double[0], period, percent);

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

            return new MOSTResult(most, exmov, period, percent);
        }

        /// <summary>
        /// Average Directional Index
        /// Measures trend strength (not direction)
        /// ADX > 25 = strong trend, ADX < 20 = weak trend/ranging
        /// Returns only ADX values
        /// </summary>
        public double[] ADX(int period = 14)
        {
            var result = ADXWithDI(period);
            return result.ADX;
        }

        /// <summary>
        /// Average Directional Index with Directional Indicators
        /// Returns ADX along with +DI and -DI for complete directional analysis
        /// +DI > -DI = uptrend, -DI > +DI = downtrend
        /// </summary>
        public ADXResult ADXWithDI(int period = 14)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();
            var closes = _manager.GetClosePrices();

            var length = closes.Length;
            var adx = new double[length];
            var plusDI = new double[length];
            var minusDI = new double[length];

            if (length < period + 1)
            {
                for (int i = 0; i < length; i++)
                {
                    adx[i] = double.NaN;
                    plusDI[i] = double.NaN;
                    minusDI[i] = double.NaN;
                }
                return new ADXResult(adx, plusDI, minusDI, period);
            }

            // Calculate True Range and Directional Movements
            var tr = new double[length];
            var plusDM = new double[length];
            var minusDM = new double[length];

            tr[0] = highs[0] - lows[0];
            plusDM[0] = 0;
            minusDM[0] = 0;

            for (int i = 1; i < length; i++)
            {
                // True Range
                var tr1 = highs[i] - lows[i];
                var tr2 = Math.Abs(highs[i] - closes[i - 1]);
                var tr3 = Math.Abs(lows[i] - closes[i - 1]);
                tr[i] = Math.Max(tr1, Math.Max(tr2, tr3));

                // Directional Movements
                var highDiff = highs[i] - highs[i - 1];
                var lowDiff = lows[i - 1] - lows[i];

                if (highDiff > lowDiff && highDiff > 0)
                    plusDM[i] = highDiff;
                else
                    plusDM[i] = 0;

                if (lowDiff > highDiff && lowDiff > 0)
                    minusDM[i] = lowDiff;
                else
                    minusDM[i] = 0;
            }

            // Apply Wilder's smoothing
            var smoothedTR = _manager.MA.Wilder(tr, period);
            var smoothedPlusDM = _manager.MA.Wilder(plusDM, period);
            var smoothedMinusDM = _manager.MA.Wilder(minusDM, period);

            // Calculate +DI and -DI
            var dx = new double[length];

            for (int i = 0; i < length; i++)
            {
                if (i < period - 1)
                {
                    plusDI[i] = double.NaN;
                    minusDI[i] = double.NaN;
                    dx[i] = 0;
                    adx[i] = double.NaN;
                }
                else
                {
                    if (smoothedTR[i] > 0)
                    {
                        plusDI[i] = 100.0 * smoothedPlusDM[i] / smoothedTR[i];
                        minusDI[i] = 100.0 * smoothedMinusDM[i] / smoothedTR[i];
                    }
                    else
                    {
                        plusDI[i] = 0;
                        minusDI[i] = 0;
                    }

                    // Calculate DX
                    var diSum = plusDI[i] + minusDI[i];
                    if (diSum > 0)
                    {
                        dx[i] = 100.0 * Math.Abs(plusDI[i] - minusDI[i]) / diSum;
                    }
                    else
                    {
                        dx[i] = 0;
                    }
                }
            }

            // Calculate ADX (Wilder's smoothing of DX)
            var smoothedDX = _manager.MA.Wilder(dx, period);

            for (int i = 0; i < length; i++)
            {
                if (i < (period * 2) - 2)
                {
                    adx[i] = double.NaN;
                }
                else
                {
                    adx[i] = smoothedDX[i];
                }
            }

            return new ADXResult(adx, plusDI, minusDI, period);
        }

        /// <summary>
        /// Parabolic SAR (Stop and Reverse)
        /// Trailing stop indicator that follows price
        /// SAR below price = uptrend, SAR above price = downtrend
        /// Trend reverses when price crosses SAR
        /// </summary>
        /// <param name="step">Acceleration factor step (default: 0.02)</param>
        /// <param name="max">Maximum acceleration factor (default: 0.2)</param>
        /// <returns>ParabolicSARResult containing sar values and trend direction</returns>
        public ParabolicSARResult ParabolicSAR(double step = 0.02, double max = 0.2)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (step <= 0 || max <= 0)
                throw new ArgumentException("Step and max must be positive");
            if (step > max)
                throw new ArgumentException("Step cannot be greater than max");

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();
            var closes = _manager.GetClosePrices();

            var length = closes.Length;
            var sar = new double[length];
            var trend = new bool[length]; // true = uptrend, false = downtrend

            if (length < 2)
            {
                if (length == 1)
                {
                    sar[0] = closes[0];
                    trend[0] = true;
                }
                return new ParabolicSARResult(sar, trend, step, max);
            }

            // Initialize
            var af = step; // Acceleration Factor
            var ep = highs[0]; // Extreme Point
            trend[0] = true; // Start with uptrend
            sar[0] = lows[0];

            for (int i = 1; i < length; i++)
            {
                // Calculate SAR for current bar
                sar[i] = sar[i - 1] + af * (ep - sar[i - 1]);

                var isUptrend = trend[i - 1];

                if (isUptrend)
                {
                    // Uptrend: SAR should be below price
                    // SAR cannot be above prior two lows
                    if (i >= 1 && sar[i] > lows[i - 1])
                        sar[i] = lows[i - 1];
                    if (i >= 2 && sar[i] > lows[i - 2])
                        sar[i] = lows[i - 2];

                    // Check for trend reversal
                    if (lows[i] < sar[i])
                    {
                        // Trend reverses to downtrend
                        trend[i] = false;
                        sar[i] = ep; // SAR becomes previous EP
                        ep = lows[i]; // EP becomes current low
                        af = step; // Reset AF
                    }
                    else
                    {
                        // Continue uptrend
                        trend[i] = true;

                        // Update EP if new high
                        if (highs[i] > ep)
                        {
                            ep = highs[i];
                            af = Math.Min(af + step, max);
                        }
                    }
                }
                else
                {
                    // Downtrend: SAR should be above price
                    // SAR cannot be below prior two highs
                    if (i >= 1 && sar[i] < highs[i - 1])
                        sar[i] = highs[i - 1];
                    if (i >= 2 && sar[i] < highs[i - 2])
                        sar[i] = highs[i - 2];

                    // Check for trend reversal
                    if (highs[i] > sar[i])
                    {
                        // Trend reverses to uptrend
                        trend[i] = true;
                        sar[i] = ep; // SAR becomes previous EP
                        ep = highs[i]; // EP becomes current high
                        af = step; // Reset AF
                    }
                    else
                    {
                        // Continue downtrend
                        trend[i] = false;

                        // Update EP if new low
                        if (lows[i] < ep)
                        {
                            ep = lows[i];
                            af = Math.Min(af + step, max);
                        }
                    }
                }
            }

            return new ParabolicSARResult(sar, trend, step, max);
        }

        /// <summary>
        /// Aroon Indicator
        /// Identifies trend strength and potential reversals
        /// Aroon Up = ((period - periods since highest high) / period) * 100
        /// Aroon Down = ((period - periods since lowest low) / period) * 100
        /// Values range 0-100:
        /// - Aroon Up > 70 with Aroon Down < 30 = strong uptrend
        /// - Aroon Down > 70 with Aroon Up < 30 = strong downtrend
        /// - Both near 50 = consolidation/weak trend
        /// </summary>
        /// <param name="period">Lookback period (default: 25)</param>
        /// <returns>AroonResult containing aroon up and aroon down values</returns>
        public AroonResult Aroon(int period = 25)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();

            var length = highs.Length;
            var aroonUp = new double[length];
            var aroonDown = new double[length];

            if (length < period)
            {
                for (int i = 0; i < length; i++)
                {
                    aroonUp[i] = double.NaN;
                    aroonDown[i] = double.NaN;
                }
                return new AroonResult(aroonUp, aroonDown, period);
            }

            // Initialize result with NaN for warmup period
            for (int i = 0; i < period - 1; i++)
            {
                aroonUp[i] = double.NaN;
                aroonDown[i] = double.NaN;
            }

            // Calculate Aroon for each bar
            for (int i = period - 1; i < length; i++)
            {
                // Find periods since highest high and lowest low
                int periodsSinceHigh = 0;
                int periodsSinceLow = 0;
                double highestHigh = highs[i];
                double lowestLow = lows[i];

                // Look back over the period
                for (int j = 0; j < period; j++)
                {
                    int idx = i - j;

                    if (highs[idx] >= highestHigh)
                    {
                        highestHigh = highs[idx];
                        periodsSinceHigh = j;
                    }

                    if (lows[idx] <= lowestLow)
                    {
                        lowestLow = lows[idx];
                        periodsSinceLow = j;
                    }
                }

                // Calculate Aroon Up and Down
                aroonUp[i] = ((double)(period - periodsSinceHigh) / period) * 100.0;
                aroonDown[i] = ((double)(period - periodsSinceLow) / period) * 100.0;
            }

            return new AroonResult(aroonUp, aroonDown, period);
        }

        /// <summary>
        /// Vortex Indicator
        /// Identifies trend reversals and confirms trend direction
        /// VI+ = Sum(|High - Previous Low|) / Sum(True Range)
        /// VI- = Sum(|Low - Previous High|) / Sum(True Range)
        /// Trading signals:
        /// - VI+ crosses above VI- = bullish signal (buy)
        /// - VI- crosses above VI+ = bearish signal (sell)
        /// - VI+ > VI- = uptrend, VI- > VI+ = downtrend
        /// </summary>
        /// <param name="period">Lookback period (default: 14)</param>
        /// <returns>VortexResult containing VI+ and VI- values</returns>
        public VortexResult Vortex(int period = 14)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();
            var closes = _manager.GetClosePrices();

            var length = closes.Length;
            var viPlus = new double[length];
            var viMinus = new double[length];

            if (length < period + 1)
            {
                for (int i = 0; i < length; i++)
                {
                    viPlus[i] = double.NaN;
                    viMinus[i] = double.NaN;
                }
                return new VortexResult(viPlus, viMinus, period);
            }

            // Calculate Vortex Movements and True Range
            var vmPlus = new double[length];
            var vmMinus = new double[length];
            var tr = new double[length];

            // First bar has no vortex movement
            vmPlus[0] = 0;
            vmMinus[0] = 0;
            tr[0] = highs[0] - lows[0];

            for (int i = 1; i < length; i++)
            {
                // Vortex Movement+: |Current High - Previous Low|
                vmPlus[i] = Math.Abs(highs[i] - lows[i - 1]);

                // Vortex Movement-: |Current Low - Previous High|
                vmMinus[i] = Math.Abs(lows[i] - highs[i - 1]);

                // True Range
                var tr1 = highs[i] - lows[i];
                var tr2 = Math.Abs(highs[i] - closes[i - 1]);
                var tr3 = Math.Abs(lows[i] - closes[i - 1]);
                tr[i] = Math.Max(tr1, Math.Max(tr2, tr3));
            }

            // Initialize result with NaN for warmup period
            for (int i = 0; i < period; i++)
            {
                viPlus[i] = double.NaN;
                viMinus[i] = double.NaN;
            }

            // Calculate Vortex Indicator
            for (int i = period; i < length; i++)
            {
                double sumVMPlus = 0;
                double sumVMMinus = 0;
                double sumTR = 0;

                // Sum over the period
                for (int j = 0; j < period; j++)
                {
                    int idx = i - j;
                    sumVMPlus += vmPlus[idx];
                    sumVMMinus += vmMinus[idx];
                    sumTR += tr[idx];
                }

                // Calculate VI+ and VI-
                if (sumTR > 0)
                {
                    viPlus[i] = sumVMPlus / sumTR;
                    viMinus[i] = sumVMMinus / sumTR;
                }
                else
                {
                    viPlus[i] = 0;
                    viMinus[i] = 0;
                }
            }

            return new VortexResult(viPlus, viMinus, period);
        }

        /// <summary>
        /// Ichimoku Cloud (Ichimoku Kinko Hyo)
        /// Complete trend-following system with multiple components
        /// Components:
        /// - Tenkan-sen (Conversion Line): (9-high + 9-low) / 2
        /// - Kijun-sen (Base Line): (26-high + 26-low) / 2
        /// - Senkou Span A (Leading Span A): (Tenkan + Kijun) / 2, shifted 26 ahead
        /// - Senkou Span B (Leading Span B): (52-high + 52-low) / 2, shifted 26 ahead
        /// - Chikou Span (Lagging Span): Close, shifted 26 back
        /// Trading signals:
        /// - Price above cloud = uptrend, below cloud = downtrend
        /// - Tenkan crosses above Kijun = buy signal
        /// - Tenkan crosses below Kijun = sell signal
        /// - Cloud color (Senkou A vs B) indicates trend strength
        /// </summary>
        /// <param name="tenkanPeriod">Tenkan-sen period (default: 9)</param>
        /// <param name="kijunPeriod">Kijun-sen period (default: 26)</param>
        /// <param name="senkouPeriod">Senkou Span B period (default: 52)</param>
        /// <param name="displacement">Cloud displacement (default: 26)</param>
        /// <returns>IchimokuResult containing all components</returns>
        public IchimokuResult Ichimoku(int tenkanPeriod = 9, int kijunPeriod = 26, int senkouPeriod = 52, int displacement = 26)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (tenkanPeriod <= 0 || kijunPeriod <= 0 || senkouPeriod <= 0 || displacement < 0)
                throw new ArgumentException("Periods must be positive and displacement non-negative");

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();
            var closes = _manager.GetClosePrices();

            var length = closes.Length;
            var tenkan = new double[length];
            var kijun = new double[length];
            var senkouA = new double[length];
            var senkouB = new double[length];
            var chikou = new double[length];

            // Helper function to calculate midpoint of highest high and lowest low
            double CalculateMidpoint(int index, int period)
            {
                if (index < period - 1)
                    return double.NaN;

                double highest = highs[index];
                double lowest = lows[index];

                for (int j = 1; j < period; j++)
                {
                    int idx = index - j;
                    if (highs[idx] > highest)
                        highest = highs[idx];
                    if (lows[idx] < lowest)
                        lowest = lows[idx];
                }

                return (highest + lowest) / 2.0;
            }

            // Calculate Tenkan-sen (Conversion Line)
            for (int i = 0; i < length; i++)
            {
                tenkan[i] = CalculateMidpoint(i, tenkanPeriod);
            }

            // Calculate Kijun-sen (Base Line)
            for (int i = 0; i < length; i++)
            {
                kijun[i] = CalculateMidpoint(i, kijunPeriod);
            }

            // Calculate Senkou Span B (Leading Span B) - 52-period midpoint
            var senkouBBase = new double[length];
            for (int i = 0; i < length; i++)
            {
                senkouBBase[i] = CalculateMidpoint(i, senkouPeriod);
            }

            // Calculate Senkou Span A (Leading Span A) - (Tenkan + Kijun) / 2
            var senkouABase = new double[length];
            for (int i = 0; i < length; i++)
            {
                if (double.IsNaN(tenkan[i]) || double.IsNaN(kijun[i]))
                    senkouABase[i] = double.NaN;
                else
                    senkouABase[i] = (tenkan[i] + kijun[i]) / 2.0;
            }

            // Shift Senkou Spans forward by displacement periods
            for (int i = 0; i < length; i++)
            {
                senkouA[i] = double.NaN;
                senkouB[i] = double.NaN;
            }

            for (int i = 0; i < length - displacement; i++)
            {
                int targetIdx = i + displacement;
                if (targetIdx < length)
                {
                    senkouA[targetIdx] = senkouABase[i];
                    senkouB[targetIdx] = senkouBBase[i];
                }
            }

            // Calculate Chikou Span (Lagging Span) - Close shifted back
            for (int i = 0; i < length; i++)
            {
                if (i >= displacement)
                {
                    chikou[i - displacement] = closes[i];
                }
            }

            // Fill remaining chikou values with NaN
            for (int i = length - displacement; i < length; i++)
            {
                if (i >= 0)
                    chikou[i] = double.NaN;
            }

            return new IchimokuResult(tenkan, kijun, senkouA, senkouB, chikou,
                tenkanPeriod, kijunPeriod, senkouPeriod, displacement);
        }
    }
}
