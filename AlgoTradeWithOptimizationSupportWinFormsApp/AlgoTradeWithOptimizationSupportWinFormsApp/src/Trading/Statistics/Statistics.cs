using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics
{
    /// <summary>
    /// Statistics - Comprehensive trading statistics calculator
    /// İstatistik hesaplamaları ve raporlama
    /// </summary>
    public class Statistics
    {
        #region Private Fields

        private SingleTrader Trader { get; set; }

        #endregion

        #region Identification
        public int Id => Trader?.Id ?? 0;
        public string Name => Trader?.Name ?? "...";
        #endregion

        #region System Info
        public string GrafikSembol => Trader?.SymbolName ?? "...";
        public string GrafikPeriyot => Trader?.SymbolPeriod.ToString() ?? "...";
        public string SistemId => Trader?.SystemId ?? "...";
        public string SistemName => Trader?.SystemName ?? "...";
        public string StrategyId => Trader?.StrategyId ?? "...";
        public string StrategyName => Trader?.StrategyName ?? "...";
        #endregion

        #region Execution Info

        public string LastExecutionId => Trader?.LastExecutionId ?? "...";
        public string LastExecutionTime => Trader?.LastExecutionTime ?? "";
        public string LastExecutionTimeStart => Trader?.LastExecutionTimeStart ?? "";
        public string LastExecutionTimeStop => Trader?.LastExecutionTimeStop ?? "";
        public string LastExecutionTimeInMSec => Trader?.LastExecutionTimeInMSec ?? "";
        public string LastResetTime => Trader?.LastResetTime ?? "";
        public string LastStatisticsCalculationTime => Trader?.LastStatisticsCalculationTime ?? "";

        #endregion

        #region Bar Info

        public int ToplamBarSayisi { get; set; }
        public int IlkBarIndex { get; set; }
        public int SonBarIndex { get; set; }
        public int SecilenBarNumarasi { get; set; }

        public string IlkBarTarihSaati { get; set; }
        public string IlkBarTarihi { get; set; }
        public string IlkBarSaati { get; set; }

        public string SonBarTarihSaati { get; set; }
        public string SonBarTarihi { get; set; }
        public string SonBarSaati { get; set; }

        public string SecilenBarTarihSaati { get; set; }
        public string SecilenBarTarihi { get; set; }
        public string SecilenBarSaati { get; set; }

        public double SecilenBarAcilisFiyati { get; set; }
        public double SecilenBarYuksekFiyati { get; set; }
        public double SecilenBarDusukFiyati { get; set; }
        public double SecilenBarKapanisFiyati { get; set; }
        public double SonBarAcilisFiyati { get; set; }
        public double SonBarYuksekFiyati { get; set; }
        public double SonBarDusukFiyati { get; set; }
        public double SonBarKapanisFiyati { get; set; }

        #endregion

        #region Time Statistics

        public double ToplamGecenSureAy { get; set; }
        public int ToplamGecenSureGun { get; set; }
        public int ToplamGecenSureSaat { get; set; }
        public int ToplamGecenSureDakika { get; set; }
        public double OrtAylikIslemSayisi { get; set; }
        public double OrtHaftalikIslemSayisi { get; set; }
        public double OrtGunlukIslemSayisi { get; set; }
        public double OrtSaatlikIslemSayisi { get; set; }

        #endregion

        #region Trade Counts

        public int IslemSayisi => Trader?.status?.IslemSayisi ?? 0;
        public int AlisSayisi => Trader?.status?.AlisSayisi ?? 0;
        public int SatisSayisi => Trader?.status?.SatisSayisi ?? 0;
        public int FlatSayisi => Trader?.status?.FlatSayisi ?? 0;
        public int PassSayisi => Trader?.status?.PassSayisi ?? 0;
        public int KarAlSayisi => Trader?.status?.KarAlSayisi ?? 0;
        public int ZararKesSayisi => Trader?.status?.ZararKesSayisi ?? 0;
        public int KazandiranIslemSayisi => Trader?.status?.KazandiranIslemSayisi ?? 0;
        public int KaybettirenIslemSayisi => Trader?.status?.KaybettirenIslemSayisi ?? 0;
        public int NotrIslemSayisi => Trader?.status?.NotrIslemSayisi ?? 0;
        public int KazandiranAlisSayisi => Trader?.status?.KazandiranAlisSayisi ?? 0;
        public int KaybettirenAlisSayisi => Trader?.status?.KaybettirenAlisSayisi ?? 0;
        public int NotrAlisSayisi => Trader?.status?.NotrAlisSayisi ?? 0;
        public int KazandiranSatisSayisi => Trader?.status?.KazandiranSatisSayisi ?? 0;
        public int KaybettirenSatisSayisi => Trader?.status?.KaybettirenSatisSayisi ?? 0;
        public int NotrSatisSayisi => Trader?.status?.NotrSatisSayisi ?? 0;

        #endregion

        #region Command Counts

        public int AlKomutSayisi => Trader?.status?.AlKomutSayisi ?? 0;
        public int SatKomutSayisi => Trader?.status?.SatKomutSayisi ?? 0;
        public int PasGecKomutSayisi => Trader?.status?.PasGecKomutSayisi ?? 0;
        public int KarAlKomutSayisi => Trader?.status?.KarAlKomutSayisi ?? 0;
        public int ZararKesKomutSayisi => Trader?.status?.ZararKesKomutSayisi ?? 0;
        public int FlatOlKomutSayisi => Trader?.status?.FlatOlKomutSayisi ?? 0;

        #endregion

        #region Bar Status

        public int KardaBarSayisi => Trader?.status?.KardaBarSayisi ?? 0;
        public int ZarardaBarSayisi => Trader?.status?.ZarardaBarSayisi ?? 0;

        #endregion

        #region PnL

        public double KarZararFiyat => Trader?.status?.KarZararFiyat ?? 0;
        public double KarZararPuan => Trader?.status?.KarZararPuan ?? 0;
        public double KarZararFiyatYuzde => Trader?.status?.KarZararFiyatYuzde ?? 0;
        public double ToplamKarFiyat => Trader?.status?.ToplamKarFiyat ?? 0;
        public double ToplamZararFiyat => Trader?.status?.ToplamZararFiyat ?? 0;
        public double NetKarFiyat => Trader?.status?.NetKarFiyat ?? 0;
        public double ToplamKarPuan => Trader?.status?.ToplamKarPuan ?? 0;
        public double ToplamZararPuan => Trader?.status?.ToplamZararPuan ?? 0;
        public double NetKarPuan => Trader?.status?.NetKarPuan ?? 0;
        public double MaxKarFiyat { get; set; }
        public double MaxZararFiyat { get; set; }
        public double MaxKarPuan { get; set; }
        public double MaxZararPuan { get; set; }
        public int MaxZararFiyatIndex { get; set; }
        public int MaxKarFiyatIndex { get; set; }
        public int MaxZararPuanIndex { get; set; }
        public int MaxKarPuanIndex { get; set; }

        #endregion

        #region Commission

        public int KomisyonIslemSayisi => Trader?.status?.KomisyonIslemSayisi ?? 0;
        public double KomisyonVarlikAdedSayisi => Trader?.status?.KomisyonVarlikAdedSayisi ?? 0;
        public double KomisyonVarlikAdedSayisiMicro => Trader?.status?.KomisyonVarlikAdedSayisiMicro ?? 0;
        public double KomisyonCarpan => Trader?.status?.KomisyonCarpan ?? 0;
        public double KomisyonFiyat { get; set; }  // Toplam komisyon (Hesapla() metodunda hesaplanır)
        public double KomisyonFiyatYuzde { get; set; }
        public bool KomisyonuDahilEt => Trader?.flags?.KomisyonuDahilEt ?? false;

        #endregion

        #region Balance

        public double IlkBakiyeFiyat => Trader?.status?.IlkBakiyeFiyat ?? 0;
        public double IlkBakiyePuan => Trader?.status?.IlkBakiyePuan ?? 0;
        public double BakiyeFiyat => Trader?.status?.BakiyeFiyat ?? 0;
        public double BakiyePuan => Trader?.status?.BakiyePuan ?? 0;
        public double GetiriFiyat => Trader?.status?.GetiriFiyat ?? 0;
        public double GetiriPuan => Trader?.status?.GetiriPuan ?? 0;
        public double GetiriFiyatYuzde => Trader?.status?.GetiriFiyatYuzde ?? 0;
        public double GetiriPuanYuzde => Trader?.status?.GetiriPuanYuzde ?? 0;
        public double BakiyeFiyatNet => Trader?.status?.BakiyeFiyatNet ?? 0;
        public double BakiyePuanNet => Trader?.status?.BakiyePuanNet ?? 0;
        public double GetiriFiyatNet => Trader?.status?.GetiriFiyatNet ?? 0;
        public double GetiriPuanNet => Trader?.status?.GetiriPuanNet ?? 0;
        public double GetiriFiyatYuzdeNet => Trader?.status?.GetiriFiyatYuzdeNet ?? 0;
        public double GetiriPuanYuzdeNet => Trader?.status?.GetiriPuanYuzdeNet ?? 0;
        public double GetiriKz => Trader?.status?.GetiriKz ?? 0;
        public double GetiriKzNet => Trader?.status?.GetiriKzNet ?? 0;
        public double GetiriKzSistem => Trader?.status?.GetiriKzSistem ?? 0;
        public double GetiriKzNetSistem => Trader?.status?.GetiriKzNetSistem ?? 0;
        public double GetiriKzSistemYuzde { get; set; }
        public double GetiriKzNetSistemYuzde { get; set; }
        public int GetiriFiyatTipi { get; set; }

        #endregion

        #region Balance Min/Max

        public double MinBakiyeFiyat { get; set; }
        public double MaxBakiyeFiyat { get; set; }
        public double MinBakiyePuan { get; set; }
        public double MaxBakiyePuan { get; set; }
        public double MinBakiyeFiyatYuzde { get; set; }
        public double MaxBakiyeFiyatYuzde { get; set; }
        public int MinBakiyeFiyatIndex { get; set; }
        public int MaxBakiyeFiyatIndex { get; set; }
        public int MinBakiyePuanIndex { get; set; }
        public int MaxBakiyePuanIndex { get; set; }
        public double MinBakiyeFiyatNet { get; set; }
        public double MaxBakiyeFiyatNet { get; set; }
        public int MinBakiyeFiyatNetIndex { get; set; }
        public int MaxBakiyeFiyatNetIndex { get; set; }
        public double MinBakiyeFiyatNetYuzde { get; set; }
        public double MaxBakiyeFiyatNetYuzde { get; set; }

        #endregion

        #region Drawdown

        public double GetiriMaxDD { get; set; }
        public string GetiriMaxDDTarih { get; set; }
        public double GetiriMaxKayip { get; set; }

        #endregion

        #region Performance Metrics

        public double ProfitFactor { get; set; }
        public double ProfitFactorSistem { get; set; }
        public double KarliIslemOrani { get; set; }

        #endregion

        #region Asset Info

        public double HisseSayisi => Trader?.status?.HisseSayisi ?? 0;
        public double KontratSayisi => Trader?.status?.KontratSayisi ?? 0;
        public double VarlikAdedCarpani => Trader?.status?.VarlikAdedCarpani ?? 0;
        public double VarlikAdedSayisi => Trader?.status?.VarlikAdedSayisi ?? 0;
        public double VarlikAdedSayisiMicro => Trader?.status?.VarlikAdedSayisiMicro ?? 0;
        public double KaymaMiktari => Trader?.status?.KaymaMiktari ?? 0;
        public bool KaymayiDahilEt => Trader?.flags?.KaymayiDahilEt ?? false;

        // New Pyramiding fields
        public bool PyramidingEnabled => Trader?.pozisyonBuyuklugu?.PyramidingEnabled ?? false;
        public bool MaxPositionSizeEnabled => Trader?.pozisyonBuyuklugu?.MaxPositionSizeEnabled ?? false;
        public double MaxPositionSize => Trader?.pozisyonBuyuklugu?.MaxPositionSize ?? 0;
        public double MaxPositionSizeMicro => Trader?.pozisyonBuyuklugu?.MaxPositionSizeMicro ?? 0;
        public bool MicroLotSizeEnabled => Trader?.pozisyonBuyuklugu?.MicroLotSizeEnabled ?? false;
        #endregion

        #region Signals

        public string Sinyal => Trader?.signals?.Sinyal ?? "";
        public string SonYon => Trader?.signals?.SonYon ?? "";
        public string PrevYon => Trader?.signals?.PrevYon ?? "";
        public double SonFiyat => Trader?.signals?.SonFiyat ?? 0;
        public double SonAFiyat => Trader?.signals?.SonAFiyat ?? 0;
        public double SonSFiyat => Trader?.signals?.SonSFiyat ?? 0;
        public double SonFFiyat => Trader?.signals?.SonFFiyat ?? 0;
        public double SonPFiyat => Trader?.signals?.SonPFiyat ?? 0;
        public double PrevFiyat => Trader?.signals?.PrevFiyat ?? 0;
        public double PrevAFiyat => Trader?.signals?.PrevAFiyat ?? 0;
        public double PrevSFiyat => Trader?.signals?.PrevSFiyat ?? 0;
        public double PrevFFiyat => Trader?.signals?.PrevFFiyat ?? 0;
        public double PrevPFiyat => Trader?.signals?.PrevPFiyat ?? 0;
        public int SonBarNo => Trader?.signals?.SonBarNo ?? 0;
        public int SonABarNo => Trader?.signals?.SonABarNo ?? 0;
        public int SonSBarNo => Trader?.signals?.SonSBarNo ?? 0;
        public int SonFBarNo => Trader?.signals?.SonFBarNo ?? 0;
        public int SonPBarNo => Trader?.signals?.SonPBarNo ?? 0;
        public int PrevBarNo => Trader?.signals?.PrevBarNo ?? 0;
        public int PrevABarNo => Trader?.signals?.PrevABarNo ?? 0;
        public int PrevSBarNo => Trader?.signals?.PrevSBarNo ?? 0;
        public int PrevFBarNo => Trader?.signals?.PrevFBarNo ?? 0;
        public int PrevPBarNo => Trader?.signals?.PrevPBarNo ?? 0;
        public int EmirKomut => Trader?.signals?.EmirKomut ?? 0;
        public int EmirStatus => Trader?.signals?.EmirStatus ?? 0;

        // New Micro Signal fields
        public double SonVarlikAdedSayisiMicro => Trader?.signals?.SonVarlikAdedSayisiMicro ?? 0;
        public double PrevVarlikAdedSayisiMicro => Trader?.signals?.PrevVarlikAdedSayisiMicro ?? 0;
        #endregion

        #region Periodic Returns - Month

        public double GetiriFiyatBuAy { get; set; }
        public double GetiriFiyatAy1 { get; set; }
        public double GetiriFiyatAy2 { get; set; }
        public double GetiriFiyatAy3 { get; set; }
        public double GetiriFiyatAy4 { get; set; }
        public double GetiriFiyatAy5 { get; set; }
        public double GetiriPuanBuAy { get; set; }
        public double GetiriPuanAy1 { get; set; }
        public double GetiriPuanAy2 { get; set; }
        public double GetiriPuanAy3 { get; set; }
        public double GetiriPuanAy4 { get; set; }
        public double GetiriPuanAy5 { get; set; }

        #endregion

        #region Periodic Returns - Week

        public double GetiriFiyatBuHafta { get; set; }
        public double GetiriFiyatHafta1 { get; set; }
        public double GetiriFiyatHafta2 { get; set; }
        public double GetiriFiyatHafta3 { get; set; }
        public double GetiriFiyatHafta4 { get; set; }
        public double GetiriFiyatHafta5 { get; set; }
        public double GetiriPuanBuHafta { get; set; }
        public double GetiriPuanHafta1 { get; set; }
        public double GetiriPuanHafta2 { get; set; }
        public double GetiriPuanHafta3 { get; set; }
        public double GetiriPuanHafta4 { get; set; }
        public double GetiriPuanHafta5 { get; set; }

        #endregion

        #region Periodic Returns - Day

        public double GetiriFiyatBuGun { get; set; }
        public double GetiriFiyatGun1 { get; set; }
        public double GetiriFiyatGun2 { get; set; }
        public double GetiriFiyatGun3 { get; set; }
        public double GetiriFiyatGun4 { get; set; }
        public double GetiriFiyatGun5 { get; set; }
        public double GetiriPuanBuGun { get; set; }
        public double GetiriPuanGun1 { get; set; }
        public double GetiriPuanGun2 { get; set; }
        public double GetiriPuanGun3 { get; set; }
        public double GetiriPuanGun4 { get; set; }
        public double GetiriPuanGun5 { get; set; }

        #endregion

        #region Periodic Returns - Hour

        public double GetiriFiyatBuSaat { get; set; }
        public double GetiriFiyatSaat1 { get; set; }
        public double GetiriFiyatSaat2 { get; set; }
        public double GetiriFiyatSaat3 { get; set; }
        public double GetiriFiyatSaat4 { get; set; }
        public double GetiriFiyatSaat5 { get; set; }
        public double GetiriPuanBuSaat { get; set; }
        public double GetiriPuanSaat1 { get; set; }
        public double GetiriPuanSaat2 { get; set; }
        public double GetiriPuanSaat3 { get; set; }
        public double GetiriPuanSaat4 { get; set; }
        public double GetiriPuanSaat5 { get; set; }

        #endregion

        #region Statistics Map

        private const string SEPARATOR = "#SEPARATOR#";

        public Dictionary<string, string> StatisticsMap { get; set; }
        public Dictionary<string, string> StatisticsMapMinimal { get; set; }

        #endregion

        #region Constructor

        public Statistics()
        {
            StatisticsMap = new Dictionary<string, string>();
            StatisticsMapMinimal = new Dictionary<string, string>();
            IlkBarIndex = 0;
        }

        #endregion

        #region Methods

        public Statistics Initialize(SingleTrader trader)
        {
            Trader = trader;
            return this;
        }

        public Statistics Init(SingleTrader trader)
        {
            Trader = trader;
            return this;
        }

        public Statistics Reset()
        {
            StatisticsMap.Clear();
            StatisticsMapMinimal.Clear();
            return this;
        }

        public int Hesapla(int secilenBarNumarasi)
        {
            int result = 0;

            if (Trader == null)
                return result;

            ReadValues();

            Trader.LastStatisticsCalculationTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

            int firstBarIndex = 0;
            int lastBarIndex  = Trader.Data.Count - 1;
            ToplamBarSayisi   = Trader.Data.Count;

            this.SecilenBarNumarasi = secilenBarNumarasi;
            if (this.SecilenBarNumarasi < firstBarIndex) {
                this.SecilenBarNumarasi = firstBarIndex;
            }
            else if (this.SecilenBarNumarasi > lastBarIndex) {
                this.SecilenBarNumarasi = lastBarIndex;
            }

            IlkBarTarihSaati        = Trader.Data[firstBarIndex].DateTime.ToString("yyyy.MM.dd HH:mm:ss");
            IlkBarTarihi            = Trader.Data[firstBarIndex].DateTime.ToString("yyyy.MM.dd");
            IlkBarSaati             = Trader.Data[firstBarIndex].DateTime.ToString("HH:mm:ss");

            SonBarTarihSaati        = Trader.Data[lastBarIndex].DateTime.ToString("yyyy.MM.dd HH:mm:ss");
            SonBarTarihi            = Trader.Data[lastBarIndex].DateTime.ToString("yyyy.MM.dd");
            SonBarSaati             = Trader.Data[lastBarIndex].DateTime.ToString("HH:mm:ss");

            SecilenBarTarihSaati    = Trader.Data[this.SecilenBarNumarasi].DateTime.ToString("yyyy.MM.dd HH:mm:ss");
            SecilenBarTarihi        = Trader.Data[this.SecilenBarNumarasi].DateTime.ToString("yyyy.MM.dd");
            SecilenBarSaati         = Trader.Data[this.SecilenBarNumarasi].DateTime.ToString("HH:mm:ss");

            SecilenBarAcilisFiyati  = Trader.Data[this.SecilenBarNumarasi].Open;
            SecilenBarYuksekFiyati  = Trader.Data[this.SecilenBarNumarasi].High;
            SecilenBarDusukFiyati   = Trader.Data[this.SecilenBarNumarasi].Low;
            SecilenBarKapanisFiyati = Trader.Data[this.SecilenBarNumarasi].Close;

            SonBarAcilisFiyati      = Trader.Data[lastBarIndex].Open;
            SonBarYuksekFiyati      = Trader.Data[lastBarIndex].High;
            SonBarDusukFiyati       = Trader.Data[lastBarIndex].Low;
            SonBarKapanisFiyati     = Trader.Data[lastBarIndex].Close;
            SonBarIndex             = lastBarIndex;

            // Calculate time elapsed
            DateTime firstDate = Trader.Data[0].Date;
            TimeSpan elapsed = DateTime.Now - firstDate;
            double sureDakika = elapsed.TotalMinutes;
            double sureSaat = elapsed.TotalHours;
            int sureGun = elapsed.Days;
            double sureAy = sureGun / 30.4;

            ToplamGecenSureAy      = sureAy;
            ToplamGecenSureGun     = sureGun;
            ToplamGecenSureSaat    = (int)sureSaat;
            ToplamGecenSureDakika  = (int)sureDakika;
            OrtAylikIslemSayisi    = ToplamGecenSureAy > 0 ? 1.0 * IslemSayisi / ToplamGecenSureAy : 0;
            OrtHaftalikIslemSayisi = 0.0;
            OrtGunlukIslemSayisi   = ToplamGecenSureGun > 0 ? 1.0 * IslemSayisi / ToplamGecenSureGun : 0;
            OrtSaatlikIslemSayisi  = ToplamGecenSureSaat > 0 ? 1.0 * IslemSayisi / ToplamGecenSureSaat : 0;

            // Maximum Drawdown hesaplaması
            double maxBakiye = IlkBakiyeFiyat;
            double maxDD = 0.0;
            double maxDDYuzde = 0.0;
            string maxDDTarih = "";

            for (int i = 0; i < Trader.Data.Count; i++)
            {
                double mevcutBakiye = Trader.lists.BakiyeFiyatList[i];

                // Yeni maksimum bakiye kontrolü
                if (mevcutBakiye > maxBakiye)
                {
                    maxBakiye = mevcutBakiye;
                }

                // Drawdown hesapla (maksimum bakiyeden ne kadar düşmüş)
                double drawdown = maxBakiye - mevcutBakiye;
                double drawdownYuzde = maxBakiye > 0 ? (drawdown / maxBakiye) * 100.0 : 0.0;

                // En büyük drawdown kontrolü
                if (drawdownYuzde > maxDDYuzde)
                {
                    maxDDYuzde = drawdownYuzde;
                    maxDD = drawdown;
                    maxDDTarih = Trader.Data[i].DateTime.ToString("yyyy.MM.dd HH:mm:ss");
                }
            }

            GetiriMaxDD            = maxDDYuzde;
            GetiriMaxDDTarih       = maxDDTarih;
            GetiriMaxKayip         = maxDD;

            MaxKarFiyat            = 0.0;
            MaxZararFiyat          = 0.0;
            MaxKarPuan             = 0.0;
            MaxZararPuan           = 0.0;
            MinBakiyeFiyat         = IlkBakiyeFiyat;
            MaxBakiyeFiyat         = IlkBakiyeFiyat;
            MinBakiyeFiyatNet      = IlkBakiyeFiyat;
            MaxBakiyeFiyatNet      = IlkBakiyeFiyat;
            MinBakiyePuan          = IlkBakiyePuan;
            MaxBakiyePuan          = IlkBakiyePuan;

            // Toplam komisyonu al (son bar'daki kümülatif değer)
            KomisyonFiyat = lastBarIndex >= 0 ? Trader.lists.KomisyonFiyatList[lastBarIndex] : 0.0;

            // Find min/max values
            for (int i = 1; i < Trader.Data.Count; i++)
            {
                if (Trader.lists.KarZararFiyatList[i] < MaxZararFiyat)
                {
                    MaxZararFiyat = Trader.lists.KarZararFiyatList[i];
                    MaxZararFiyatIndex = i;
                }
                if (Trader.lists.KarZararFiyatList[i] > MaxKarFiyat)
                {
                    MaxKarFiyat = Trader.lists.KarZararFiyatList[i];
                    MaxKarFiyatIndex = i;
                }
                if (Trader.lists.KarZararPuanList[i] < MaxZararPuan)
                {
                    MaxZararPuan = Trader.lists.KarZararPuanList[i];
                    MaxZararPuanIndex = i;
                }
                if (Trader.lists.KarZararPuanList[i] > MaxKarPuan)
                {
                    MaxKarPuan = Trader.lists.KarZararPuanList[i];
                    MaxKarPuanIndex = i;
                }
                if (Trader.lists.BakiyeFiyatList[i] < MinBakiyeFiyat)
                {
                    MinBakiyeFiyat = Trader.lists.BakiyeFiyatList[i];
                    MinBakiyeFiyatIndex = i;
                }
                if (Trader.lists.BakiyeFiyatList[i] > MaxBakiyeFiyat)
                {
                    MaxBakiyeFiyat = Trader.lists.BakiyeFiyatList[i];
                    MaxBakiyeFiyatIndex = i;
                }
                if (Trader.lists.BakiyeFiyatNetList[i] < MinBakiyeFiyatNet)
                {
                    MinBakiyeFiyatNet = Trader.lists.BakiyeFiyatNetList[i];
                    MinBakiyeFiyatNetIndex = i;
                }
                if (Trader.lists.BakiyeFiyatNetList[i] > MaxBakiyeFiyatNet)
                {
                    MaxBakiyeFiyatNet = Trader.lists.BakiyeFiyatNetList[i];
                    MaxBakiyeFiyatNetIndex = i;
                }
                if (Trader.lists.BakiyePuanList[i] < MinBakiyePuan)
                {
                    MinBakiyePuan = Trader.lists.BakiyePuanList[i];
                    MinBakiyePuanIndex = i;
                }
                if (Trader.lists.BakiyePuanList[i] > MaxBakiyePuan)
                {
                    MaxBakiyePuan = Trader.lists.BakiyePuanList[i];
                    MaxBakiyePuanIndex = i;
                }
            }

            // Calculate performance metrics
            ProfitFactor           = Math.Abs(ToplamZararPuan) > 0 ? ToplamKarPuan / Math.Abs(ToplamZararPuan) : 0;
            ProfitFactorSistem     = 0.0;
            KarliIslemOrani        = IslemSayisi > 0 ? (1.0 * KazandiranIslemSayisi) / (1.0 * IslemSayisi) * 100.0 : 0;

            MinBakiyeFiyatYuzde    = IlkBakiyeFiyat != 0 ? (MinBakiyeFiyat - IlkBakiyeFiyat) * 100.0 / IlkBakiyeFiyat : 0;
            MaxBakiyeFiyatYuzde    = IlkBakiyeFiyat != 0 ? (MaxBakiyeFiyat - IlkBakiyeFiyat) * 100.0 / IlkBakiyeFiyat : 0;
            MinBakiyeFiyatNetYuzde = IlkBakiyeFiyat != 0 ? (MinBakiyeFiyatNet - IlkBakiyeFiyat) * 100.0 / IlkBakiyeFiyat : 0;
            MaxBakiyeFiyatNetYuzde = IlkBakiyeFiyat != 0 ? (MaxBakiyeFiyatNet - IlkBakiyeFiyat) * 100.0 / IlkBakiyeFiyat : 0;
            KomisyonFiyatYuzde     = GetiriFiyatYuzde - GetiriFiyatYuzdeNet;
            GetiriKzSistemYuzde    = 0.0;
            GetiriKzNetSistemYuzde = 0.0;

            GetiriIstatistikleriHesapla();

            AssignToMap();

            AssignToMapMinimal();

            return result;
        }

        private void ReadValues()
        {
            // All identification, system, and execution properties are now proxies.
            // No manual assignments needed here.
        }

        private void GetiriIstatistikleriHesapla()
        {
            // TODO: Implement periodic return calculations
            // This would calculate monthly, weekly, daily, and hourly returns
        }

        private void AssignToMap()
        {
            int keyId = 0;

            StatisticsMap.Clear();

            // Helper to add null-safe and formatted values
            void Add(string key, object value, string format = "")
            {
                if (value == null || (value is string s && string.IsNullOrEmpty(s))) 
                {
                    StatisticsMap[key] = "...";
                    return;
                }
                StatisticsMap[key] = string.IsNullOrEmpty(format) ? value.ToString() : string.Format("{0:" + format + "}", value);
            }

            // --- Identification ---
            Add("TraderId", Id);
            Add("TraderName", Name);

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- System & Execution Info ---
            Add("SymbolName", GrafikSembol);
            Add("SymbolPeriod", GrafikPeriyot);
            Add("SystemId", SistemId);
            Add("SystemName", SistemName);
            Add("StrategyId", StrategyId);
            Add("StrategyName", StrategyName);

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            Add("LastExecutionId", LastExecutionId);
            Add("LastExecutionTime", LastExecutionTime);
            Add("LastExecutionTimeStart", LastExecutionTimeStart);
            Add("LastExecutionTimeStop", LastExecutionTimeStop);
            Add("LastExecutionTimeInMSec", LastExecutionTimeInMSec);
            Add("LastResetTime", LastResetTime);
            Add("LastStatisticsCalculationTime", LastStatisticsCalculationTime);

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Bar Info ---
            Add("ToplamBarSayisi", ToplamBarSayisi);
            Add("SecilenBarNumarasi", SecilenBarNumarasi);
            Add("SecilenBarTarihSaati", SecilenBarTarihSaati);
            Add("SecilenBarTarihi", SecilenBarTarihi);
            Add("SecilenBarSaati", SecilenBarSaati);

            Add("IlkBarTarihSaati", IlkBarTarihSaati);
            Add("IlkBarTarihi", IlkBarTarihi);
            Add("IlkBarSaati", IlkBarSaati);

            Add("SonBarTarihSaati", SonBarTarihSaati);
            Add("SonBarTarihi", SonBarTarihi);
            Add("SonBarSaati", SonBarSaati);

            Add("IlkBarIndex", IlkBarIndex);
            Add("SonBarIndex", SonBarIndex);
            Add("SonBarAcilisFiyati", SonBarAcilisFiyati, "F4");
            Add("SonBarYuksekFiyati", SonBarYuksekFiyati, "F4");
            Add("SonBarDusukFiyati", SonBarDusukFiyati, "F4");
            Add("SonBarKapanisFiyati", SonBarKapanisFiyati, "F4");

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Time Statistics ---
            Add("ToplamGecenSureAy", ToplamGecenSureAy, "F1");
            Add("ToplamGecenSureGun", ToplamGecenSureGun);
            Add("ToplamGecenSureSaat", ToplamGecenSureSaat);
            Add("ToplamGecenSureDakika", ToplamGecenSureDakika);
            Add("OrtAylikIslemSayisi", OrtAylikIslemSayisi, "F2");
            Add("OrtHaftalikIslemSayisi", OrtHaftalikIslemSayisi, "F2");
            Add("OrtGunlukIslemSayisi", OrtGunlukIslemSayisi, "F2");
            Add("OrtSaatlikIslemSayisi", OrtSaatlikIslemSayisi, "F2");

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Balance & Returns ---
            Add("IlkBakiyeFiyat", IlkBakiyeFiyat, "F2");
            Add("IlkBakiyePuan", IlkBakiyePuan, "F2");
            Add("BakiyeFiyat", BakiyeFiyat, "F2");
            Add("BakiyePuan", BakiyePuan, "F2");
            Add("GetiriFiyat", GetiriFiyat, "F2");
            Add("GetiriPuan", GetiriPuan, "F2");
            Add("GetiriFiyatYuzde", GetiriFiyatYuzde, "F2");
            Add("GetiriPuanYuzde", GetiriPuanYuzde, "F2");
            Add("BakiyeFiyatNet", BakiyeFiyatNet, "F2");
            Add("BakiyePuanNet", BakiyePuanNet, "F2");
            Add("GetiriFiyatNet", GetiriFiyatNet, "F2");
            Add("GetiriPuanNet", GetiriPuanNet, "F2");
            Add("GetiriFiyatYuzdeNet", GetiriFiyatYuzdeNet, "F2");
            Add("GetiriPuanYuzdeNet", GetiriPuanYuzdeNet, "F2");
            Add("GetiriKz", GetiriKz, "F4");
            Add("GetiriKzNet", GetiriKzNet, "F4");
            Add("GetiriKzSistem", GetiriKzSistem, "F4");
            Add("GetiriKzSistemYuzde", GetiriKzSistemYuzde, "F2");
            Add("GetiriKzNetSistem", GetiriKzNetSistem, "F4");
            Add("GetiriKzNetSistemYuzde", GetiriKzNetSistemYuzde, "F2");

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Min/Max Balance ---
            Add("MinBakiyeFiyat", MinBakiyeFiyat, "F2");
            Add("MaxBakiyeFiyat", MaxBakiyeFiyat, "F2");
            Add("MinBakiyePuan", MinBakiyePuan, "F2");
            Add("MaxBakiyePuan", MaxBakiyePuan, "F2");
            Add("MinBakiyeFiyatYuzde", MinBakiyeFiyatYuzde, "F2");
            Add("MaxBakiyeFiyatYuzde", MaxBakiyeFiyatYuzde, "F2");
            Add("MinBakiyeFiyatIndex", MinBakiyeFiyatIndex);
            Add("MaxBakiyeFiyatIndex", MaxBakiyeFiyatIndex);
            Add("MinBakiyeFiyatNet", MinBakiyeFiyatNet, "F2");
            Add("MaxBakiyeFiyatNet", MaxBakiyeFiyatNet, "F2");

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Trade Counts ---
            Add("IslemSayisi", IslemSayisi);
            Add("AlisSayisi", AlisSayisi);
            Add("SatisSayisi", SatisSayisi);
            Add("FlatSayisi", FlatSayisi);
            Add("PassSayisi", PassSayisi);
            Add("KarAlSayisi", KarAlSayisi);
            Add("ZararKesSayisi", ZararKesSayisi);
            Add("KazandiranIslemSayisi", KazandiranIslemSayisi);
            Add("KaybettirenIslemSayisi", KaybettirenIslemSayisi);
            Add("NotrIslemSayisi", NotrIslemSayisi);
            Add("KazandiranAlisSayisi", KazandiranAlisSayisi);
            Add("KaybettirenAlisSayisi", KaybettirenAlisSayisi);
            Add("NotrAlisSayisi", NotrAlisSayisi);
            Add("KazandiranSatisSayisi", KazandiranSatisSayisi);
            Add("KaybettirenSatisSayisi", KaybettirenSatisSayisi);
            Add("NotrSatisSayisi", NotrSatisSayisi);

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Command Counts ---
            Add("AlKomutSayisi", AlKomutSayisi);
            Add("SatKomutSayisi", SatKomutSayisi);
            Add("PasGecKomutSayisi", PasGecKomutSayisi);
            Add("KarAlKomutSayisi", KarAlKomutSayisi);
            Add("ZararKesKomutSayisi", ZararKesKomutSayisi);
            Add("FlatOlKomutSayisi", FlatOlKomutSayisi);

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Commission ---
            Add("KomisyonIslemSayisi", KomisyonIslemSayisi);
            Add("KomisyonVarlikAdedSayisi", KomisyonVarlikAdedSayisi, "F2");
            Add("KomisyonVarlikAdedSayisiMicro", KomisyonVarlikAdedSayisiMicro, "F4");
            Add("KomisyonCarpan", KomisyonCarpan, "F4");
            Add("KomisyonFiyat", KomisyonFiyat, "F2");
            Add("KomisyonFiyatYuzde", KomisyonFiyatYuzde, "F4");
            Add("KomisyonuDahilEt", KomisyonuDahilEt);

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- PnL Aggregates ---
            Add("KarZararFiyat", KarZararFiyat, "F2");
            Add("KarZararFiyatYuzde", KarZararFiyatYuzde, "F2");
            Add("KarZararPuan", KarZararPuan, "F4");
            Add("ToplamKarFiyat", ToplamKarFiyat, "F2");
            Add("ToplamZararFiyat", ToplamZararFiyat, "F2");
            Add("NetKarFiyat", NetKarFiyat, "F2");
            Add("ToplamKarPuan", ToplamKarPuan, "F4");
            Add("ToplamZararPuan", ToplamZararPuan, "F4");
            Add("NetKarPuan", NetKarPuan, "F4");
            Add("MaxKarFiyat", MaxKarFiyat, "F2");
            Add("MaxZararFiyat", MaxZararFiyat, "F2");
            Add("MaxKarPuan", MaxKarPuan, "F4");
            Add("MaxZararPuan", MaxZararPuan, "F4");
            Add("KardaBarSayisi", KardaBarSayisi);
            Add("ZarardaBarSayisi", ZarardaBarSayisi);
            Add("KarliIslemOrani", KarliIslemOrani, "F2");

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Risk Metrics ---
            Add("GetiriMaxDD", GetiriMaxDD, "F2");
            Add("GetiriMaxDDTarih", GetiriMaxDDTarih);
            Add("GetiriMaxKayip", GetiriMaxKayip, "F2");
            Add("ProfitFactor", ProfitFactor, "F2");
            Add("ProfitFactorSistem", ProfitFactorSistem, "F2");

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Signals & Execution ---
            Add("Sinyal", Sinyal);
            Add("SonYon", SonYon);
            Add("PrevYon", PrevYon);
            Add("SonFiyat", SonFiyat, "F4");
            Add("SonAFiyat", SonAFiyat, "F4");
            Add("SonSFiyat", SonSFiyat, "F4");
            Add("SonFFiyat", SonFFiyat, "F4");
            Add("SonPFiyat", SonPFiyat, "F4");
            Add("PrevFiyat", PrevFiyat, "F4");
            Add("SonBarNo", SonBarNo);
            Add("SonABarNo", SonABarNo);
            Add("SonSBarNo", SonSBarNo);
            Add("EmirKomut", EmirKomut);
            Add("EmirStatus", EmirStatus);

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Asset & Position Info ---
            Add("HisseSayisi", HisseSayisi, "F2");
            Add("KontratSayisi", KontratSayisi, "F2");
            Add("VarlikAdedCarpani", VarlikAdedCarpani, "F2");
            Add("VarlikAdedSayisi", VarlikAdedSayisi, "F2");
            Add("VarlikAdedSayisiMicro", VarlikAdedSayisiMicro, "F4");
            Add("KaymaMiktari", KaymaMiktari, "F4");
            Add("KaymayiDahilEt", KaymayiDahilEt);

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            Add("MicroLotSizeEnabled", MicroLotSizeEnabled);
            Add("PyramidingEnabled", PyramidingEnabled);
            Add("MaxPositionSizeEnabled", MaxPositionSizeEnabled);
            Add("MaxPositionSize", MaxPositionSize, "F4");
            Add("MaxPositionSizeMicro", MaxPositionSizeMicro, "F4");

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            // --- Periodic Returns ---
            Add("GetiriFiyatBuAy", GetiriFiyatBuAy, "F2");
            Add("GetiriFiyatAy1", GetiriFiyatAy1, "F2");
            Add("GetiriFiyatBuHafta", GetiriFiyatBuHafta, "F2");
            Add("GetiriFiyatHafta1", GetiriFiyatHafta1, "F2");
            Add("GetiriFiyatBuGun", GetiriFiyatBuGun, "F2");
            Add("GetiriFiyatGun1", GetiriFiyatGun1, "F2");
            Add("GetiriFiyatBuSaat", GetiriFiyatBuSaat, "F2");
            Add("GetiriFiyatSaat1", GetiriFiyatSaat1, "F2");

            StatisticsMap[SEPARATOR + keyId++.ToString()] = "";

            Add("GetiriPuanBuAy", GetiriPuanBuAy, "F4");
            Add("GetiriPuanAy1", GetiriPuanAy1, "F4");
            Add("GetiriPuanBuHafta", GetiriPuanBuHafta, "F4");
            Add("GetiriPuanHafta1", GetiriPuanHafta1, "F4");
            Add("GetiriPuanBuGun", GetiriPuanBuGun, "F4");
            Add("GetiriPuanGun1", GetiriPuanGun1, "F4");
            Add("GetiriPuanBuSaat", GetiriPuanBuSaat, "F4");
            Add("GetiriPuanSaat1", GetiriPuanSaat1, "F4");
        }

        /// <summary>
        /// AssignToMapMinimal - Minimal statistics set for quick reporting
        /// Only essential metrics for optimization and quick analysis
        /// </summary>
        private void AssignToMapMinimal()
        {
            int keyId = 0;

            StatisticsMapMinimal.Clear();

            // Helper to add null-safe and formatted values
            void Add(string key, object value, string format = "")
            {
                if (value == null || (value is string s && string.IsNullOrEmpty(s)))
                {
                    StatisticsMapMinimal[key] = "...";
                    return;
                }
                StatisticsMapMinimal[key] = string.IsNullOrEmpty(format) ? value.ToString() : string.Format("{0:" + format + "}", value);
            }

            // --- Identification ---
            Add("TraderId", Id);
            Add("TraderName", Name);

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            // --- System & Execution Info ---
            Add("SymbolName", GrafikSembol);
            Add("SymbolPeriod", GrafikPeriyot);
            Add("SystemId", SistemId);
            Add("SystemName", SistemName);
            Add("StrategyId", StrategyId);
            Add("StrategyName", StrategyName);

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            Add("LastExecutionId", LastExecutionId);
            Add("LastExecutionTime", LastExecutionTime);
            Add("LastExecutionTimeStart", LastExecutionTimeStart);
            Add("LastExecutionTimeStop", LastExecutionTimeStop);
            Add("LastExecutionTimeInMSec", LastExecutionTimeInMSec);
            Add("LastResetTime", LastResetTime);
            Add("LastStatisticsCalculationTime", LastStatisticsCalculationTime);

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            // --- Bar Info ---
            Add("ToplamBarSayisi", ToplamBarSayisi);
            Add("IlkBarTarihSaati", IlkBarTarihSaati);
            Add("IlkBarTarihi", IlkBarTarihi);
            Add("IlkBarSaati", IlkBarSaati);
            Add("SonBarTarihSaati", SonBarTarihSaati);
            Add("SonBarTarihi", SonBarTarihi);
            Add("SonBarSaati", SonBarSaati);
            Add("IlkBarIndex", IlkBarIndex);
            Add("SonBarIndex", SonBarIndex);

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            // --- Time Statistics ---
            Add("ToplamGecenSureAy", ToplamGecenSureAy, "F1");
            Add("ToplamGecenSureGun", ToplamGecenSureGun);
            Add("ToplamGecenSureSaat", ToplamGecenSureSaat);
            Add("ToplamGecenSureDakika", ToplamGecenSureDakika);
            Add("OrtAylikIslemSayisi", OrtAylikIslemSayisi, "F2");
            Add("OrtHaftalikIslemSayisi", OrtHaftalikIslemSayisi, "F2");
            Add("OrtGunlukIslemSayisi", OrtGunlukIslemSayisi, "F2");
            Add("OrtSaatlikIslemSayisi", OrtSaatlikIslemSayisi, "F2");

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            // --- Balance (Initial) ---
            Add("IlkBakiyeFiyat", IlkBakiyeFiyat, "F2");
            // --- Balance (Current) ---
            Add("BakiyeFiyat", BakiyeFiyat, "F2");
            Add("GetiriFiyat", GetiriFiyat, "F2");
            Add("GetiriFiyatYuzde", GetiriFiyatYuzde, "F2");
            Add("BakiyeFiyatNet", BakiyeFiyatNet, "F2");
            Add("GetiriFiyatNet", GetiriFiyatNet, "F2");
            Add("GetiriFiyatYuzdeNet", GetiriFiyatYuzdeNet, "F2");

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            // --- Balance (Min/Max) ---
            Add("MinBakiyeFiyat", MinBakiyeFiyat, "F2");
            Add("MaxBakiyeFiyat", MaxBakiyeFiyat, "F2");
            Add("MinBakiyeFiyatYuzde", MinBakiyeFiyatYuzde, "F2");
            Add("MaxBakiyeFiyatYuzde", MaxBakiyeFiyatYuzde, "F2");
            Add("MinBakiyeFiyatIndex", MinBakiyeFiyatIndex);
            Add("MaxBakiyeFiyatIndex", MaxBakiyeFiyatIndex);
            Add("MinBakiyeFiyatNet", MinBakiyeFiyatNet, "F2");
            Add("MaxBakiyeFiyatNet", MaxBakiyeFiyatNet, "F2");

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            // --- Trade Counts ---
            Add("IslemSayisi", IslemSayisi);
            Add("AlisSayisi", AlisSayisi);
            Add("SatisSayisi", SatisSayisi);
            Add("FlatSayisi", FlatSayisi);
            Add("PassSayisi", PassSayisi);
            Add("KarAlSayisi", KarAlSayisi);
            Add("ZararKesSayisi", ZararKesSayisi);
            Add("KazandiranIslemSayisi", KazandiranIslemSayisi);
            Add("KaybettirenIslemSayisi", KaybettirenIslemSayisi);
            Add("NotrIslemSayisi", NotrIslemSayisi);

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            // --- Commission ---
            Add("KomisyonIslemSayisi", KomisyonIslemSayisi);
            Add("KomisyonVarlikAdedSayisi", KomisyonVarlikAdedSayisi, "F2");
            Add("KomisyonVarlikAdedSayisiMicro", KomisyonVarlikAdedSayisiMicro, "F4");
            Add("KomisyonCarpan", KomisyonCarpan, "F4");
            Add("KomisyonFiyat", KomisyonFiyat, "F2");
            Add("KomisyonFiyatYuzde", KomisyonFiyatYuzde, "F4");
            Add("KomisyonuDahilEt", KomisyonuDahilEt);

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            // --- Performance Metrics ---
            Add("KarliIslemOrani", KarliIslemOrani, "F2");
            Add("GetiriMaxDD", GetiriMaxDD, "F2");
            Add("GetiriMaxDDTarih", GetiriMaxDDTarih);
            Add("GetiriMaxKayip", GetiriMaxKayip, "F2");
            Add("ProfitFactor", ProfitFactor, "F2");

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";

            // --- Asset & Position Info ---
            Add("HisseSayisi", HisseSayisi, "F2");
            Add("KontratSayisi", KontratSayisi, "F2");
            Add("VarlikAdedCarpani", VarlikAdedCarpani, "F2");
            Add("VarlikAdedSayisi", VarlikAdedSayisi, "F2");
            Add("VarlikAdedSayisiMicro", VarlikAdedSayisiMicro, "F4");
            Add("KaymaMiktari", KaymaMiktari, "F4");
            Add("KaymayiDahilEt", KaymayiDahilEt);

            StatisticsMapMinimal[SEPARATOR + keyId++.ToString()] = "";
            Add("MicroLotSizeEnabled", MicroLotSizeEnabled);
            Add("PyramidingEnabled", PyramidingEnabled);
            Add("MaxPositionSizeEnabled", MaxPositionSizeEnabled);
            Add("MaxPositionSize", MaxPositionSize, "F4");
            Add("MaxPositionSizeMicro", MaxPositionSizeMicro, "F4");
        }
        
        public void SaveToTxt(string filePath)
        {
            AssignToMap();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"TRADING STATISTICS REPORT - {SistemName} ({GrafikSembol})");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy.MM.dd HH:mm:ss}");
            sb.AppendLine("================================================================================");
            sb.AppendLine($"{"Property Name".PadRight(40)} : Value");
            sb.AppendLine("--------------------------------------------------------------------------------");

            foreach (var kvp in StatisticsMap)
            {
                if (kvp.Key.StartsWith(SEPARATOR))
                    sb.AppendLine();  // Boş satır
                else
                    sb.AppendLine($"{kvp.Key.PadRight(40)} : {kvp.Value}");
            }

            sb.AppendLine("================================================================================");
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public void SaveToCsv(string filePath)
        {
            AssignToMap();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Key;Value");

            foreach (var kvp in StatisticsMap)
            {
                if (!kvp.Key.StartsWith(SEPARATOR))  // SEPARATOR satırlarını CSV'ye ekleme
                    sb.AppendLine($"{kvp.Key};{kvp.Value}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Save minimal statistics to TXT file (faster, essential metrics only)
        /// Ideal for optimization runs and quick analysis
        /// </summary>
        public void SaveToTxtMinimal(string filePath)
        {
            AssignToMapMinimal();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"TRADING STATISTICS REPORT (MINIMAL) - {SistemName} ({GrafikSembol})");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy.MM.dd HH:mm:ss}");
            sb.AppendLine("================================================================================");
            sb.AppendLine($"{"Property Name".PadRight(40)} : Value");
            sb.AppendLine("--------------------------------------------------------------------------------");

            foreach (var kvp in StatisticsMapMinimal)
            {
                if (kvp.Key.StartsWith(SEPARATOR))
                    sb.AppendLine();  // Boş satır
                else
                    sb.AppendLine($"{kvp.Key.PadRight(40)} : {kvp.Value}");
            }

            sb.AppendLine("================================================================================");
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Save minimal statistics to CSV file (faster, essential metrics only)
        /// Ideal for optimization runs and data analysis
        /// </summary>
        public void SaveToCsvMinimal(string filePath)
        {
            AssignToMapMinimal();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Key;Value");

            foreach (var kvp in StatisticsMapMinimal)
            {
                if (!kvp.Key.StartsWith(SEPARATOR))  // SEPARATOR satırlarını CSV'ye ekleme
                    sb.AppendLine($"{kvp.Key};{kvp.Value}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        #endregion
    }
}
