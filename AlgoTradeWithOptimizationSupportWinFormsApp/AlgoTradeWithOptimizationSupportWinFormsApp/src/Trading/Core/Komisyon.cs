using static SkiaSharp.HarfBuzz.SKShaper;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Commission - Manages trading commission calculations
    /// Komisyon hesaplamaları ve yönetimi
    /// </summary>
    public class Komisyon
    {
        private SingleTrader Trader { get; set; }

        #region Enable/Disable

        public bool Enabled { get; set; }
        public bool KomisyonuDahilEt { get; set; }

        #endregion

        #region Commission Rates

        public double KomisyonOrani { get; set; }           // % cinsinden
        public double MinKomisyon { get; set; }             // Minimum komisyon tutarı
        public double MaxKomisyon { get; set; }             // Maximum komisyon tutarı
        public double SabitKomisyon { get; set; }           // Sabit komisyon (işlem başına)

        #endregion

        #region Commission Calculation

        public double IslemBasiKomisyon { get; set; }       // Her işlem için komisyon
        public double VarlikAdediBasiKomisyon { get; set; } // Kontrat/Lot başına komisyon
        public double ToplamKomisyon { get; set; }          // Toplam ödenen komisyon

        #endregion

        #region Commission Multipliers

        public double KomisyonCarpani { get; set; }         // Komisyon çarpanı
        public int KomisyonIslemSayisi { get; set; }        // Komisyon ödenecek işlem sayısı
        public int KomisyonVarlikAdedi { get; set; }        // Komisyon ödenecek varlık adedi

        #endregion

        #region Market Specific

        public double BorsaTakas { get; set; }              // Borsa & Takas ücreti
        public double BSMV { get; set; }                    // Banka Sigorta Muamele Vergisi
        public double DigerMasraflar { get; set; }          // Diğer masraflar

        #endregion

        #region Constructor

        public Komisyon()
        {
            Reset();
        }

        #endregion

        #region Methods

        public void SetTrader(SingleTrader trader)
        {
            Trader = trader;
        }

        /// <summary>
        /// Reset all commission values to defaults
        /// </summary>
        public Komisyon Reset()
        {
            Enabled = false;
            KomisyonuDahilEt = true;

            KomisyonOrani = 0.0;
            MinKomisyon = 0.0;
            MaxKomisyon = 0.0;
            SabitKomisyon = 0.0;

            IslemBasiKomisyon = 0.0;
            VarlikAdediBasiKomisyon = 0.0;
            ToplamKomisyon = 0.0;

            KomisyonCarpani = 1.0;
            KomisyonIslemSayisi = 0;
            KomisyonVarlikAdedi = 0;

            BorsaTakas = 0.0;
            BSMV = 0.0;
            DigerMasraflar = 0.0;

            return this;
        }

        /// <summary>
        /// Initialize - Does nothing currently but returns this for method chaining
        /// </summary>
        public Komisyon Init()
        {
            return this;
        }

        public Komisyon InitOrReuse()
        {
            // No internal buffers to allocate; reuse
            return this;
        }

        /// <summary>
        /// Komisyon hesapla - Dinamik lot desteği ile
        /// Ters yön değişimlerinde 2 ayrı işlem için komisyon hesaplar
        /// Pyramiding (pozisyon artırma) durumlarında sadece eklenen lot için komisyon hesaplar
        ///
        /// EmirStatus Değerleri:
        /// 1: F → A (Flat'ten Long) - 1 işlem
        /// 2: S → A (Short kapat + Long aç) - 2 işlem ⚠️ ÖZEL DURUM
        /// 3: F → S (Flat'ten Short) - 1 işlem
        /// 4: A → S (Long kapat + Short aç) - 2 işlem ⚠️ ÖZEL DURUM
        /// 5: A → F (Long kapat) - 1 işlem
        /// 6: S → F (Short kapat) - 1 işlem
        /// 10: A → A (Long pozisyon artırma) - 1 işlem (pyramiding)
        /// 11: S → S (Short pozisyon artırma) - 1 işlem (pyramiding)
        /// </summary>
        public void Hesapla(int i)
        {
            if (Trader == null)
                return;

            double totalCommission = 0.0;
            double komisyonCarpan = Trader.status.KomisyonCarpan;

            // O bar'daki komisyon işlem sayısı artışını hesapla (toplam değil!)
            int oncekiKomisyonIslemSayisi = i > 0 ? (int)Trader.lists.KomisyonIslemSayisiList[i - 1] : 0;
            int mevcutKomisyonIslemSayisi = (int)Trader.lists.KomisyonIslemSayisiList[i];
            int komisyonIslemSayisi = mevcutKomisyonIslemSayisi - oncekiKomisyonIslemSayisi;

            // EmirStatus kontrol et
            int emirStatus = Trader.signals.EmirStatus;

            if (Trader.flags.KomisyonGuncelle == false)
            {
                // Komisyon güncellenmese bile listeyi son durumla güncelle
                Trader.lists.KomisyonFiyatList[i] = Trader.status.KomisyonFiyat;
                return;
            }

            bool isMicroLot = Trader.pozisyonBuyuklugu.MicroLotSizeEnabled;

            // Ters yön değişimi kontrolü (2 ayrı işlem)
            if (emirStatus == 2 || emirStatus == 4)
            {
                // DURUM 1: Ters Yön Değişimi (S→A veya A→S)
                // 2 ayrı işlem: Close + Open
                // Her işlem için sabit komisyon varlık adedi kullan
                double komisyonVolume = isMicroLot
                    ? Trader.status.KomisyonVarlikAdedSayisiMicro
                    : Trader.status.KomisyonVarlikAdedSayisi;

                // Her iki işlem için de ayrı komisyon hesapla
                double closeCommission = komisyonCarpan * komisyonVolume;
                double openCommission = komisyonCarpan * komisyonVolume;

                totalCommission = closeCommission + openCommission;
            }
            else if (komisyonIslemSayisi > 0)
            {
                // DURUM 2: Normal işlem (Tek işlem - açma veya kapatma)
                // EmirStatus = 1, 3, 5, 6

                // Sabit komisyon varlık adedi kullan (dinamik volume yerine)
                double komisyonVolume = isMicroLot
                    ? Trader.status.KomisyonVarlikAdedSayisiMicro
                    : Trader.status.KomisyonVarlikAdedSayisi;

                if (emirStatus == 10 || emirStatus == 11)
                {
                    // DURUM 3: Pyramiding (A→A veya S→S: Pozisyon artırma)
                    // Sadece eklenen lot için komisyon hesapla
                    // Eklenen lot = Toplam lot - Önceki lot
                    double sonLot = isMicroLot
                        ? Trader.signals.SonVarlikAdedSayisiMicro
                        : Trader.signals.SonVarlikAdedSayisi;

                    double prevLot = isMicroLot
                        ? Trader.signals.PrevVarlikAdedSayisiMicro
                        : Trader.signals.PrevVarlikAdedSayisi;

                    komisyonVolume = sonLot - prevLot;  // Eklenen lot miktarı
                }

                totalCommission = komisyonIslemSayisi * komisyonCarpan * komisyonVolume;
            }

            // Sonucu kaydet (kümülatif toplam - diğer metriklerle tutarlı)
            Trader.status.KomisyonFiyat += totalCommission;  // Bu bar'daki komisyonu toplama ekle
            Trader.lists.KomisyonFiyatList[i] = Trader.status.KomisyonFiyat;  // Her bar güncel kümülatif değeri kaydet
        }

        /// <summary>
        /// Calculate commission for a trade
        /// </summary>
        public double Hesapla2(double islemTutari, int varlikAdedi = 1)
        {
            if (!Enabled || !KomisyonuDahilEt)
                return 0.0;

            double komisyon = 0.0;

            // Sabit komisyon varsa
            if (SabitKomisyon > 0)
            {
                komisyon = SabitKomisyon;
            }
            // Oran üzerinden hesaplama
            else if (KomisyonOrani > 0)
            {
                komisyon = islemTutari * (KomisyonOrani / 100.0);

                // Min/Max kontrolleri
                if (MinKomisyon > 0 && komisyon < MinKomisyon)
                    komisyon = MinKomisyon;

                if (MaxKomisyon > 0 && komisyon > MaxKomisyon)
                    komisyon = MaxKomisyon;
            }

            // Varlık adedi bazlı komisyon
            if (VarlikAdediBasiKomisyon > 0)
            {
                komisyon += VarlikAdediBasiKomisyon * varlikAdedi;
            }

            // Borsa & Takas ekle
            komisyon += BorsaTakas;

            // BSMV ekle
            if (BSMV > 0)
            {
                komisyon += komisyon * (BSMV / 100.0);
            }

            // Diğer masraflar
            komisyon += DigerMasraflar;

            // Çarpan uygula
            komisyon *= KomisyonCarpani;

            // Toplama ekle
            ToplamKomisyon += komisyon;
            KomisyonIslemSayisi++;

            return komisyon;
        }

        /// <summary>
        /// Set standard BIST (Borsa Istanbul) commission rates for stocks
        /// </summary>
        public void BISTHisseKomisyonuAyarla()
        {
            Enabled = true;
            KomisyonuDahilEt = true;
            KomisyonOrani = 0.188;  // %0.188 (typical BIST stock commission)
            BSMV = 0.1;             // %0.1 BSMV
            BorsaTakas = 0.0;
            MinKomisyon = 5.0;      // Minimum 5 TL
        }

        /// <summary>
        /// Set standard VIOP (futures) commission rates
        /// </summary>
        public void VIOPKomisyonuAyarla(double kontratBasiKomisyon = 2.0)
        {
            Enabled = true;
            KomisyonuDahilEt = true;
            VarlikAdediBasiKomisyon = kontratBasiKomisyon;  // Kontrat başına komisyon
            BSMV = 0.1;  // %0.1 BSMV
        }

        /// <summary>
        /// Set standard Forex commission rates
        /// </summary>
        public void ForexKomisyonuAyarla(double spreadPuan = 0.0)
        {
            Enabled = true;
            KomisyonuDahilEt = true;
            // Forex typically uses spread, not commission
            SabitKomisyon = spreadPuan;
        }

        #endregion
    }
}
