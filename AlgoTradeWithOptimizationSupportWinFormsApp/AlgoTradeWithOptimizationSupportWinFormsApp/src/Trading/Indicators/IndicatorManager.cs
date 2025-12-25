using System;
using System.Collections.Generic;
using System.Linq;
using AlgoTradeWithOptimizationSupportWinFormsApp.DataReader;
using AlgoTradeWithOptimizationSupportWinFormsApp.DataProvider;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Base;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.MovingAverages;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Trend;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Momentum;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Volatility;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Volume;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.PriceAction;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Utils;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.Timer;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Indicators
{
    /// <summary>
    /// Main Indicator Manager - Top-level class for all technical indicators
    ///
    /// Architecture:
    /// - Categorized sub-managers (MA, Trend, Momentum, Volatility, Volume, PriceAction)
    /// - Utility functions (PriceUtils)
    /// - Caching for performance
    /// - Logging and timing support
    ///
    /// Usage:
    ///   var manager = new IndicatorManager();
    ///   manager.Initialize(stockDataList);
    ///
    ///   var sma20 = manager.MA.SMA(closes, 20);
    ///   var rsi = manager.Momentum.RSI(closes, 14);
    ///   var supertrend = manager.Trend.SuperTrend(10, 3.0);
    /// </summary>
    public class IndicatorManager : MarketDataProvider, IDisposable
    {
        #region Fields

        private readonly IndicatorConfig _config;
        private readonly LogManager? _logManager;
        private readonly TimeManager? _timeManager;
        private readonly Dictionary<string, double[]> _cache;
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>Configuration</summary>
        public IndicatorConfig Config => _config;

        /// <summary>Market data (initialized via Initialize method)</summary>
        private List<StockData> _data = new List<StockData>();

        /// <summary>Market data override from base class</summary>
        public override List<StockData> Data => _data;

        /// <summary>Number of bars/candles</summary>
        public int BarCount => Data?.Count ?? 0;

        // ==================== Sub-Managers (Category-based Access) ====================

        /// <summary>Moving Average Calculator (70+ MA types)</summary>
        public MovingAverageCalculator MA { get; }

        /// <summary>Trend Indicators (SuperTrend, MOST, ADX, Parabolic SAR, etc.)</summary>
        public TrendIndicators Trend { get; }

        /// <summary>Momentum Indicators (RSI, MACD, Stochastic, CCI, etc.)</summary>
        public MomentumIndicators Momentum { get; }

        /// <summary>Volatility Indicators (ATR, Bollinger Bands, Keltner Channel, etc.)</summary>
        public VolatilityIndicators Volatility { get; }

        /// <summary>Volume Indicators (OBV, VWAP, MFI, CMF, etc.)</summary>
        public VolumeIndicators VolumeInd { get; }

        /// <summary>Price Action Indicators (HH/LL, Swing Points, ZigZag, etc.)</summary>
        public PriceActionIndicators PriceAction { get; }

        /// <summary>Utility functions (HHV, LLV, StdDev, Sum, etc.)</summary>
        public PriceUtils Utils { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize Indicator Manager
        /// </summary>
        /// <param name="config">Optional configuration</param>
        public IndicatorManager(List<StockData> data, IndicatorConfig? config = null)
        {
            _config = config ?? new IndicatorConfig();
            _cache = new Dictionary<string, double[]>();
            _data = new List<StockData>(data);

            // Setup logging
            if (_config.EnableDebugLogging)
            {
                _logManager = LogManager.Instance;
                _logManager.WriteLog("IndicatorManager initialized with debug logging enabled");
            }

            // Setup timing
            if (_config.EnablePerformanceTiming)
            {
                _timeManager = TimeManager.Instance;
            }

            // Initialize sub-managers
            MA = new MovingAverageCalculator(this, _config);
            Trend = new TrendIndicators(this, _config);
            Momentum = new MomentumIndicators(this, _config);
            Volatility = new VolatilityIndicators(this, _config);
            VolumeInd = new VolumeIndicators(this, _config);
            PriceAction = new PriceActionIndicators(this, _config);
            Utils = new PriceUtils(_config.EnableDebugLogging);

            _logManager?.WriteLog("All sub-managers initialized successfully");
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize with market data
        /// </summary>
        /// <param name="data">Stock data list</param>
        /// <returns>Self for method chaining</returns>
        public IndicatorManager SetData(List<StockData> data)
        {
            if (data == null || data.Count == 0)
                throw new ArgumentException("Data cannot be null or empty", nameof(data));

            _data = data;
            _logManager?.WriteLog($"IndicatorManager initialized with {data.Count} bars");

            return this;
        }

        /// <summary>
        /// Reset cache and clear data
        /// </summary>
        public void Reset()
        {
            _cache.Clear();
            Data?.Clear();
            _logManager?.WriteLog("IndicatorManager reset - cache and data cleared");
        }

        #endregion

        #region Cache Management

        /// <summary>
        /// Get cached result or calculate new (internal helper for sub-managers)
        /// </summary>
        internal double[] GetOrCalculate(string cacheKey, Func<double[]> calculator)
        {
            // Check cache
            if (_cache.TryGetValue(cacheKey, out var cached))
            {
                _logManager?.WriteLog($"Cache HIT: {cacheKey}");
                return cached;
            }

            _logManager?.WriteLog($"Cache MISS: {cacheKey} - calculating...");

            // Start timing
            string? timerId = null;
            if (_timeManager != null)
            {
                timerId = $"Calc_{cacheKey}";
                _timeManager.StartTimer(timerId);
            }

            // Calculate
            var result = calculator();

            // Stop timing
            if (_timeManager != null && timerId != null)
            {
                _timeManager.StopTimer(timerId);
                var elapsed = _timeManager.GetElapsedTime(timerId);
                _logManager?.WriteLog($"Calculated {cacheKey} in {elapsed}ms");
            }

            // Cache if not full
            if (_cache.Count < _config.CacheSize)
            {
                _cache[cacheKey] = result;
                _logManager?.WriteLog($"Cached result for {cacheKey}");
            }
            else
            {
                _logManager?.WriteLog($"Cache FULL ({_cache.Count}/{_config.CacheSize}) - not caching {cacheKey}");
            }

            return result;
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public Dictionary<string, int> GetCacheStats()
        {
            return new Dictionary<string, int>
            {
                ["CacheSize"] = _cache.Count,
                ["MaxCacheSize"] = _config.CacheSize,
                ["BarCount"] = BarCount
            };
        }

        /// <summary>
        /// Clear all cached indicators
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
            _logManager?.WriteLog("Cache cleared");
        }

        #endregion

        #region Helper Methods

        // Data extraction methods (GetClosePrices, GetOpenPrices, GetHighPrices, GetLowPrices, GetVolume, etc.)
        // are inherited from MarketDataProvider base class

        #endregion

        #region IDisposable

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            ClearCache();
            Data?.Clear();

            _logManager?.WriteLog("IndicatorManager disposed");

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        #region String Representation

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            var stats = GetCacheStats();
            return $"IndicatorManager(Bars={stats["BarCount"]}, Cache={stats["CacheSize"]}/{stats["MaxCacheSize"]})";
        }

        #endregion
    }
}

/*
  📊 Implement Edilmemiş Indicator'lar

  1️⃣ Trend Indicators (TrendIndicators.cs)

  - ❌ SuperTrend - SuperTrend(period, multiplier)
  - ❌ MOST - MOST(period, percent) - Moving Stop Loss
  - ❌ ADX - ADX(period) - Average Directional Index
  - ❌ Parabolic SAR - ParabolicSAR(step, max)

  2️⃣ Momentum Indicators (MomentumIndicators.cs)

  - ❌ RSI - RSI(source, period) - Relative Strength Index
  - ❌ MACD - MACD(source, fastPeriod, slowPeriod, signalPeriod)
  - ❌ Stochastic - Stochastic(kPeriod, dPeriod)
  - ❌ CCI - CCI(period) - Commodity Channel Index
  - ❌ Williams %R - WilliamsR(period)

  3️⃣ Volatility Indicators (VolatilityIndicators.cs)

  - ❌ ATR - ATR(period) - Average True Range
  - ❌ Bollinger Bands - BollingerBands(source, period, stdDev)
  - ❌ Keltner Channel - KeltnerChannel(period, multiplier)

  4️⃣ Volume Indicators (VolumeIndicators.cs)

  - ❌ OBV - OBV() - On Balance Volume
  - ❌ VWAP - VWAP() - Volume Weighted Average Price
  - ❌ MFI - MFI(period) - Money Flow Index
  - ❌ CMF - CMF(period) - Chaikin Money Flow

  5️⃣ Price Action Indicators (PriceActionIndicators.cs)

  - ❌ HH/LL Pattern - HigherHighLowerLow() - Higher High / Lower Low
  - ❌ Swing Points - SwingPoints(leftBars, rightBars)
  - ❌ ZigZag - ZigZag(deviation)
*/