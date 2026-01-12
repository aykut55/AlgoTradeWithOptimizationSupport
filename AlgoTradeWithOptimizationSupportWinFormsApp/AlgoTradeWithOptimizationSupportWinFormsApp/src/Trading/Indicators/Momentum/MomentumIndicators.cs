using System;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Base;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Momentum.Results;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Momentum
{
    /// <summary>
    /// Momentum Indicators - RSI, MACD, Stochastic, CCI, Williams%R, etc.
    /// TODO: Implement remaining indicators
    /// </summary>
    public class MomentumIndicators
    {
        private readonly IndicatorManager _manager;
        private readonly IndicatorConfig _config;

        public MomentumIndicators(IndicatorManager manager, IndicatorConfig config)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Relative Strength Index
        /// Measures momentum by comparing upward and downward price movements
        /// Values range from 0-100 (typically overbought >70, oversold <30)
        /// </summary>
        public RSIResult RSI(double[] source, int period = 14)
        {
            if (source == null || source.Length == 0)
                throw new ArgumentException("Source cannot be null or empty", nameof(source));
            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            var values = new double[source.Length];

            if (source.Length < period + 1)
            {
                // Not enough data
                for (int i = 0; i < source.Length; i++)
                    values[i] = double.NaN;
                return new RSIResult(values, period);
            }

            // Calculate price differences
            var gains = new double[source.Length];
            var losses = new double[source.Length];

            for (int i = 1; i < source.Length; i++)
            {
                var diff = source[i] - source[i - 1];
                if (diff > 0)
                {
                    gains[i] = diff;
                    losses[i] = 0;
                }
                else
                {
                    gains[i] = 0;
                    losses[i] = Math.Abs(diff);
                }
            }

            // Initialize result array with NaN
            for (int i = 0; i < period; i++)
            {
                values[i] = double.NaN;
            }

            // Calculate initial averages (SMA for first period)
            var avgGain = 0.0;
            var avgLoss = 0.0;

            for (int i = 1; i <= period; i++)
            {
                avgGain += gains[i];
                avgLoss += losses[i];
            }
            avgGain /= period;
            avgLoss /= period;

            // Calculate first RSI value
            if (avgLoss != 0)
            {
                var rs = avgGain / avgLoss;
                values[period] = 100.0 - (100.0 / (1.0 + rs));
            }
            else
            {
                values[period] = avgGain > 0 ? 100.0 : 50.0;
            }

            // Continue with Wilder's smoothing
            var alpha = 1.0 / period;

            for (int i = period + 1; i < source.Length; i++)
            {
                // Wilder's smoothing: avg = alpha * current + (1 - alpha) * previous_avg
                avgGain = alpha * gains[i] + (1 - alpha) * avgGain;
                avgLoss = alpha * losses[i] + (1 - alpha) * avgLoss;

                // Calculate RSI
                if (avgLoss != 0)
                {
                    var rs = avgGain / avgLoss;
                    values[i] = 100.0 - (100.0 / (1.0 + rs));
                }
                else
                {
                    values[i] = avgGain > 0 ? 100.0 : 50.0;
                }
            }

            return new RSIResult(values, period);
        }

        /// <summary>
        /// Moving Average Convergence Divergence
        /// Trend-following momentum indicator showing relationship between two EMAs
        /// MACD Line = Fast EMA - Slow EMA
        /// Signal Line = EMA of MACD Line
        /// Histogram = MACD Line - Signal Line
        /// </summary>
        public MACDResult MACD(double[] source, int fastPeriod = 12, int slowPeriod = 26, int signalPeriod = 9)
        {
            if (source == null || source.Length == 0)
                throw new ArgumentException("Source cannot be null or empty", nameof(source));
            if (fastPeriod >= slowPeriod)
                throw new ArgumentException("Fast period must be less than slow period");
            if (fastPeriod <= 0 || slowPeriod <= 0 || signalPeriod <= 0)
                throw new ArgumentException("All periods must be positive");

            // Calculate fast and slow EMAs
            var fastEMA = _manager.MA.EMA(source, fastPeriod);
            var slowEMA = _manager.MA.EMA(source, slowPeriod);

            // MACD Line = Fast EMA - Slow EMA
            var macdLine = new double[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                if (double.IsNaN(fastEMA[i]) || double.IsNaN(slowEMA[i]))
                    macdLine[i] = double.NaN;
                else
                    macdLine[i] = fastEMA[i] - slowEMA[i];
            }

            // Signal Line = EMA of MACD Line
            var signalLine = _manager.MA.EMA(macdLine, signalPeriod);

            // Histogram = MACD Line - Signal Line
            var histogram = new double[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                if (double.IsNaN(macdLine[i]) || double.IsNaN(signalLine[i]))
                    histogram[i] = double.NaN;
                else
                    histogram[i] = macdLine[i] - signalLine[i];
            }

            return new MACDResult(macdLine, signalLine, histogram, fastPeriod, slowPeriod, signalPeriod);
        }

        /// <summary>
        /// Stochastic Oscillator
        /// Momentum indicator comparing closing price to price range over time
        /// %K = (Close - LLV) / (HHV - LLV) * 100
        /// %D = SMA of %K
        /// Values range from 0-100 (overbought >80, oversold <20)
        /// </summary>
        public StochasticResult Stochastic(int kPeriod = 14, int dPeriod = 3)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (kPeriod <= 0 || dPeriod <= 0)
                throw new ArgumentException("Periods must be positive");

            var closes = _manager.GetClosePrices();
            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();

            var k = new double[closes.Length];

            // Calculate %K
            for (int i = 0; i < closes.Length; i++)
            {
                if (i < kPeriod - 1)
                {
                    k[i] = double.NaN;
                    continue;
                }

                // Find highest high and lowest low in the period
                var hhv = _manager.Utils.HHV(highs, kPeriod)[i];
                var llv = _manager.Utils.LLV(lows, kPeriod)[i];

                var range = hhv - llv;
                if (range > 0)
                {
                    k[i] = ((closes[i] - llv) / range) * 100.0;
                }
                else
                {
                    k[i] = 50.0; // Neutral value when no range
                }
            }

            // Calculate %D (SMA of %K)
            var d = _manager.MA.SMA(k, dPeriod);

            return new StochasticResult(k, d, kPeriod, dPeriod);
        }

        /// <summary>
        /// Commodity Channel Index
        /// Measures deviation from statistical mean
        /// CCI = (Typical Price - SMA) / (0.015 * Mean Deviation)
        /// Typical Price = (High + Low + Close) / 3
        /// Values typically range from -100 to +100 (>+100 overbought, <-100 oversold)
        /// </summary>
        public double[] CCI(int period = 20)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();
            var closes = _manager.GetClosePrices();

            var cci = new double[closes.Length];

            if (closes.Length < period)
            {
                for (int i = 0; i < closes.Length; i++)
                    cci[i] = double.NaN;
                return cci;
            }

            // Calculate Typical Price
            var typicalPrice = new double[closes.Length];
            for (int i = 0; i < closes.Length; i++)
            {
                typicalPrice[i] = (highs[i] + lows[i] + closes[i]) / 3.0;
            }

            // Calculate SMA of Typical Price
            var sma = _manager.MA.SMA(typicalPrice, period);

            // Initialize result with NaN
            for (int i = 0; i < period - 1; i++)
            {
                cci[i] = double.NaN;
            }

            // Calculate CCI
            for (int i = period - 1; i < closes.Length; i++)
            {
                // Calculate Mean Deviation
                double sumAbsDeviation = 0;
                for (int j = 0; j < period; j++)
                {
                    sumAbsDeviation += Math.Abs(typicalPrice[i - j] - sma[i]);
                }
                double meanDeviation = sumAbsDeviation / period;

                // Calculate CCI
                if (meanDeviation > 0)
                {
                    cci[i] = (typicalPrice[i] - sma[i]) / (0.015 * meanDeviation);
                }
                else
                {
                    cci[i] = 0;
                }
            }

            return cci;
        }

        /// <summary>
        /// Williams %R (Williams Percent Range)
        /// Momentum indicator measuring overbought/oversold levels
        /// %R = (Highest High - Close) / (Highest High - Lowest Low) * -100
        /// Values range from 0 to -100 (above -20 = overbought, below -80 = oversold)
        /// Inverse of Stochastic %K
        /// </summary>
        public double[] WilliamsR(int period = 14)
        {
            if (!_manager.IsInitialized)
                throw new InvalidOperationException("Manager not initialized with data");
            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            var highs = _manager.GetHighPrices();
            var lows = _manager.GetLowPrices();
            var closes = _manager.GetClosePrices();

            var williamsR = new double[closes.Length];

            if (closes.Length < period)
            {
                for (int i = 0; i < closes.Length; i++)
                    williamsR[i] = double.NaN;
                return williamsR;
            }

            // Initialize result with NaN
            for (int i = 0; i < period - 1; i++)
            {
                williamsR[i] = double.NaN;
            }

            // Calculate Williams %R
            for (int i = period - 1; i < closes.Length; i++)
            {
                // Find highest high and lowest low in the period
                var hhv = _manager.Utils.HHV(highs, period)[i];
                var llv = _manager.Utils.LLV(lows, period)[i];

                var range = hhv - llv;
                if (range > 0)
                {
                    williamsR[i] = ((hhv - closes[i]) / range) * -100.0;
                }
                else
                {
                    williamsR[i] = -50.0; // Neutral value when no range
                }
            }

            return williamsR;
        }

        /// <summary>
        /// Rate of Change
        /// Measures percentage change in price over a period
        /// ROC = ((Close - Close[period ago]) / Close[period ago]) * 100
        /// Positive values indicate upward momentum, negative indicate downward momentum
        /// </summary>
        public double[] ROC(double[] source, int period = 12)
        {
            if (source == null || source.Length == 0)
                throw new ArgumentException("Source cannot be null or empty", nameof(source));
            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            var roc = new double[source.Length];

            if (source.Length <= period)
            {
                for (int i = 0; i < source.Length; i++)
                    roc[i] = double.NaN;
                return roc;
            }

            // Initialize result with NaN
            for (int i = 0; i < period; i++)
            {
                roc[i] = double.NaN;
            }

            // Calculate ROC
            for (int i = period; i < source.Length; i++)
            {
                if (source[i - period] != 0)
                {
                    roc[i] = ((source[i] - source[i - period]) / source[i - period]) * 100.0;
                }
                else
                {
                    roc[i] = 0;
                }
            }

            return roc;
        }
    }
}
