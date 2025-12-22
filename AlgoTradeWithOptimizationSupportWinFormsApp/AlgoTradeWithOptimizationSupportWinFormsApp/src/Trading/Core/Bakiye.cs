using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Account balance and equity management
    /// </summary>
    public class Bakiye
    {
        public double InitialBalance { get; set; }
        public double CurrentBalance { get; set; }
        public double AvailableBalance { get; set; }
        public double Equity { get; set; }
        public double Margin { get; set; }

        private SingleTrader Trader { get; set; }

        public Bakiye(double initialBalance = 100000)
        {
            InitialBalance = initialBalance;
            CurrentBalance = initialBalance;
            AvailableBalance = initialBalance;
            Equity = initialBalance;
            Margin = 0;
        }

        public void SetTrader(SingleTrader trader)
        {
            Trader = trader;
        }

        /// <summary>
        /// Total profit/loss
        /// </summary>
        public double TotalPnL => CurrentBalance - InitialBalance;

        /// <summary>
        /// Total profit/loss percentage
        /// </summary>
        public double TotalPnLPct => InitialBalance > 0 ? (TotalPnL / InitialBalance) * 100.0 : 0;

        /// <summary>
        /// Update balance after trade
        /// </summary>
        public void UpdateBalance(double pnl)
        {
            CurrentBalance += pnl;
            AvailableBalance = CurrentBalance - Margin;
            Equity = CurrentBalance;
        }

        /// <summary>
        /// Reserve margin
        /// </summary>
        public void ReserveMargin(double amount)
        {
            Margin += amount;
            AvailableBalance = CurrentBalance - Margin;
        }

        /// <summary>
        /// Release margin
        /// </summary>
        public void ReleaseMargin(double amount)
        {
            Margin -= amount;
            if (Margin < 0) Margin = 0;
            AvailableBalance = CurrentBalance - Margin;
        }

        /// <summary>
        /// Reset to initial state
        /// </summary>
        public void Reset()
        {
            CurrentBalance = InitialBalance;
            AvailableBalance = InitialBalance;
            Equity = InitialBalance;
            Margin = 0;
        }

        public void Init()
        {

        }

        public int Hesapla(int BarIndex)
        {
            int result = 0;
            int i = BarIndex;

            if (Trader == null)
                return result;

            // Bakiye (Puan)
            Trader.lists.BakiyePuanList[i] = Trader.status.BakiyePuan + Trader.lists.KarZararPuanList[i];
            Trader.lists.GetiriPuanList[i] = Trader.lists.BakiyePuanList[i] - Trader.status.IlkBakiyePuan;

            if (Trader.flags.BakiyeGuncelle)
            {
                Trader.status.BakiyePuan = Trader.lists.BakiyePuanList[i];
                Trader.status.GetiriPuan = Trader.lists.GetiriPuanList[i];

                if (Trader.lists.KarZararPuanList[i] >= 0)
                {
                    Trader.status.ToplamKarPuan += Trader.lists.KarZararPuanList[i];
                }
                else if (Trader.lists.KarZararPuanList[i] < 0)
                {
                    Trader.status.ToplamZararPuan += Trader.lists.KarZararPuanList[i];
                }

                Trader.status.NetKarPuan = Trader.status.ToplamKarPuan + Trader.status.ToplamZararPuan;
            }

            // Bakiye (Fiyat)
            Trader.lists.BakiyeFiyatList[i] = Trader.status.BakiyeFiyat + Trader.lists.KarZararFiyatList[i];
            Trader.lists.GetiriFiyatList[i] = Trader.lists.BakiyeFiyatList[i] - Trader.status.IlkBakiyeFiyat;

            if (Trader.flags.BakiyeGuncelle)
            {
                Trader.status.BakiyeFiyat = Trader.lists.BakiyeFiyatList[i];
                Trader.status.GetiriFiyat = Trader.lists.GetiriFiyatList[i];

                if (Trader.lists.KarZararFiyatList[i] >= 0)
                {
                    Trader.status.ToplamKarFiyat += Trader.lists.KarZararFiyatList[i];
                }
                else if (Trader.lists.KarZararFiyatList[i] < 0)
                {
                    Trader.status.ToplamZararFiyat += Trader.lists.KarZararFiyatList[i];
                }

                Trader.status.NetKarFiyat = Trader.status.ToplamKarFiyat + Trader.status.ToplamZararFiyat;
            }

            // Yüzde hesaplamaları (Puan)
            if (Trader.status.IlkBakiyePuan != 0.0)
            {
                Trader.lists.GetiriPuanYuzdeList[i] = 100.0 * Trader.lists.GetiriPuanList[i] / Trader.status.IlkBakiyePuan;
            }
            else
            {
                Trader.lists.GetiriPuanYuzdeList[i] = 0.0;
            }

            // Yüzde hesaplamaları (Fiyat)
            if (Trader.status.IlkBakiyeFiyat != 0.0)
            {
                Trader.lists.GetiriFiyatYuzdeList[i] = 100.0 * Trader.lists.GetiriFiyatList[i] / Trader.status.IlkBakiyeFiyat;
            }
            else
            {
                Trader.lists.GetiriFiyatYuzdeList[i] = 0.0;
            }

            if (Trader.flags.BakiyeGuncelle)
            {
                Trader.status.GetiriPuanYuzde = Trader.lists.GetiriPuanYuzdeList[i];
                Trader.status.GetiriFiyatYuzde = Trader.lists.GetiriFiyatYuzdeList[i];
            }

            // Net hesaplamalar (komisyon dahil)
            double k = Trader.status.KomisyonCarpan != 0.0 ? 1.0 : 0.0;

            Trader.lists.GetiriFiyatNetList[i] = Trader.lists.GetiriFiyatList[i] - Trader.lists.KomisyonFiyatList[i] * k;
            Trader.lists.BakiyeFiyatNetList[i] = Trader.lists.GetiriFiyatNetList[i] + Trader.status.IlkBakiyeFiyat;

            Trader.lists.GetiriFiyatYuzdeNetList[i] = 0.0;
            if (Trader.status.IlkBakiyeFiyat != 0.0)
            {
                Trader.lists.GetiriFiyatYuzdeNetList[i] = 100.0 * Trader.lists.GetiriFiyatNetList[i] / Trader.status.IlkBakiyeFiyat;
            }

            // MicroLotSizeEnabled flag'ine göre doğru varlık adedini kullan
            double varlikAdedSayisi = Trader.pozisyonBuyuklugu.MicroLotSizeEnabled
                ? Trader.pozisyonBuyuklugu.VarlikAdedSayisiMicro
                : Trader.pozisyonBuyuklugu.VarlikAdedSayisi;

            // Sıfıra bölme kontrolü
            if (varlikAdedSayisi != 0)
            {
                Trader.lists.GetiriKz[i] = Trader.lists.GetiriFiyatList[i] / varlikAdedSayisi;
                Trader.lists.GetiriKzNet[i] = Trader.lists.GetiriFiyatNetList[i] / varlikAdedSayisi;
            }
            else
            {
                Trader.lists.GetiriKz[i] = 0.0;
                Trader.lists.GetiriKzNet[i] = 0.0;
            }

            // Son bar kontrolü
            int barCount = Trader.Data.Count;
            if (i == barCount - 1)
            {
                Trader.status.BakiyeFiyat = Trader.lists.BakiyeFiyatList[barCount - 1];
                Trader.status.GetiriFiyat = Trader.lists.GetiriFiyatList[barCount - 1];
                Trader.status.GetiriKz = Trader.lists.GetiriKz[barCount - 1];
                Trader.status.GetiriFiyatYuzde = Trader.lists.GetiriFiyatYuzdeList[barCount - 1];
                Trader.status.BakiyeFiyatNet = Trader.lists.BakiyeFiyatNetList[barCount - 1];
                Trader.status.GetiriFiyatNet = Trader.lists.GetiriFiyatNetList[barCount - 1];
                Trader.status.GetiriKzNet = Trader.lists.GetiriKzNet[barCount - 1];
                Trader.status.GetiriFiyatYuzdeNet = Trader.lists.GetiriFiyatYuzdeNetList[barCount - 1];
                Trader.status.BakiyePuan = Trader.lists.BakiyePuanList[barCount - 1];
                Trader.status.GetiriPuan = Trader.lists.GetiriPuanList[barCount - 1];
                Trader.status.BakiyePuanNet = Trader.lists.BakiyePuanNetList[barCount - 1];
                Trader.status.GetiriPuanNet = Trader.lists.GetiriPuanNetList[barCount - 1];
                Trader.status.GetiriPuanYuzdeNet = Trader.lists.GetiriPuanYuzdeNetList[barCount - 1];
            }

            return result;
        }
    }
}
