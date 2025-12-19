namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Take Profit and Stop Loss - Manages profit taking and loss cutting levels
    /// Kar Al (Take Profit) and Zarar Kes (Stop Loss) management
    /// </summary>
    public class KarAlZararKes
    {
        #region Enable/Disable

        public bool KarAlEnabled { get; set; }
        public bool ZararKesEnabled { get; set; }
        public bool IzleyenStopEnabled { get; set; }

        #endregion

        #region Take Profit (Kar Al)

        public double KarAlSeviye { get; set; }
        public double KarAlYuzde { get; set; }
        public double KarAlPuan { get; set; }
        public double KarAlFiyat { get; set; }

        #endregion

        #region Stop Loss (Zarar Kes)

        public double ZararKesSeviye { get; set; }
        public double ZararKesYuzde { get; set; }
        public double ZararKesPuan { get; set; }
        public double ZararKesFiyat { get; set; }

        #endregion

        #region Trailing Stop (İzleyen Stop)

        public double IzleyenStopSeviye { get; set; }
        public double IzleyenStopYuzde { get; set; }
        public double IzleyenStopPuan { get; set; }
        public double IzleyenStopFiyat { get; set; }
        public double IzleyenStopEnYuksekFiyat { get; set; }
        public double IzleyenStopEnDusukFiyat { get; set; }

        #endregion

        #region Calculation Method

        public bool YuzdeIleHesapla { get; set; }
        public bool PuanIleHesapla { get; set; }
        public bool SeviyeIleHesapla { get; set; }

        #endregion

        #region Status

        public bool KarAlindi { get; set; }
        public bool ZararKesildi { get; set; }
        public bool IzleyenStopTetiklendi { get; set; }

        #endregion

        #region Constructor

        public KarAlZararKes()
        {
            Reset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset all values to defaults
        /// </summary>
        public KarAlZararKes Reset()
        {
            KarAlEnabled = false;
            ZararKesEnabled = false;
            IzleyenStopEnabled = false;

            KarAlSeviye = 0.0;
            KarAlYuzde = 0.0;
            KarAlPuan = 0.0;
            KarAlFiyat = 0.0;

            ZararKesSeviye = 0.0;
            ZararKesYuzde = 0.0;
            ZararKesPuan = 0.0;
            ZararKesFiyat = 0.0;

            IzleyenStopSeviye = 0.0;
            IzleyenStopYuzde = 0.0;
            IzleyenStopPuan = 0.0;
            IzleyenStopFiyat = 0.0;
            IzleyenStopEnYuksekFiyat = 0.0;
            IzleyenStopEnDusukFiyat = 0.0;

            YuzdeIleHesapla = false;
            PuanIleHesapla = false;
            SeviyeIleHesapla = false;

            KarAlindi = false;
            ZararKesildi = false;
            IzleyenStopTetiklendi = false;

            return this;
        }

        /// <summary>
        /// Initialize - Does nothing currently but returns this for method chaining
        /// </summary>
        public KarAlZararKes Init()
        {
            return this;
        }

        /// <summary>
        /// Set take profit level by percentage
        /// </summary>
        public void KarAlYuzdeAyarla(double yuzde)
        {
            KarAlYuzde = yuzde;
            YuzdeIleHesapla = true;
            KarAlEnabled = true;
        }

        /// <summary>
        /// Set stop loss level by percentage
        /// </summary>
        public void ZararKesYuzdeAyarla(double yuzde)
        {
            ZararKesYuzde = yuzde;
            YuzdeIleHesapla = true;
            ZararKesEnabled = true;
        }

        /// <summary>
        /// Set trailing stop by percentage
        /// </summary>
        public void IzleyenStopYuzdeAyarla(double yuzde)
        {
            IzleyenStopYuzde = yuzde;
            YuzdeIleHesapla = true;
            IzleyenStopEnabled = true;
        }

        /// <summary>
        /// Update trailing stop based on current price
        /// </summary>
        public void IzleyenStopGuncelle(double currentPrice, bool isLongPosition)
        {
            if (!IzleyenStopEnabled)
                return;

            if (isLongPosition)
            {
                // Long position: track highest price and set stop below it
                if (currentPrice > IzleyenStopEnYuksekFiyat)
                {
                    IzleyenStopEnYuksekFiyat = currentPrice;
                    IzleyenStopFiyat = IzleyenStopEnYuksekFiyat * (1 - IzleyenStopYuzde / 100.0);
                }
            }
            else
            {
                // Short position: track lowest price and set stop above it
                if (IzleyenStopEnDusukFiyat == 0 || currentPrice < IzleyenStopEnDusukFiyat)
                {
                    IzleyenStopEnDusukFiyat = currentPrice;
                    IzleyenStopFiyat = IzleyenStopEnDusukFiyat * (1 + IzleyenStopYuzde / 100.0);
                }
            }
        }

        #endregion
    }
}
