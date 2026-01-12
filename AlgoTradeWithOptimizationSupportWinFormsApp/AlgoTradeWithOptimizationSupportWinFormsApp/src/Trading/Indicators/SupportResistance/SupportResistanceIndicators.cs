using System;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Base;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.SupportResistance.Results;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.SupportResistance
{
    /// <summary>
    /// Support/Resistance Indicators - Pivot Points, Fibonacci Retracement, etc.
    /// </summary>
    public class SupportResistanceIndicators
    {
        private readonly IndicatorManager _manager;
        private readonly IndicatorConfig _config;

        public SupportResistanceIndicators(IndicatorManager manager, IndicatorConfig config)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Classic Pivot Points
        /// Calculates support and resistance levels based on previous period's high, low, close
        /// Formula:
        /// - Pivot Point (PP) = (High + Low + Close) / 3
        /// - R1 = (2 * PP) - Low
        /// - R2 = PP + (High - Low)
        /// - R3 = High + 2 * (PP - Low)
        /// - S1 = (2 * PP) - High
        /// - S2 = PP - (High - Low)
        /// - S3 = Low - 2 * (High - PP)
        /// Used for intraday trading to identify potential reversal points
        /// </summary>
        /// <param name="useDaily">If true, uses previous day's data for each calculation (default: true)</param>
        /// <returns>PivotPointsResult containing pivot and support/resistance levels</returns>
        public PivotPointsResult PivotPoints(bool useDaily = true)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();
            var closes = _manager.GetClosePrices();

            var length = closes.Length;
            var pivot = new double[length];
            var r1 = new double[length];
            var r2 = new double[length];
            var r3 = new double[length];
            var s1 = new double[length];
            var s2 = new double[length];
            var s3 = new double[length];

            if (length < 2)
            {
                // Not enough data
                for (int i = 0; i < length; i++)
                {
                    pivot[i] = double.NaN;
                    r1[i] = double.NaN;
                    r2[i] = double.NaN;
                    r3[i] = double.NaN;
                    s1[i] = double.NaN;
                    s2[i] = double.NaN;
                    s3[i] = double.NaN;
                }
                return new PivotPointsResult(pivot, r1, r2, r3, s1, s2, s3);
            }

            // First bar uses its own values
            CalculatePivotLevels(highs[0], lows[0], closes[0],
                out pivot[0], out r1[0], out r2[0], out r3[0],
                out s1[0], out s2[0], out s3[0]);

            // Calculate pivot points for each bar using previous bar's data
            for (int i = 1; i < length; i++)
            {
                // Use previous bar's high, low, close
                double prevHigh = highs[i - 1];
                double prevLow = lows[i - 1];
                double prevClose = closes[i - 1];

                CalculatePivotLevels(prevHigh, prevLow, prevClose,
                    out pivot[i], out r1[i], out r2[i], out r3[i],
                    out s1[i], out s2[i], out s3[i]);
            }

            return new PivotPointsResult(pivot, r1, r2, r3, s1, s2, s3);
        }

        /// <summary>
        /// Helper method to calculate pivot levels from HLC values
        /// </summary>
        private void CalculatePivotLevels(double high, double low, double close,
            out double pivot, out double r1, out double r2, out double r3,
            out double s1, out double s2, out double s3)
        {
            // Pivot Point
            pivot = (high + low + close) / 3.0;

            // Resistance levels
            r1 = (2.0 * pivot) - low;
            r2 = pivot + (high - low);
            r3 = high + 2.0 * (pivot - low);

            // Support levels
            s1 = (2.0 * pivot) - high;
            s2 = pivot - (high - low);
            s3 = low - 2.0 * (high - pivot);
        }

        /// <summary>
        /// Fibonacci Retracement Levels
        /// Calculates Fibonacci retracement levels between high and low
        /// Levels: 0%, 23.6%, 38.2%, 50%, 61.8%, 78.6%, 100%
        /// Used to identify potential reversal points during pullbacks
        /// </summary>
        /// <param name="high">High price for the move</param>
        /// <param name="low">Low price for the move</param>
        /// <param name="isUptrend">True if calculating for uptrend (retracement from top), false for downtrend</param>
        /// <returns>FibonacciRetracementResult containing all Fibonacci levels</returns>
        public FibonacciRetracementResult FibonacciRetracement(double high, double low, bool isUptrend = true)
        {
            if (high <= low)
                throw new ArgumentException("High must be greater than low");

            double range = high - low;

            if (isUptrend)
            {
                // Retracement from high (downward levels)
                return new FibonacciRetracementResult(
                    level_0: high,                          // 0% - High
                    level_236: high - (range * 0.236),      // 23.6%
                    level_382: high - (range * 0.382),      // 38.2%
                    level_50: high - (range * 0.50),        // 50%
                    level_618: high - (range * 0.618),      // 61.8%
                    level_786: high - (range * 0.786),      // 78.6%
                    level_100: low,                         // 100% - Low
                    high: high,
                    low: low,
                    isUptrend: isUptrend
                );
            }
            else
            {
                // Retracement from low (upward levels)
                return new FibonacciRetracementResult(
                    level_0: low,                           // 0% - Low
                    level_236: low + (range * 0.236),       // 23.6%
                    level_382: low + (range * 0.382),       // 38.2%
                    level_50: low + (range * 0.50),         // 50%
                    level_618: low + (range * 0.618),       // 61.8%
                    level_786: low + (range * 0.786),       // 78.6%
                    level_100: high,                        // 100% - High
                    high: high,
                    low: low,
                    isUptrend: isUptrend
                );
            }
        }

        /// <summary>
        /// Fibonacci Retracement Levels (Auto-detect from data)
        /// Automatically finds the high and low from a specified period and calculates Fibonacci levels
        /// </summary>
        /// <param name="period">Lookback period to find high/low (default: 100)</param>
        /// <returns>FibonacciRetracementAutoResult containing arrays of Fibonacci levels for each bar</returns>
        public FibonacciRetracementAutoResult FibonacciRetracementAuto(int period = 100)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();

            var length = highs.Length;
            var level_0 = new double[length];
            var level_236 = new double[length];
            var level_382 = new double[length];
            var level_50 = new double[length];
            var level_618 = new double[length];
            var level_786 = new double[length];
            var level_100 = new double[length];

            // Initialize with NaN
            for (int i = 0; i < Math.Min(period - 1, length); i++)
            {
                level_0[i] = double.NaN;
                level_236[i] = double.NaN;
                level_382[i] = double.NaN;
                level_50[i] = double.NaN;
                level_618[i] = double.NaN;
                level_786[i] = double.NaN;
                level_100[i] = double.NaN;
            }

            // Calculate Fibonacci levels for each bar
            for (int i = period - 1; i < length; i++)
            {
                // Find highest high and lowest low in the period
                double periodHigh = highs[i];
                double periodLow = lows[i];

                for (int j = 0; j < period; j++)
                {
                    int idx = i - j;
                    if (highs[idx] > periodHigh)
                        periodHigh = highs[idx];
                    if (lows[idx] < periodLow)
                        periodLow = lows[idx];
                }

                // Determine if uptrend or downtrend (compare first and last in period)
                bool isUptrend = highs[i] > highs[i - period + 1];

                // Calculate Fibonacci levels
                var levels = FibonacciRetracement(periodHigh, periodLow, isUptrend);

                level_0[i] = levels.Level_0;
                level_236[i] = levels.Level_236;
                level_382[i] = levels.Level_382;
                level_50[i] = levels.Level_50;
                level_618[i] = levels.Level_618;
                level_786[i] = levels.Level_786;
                level_100[i] = levels.Level_100;
            }

            return new FibonacciRetracementAutoResult(level_0, level_236, level_382, level_50, level_618, level_786, level_100, period);
        }
    }
}
