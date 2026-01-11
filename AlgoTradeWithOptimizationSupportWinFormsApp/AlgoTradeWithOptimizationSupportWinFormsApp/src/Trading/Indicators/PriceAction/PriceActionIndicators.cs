using System;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Base;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.PriceAction
{
    /// <summary>
    /// Price Action Indicators - HH/LL, Swing Points, ZigZag, Fractals, etc.
    /// TODO: Implement remaining indicators
    /// </summary>
    public class PriceActionIndicators
    {
        private readonly IndicatorManager _manager;
        private readonly IndicatorConfig _config;

        public PriceActionIndicators(IndicatorManager manager, IndicatorConfig config)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Higher High / Lower Low pattern detection
        /// Identifies trend structure by comparing current highs/lows with previous ones
        /// - Higher High (HH): Current high > Previous high (bullish)
        /// - Lower High (LH): Current high < Previous high (bearish)
        /// - Higher Low (HL): Current low > Previous low (bullish)
        /// - Lower Low (LL): Current low < Previous low (bearish)
        /// Used to identify trend continuation (HH+HL = uptrend, LL+LH = downtrend)
        /// </summary>
        public (bool[] higherHigh, bool[] lowerHigh, bool[] higherLow, bool[] lowerLow) HigherHighLowerLow()
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();

            var length = highs.Length;
            var higherHigh = new bool[length];
            var lowerHigh = new bool[length];
            var higherLow = new bool[length];
            var lowerLow = new bool[length];

            if (length < 2)
                return (higherHigh, lowerHigh, higherLow, lowerLow);

            // First bar has no comparison
            higherHigh[0] = false;
            lowerHigh[0] = false;
            higherLow[0] = false;
            lowerLow[0] = false;

            // Compare each bar with previous bar
            for (int i = 1; i < length; i++)
            {
                // Higher High: current high > previous high
                higherHigh[i] = highs[i] > highs[i - 1];

                // Lower High: current high < previous high
                lowerHigh[i] = highs[i] < highs[i - 1];

                // Higher Low: current low > previous low
                higherLow[i] = lows[i] > lows[i - 1];

                // Lower Low: current low < previous low
                lowerLow[i] = lows[i] < lows[i - 1];
            }

            return (higherHigh, lowerHigh, higherLow, lowerLow);
        }

        /// <summary>
        /// Swing High/Low detection
        /// Identifies significant turning points in price action
        /// - Swing High: High that is higher than leftBars highs before and rightBars highs after
        /// - Swing Low: Low that is lower than leftBars lows before and rightBars lows after
        /// Used for support/resistance levels and trend analysis
        /// </summary>
        /// <param name="leftBars">Number of bars to the left (default: 5)</param>
        /// <param name="rightBars">Number of bars to the right (default: 5)</param>
        /// <returns>Boolean arrays indicating swing high/low at each bar</returns>
        public (bool[] swingHighs, bool[] swingLows) SwingPoints(int leftBars = 5, int rightBars = 5)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (leftBars < 1 || rightBars < 1)
                throw new ArgumentException("Left and right bars must be at least 1");

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();

            var length = highs.Length;
            var swingHighs = new bool[length];
            var swingLows = new bool[length];

            // Need enough bars on both sides
            int minIndex = leftBars;
            int maxIndex = length - rightBars - 1;

            if (maxIndex < minIndex)
            {
                // Not enough data
                return (swingHighs, swingLows);
            }

            // Check each potential swing point
            for (int i = minIndex; i <= maxIndex; i++)
            {
                bool isSwingHigh = true;
                bool isSwingLow = true;

                // Check left side
                for (int j = 1; j <= leftBars; j++)
                {
                    if (highs[i - j] >= highs[i])
                        isSwingHigh = false;
                    if (lows[i - j] <= lows[i])
                        isSwingLow = false;
                }

                // Check right side
                for (int j = 1; j <= rightBars; j++)
                {
                    if (highs[i + j] >= highs[i])
                        isSwingHigh = false;
                    if (lows[i + j] <= lows[i])
                        isSwingLow = false;
                }

                swingHighs[i] = isSwingHigh;
                swingLows[i] = isSwingLow;
            }

            return (swingHighs, swingLows);
        }

        /// <summary>
        /// ZigZag indicator
        /// Filters out small price movements and highlights significant trends
        /// Only shows price movements that exceed the deviation threshold
        /// - Helps identify support/resistance levels
        /// - Removes market noise
        /// - Shows clearer trend structure
        /// </summary>
        /// <param name="deviation">Minimum price change percentage to register (default: 5.0%)</param>
        /// <returns>Tuple of (zigzag values, pivot types: 1=high, -1=low, 0=none)</returns>
        public (double[] zigzag, int[] pivots) ZigZag(double deviation = 5.0)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (deviation <= 0)
                throw new ArgumentException("Deviation must be positive", nameof(deviation));

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();
            var closes = _manager.GetClosePrices();

            var length = highs.Length;
            var zigzag = new double[length];
            var pivots = new int[length];

            // Initialize with NaN
            for (int i = 0; i < length; i++)
            {
                zigzag[i] = double.NaN;
                pivots[i] = 0;
            }

            if (length < 3)
                return (zigzag, pivots);

            // Start with first bar
            int lastPivotIndex = 0;
            double lastPivotValue = closes[0];
            int direction = 0; // 0=unknown, 1=looking for high, -1=looking for low

            zigzag[0] = closes[0];
            pivots[0] = 0;

            // Find initial direction by looking for first significant move
            for (int i = 1; i < length && direction == 0; i++)
            {
                double highChange = ((highs[i] - lastPivotValue) / lastPivotValue) * 100.0;
                double lowChange = ((lastPivotValue - lows[i]) / lastPivotValue) * 100.0;

                if (highChange >= deviation)
                {
                    // Start with upward movement
                    direction = 1; // Now looking for high
                    lastPivotValue = highs[i];
                    lastPivotIndex = i;
                }
                else if (lowChange >= deviation)
                {
                    // Start with downward movement
                    direction = -1; // Now looking for low
                    lastPivotValue = lows[i];
                    lastPivotIndex = i;
                }
            }

            // Find zigzag pivots
            for (int i = lastPivotIndex + 1; i < length; i++)
            {
                if (direction == 1) // Looking for high
                {
                    // Update if new high
                    if (highs[i] > lastPivotValue)
                    {
                        lastPivotValue = highs[i];
                        lastPivotIndex = i;
                    }

                    // Check for reversal (low drops below threshold)
                    double change = ((lastPivotValue - lows[i]) / lastPivotValue) * 100.0;
                    if (change >= deviation)
                    {
                        // Confirmed high pivot
                        zigzag[lastPivotIndex] = lastPivotValue;
                        pivots[lastPivotIndex] = 1; // High pivot

                        // Switch to looking for low
                        direction = -1;
                        lastPivotValue = lows[i];
                        lastPivotIndex = i;
                    }
                }
                else if (direction == -1) // Looking for low
                {
                    // Update if new low
                    if (lows[i] < lastPivotValue)
                    {
                        lastPivotValue = lows[i];
                        lastPivotIndex = i;
                    }

                    // Check for reversal (high rises above threshold)
                    double change = ((highs[i] - lastPivotValue) / lastPivotValue) * 100.0;
                    if (change >= deviation)
                    {
                        // Confirmed low pivot
                        zigzag[lastPivotIndex] = lastPivotValue;
                        pivots[lastPivotIndex] = -1; // Low pivot

                        // Switch to looking for high
                        direction = 1;
                        lastPivotValue = highs[i];
                        lastPivotIndex = i;
                    }
                }
            }

            // Mark the last pivot (even if not confirmed)
            if (lastPivotIndex > 0)
            {
                zigzag[lastPivotIndex] = lastPivotValue;
                pivots[lastPivotIndex] = direction;
            }

            return (zigzag, pivots);
        }

        /// <summary>
        /// Williams Fractals
        /// Identifies potential reversal points
        /// - Fractal High: High with 2 lower highs on each side
        /// - Fractal Low: Low with 2 higher lows on each side
        /// Classic Bill Williams indicator for support/resistance
        /// </summary>
        /// <returns>Boolean arrays indicating fractal highs and lows at each bar</returns>
        public (bool[] fractalHighs, bool[] fractalLows) Fractals()
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();

            var length = highs.Length;
            var fractalHighs = new bool[length];
            var fractalLows = new bool[length];

            // Need at least 5 bars (2 on each side + center)
            if (length < 5)
                return (fractalHighs, fractalLows);

            // Check each bar (except first 2 and last 2)
            for (int i = 2; i < length - 2; i++)
            {
                // Fractal High: center high is highest among 5 bars
                bool isFractalHigh = highs[i] > highs[i - 1] &&
                                     highs[i] > highs[i - 2] &&
                                     highs[i] > highs[i + 1] &&
                                     highs[i] > highs[i + 2];

                // Fractal Low: center low is lowest among 5 bars
                bool isFractalLow = lows[i] < lows[i - 1] &&
                                    lows[i] < lows[i - 2] &&
                                    lows[i] < lows[i + 1] &&
                                    lows[i] < lows[i + 2];

                fractalHighs[i] = isFractalHigh;
                fractalLows[i] = isFractalLow;
            }

            return (fractalHighs, fractalLows);
        }
    }
}
