using System;
using System.Collections.Generic;
using System.Linq;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.DataProvider
{
    /// <summary>
    /// Base class for market data access
    /// Provides common methods to extract price, volume, and time data from StockData
    /// </summary>
    public abstract class MarketDataProvider
    {
        /// <summary>
        /// Market data (OHLCV) - Must be implemented by derived classes
        /// </summary>
        public abstract List<StockData> Data { get; }

        /// <summary>
        /// Is the provider initialized with data?
        /// </summary>
        public virtual bool IsInitialized => Data != null && Data.Count > 0;

        #region Price Data Extraction

        /// <summary>
        /// Extract close prices from data
        /// </summary>
        public virtual double[] GetClosePrices()
        {
            ValidateInitialized();
            return Data.Select(d => d.Close).ToArray();
        }

        /// <summary>
        /// Extract open prices from data
        /// </summary>
        public virtual double[] GetOpenPrices()
        {
            ValidateInitialized();
            return Data.Select(d => d.Open).ToArray();
        }

        /// <summary>
        /// Extract high prices from data
        /// </summary>
        public virtual double[] GetHighPrices()
        {
            ValidateInitialized();
            return Data.Select(d => d.High).ToArray();
        }

        /// <summary>
        /// Extract low prices from data
        /// </summary>
        public virtual double[] GetLowPrices()
        {
            ValidateInitialized();
            return Data.Select(d => d.Low).ToArray();
        }

        /// <summary>
        /// Extract volume from data
        /// </summary>
        public virtual long[] GetVolume()
        {
            ValidateInitialized();
            return Data.Select(d => (long)d.Volume).ToArray();
        }

        /// <summary>
        /// Extract lot sizes from data
        /// </summary>
        public virtual long[] GetLotSizes()
        {
            ValidateInitialized();
            return Data.Select(d => (long)d.Size).ToArray();
        }

        #endregion

        #region Time Data Extraction

        /// <summary>
        /// Extract DateTime values from data
        /// </summary>
        public virtual DateTime[] GetDateTimes()
        {
            ValidateInitialized();
            return Data.Select(d => d.DateTime).ToArray();
        }

        /// <summary>
        /// Extract dates (DateOnly) from data
        /// </summary>
        public virtual DateOnly[] GetDates()
        {
            ValidateInitialized();
            return Data.Select(d => DateOnly.FromDateTime(d.DateTime)).ToArray();
        }

        /// <summary>
        /// Extract times (TimeOnly) from data
        /// </summary>
        public virtual TimeOnly[] GetTimes()
        {
            ValidateInitialized();
            return Data.Select(d => TimeOnly.FromDateTime(d.DateTime)).ToArray();
        }

        /// <summary>
        /// Extract EpochTime values from data
        /// </summary>
        public virtual long[] GetEpochTimes()
        {
            ValidateInitialized();
            return Data.Select(d => d.EpochTime).ToArray();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validate that the provider is initialized
        /// </summary>
        protected virtual void ValidateInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException($"{GetType().Name} not initialized. Data is null or empty.");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get data count
        /// </summary>
        public virtual int GetDataCount() => Data?.Count ?? 0;

        /// <summary>
        /// Get last bar index (0-based index of the last bar)
        /// Returns -1 if no data available
        /// </summary>
        public virtual int GetLastBarIndex()
        {
            int count = GetDataCount();
            return count > 0 ? count - 1 : -1;
        }

        /// <summary>
        /// Get last bar index property
        /// Returns -1 if no data available
        /// </summary>
        public virtual int LastBarIndex => GetLastBarIndex();

        /// <summary>
        /// Get data range (first and last DateTime)
        /// </summary>
        public virtual (DateTime Start, DateTime End) GetDataRange()
        {
            ValidateInitialized();
            return (Data.First().DateTime, Data.Last().DateTime);
        }

        #endregion
    }
}
