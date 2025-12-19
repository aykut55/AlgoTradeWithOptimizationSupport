using System;
using System.Collections.Generic;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Time filter - Advanced time-based filtering for trading operations
    /// Controls when trading is allowed based on time, day, and session rules
    /// </summary>
    public class TimeFilter
    {
        #region Filter Enable/Disable

        public bool Enabled { get; set; }
        public bool SeansFiltresiEnabled { get; set; }
        public bool GunFiltresiEnabled { get; set; }
        public bool SaatFiltresiEnabled { get; set; }

        #endregion

        #region Session Times

        public TimeOnly SeansBaslangic { get; set; }
        public TimeOnly SeansBitis { get; set; }
        public TimeOnly OgleArasi_Baslangic { get; set; }
        public TimeOnly OgleArasi_Bitis { get; set; }

        #endregion

        #region Trading Time Windows

        public TimeOnly IslemBaslangicSaati { get; set; }
        public TimeOnly IslemBitisSaati { get; set; }
        public TimeOnly AcilisIslemSaati { get; set; }
        public TimeOnly KapanisIslemSaati { get; set; }

        #endregion

        #region Day of Week Filter

        public bool PazartesiIslemYapilsin { get; set; }
        public bool SaliIslemYapilsin { get; set; }
        public bool CarsambaIslemYapilsin { get; set; }
        public bool PersembeIslemYapilsin { get; set; }
        public bool CumaIslemYapilsin { get; set; }
        public bool CumartesiIslemYapilsin { get; set; }
        public bool PazarIslemYapilsin { get; set; }

        #endregion

        #region Special Days

        public bool TatilGunuIslemYapilsin { get; set; }
        public List<DateOnly> TatilGunleri { get; set; }

        #endregion

        #region Position Closing Rules

        public bool GunSonuPozisyonKapat { get; set; }
        public TimeOnly GunSonuKapatmaSaati { get; set; }
        public bool HaftaSonuPozisyonKapat { get; set; }
        public TimeOnly HaftaSonuKapatmaSaati { get; set; }

        #endregion

        #region Session Breaks

        public bool OgleArasiEnabled { get; set; }
        public bool OgleArasiPozisyonKapat { get; set; }

        #endregion

        #region Constructor

        public TimeFilter()
        {
            TatilGunleri = new List<DateOnly>();
            Reset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset all time filter values to defaults
        /// </summary>
        public TimeFilter Reset()
        {
            Enabled = false;
            SeansFiltresiEnabled = false;
            GunFiltresiEnabled = false;
            SaatFiltresiEnabled = false;

            // Default Istanbul Stock Exchange hours (09:30 - 18:00)
            SeansBaslangic = new TimeOnly(9, 30, 0);
            SeansBitis = new TimeOnly(18, 0, 0);
            OgleArasi_Baslangic = new TimeOnly(12, 30, 0);
            OgleArasi_Bitis = new TimeOnly(14, 0, 0);

            // Default trading window
            IslemBaslangicSaati = new TimeOnly(10, 0, 0);
            IslemBitisSaati = new TimeOnly(17, 30, 0);
            AcilisIslemSaati = new TimeOnly(9, 35, 0);
            KapanisIslemSaati = new TimeOnly(17, 55, 0);

            // Allow trading on weekdays by default
            PazartesiIslemYapilsin = true;
            SaliIslemYapilsin = true;
            CarsambaIslemYapilsin = true;
            PersembeIslemYapilsin = true;
            CumaIslemYapilsin = true;
            CumartesiIslemYapilsin = false;
            PazarIslemYapilsin = false;

            // Special days
            TatilGunuIslemYapilsin = false;
            TatilGunleri?.Clear();

            // Position closing
            GunSonuPozisyonKapat = true;
            GunSonuKapatmaSaati = new TimeOnly(17, 55, 0);
            HaftaSonuPozisyonKapat = true;
            HaftaSonuKapatmaSaati = new TimeOnly(17, 50, 0);

            // Session breaks
            OgleArasiEnabled = false;
            OgleArasiPozisyonKapat = false;

            return this;
        }

        /// <summary>
        /// Initialize time filter - Does nothing currently but returns this for method chaining
        /// </summary>
        public TimeFilter Init()
        {
            return this;
        }

        /// <summary>
        /// Check if trading is allowed at the given time
        /// </summary>
        public bool IslemYapilabilirMi(DateTime barTime)
        {
            if (!Enabled)
                return true;

            var currentTime = TimeOnly.FromDateTime(barTime);
            var currentDate = DateOnly.FromDateTime(barTime);
            var dayOfWeek = barTime.DayOfWeek;

            // Check holiday
            if (TatilGunleri.Contains(currentDate) && !TatilGunuIslemYapilsin)
                return false;

            // Check day of week filter
            if (GunFiltresiEnabled)
            {
                bool dayAllowed = dayOfWeek switch
                {
                    DayOfWeek.Monday => PazartesiIslemYapilsin,
                    DayOfWeek.Tuesday => SaliIslemYapilsin,
                    DayOfWeek.Wednesday => CarsambaIslemYapilsin,
                    DayOfWeek.Thursday => PersembeIslemYapilsin,
                    DayOfWeek.Friday => CumaIslemYapilsin,
                    DayOfWeek.Saturday => CumartesiIslemYapilsin,
                    DayOfWeek.Sunday => PazarIslemYapilsin,
                    _ => false
                };

                if (!dayAllowed)
                    return false;
            }

            // Check session time filter
            if (SeansFiltresiEnabled)
            {
                if (currentTime < SeansBaslangic || currentTime > SeansBitis)
                    return false;
            }

            // Check lunch break
            if (OgleArasiEnabled)
            {
                if (currentTime >= OgleArasi_Baslangic && currentTime <= OgleArasi_Bitis)
                    return false;
            }

            // Check trading hours filter
            if (SaatFiltresiEnabled)
            {
                if (currentTime < IslemBaslangicSaati || currentTime > IslemBitisSaati)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if it's time to close positions for end of day
        /// </summary>
        public bool GunSonuPozisyonKapatilsinMi(DateTime barTime)
        {
            if (!GunSonuPozisyonKapat)
                return false;

            var currentTime = TimeOnly.FromDateTime(barTime);
            return currentTime >= GunSonuKapatmaSaati;
        }

        /// <summary>
        /// Check if it's time to close positions for weekend
        /// </summary>
        public bool HaftaSonuPozisyonKapatilsinMi(DateTime barTime)
        {
            if (!HaftaSonuPozisyonKapat)
                return false;

            var dayOfWeek = barTime.DayOfWeek;
            var currentTime = TimeOnly.FromDateTime(barTime);

            // Friday end of day
            if (dayOfWeek == DayOfWeek.Friday && currentTime >= HaftaSonuKapatmaSaati)
                return true;

            return false;
        }

        /// <summary>
        /// Check if it's lunch break time and positions should be closed
        /// </summary>
        public bool OgleArasiPozisyonKapatilsinMi(DateTime barTime)
        {
            if (!OgleArasiEnabled || !OgleArasiPozisyonKapat)
                return false;

            var currentTime = TimeOnly.FromDateTime(barTime);
            return currentTime >= OgleArasi_Baslangic && currentTime < OgleArasi_Bitis;
        }

        /// <summary>
        /// Add holiday date to the list
        /// </summary>
        public void TatilGunuEkle(DateOnly date)
        {
            if (!TatilGunleri.Contains(date))
            {
                TatilGunleri.Add(date);
            }
        }

        /// <summary>
        /// Remove holiday date from the list
        /// </summary>
        public void TatilGunuCikar(DateOnly date)
        {
            TatilGunleri.Remove(date);
        }

        #endregion
    }
}
