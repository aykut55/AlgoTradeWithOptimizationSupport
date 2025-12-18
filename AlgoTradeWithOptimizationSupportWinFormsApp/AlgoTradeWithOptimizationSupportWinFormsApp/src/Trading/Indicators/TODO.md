# Technical Indicators - TODO List

## ✅ COMPLETED

### Base Infrastructure
- [x] **MAMethod.cs** - 70+ MA enum (Python match)
- [x] **IndicatorConfig.cs** - Configuration class
- [x] **IndicatorManager.cs** - Main manager (cache, logging, timing)
- [x] **IndicatorTest.cs** - Comprehensive test suite

### Moving Averages (17 implemented)
- [x] **SMA** - Simple Moving Average
- [x] **EMA** - Exponential Moving Average
- [x] **WMA** - Weighted Moving Average
- [x] **Hull MA** - Hull Moving Average
- [x] **DEMA** - Double EMA
- [x] **TEMA** - Triple EMA
- [x] **VWMA** - Volume Weighted MA
- [x] **LSMA** - Least Squares MA
- [x] **KAMA** - Kaufman's Adaptive MA
- [x] **VIDYA** - Variable Index Dynamic Average
- [x] **ZLEMA** - Zero-Lag EMA
- [x] **T3** - Tillson T3
- [x] **ALMA** - Arnaud Legoux MA
- [x] **JMA** - Jurik MA
- [x] **Triangular** - SMA of SMA
- [x] **Wilder** - Wilder's Smoothing
- [x] **SMMA** - Smoothed MA

### Utility Functions
- [x] **HHV** - Highest High Value
- [x] **LLV** - Lowest Low Value
- [x] **Sum** - Sum over period
- [x] **Mean** - Average
- [x] **StdDev** - Standard Deviation
- [x] **Variance** - Variance
- [x] **TrueRange** - True Range
- [x] **Diff** - Price differences
- [x] **PercentChange** - Percentage change

### Result Classes
- [x] **RSIResult** - RSI result container
- [x] **MACDResult** - MACD result container
- [x] **SuperTrendResult** - SuperTrend result container
- [x] **BollingerBandsResult** - BB result container

### Placeholders (Structure created)
- [x] **MomentumIndicators.cs** - Placeholder class
- [x] **TrendIndicators.cs** - Placeholder class
- [x] **VolatilityIndicators.cs** - Placeholder class
- [x] **VolumeIndicators.cs** - Placeholder class
- [x] **PriceActionIndicators.cs** - Placeholder class

---

## 🚧 IN PROGRESS

None currently.

---

## 📋 TODO - HIGH PRIORITY

### Momentum Indicators
- [ ] **RSI** - Relative Strength Index
  - Python ref: IndicatorManager.py line 1125-1191
  - Params: period (default 14)
  - Returns: RSIResult (values, overbought, oversold)
  - Wilder's smoothing already implemented

- [ ] **MACD** - Moving Average Convergence Divergence
  - Python ref: IndicatorManager.py line 1195-1241
  - Params: fastPeriod (12), slowPeriod (26), signalPeriod (9)
  - Returns: MACDResult (macd, signal, histogram)
  - EMA already implemented

- [ ] **Stochastic** - Stochastic Oscillator
  - Params: kPeriod (14), dPeriod (3)
  - Returns: (k, d) arrays
  - Formula: %K = (Close - LLV) / (HHV - LLV) * 100
  - HHV/LLV already implemented

### Trend Indicators
- [ ] **SuperTrend** - Trend following indicator
  - Python ref: IndicatorManager.py line 1422-1486
  - Params: period (10), multiplier (3.0)
  - Returns: SuperTrendResult (supertrend, direction)
  - Uses ATR (TODO)

- [ ] **MOST** - Moving Stop Loss
  - Python ref: IndicatorManager.py line 1360-1420
  - Params: period (21), percent (1.0)
  - Returns: (most, exmov)
  - Uses EMA (already implemented)

- [ ] **ADX** - Average Directional Index
  - Params: period (14)
  - Returns: double[] (ADX values)
  - Uses True Range (already implemented)

### Volatility Indicators
- [ ] **ATR** - Average True Range
  - Params: period (14)
  - Returns: double[] (ATR values)
  - TrueRange already implemented
  - Use Wilder smoothing (already implemented)

- [ ] **Bollinger Bands** - Volatility bands
  - Params: period (20), stdDev (2.0)
  - Returns: BollingerBandsResult (upper, middle, lower, bandwidth, %B)
  - SMA and StdDev already implemented

---

## 📋 TODO - MEDIUM PRIORITY

### Volume Indicators
- [ ] **OBV** - On Balance Volume
  - Formula: OBV = OBV_prev + (Close > Close_prev ? Volume : -Volume)
  - Returns: double[] (cumulative volume)

- [ ] **VWAP** - Volume Weighted Average Price
  - Formula: VWAP = Sum(Price * Volume) / Sum(Volume)
  - Returns: double[] (VWAP values)

- [ ] **MFI** - Money Flow Index
  - Params: period (14)
  - Returns: double[] (MFI values, 0-100)
  - Uses typical price and volume

- [ ] **CMF** - Chaikin Money Flow
  - Params: period (20)
  - Returns: double[] (CMF values)

### Momentum Indicators (Continued)
- [ ] **CCI** - Commodity Channel Index
  - Params: period (20)
  - Returns: double[] (CCI values)

- [ ] **Williams %R** - Williams Percent Range
  - Params: period (14)
  - Returns: double[] (-100 to 0)

- [ ] **ROC** - Rate of Change
  - Params: period (12)
  - Returns: double[] (percentage changes)

### Volatility Indicators (Continued)
- [ ] **Keltner Channel** - Volatility channel
  - Params: period (20), multiplier (2.0)
  - Returns: (upper, middle, lower)

- [ ] **Donchian Channel** - Price channel
  - Params: period (20)
  - Returns: (upper, middle, lower)
  - Uses HHV/LLV (already implemented)

---

## 📋 TODO - LOW PRIORITY

### Trend Indicators (Continued)
- [ ] **Parabolic SAR** - Stop and Reverse
  - Params: step (0.02), max (0.2)
  - Returns: (sar, trend)

- [ ] **Ichimoku Cloud** - Complete Ichimoku system
  - Returns: (tenkan, kijun, senkou_a, senkou_b, chikou)

- [ ] **Aroon** - Aroon Up/Down
  - Params: period (25)
  - Returns: (aroon_up, aroon_down)

- [ ] **Vortex Indicator** - Trend reversal
  - Params: period (14)
  - Returns: (vi_plus, vi_minus)

### Price Action Indicators
- [ ] **HH/LL Pattern** - Higher High / Lower Low
  - Returns: (higherHigh, lowerHigh, higherLow, lowerLow) bool arrays

- [ ] **Swing Points** - Swing High/Low detection
  - Params: leftBars (5), rightBars (5)
  - Returns: (swingHighs, swingLows) index arrays

- [ ] **ZigZag** - Price zigzag
  - Params: deviation (5.0)
  - Returns: (zigzag, pivots)

- [ ] **Fractals** - Williams Fractals
  - Returns: (fractalHighs, fractalLows)

### Support/Resistance
- [ ] **Pivot Points** - Classic pivot points
  - Returns: (pivot, r1, r2, r3, s1, s2, s3)

- [ ] **Fibonacci Retracement** - Fib levels
  - Params: high, low
  - Returns: levels (0%, 23.6%, 38.2%, 50%, 61.8%, 100%)

---

## 📋 TODO - ADVANCED (53+ MAs)

### Exotic Moving Averages (Python defined, not implemented)
- [ ] **FRAMA** - Fractal Adaptive MA
- [ ] **MAMA** - MESA Adaptive MA
- [ ] **MCGINLEY** - McGinley Dynamic
- [ ] **VAMA** - Volatility Adjusted MA
- [ ] **DHULL** - Double Hull MA
- [ ] **THULL** - Triple Hull MA
- [ ] **DZLEMA** - Double ZLEMA
- [ ] **TZLEMA** - Triple ZLEMA
- [ ] **GMA** - Geometric MA
- [ ] **MEDIAN** - Median MA
- [ ] **ZSMA** - Zero-Lag Simple MA
- [ ] Plus 40+ more (see MAMethod.cs)

---

## 🔧 REFACTORING / IMPROVEMENTS

### Code Quality
- [ ] Add XML documentation for all public methods
- [ ] Add unit tests (NUnit/xUnit)
- [ ] Performance benchmarks
- [ ] Error handling improvements

### Features
- [ ] Add signal generation (crossovers, divergences)
- [ ] Add indicator comparisons (e.g., RSI vs Stochastic RSI)
- [ ] Add backtesting integration
- [ ] Add multi-timeframe support
- [ ] Add indicator combination strategies

### Performance
- [ ] Parallel calculation for bulk operations
- [ ] Memory optimization for large datasets
- [ ] SIMD optimization for array operations

---

## 📊 IMPLEMENTATION PRIORITY RATIONALE

### Why RSI First?
- Most popular momentum indicator (TradingView #1)
- Python implementation available (line 1125)
- Wilder smoothing already implemented
- Simple to test and validate

### Why MACD Second?
- Second most popular (TradingView #2)
- Python implementation available (line 1195)
- EMA already implemented
- Widely used in algo trading

### Why SuperTrend Third?
- Very popular in TradingView
- Python implementation available (line 1422)
- Only needs ATR (next on list)
- Great for trend following

### Why MOST Fourth?
- Custom indicator (Python line 1360)
- Already implemented in Python
- Uses EMA (already done)
- Unique to our system

---

## 📝 NOTES

### Python Reference Locations
- **IndicatorManager.py**: `src/Indicators/IndicatorManager.py`
- **MA Enum**: Line 23-93 (70+ types)
- **RSI**: Line 1125-1191
- **MACD**: Line 1195-1241
- **MOST**: Line 1360-1420
- **SuperTrend**: Line 1422-1486

### Testing Strategy
- Add test method to `IndicatorTest.cs` for each new indicator
- Compare results with Python implementation
- Validate edge cases (NaN handling, period < data length)
- Performance test with large datasets (100K+ bars)

### Documentation
- Update `README.md` when completing each indicator
- Add usage example to `IndicatorTest.cs`
- Update this TODO list status

---

**Last Updated:** 2025-01-18
**Next Priority:** RSI implementation (Python port)
