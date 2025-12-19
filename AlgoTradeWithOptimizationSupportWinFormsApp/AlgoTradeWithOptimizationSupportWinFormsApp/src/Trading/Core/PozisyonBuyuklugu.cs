namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Position Size - Manages position sizing for different asset types
    /// Pozisyon büyüklüğü yönetimi (Kontrat, Lot, Hisse)
    /// </summary>
    public class PozisyonBuyuklugu
    {
        #region Asset Counts

        public int KontratSayisi { get; set; }          // Contract count (futures/derivatives)
        public double LotSayisi { get; set; }           // Lot count (forex)
        public int HisseSayisi { get; set; }            // Stock count (equities)
        public int VarlikAdedi { get; set; }            // General asset count

        #endregion

        #region Micro Lot Support

        public bool MicroLotEnabled { get; set; }       // Enable fractional lot trading
        public double MicroLotSayisi { get; set; }      // Micro lot count (0.01, 0.1, etc.)

        #endregion

        #region Multipliers

        public double VarlikAdedCarpani { get; set; }   // Asset count multiplier
        public double KontratCarpani { get; set; }      // Contract multiplier
        public double LotCarpani { get; set; }          // Lot multiplier

        #endregion

        #region Position Limits

        public int MaxKontratSayisi { get; set; }       // Maximum contract count
        public double MaxLotSayisi { get; set; }        // Maximum lot count
        public int MaxHisseSayisi { get; set; }         // Maximum stock count
        public int MinKontratSayisi { get; set; }       // Minimum contract count
        public double MinLotSayisi { get; set; }        // Minimum lot count
        public int MinHisseSayisi { get; set; }         // Minimum stock count

        #endregion

        #region Position Sizing Rules

        public bool SabitPozisyonBuyuklugu { get; set; }    // Use fixed position size
        public bool OranliPozisyonBuyuklugu { get; set; }   // Use proportional position size
        public double PozisyonOrani { get; set; }           // Position size ratio (% of capital)
        public double RiskOrani { get; set; }               // Risk ratio per trade (%)

        #endregion

        #region Calculation Method

        public bool BakiyeIleHesapla { get; set; }      // Calculate based on balance
        public bool RiskIleHesapla { get; set; }        // Calculate based on risk
        public bool SabitMiktarKullan { get; set; }     // Use fixed amount

        #endregion

        #region Status

        public int AktifPozisyonSayisi { get; set; }    // Active position count
        public double ToplamVarlikAdedi { get; set; }   // Total asset count in positions

        #endregion

        #region Constructor

        public PozisyonBuyuklugu()
        {
            Reset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset all position size values to defaults
        /// </summary>
        public PozisyonBuyuklugu Reset()
        {
            KontratSayisi = 1;
            LotSayisi = 1.0;
            HisseSayisi = 100;
            VarlikAdedi = 1;

            MicroLotEnabled = false;
            MicroLotSayisi = 0.01;

            VarlikAdedCarpani = 1.0;
            KontratCarpani = 1.0;
            LotCarpani = 1.0;

            MaxKontratSayisi = 100;
            MaxLotSayisi = 100.0;
            MaxHisseSayisi = 100000;
            MinKontratSayisi = 1;
            MinLotSayisi = 0.01;
            MinHisseSayisi = 1;

            SabitPozisyonBuyuklugu = true;
            OranliPozisyonBuyuklugu = false;
            PozisyonOrani = 100.0;      // %100 of capital by default
            RiskOrani = 2.0;            // 2% risk per trade

            BakiyeIleHesapla = false;
            RiskIleHesapla = false;
            SabitMiktarKullan = true;

            AktifPozisyonSayisi = 0;
            ToplamVarlikAdedi = 0.0;

            return this;
        }

        /// <summary>
        /// Initialize - Does nothing currently but returns this for method chaining
        /// </summary>
        public PozisyonBuyuklugu Init()
        {
            return this;
        }

        /// <summary>
        /// Calculate position size based on balance and risk
        /// </summary>
        public int HesaplaKontrat(double bakiye, double riskMiktari = 0.0)
        {
            if (SabitMiktarKullan)
                return KontratSayisi;

            if (RiskIleHesapla && riskMiktari > 0)
            {
                // Calculate based on risk amount
                double riskTutari = bakiye * (RiskOrani / 100.0);
                int kontratSayisi = (int)(riskTutari / riskMiktari);
                return LimitKontrol(kontratSayisi);
            }

            if (BakiyeIleHesapla)
            {
                // Calculate based on balance ratio
                double hedefTutar = bakiye * (PozisyonOrani / 100.0);
                // This would need contract value to calculate properly
                // For now, return default
                return LimitKontrol(KontratSayisi);
            }

            return LimitKontrol(KontratSayisi);
        }

        /// <summary>
        /// Calculate lot size based on balance and risk
        /// </summary>
        public double HesaplaLot(double bakiye, double riskMiktari = 0.0)
        {
            if (SabitMiktarKullan)
                return LotSayisi;

            if (RiskIleHesapla && riskMiktari > 0)
            {
                double riskTutari = bakiye * (RiskOrani / 100.0);
                double lotSayisi = riskTutari / riskMiktari;
                return LimitKontrolLot(lotSayisi);
            }

            if (BakiyeIleHesapla)
            {
                double hedefTutar = bakiye * (PozisyonOrani / 100.0);
                // This would need lot value to calculate properly
                return LimitKontrolLot(LotSayisi);
            }

            return LimitKontrolLot(LotSayisi);
        }

        /// <summary>
        /// Calculate stock count based on balance and price
        /// </summary>
        public int HesaplaHisse(double bakiye, double hisseFiyati)
        {
            if (SabitMiktarKullan)
                return HisseSayisi;

            if (hisseFiyati <= 0)
                return HisseSayisi;

            if (BakiyeIleHesapla || OranliPozisyonBuyuklugu)
            {
                double hedefTutar = bakiye * (PozisyonOrani / 100.0);
                int hisseSayisi = (int)(hedefTutar / hisseFiyati);
                return LimitKontrolHisse(hisseSayisi);
            }

            return LimitKontrolHisse(HisseSayisi);
        }

        /// <summary>
        /// Apply min/max limits to contract count
        /// </summary>
        private int LimitKontrol(int kontratSayisi)
        {
            if (kontratSayisi < MinKontratSayisi)
                return MinKontratSayisi;
            if (kontratSayisi > MaxKontratSayisi)
                return MaxKontratSayisi;
            return kontratSayisi;
        }

        /// <summary>
        /// Apply min/max limits to lot size
        /// </summary>
        private double LimitKontrolLot(double lotSayisi)
        {
            if (lotSayisi < MinLotSayisi)
                return MinLotSayisi;
            if (lotSayisi > MaxLotSayisi)
                return MaxLotSayisi;

            // Round to 2 decimal places for standard lots, or more for micro lots
            if (MicroLotEnabled)
                return Math.Round(lotSayisi, 3);
            else
                return Math.Round(lotSayisi, 2);
        }

        /// <summary>
        /// Apply min/max limits to stock count
        /// </summary>
        private int LimitKontrolHisse(int hisseSayisi)
        {
            if (hisseSayisi < MinHisseSayisi)
                return MinHisseSayisi;
            if (hisseSayisi > MaxHisseSayisi)
                return MaxHisseSayisi;
            return hisseSayisi;
        }

        /// <summary>
        /// Set fixed position size for contracts
        /// </summary>
        public void SabitKontratAyarla(int kontratSayisi)
        {
            KontratSayisi = kontratSayisi;
            SabitMiktarKullan = true;
            BakiyeIleHesapla = false;
            RiskIleHesapla = false;
        }

        /// <summary>
        /// Set fixed position size for lots
        /// </summary>
        public void SabitLotAyarla(double lotSayisi)
        {
            LotSayisi = lotSayisi;
            SabitMiktarKullan = true;
            BakiyeIleHesapla = false;
            RiskIleHesapla = false;
        }

        /// <summary>
        /// Set fixed position size for stocks
        /// </summary>
        public void SabitHisseAyarla(int hisseSayisi)
        {
            HisseSayisi = hisseSayisi;
            SabitMiktarKullan = true;
            BakiyeIleHesapla = false;
            RiskIleHesapla = false;
        }

        /// <summary>
        /// Set proportional position sizing based on balance percentage
        /// </summary>
        public void OranliPozisyonAyarla(double pozisyonOrani)
        {
            PozisyonOrani = pozisyonOrani;
            OranliPozisyonBuyuklugu = true;
            SabitPozisyonBuyuklugu = false;
            BakiyeIleHesapla = true;
            SabitMiktarKullan = false;
        }

        /// <summary>
        /// Set risk-based position sizing
        /// </summary>
        public void RiskBazliPozisyonAyarla(double riskOrani)
        {
            RiskOrani = riskOrani;
            RiskIleHesapla = true;
            SabitMiktarKullan = false;
        }

        #endregion
    }
}
