using System;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Time utilities - Handles time-based trading filters and controls
    /// Used for session filtering, day-end operations, and time-based rules
    /// </summary>
    public class TimeUtils
    {
        #region Time Filtering

        public bool TimeFilteringEnabled { get; set; }
        public bool GunSonuPozKapatEnabled { get; set; }
        public bool GunBasiPozAcEnabled { get; set; }

        #endregion

        #region Trading Hours

        public TimeOnly BaslangicSaati { get; set; }
        public TimeOnly BitisSaati { get; set; }
        public TimeOnly GunSonuKapatmaSaati { get; set; }

        #endregion

        #region Current Time Info

        public DateTime CurrentBarTime { get; set; }
        public TimeOnly CurrentTime { get; set; }
        public DateOnly CurrentDate { get; set; }

        #endregion

        #region Session Info

        public bool IsInTradingSession { get; set; }
        public bool IsFirstBarOfDay { get; set; }
        public bool IsLastBarOfDay { get; set; }
        public bool IsAfterCloseTime { get; set; }

        #endregion

        #region Constructor

        public TimeUtils()
        {
            // Initialize with default values
            Reset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset all time utility values to defaults
        /// </summary>
        public TimeUtils Reset()
        {
            TimeFilteringEnabled = false;
            GunSonuPozKapatEnabled = false;
            GunBasiPozAcEnabled = false;

            // Default trading hours (Istanbul: 09:30 - 18:00)
            BaslangicSaati = new TimeOnly(9, 30, 0);
            BitisSaati = new TimeOnly(18, 0, 0);
            GunSonuKapatmaSaati = new TimeOnly(17, 55, 0);

            CurrentBarTime = DateTime.MinValue;
            CurrentTime = TimeOnly.MinValue;
            CurrentDate = DateOnly.MinValue;

            IsInTradingSession = false;
            IsFirstBarOfDay = false;
            IsLastBarOfDay = false;
            IsAfterCloseTime = false;

            return this;
        }

        /// <summary>
        /// Initialize time utils - Does nothing currently but returns this for method chaining
        /// </summary>
        public TimeUtils Init()
        {
            return this;
        }

        /// <summary>
        /// Update current time from bar data
        /// </summary>
        public void UpdateTime(DateTime barTime)
        {
            CurrentBarTime = barTime;
            CurrentTime = TimeOnly.FromDateTime(barTime);
            CurrentDate = DateOnly.FromDateTime(barTime);

            // Update session status
            UpdateSessionStatus();
        }

        /// <summary>
        /// Update trading session status
        /// </summary>
        private void UpdateSessionStatus()
        {
            if (!TimeFilteringEnabled)
            {
                IsInTradingSession = true;
                return;
            }

            // Check if current time is within trading hours
            IsInTradingSession = CurrentTime >= BaslangicSaati && CurrentTime <= BitisSaati;

            // Check if it's after close time for day-end position closing
            IsAfterCloseTime = CurrentTime >= GunSonuKapatmaSaati;
        }

        /// <summary>
        /// Check if current time is within trading hours
        /// </summary>
        public bool IsWithinTradingHours()
        {
            if (!TimeFilteringEnabled)
                return true;

            return IsInTradingSession;
        }

        /// <summary>
        /// Check if it's time to close day-end positions
        /// </summary>
        public bool ShouldCloseDayEndPosition()
        {
            if (!GunSonuPozKapatEnabled)
                return false;

            return IsAfterCloseTime;
        }

        #endregion
    }
}
