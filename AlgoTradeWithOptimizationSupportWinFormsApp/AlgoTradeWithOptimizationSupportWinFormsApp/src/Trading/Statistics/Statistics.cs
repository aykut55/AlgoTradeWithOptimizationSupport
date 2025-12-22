using System;
using System.Collections.Generic;
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

        #region System Info

        public int SistemId { get; set; }
        public string SistemName { get; set; }
        public string GrafikSembol { get; set; }
        public string GrafikPeriyot { get; set; }

        #endregion

        #region Execution Info

        public int LastExecutionId { get; set; }
        public string LastExecutionTime { get; set; }
        public string LastExecutionTimeStart { get; set; }
        public string LastExecutionTimeStop { get; set; }
        public int ExecutionTimeInMSec { get; set; }
        public string LastResetTime { get; set; }
        public string LastStatisticsCalculationTime { get; set; }

        #endregion

        #region Bar Info

        public int ToplamBarSayisi { get; set; }
        public int IlkBarIndex { get; set; }
        public int SonBarIndex { get; set; }
        public int SecilenBarNumarasi { get; set; }
        public string IlkBarTarihi { get; set; }
        public string SonBarTarihi { get; set; }
        public string SecilenBarTarihi { get; set; }
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

        public int IslemSayisi { get; set; }
        public int AlisSayisi { get; set; }
        public int SatisSayisi { get; set; }
        public int FlatSayisi { get; set; }
        public int PassSayisi { get; set; }
        public int KarAlSayisi { get; set; }
        public int ZararKesSayisi { get; set; }
        public int KazandiranIslemSayisi { get; set; }
        public int KaybettirenIslemSayisi { get; set; }
        public int NotrIslemSayisi { get; set; }
        public int KazandiranAlisSayisi { get; set; }
        public int KaybettirenAlisSayisi { get; set; }
        public int NotrAlisSayisi { get; set; }
        public int KazandiranSatisSayisi { get; set; }
        public int KaybettirenSatisSayisi { get; set; }
        public int NotrSatisSayisi { get; set; }

        #endregion

        #region Command Counts

        public int AlKomutSayisi { get; set; }
        public int SatKomutSayisi { get; set; }
        public int PasGecKomutSayisi { get; set; }
        public int KarAlKomutSayisi { get; set; }
        public int ZararKesKomutSayisi { get; set; }
        public int FlatOlKomutSayisi { get; set; }

        #endregion

        #region Bar Status

        public int KardaBarSayisi { get; set; }
        public int ZarardaBarSayisi { get; set; }

        #endregion

        #region PnL

        public double KarZararFiyat { get; set; }
        public double KarZararPuan { get; set; }
        public double KarZararFiyatYuzde { get; set; }
        public double ToplamKarFiyat { get; set; }
        public double ToplamZararFiyat { get; set; }
        public double NetKarFiyat { get; set; }
        public double ToplamKarPuan { get; set; }
        public double ToplamZararPuan { get; set; }
        public double NetKarPuan { get; set; }
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

        public int KomisyonIslemSayisi { get; set; }
        public double KomisyonVarlikAdedSayisi { get; set; }
        public double KomisyonCarpan { get; set; }
        public double KomisyonFiyat { get; set; }
        public double KomisyonFiyatYuzde { get; set; }
        public bool KomisyonuDahilEt { get; set; }

        #endregion

        #region Balance

        public double IlkBakiyeFiyat { get; set; }
        public double IlkBakiyePuan { get; set; }
        public double BakiyeFiyat { get; set; }
        public double BakiyePuan { get; set; }
        public double GetiriFiyat { get; set; }
        public double GetiriPuan { get; set; }
        public double GetiriFiyatYuzde { get; set; }
        public double GetiriPuanYuzde { get; set; }
        public double BakiyeFiyatNet { get; set; }
        public double BakiyePuanNet { get; set; }
        public double GetiriFiyatNet { get; set; }
        public double GetiriPuanNet { get; set; }
        public double GetiriFiyatYuzdeNet { get; set; }
        public double GetiriPuanYuzdeNet { get; set; }
        public double GetiriKz { get; set; }
        public double GetiriKzNet { get; set; }
        public double GetiriKzSistem { get; set; }
        public double GetiriKzNetSistem { get; set; }
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

        public double HisseSayisi { get; set; }
        public double KontratSayisi { get; set; }
        public double VarlikAdedCarpani { get; set; }
        public double VarlikAdedSayisi { get; set; }
        public double KaymaMiktari { get; set; }
        public bool KaymayiDahilEt { get; set; }

        #endregion

        #region Signals

        public string Sinyal { get; set; }
        public string SonYon { get; set; }
        public string PrevYon { get; set; }
        public double SonFiyat { get; set; }
        public double SonAFiyat { get; set; }
        public double SonSFiyat { get; set; }
        public double SonFFiyat { get; set; }
        public double SonPFiyat { get; set; }
        public double PrevFiyat { get; set; }
        public double PrevAFiyat { get; set; }
        public double PrevSFiyat { get; set; }
        public double PrevFFiyat { get; set; }
        public double PrevPFiyat { get; set; }
        public int SonBarNo { get; set; }
        public int SonABarNo { get; set; }
        public int SonSBarNo { get; set; }
        public int SonFBarNo { get; set; }
        public int SonPBarNo { get; set; }
        public int PrevBarNo { get; set; }
        public int PrevABarNo { get; set; }
        public int PrevSBarNo { get; set; }
        public int PrevFBarNo { get; set; }
        public int PrevPBarNo { get; set; }
        public int EmirKomut { get; set; }
        public int EmirStatus { get; set; }

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

        public Dictionary<string, string> IstatistiklerNew { get; set; }

        #endregion

        #region Constructor

        public Statistics()
        {
            IstatistiklerNew = new Dictionary<string, string>();
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
            IstatistiklerNew.Clear();
            return this;
        }

        public int Hesapla(int SecilenBarNumarasi)
        {
            int result = 0;

            if (Trader == null)
                return result;

            Trader.LastStatisticsCalculationTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

            ReadValues();

            ToplamBarSayisi = Trader.Data.Count;
            IlkBarTarihi = Trader.Data[0].Date.ToString("yyyy.MM.dd");
            SonBarTarihi = Trader.Data[Trader.Data.Count - 1].Date.ToString("yyyy.MM.dd");

            int lastBar = Trader.Data.Count - 1;

            this.SecilenBarNumarasi = SecilenBarNumarasi;
            if (this.SecilenBarNumarasi > lastBar)
            {
                this.SecilenBarNumarasi = lastBar;
            }

            SecilenBarTarihi = Trader.Data[this.SecilenBarNumarasi].Date.ToString("yyyy.MM.dd");

            if (this.SecilenBarNumarasi <= lastBar)
            {
                SecilenBarAcilisFiyati = Trader.Data[this.SecilenBarNumarasi].Open;
                SecilenBarYuksekFiyati = Trader.Data[this.SecilenBarNumarasi].High;
                SecilenBarDusukFiyati = Trader.Data[this.SecilenBarNumarasi].Low;
                SecilenBarKapanisFiyati = Trader.Data[this.SecilenBarNumarasi].Close;
            }

            SonBarTarihi = Trader.Data[lastBar].Date.ToString("yyyy.MM.dd");
            SonBarAcilisFiyati = Trader.Data[lastBar].Open;
            SonBarYuksekFiyati = Trader.Data[lastBar].High;
            SonBarDusukFiyati = Trader.Data[lastBar].Low;
            SonBarKapanisFiyati = Trader.Data[lastBar].Close;
            SonBarIndex = lastBar;

            // Calculate time elapsed
            DateTime firstDate = Trader.Data[0].Date;
            TimeSpan elapsed = DateTime.Now - firstDate;

            double sureDakika = elapsed.TotalMinutes;
            double sureSaat = elapsed.TotalHours;
            int sureGun = elapsed.Days;
            double sureAy = sureGun / 30.4;

            ToplamGecenSureAy = sureAy;
            ToplamGecenSureGun = sureGun;
            ToplamGecenSureSaat = (int)sureSaat;
            ToplamGecenSureDakika = (int)sureDakika;

            OrtAylikIslemSayisi = ToplamGecenSureAy > 0 ? 1.0 * IslemSayisi / ToplamGecenSureAy : 0;
            OrtHaftalikIslemSayisi = 0.0;
            OrtGunlukIslemSayisi = ToplamGecenSureGun > 0 ? 1.0 * IslemSayisi / ToplamGecenSureGun : 0;
            OrtSaatlikIslemSayisi = ToplamGecenSureSaat > 0 ? 1.0 * IslemSayisi / ToplamGecenSureSaat : 0;

            GetiriMaxDD = 0.0;
            GetiriMaxDDTarih = "";
            GetiriMaxKayip = VarlikAdedSayisi * -1 * GetiriMaxDD;

            MaxKarFiyat = 0.0;
            MaxZararFiyat = 0.0;
            MaxKarPuan = 0.0;
            MaxZararPuan = 0.0;
            MinBakiyeFiyat = IlkBakiyeFiyat;
            MaxBakiyeFiyat = IlkBakiyeFiyat;
            MinBakiyeFiyatNet = IlkBakiyeFiyat;
            MaxBakiyeFiyatNet = IlkBakiyeFiyat;
            MinBakiyePuan = IlkBakiyePuan;
            MaxBakiyePuan = IlkBakiyePuan;

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
            ProfitFactor = Math.Abs(ToplamZararPuan) > 0 ? ToplamKarPuan / Math.Abs(ToplamZararPuan) : 0;
            ProfitFactorSistem = 0.0;
            KarliIslemOrani = IslemSayisi > 0 ? (1.0 * KazandiranIslemSayisi) / (1.0 * IslemSayisi) * 100.0 : 0;

            MinBakiyeFiyatYuzde = IlkBakiyeFiyat != 0 ? (MinBakiyeFiyat - IlkBakiyeFiyat) * 100.0 / IlkBakiyeFiyat : 0;
            MaxBakiyeFiyatYuzde = IlkBakiyeFiyat != 0 ? (MaxBakiyeFiyat - IlkBakiyeFiyat) * 100.0 / IlkBakiyeFiyat : 0;
            MinBakiyeFiyatNetYuzde = IlkBakiyeFiyat != 0 ? (MinBakiyeFiyatNet - IlkBakiyeFiyat) * 100.0 / IlkBakiyeFiyat : 0;
            MaxBakiyeFiyatNetYuzde = IlkBakiyeFiyat != 0 ? (MaxBakiyeFiyatNet - IlkBakiyeFiyat) * 100.0 / IlkBakiyeFiyat : 0;
            KomisyonFiyatYuzde = GetiriFiyatYuzde - GetiriFiyatYuzdeNet;

            GetiriKzSistemYuzde = 0.0;
            GetiriKzNetSistemYuzde = 0.0;

            GetiriIstatistikleriHesapla();
            AssignToMap();

            return result;
        }

        private void ReadValues()
        {
            SistemId = 0;
            SistemName = "";
            GrafikSembol = "";
            GrafikPeriyot = "";

            LastExecutionId = 0;
            LastExecutionTime = Trader.LastExecutionTime;
            LastExecutionTimeStart = Trader.LastExecutionTimeStart;
            LastExecutionTimeStop = Trader.LastExecutionTimeStop;
            ExecutionTimeInMSec = Trader.ExecutionTimeInMSec;
            LastResetTime = Trader.LastResetTime;
            LastStatisticsCalculationTime = Trader.LastStatisticsCalculationTime;

            IslemSayisi = Trader.status.IslemSayisi;
            KazandiranIslemSayisi = Trader.status.KazandiranIslemSayisi;
            KaybettirenIslemSayisi = Trader.status.KaybettirenIslemSayisi;
            NotrIslemSayisi = Trader.status.NotrIslemSayisi;
            KazandiranAlisSayisi = Trader.status.KazandiranAlisSayisi;
            KaybettirenAlisSayisi = Trader.status.KaybettirenAlisSayisi;
            NotrAlisSayisi = Trader.status.NotrAlisSayisi;
            KazandiranSatisSayisi = Trader.status.KazandiranSatisSayisi;
            KaybettirenSatisSayisi = Trader.status.KaybettirenSatisSayisi;
            NotrSatisSayisi = Trader.status.NotrSatisSayisi;
            AlisSayisi = Trader.status.AlisSayisi;
            SatisSayisi = Trader.status.SatisSayisi;
            FlatSayisi = Trader.status.FlatSayisi;
            PassSayisi = Trader.status.PassSayisi;
            KarAlSayisi = Trader.status.KarAlSayisi;
            ZararKesSayisi = Trader.status.ZararKesSayisi;
            AlKomutSayisi = Trader.status.AlKomutSayisi;
            SatKomutSayisi = Trader.status.SatKomutSayisi;
            PasGecKomutSayisi = Trader.status.PasGecKomutSayisi;
            KarAlKomutSayisi = Trader.status.KarAlKomutSayisi;
            ZararKesKomutSayisi = Trader.status.ZararKesKomutSayisi;
            FlatOlKomutSayisi = Trader.status.FlatOlKomutSayisi;
            KardaBarSayisi = Trader.status.KardaBarSayisi;
            ZarardaBarSayisi = Trader.status.ZarardaBarSayisi;
            KarZararFiyat = Trader.status.KarZararFiyat;
            KarZararPuan = Trader.status.KarZararPuan;
            KarZararFiyatYuzde = Trader.status.KarZararFiyatYuzde;
            KomisyonIslemSayisi = Trader.status.KomisyonIslemSayisi;
            KomisyonVarlikAdedSayisi = Trader.status.KomisyonVarlikAdedSayisi;
            KomisyonCarpan = Trader.status.KomisyonCarpan;
            KomisyonFiyat = Trader.status.KomisyonFiyat;
            KomisyonuDahilEt = Trader.flags.KomisyonuDahilEt;
            HisseSayisi = Trader.status.HisseSayisi;
            KontratSayisi = Trader.status.KontratSayisi;
            VarlikAdedCarpani = Trader.status.VarlikAdedCarpani;
            VarlikAdedSayisi = Trader.status.VarlikAdedSayisi;
            KaymaMiktari = Trader.status.KaymaMiktari;
            KaymayiDahilEt = Trader.flags.KaymayiDahilEt;
            IlkBakiyeFiyat = Trader.status.IlkBakiyeFiyat;
            IlkBakiyePuan = Trader.status.IlkBakiyePuan;
            BakiyeFiyat = Trader.status.BakiyeFiyat;
            BakiyePuan = Trader.status.BakiyePuan;
            GetiriFiyat = Trader.status.GetiriFiyat;
            GetiriPuan = Trader.status.GetiriPuan;
            GetiriFiyatYuzde = Trader.status.GetiriFiyatYuzde;
            GetiriPuanYuzde = Trader.status.GetiriPuanYuzde;
            BakiyeFiyatNet = Trader.status.BakiyeFiyatNet;
            BakiyePuanNet = Trader.status.BakiyePuanNet;
            GetiriFiyatNet = Trader.status.GetiriFiyatNet;
            GetiriPuanNet = Trader.status.GetiriPuanNet;
            GetiriFiyatYuzdeNet = Trader.status.GetiriFiyatYuzdeNet;
            GetiriPuanYuzdeNet = Trader.status.GetiriPuanYuzdeNet;
            GetiriKz = Trader.status.GetiriKz;
            GetiriKzNet = Trader.status.GetiriKzNet;
            GetiriKzSistem = Trader.status.GetiriKzSistem;
            GetiriKzNetSistem = Trader.status.GetiriKzNetSistem;
            //GetiriFiyatTipi = Trader.status.GetiriFiyatTipi;
            NetKarPuan = Trader.status.NetKarPuan;
            ToplamKarPuan = Trader.status.ToplamKarPuan;
            ToplamZararPuan = Trader.status.ToplamZararPuan;
            NetKarFiyat = Trader.status.NetKarFiyat;
            ToplamKarFiyat = Trader.status.ToplamKarFiyat;
            ToplamZararFiyat = Trader.status.ToplamZararFiyat;

            // Prevent division by zero
            if (ToplamZararPuan == 0)
                ToplamZararPuan = 1e-12;

            Sinyal = Trader.signals.Sinyal;
            SonYon = Trader.signals.SonYon;
            PrevYon = Trader.signals.PrevYon;
            SonFiyat = Trader.signals.SonFiyat;
            SonAFiyat = Trader.signals.SonAFiyat;
            SonSFiyat = Trader.signals.SonSFiyat;
            SonFFiyat = Trader.signals.SonFFiyat;
            SonPFiyat = Trader.signals.SonPFiyat;
            PrevFiyat = Trader.signals.PrevFiyat;
            PrevAFiyat = Trader.signals.PrevAFiyat;
            PrevSFiyat = Trader.signals.PrevSFiyat;
            PrevFFiyat = Trader.signals.PrevFFiyat;
            PrevPFiyat = Trader.signals.PrevPFiyat;
            SonBarNo = Trader.signals.SonBarNo;
            SonABarNo = Trader.signals.SonABarNo;
            SonSBarNo = Trader.signals.SonSBarNo;
            SonFBarNo = Trader.signals.SonFBarNo;
            SonPBarNo = Trader.signals.SonPBarNo;
            PrevBarNo = Trader.signals.PrevBarNo;
            PrevABarNo = Trader.signals.PrevABarNo;
            PrevSBarNo = Trader.signals.PrevSBarNo;
            PrevFBarNo = Trader.signals.PrevFBarNo;
            PrevPBarNo = Trader.signals.PrevPBarNo;
            EmirKomut = Trader.signals.EmirKomut;
            EmirStatus = Trader.signals.EmirStatus;
        }

        private void GetiriIstatistikleriHesapla()
        {
            // TODO: Implement periodic return calculations
            // This would calculate monthly, weekly, daily, and hourly returns
        }

        private void AssignToMap()
        {
            IstatistiklerNew.Clear();

            IstatistiklerNew["GrafikSembol"] = GrafikSembol;
            IstatistiklerNew["GrafikPeriyot"] = GrafikPeriyot;
            IstatistiklerNew["SistemId"] = SistemId.ToString();
            IstatistiklerNew["SistemName"] = SistemName;
            IstatistiklerNew["LastExecutionTime"] = LastExecutionTime;
            IstatistiklerNew["LastExecutionTimeStart"] = LastExecutionTimeStart;
            IstatistiklerNew["LastExecutionTimeStop"] = LastExecutionTimeStop;
            IstatistiklerNew["ExecutionTimeInMSec"] = ExecutionTimeInMSec.ToString();
            IstatistiklerNew["LastExecutionId"] = LastExecutionId.ToString();
            IstatistiklerNew["LastResetTime"] = LastResetTime;
            IstatistiklerNew["LastStatisticsCalculationTime"] = LastStatisticsCalculationTime;
            IstatistiklerNew["ToplamGecenSureAy"] = $"{ToplamGecenSureAy:F1}";
            IstatistiklerNew["ToplamGecenSureGun"] = ToplamGecenSureGun.ToString();
            IstatistiklerNew["ToplamGecenSureSaat"] = ToplamGecenSureSaat.ToString();
            IstatistiklerNew["ToplamGecenSureDakika"] = ToplamGecenSureDakika.ToString();
            IstatistiklerNew["ToplamBarSayisi"] = ToplamBarSayisi.ToString();
            IstatistiklerNew["SecilenBarNumarasi"] = SecilenBarNumarasi.ToString();
            IstatistiklerNew["SecilenBarTarihi"] = SecilenBarTarihi;
            IstatistiklerNew["IlkBarTarihi"] = IlkBarTarihi;
            IstatistiklerNew["SonBarTarihi"] = SonBarTarihi;
            IstatistiklerNew["IlkBarIndex"] = IlkBarIndex.ToString();
            IstatistiklerNew["SonBarIndex"] = SonBarIndex.ToString();
            IstatistiklerNew["SonBarAcilisFiyati"] = SonBarAcilisFiyati.ToString();
            IstatistiklerNew["SonBarYuksekFiyati"] = SonBarYuksekFiyati.ToString();
            IstatistiklerNew["SonBarDusukFiyati"] = SonBarDusukFiyati.ToString();
            IstatistiklerNew["SonBarKapanisFiyati"] = SonBarKapanisFiyati.ToString();
            IstatistiklerNew["IlkBakiyeFiyat"] = IlkBakiyeFiyat.ToString();
            IstatistiklerNew["IlkBakiyePuan"] = IlkBakiyePuan.ToString();
            IstatistiklerNew["BakiyeFiyat"] = BakiyeFiyat.ToString();
            IstatistiklerNew["BakiyePuan"] = BakiyePuan.ToString();
            IstatistiklerNew["GetiriFiyat"] = GetiriFiyat.ToString();
            IstatistiklerNew["GetiriPuan"] = GetiriPuan.ToString();
            IstatistiklerNew["GetiriFiyatYuzde"] = GetiriFiyatYuzde.ToString();
            IstatistiklerNew["GetiriPuanYuzde"] = GetiriPuanYuzde.ToString();
            IstatistiklerNew["BakiyeFiyatNet"] = BakiyeFiyatNet.ToString();
            IstatistiklerNew["BakiyePuanNet"] = BakiyePuanNet.ToString();
            IstatistiklerNew["GetiriFiyatNet"] = GetiriFiyatNet.ToString();
            IstatistiklerNew["GetiriPuanNet"] = GetiriPuanNet.ToString();
            IstatistiklerNew["GetiriFiyatYuzdeNet"] = GetiriFiyatYuzdeNet.ToString();
            IstatistiklerNew["GetiriPuanYuzdeNet"] = GetiriPuanYuzdeNet.ToString();
            IstatistiklerNew["GetiriKz"] = GetiriKz.ToString();
            IstatistiklerNew["GetiriKzNet"] = GetiriKzNet.ToString();
            IstatistiklerNew["MinBakiyeFiyat"] = MinBakiyeFiyat.ToString();
            IstatistiklerNew["MaxBakiyeFiyat"] = MaxBakiyeFiyat.ToString();
            IstatistiklerNew["MinBakiyePuan"] = MinBakiyePuan.ToString();
            IstatistiklerNew["MaxBakiyePuan"] = MaxBakiyePuan.ToString();
            IstatistiklerNew["MinBakiyeFiyatYuzde"] = MinBakiyeFiyatYuzde.ToString();
            IstatistiklerNew["MaxBakiyeFiyatYuzde"] = MaxBakiyeFiyatYuzde.ToString();
            IstatistiklerNew["MinBakiyeFiyatIndex"] = MinBakiyeFiyatIndex.ToString();
            IstatistiklerNew["MaxBakiyeFiyatIndex"] = MaxBakiyeFiyatIndex.ToString();
            IstatistiklerNew["MinBakiyePuanIndex"] = MinBakiyePuanIndex.ToString();
            IstatistiklerNew["MaxBakiyePuanIndex"] = MaxBakiyePuanIndex.ToString();
            IstatistiklerNew["MinBakiyeFiyatNet"] = MinBakiyeFiyatNet.ToString();
            IstatistiklerNew["MaxBakiyeFiyatNet"] = MaxBakiyeFiyatNet.ToString();
            IstatistiklerNew["MinBakiyeFiyatNetIndex"] = MinBakiyeFiyatNetIndex.ToString();
            IstatistiklerNew["MaxBakiyeFiyatNetIndex"] = MaxBakiyeFiyatNetIndex.ToString();
            IstatistiklerNew["MinBakiyeFiyatNetYuzde"] = MinBakiyeFiyatNetYuzde.ToString();
            IstatistiklerNew["MaxBakiyeFiyatNetYuzde"] = MaxBakiyeFiyatNetYuzde.ToString();
            IstatistiklerNew["GetiriKzSistem"] = $"{GetiriKzSistem:F2}";
            IstatistiklerNew["GetiriKzSistemYuzde"] = $"{GetiriKzSistemYuzde:F2}";
            IstatistiklerNew["GetiriKzNetSistem"] = $"{GetiriKzNetSistem:F2}";
            IstatistiklerNew["GetiriKzNetSistemYuzde"] = $"{GetiriKzNetSistemYuzde:F2}";
            IstatistiklerNew["IslemSayisi"] = IslemSayisi.ToString();
            IstatistiklerNew["AlisSayisi"] = AlisSayisi.ToString();
            IstatistiklerNew["SatisSayisi"] = SatisSayisi.ToString();
            IstatistiklerNew["FlatSayisi"] = FlatSayisi.ToString();
            IstatistiklerNew["PassSayisi"] = PassSayisi.ToString();
            IstatistiklerNew["KarAlSayisi"] = KarAlSayisi.ToString();
            IstatistiklerNew["ZararKesSayisi"] = ZararKesSayisi.ToString();
            IstatistiklerNew["KazandiranIslemSayisi"] = KazandiranIslemSayisi.ToString();
            IstatistiklerNew["KaybettirenIslemSayisi"] = KaybettirenIslemSayisi.ToString();
            IstatistiklerNew["NotrIslemSayisi"] = NotrIslemSayisi.ToString();
            IstatistiklerNew["KazandiranAlisSayisi"] = KazandiranAlisSayisi.ToString();
            IstatistiklerNew["KaybettirenAlisSayisi"] = KaybettirenAlisSayisi.ToString();
            IstatistiklerNew["NotrAlisSayisi"] = NotrAlisSayisi.ToString();
            IstatistiklerNew["KazandiranSatisSayisi"] = KazandiranSatisSayisi.ToString();
            IstatistiklerNew["KaybettirenSatisSayisi"] = KaybettirenSatisSayisi.ToString();
            IstatistiklerNew["NotrSatisSayisi"] = NotrSatisSayisi.ToString();
            IstatistiklerNew["AlKomutSayisi"] = AlKomutSayisi.ToString();
            IstatistiklerNew["SatKomutSayisi"] = SatKomutSayisi.ToString();
            IstatistiklerNew["PasGecKomutSayisi"] = PasGecKomutSayisi.ToString();
            IstatistiklerNew["KarAlKomutSayisi"] = KarAlKomutSayisi.ToString();
            IstatistiklerNew["ZararKesKomutSayisi"] = ZararKesKomutSayisi.ToString();
            IstatistiklerNew["FlatOlKomutSayisi"] = FlatOlKomutSayisi.ToString();
            IstatistiklerNew["KomisyonIslemSayisi"] = KomisyonIslemSayisi.ToString();
            IstatistiklerNew["KomisyonVarlikAdedSayisi"] = KomisyonVarlikAdedSayisi.ToString();
            IstatistiklerNew["KomisyonCarpan"] = KomisyonCarpan.ToString();
            IstatistiklerNew["KomisyonFiyat"] = KomisyonFiyat.ToString();
            IstatistiklerNew["KomisyonFiyatYuzde"] = KomisyonFiyatYuzde.ToString();
            IstatistiklerNew["KomisyonuDahilEt"] = KomisyonuDahilEt.ToString();
            IstatistiklerNew["KarZararFiyat"] = KarZararFiyat.ToString();
            IstatistiklerNew["KarZararFiyatYuzde"] = KarZararFiyatYuzde.ToString();
            IstatistiklerNew["KarZararPuan"] = KarZararPuan.ToString();
            IstatistiklerNew["ToplamKarFiyat"] = ToplamKarFiyat.ToString();
            IstatistiklerNew["ToplamZararFiyat"] = ToplamZararFiyat.ToString();
            IstatistiklerNew["NetKarFiyat"] = NetKarFiyat.ToString();
            IstatistiklerNew["ToplamKarPuan"] = ToplamKarPuan.ToString();
            IstatistiklerNew["ToplamZararPuan"] = ToplamZararPuan.ToString();
            IstatistiklerNew["NetKarPuan"] = NetKarPuan.ToString();
            IstatistiklerNew["MaxKarFiyat"] = MaxKarFiyat.ToString();
            IstatistiklerNew["MaxZararFiyat"] = MaxZararFiyat.ToString();
            IstatistiklerNew["MaxKarPuan"] = MaxKarPuan.ToString();
            IstatistiklerNew["MaxZararPuan"] = MaxZararPuan.ToString();
            IstatistiklerNew["MaxZararFiyatIndex"] = MaxZararFiyatIndex.ToString();
            IstatistiklerNew["MaxKarFiyatIndex"] = MaxKarFiyatIndex.ToString();
            IstatistiklerNew["MaxZararPuanIndex"] = MaxZararPuanIndex.ToString();
            IstatistiklerNew["MaxKarPuanIndex"] = MaxKarPuanIndex.ToString();
            IstatistiklerNew["KardaBarSayisi"] = KardaBarSayisi.ToString();
            IstatistiklerNew["ZarardaBarSayisi"] = ZarardaBarSayisi.ToString();
            IstatistiklerNew["KarliIslemOrani"] = $"{KarliIslemOrani:F2}";
            IstatistiklerNew["GetiriMaxDD"] = GetiriMaxDD.ToString();
            IstatistiklerNew["GetiriMaxDDTarih"] = GetiriMaxDDTarih;
            IstatistiklerNew["GetiriMaxKayip"] = GetiriMaxKayip.ToString();
            IstatistiklerNew["ProfitFactor"] = $"{ProfitFactor:F2}";
            IstatistiklerNew["ProfitFactorSistem"] = $"{ProfitFactorSistem:F2}";
            IstatistiklerNew["OrtAylikIslemSayisi"] = $"{OrtAylikIslemSayisi:F2}";
            IstatistiklerNew["OrtHaftalikIslemSayisi"] = $"{OrtHaftalikIslemSayisi:F2}";
            IstatistiklerNew["OrtGunlukIslemSayisi"] = $"{OrtGunlukIslemSayisi:F2}";
            IstatistiklerNew["OrtSaatlikIslemSayisi"] = $"{OrtSaatlikIslemSayisi:F2}";
            IstatistiklerNew["Sinyal"] = Sinyal;
            IstatistiklerNew["SonYon"] = SonYon;
            IstatistiklerNew["PrevYon"] = PrevYon;
            IstatistiklerNew["SonFiyat"] = SonFiyat.ToString();
            IstatistiklerNew["SonAFiyat"] = SonAFiyat.ToString();
            IstatistiklerNew["SonSFiyat"] = SonSFiyat.ToString();
            IstatistiklerNew["SonFFiyat"] = SonFFiyat.ToString();
            IstatistiklerNew["SonPFiyat"] = SonPFiyat.ToString();
            IstatistiklerNew["PrevFiyat"] = PrevFiyat.ToString();
            IstatistiklerNew["PrevAFiyat"] = PrevAFiyat.ToString();
            IstatistiklerNew["PrevSFiyat"] = PrevSFiyat.ToString();
            IstatistiklerNew["PrevFFiyat"] = PrevFFiyat.ToString();
            IstatistiklerNew["PrevPFiyat"] = PrevPFiyat.ToString();
            IstatistiklerNew["SonBarNo"] = SonBarNo.ToString();
            IstatistiklerNew["SonABarNo"] = SonABarNo.ToString();
            IstatistiklerNew["SonSBarNo"] = SonSBarNo.ToString();
            IstatistiklerNew["SonFBarNo"] = SonFBarNo.ToString();
            IstatistiklerNew["SonPBarNo"] = SonPBarNo.ToString();
            IstatistiklerNew["PrevBarNo"] = PrevBarNo.ToString();
            IstatistiklerNew["PrevABarNo"] = PrevABarNo.ToString();
            IstatistiklerNew["PrevSBarNo"] = PrevSBarNo.ToString();
            IstatistiklerNew["PrevFBarNo"] = PrevFBarNo.ToString();
            IstatistiklerNew["PrevPBarNo"] = PrevPBarNo.ToString();
            IstatistiklerNew["EmirKomut"] = EmirKomut.ToString();
            IstatistiklerNew["EmirStatus"] = EmirStatus.ToString();
            IstatistiklerNew["HisseSayisi"] = HisseSayisi.ToString();
            IstatistiklerNew["KontratSayisi"] = KontratSayisi.ToString();
            IstatistiklerNew["VarlikAdedCarpani"] = VarlikAdedCarpani.ToString();
            IstatistiklerNew["VarlikAdedSayisi"] = VarlikAdedSayisi.ToString();
            IstatistiklerNew["KaymaMiktari"] = KaymaMiktari.ToString();
            IstatistiklerNew["KaymayiDahilEt"] = KaymayiDahilEt.ToString();

            // Periodic returns
            IstatistiklerNew["GetiriFiyatBuAy"] = $"{GetiriFiyatBuAy:F2}";
            IstatistiklerNew["GetiriFiyatAy1"] = $"{GetiriFiyatAy1:F2}";
            IstatistiklerNew["GetiriFiyatAy2"] = $"{GetiriFiyatAy2:F2}";
            IstatistiklerNew["GetiriFiyatAy3"] = $"{GetiriFiyatAy3:F2}";
            IstatistiklerNew["GetiriFiyatAy4"] = $"{GetiriFiyatAy4:F2}";
            IstatistiklerNew["GetiriFiyatAy5"] = $"{GetiriFiyatAy5:F2}";
            IstatistiklerNew["GetiriFiyatBuHafta"] = $"{GetiriFiyatBuHafta:F2}";
            IstatistiklerNew["GetiriFiyatHafta1"] = $"{GetiriFiyatHafta1:F2}";
            IstatistiklerNew["GetiriFiyatHafta2"] = $"{GetiriFiyatHafta2:F2}";
            IstatistiklerNew["GetiriFiyatHafta3"] = $"{GetiriFiyatHafta3:F2}";
            IstatistiklerNew["GetiriFiyatHafta4"] = $"{GetiriFiyatHafta4:F2}";
            IstatistiklerNew["GetiriFiyatHafta5"] = $"{GetiriFiyatHafta5:F2}";
            IstatistiklerNew["GetiriFiyatBuGun"] = $"{GetiriFiyatBuGun:F2}";
            IstatistiklerNew["GetiriFiyatGun1"] = $"{GetiriFiyatGun1:F2}";
            IstatistiklerNew["GetiriFiyatGun2"] = $"{GetiriFiyatGun2:F2}";
            IstatistiklerNew["GetiriFiyatGun3"] = $"{GetiriFiyatGun3:F2}";
            IstatistiklerNew["GetiriFiyatGun4"] = $"{GetiriFiyatGun4:F2}";
            IstatistiklerNew["GetiriFiyatGun5"] = $"{GetiriFiyatGun5:F2}";
            IstatistiklerNew["GetiriFiyatBuSaat"] = $"{GetiriFiyatBuSaat:F2}";
            IstatistiklerNew["GetiriFiyatSaat1"] = $"{GetiriFiyatSaat1:F2}";
            IstatistiklerNew["GetiriFiyatSaat2"] = $"{GetiriFiyatSaat2:F2}";
            IstatistiklerNew["GetiriFiyatSaat3"] = $"{GetiriFiyatSaat3:F2}";
            IstatistiklerNew["GetiriFiyatSaat4"] = $"{GetiriFiyatSaat4:F2}";
            IstatistiklerNew["GetiriFiyatSaat5"] = $"{GetiriFiyatSaat5:F2}";
            IstatistiklerNew["GetiriPuanBuAy"] = $"{GetiriPuanBuAy:F2}";
            IstatistiklerNew["GetiriPuanAy1"] = $"{GetiriPuanAy1:F2}";
            IstatistiklerNew["GetiriPuanAy2"] = $"{GetiriPuanAy2:F2}";
            IstatistiklerNew["GetiriPuanAy3"] = $"{GetiriPuanAy3:F2}";
            IstatistiklerNew["GetiriPuanAy4"] = $"{GetiriPuanAy4:F2}";
            IstatistiklerNew["GetiriPuanAy5"] = $"{GetiriPuanAy5:F2}";
            IstatistiklerNew["GetiriPuanBuHafta"] = $"{GetiriPuanBuHafta:F2}";
            IstatistiklerNew["GetiriPuanHafta1"] = $"{GetiriPuanHafta1:F2}";
            IstatistiklerNew["GetiriPuanHafta2"] = $"{GetiriPuanHafta2:F2}";
            IstatistiklerNew["GetiriPuanHafta3"] = $"{GetiriPuanHafta3:F2}";
            IstatistiklerNew["GetiriPuanHafta4"] = $"{GetiriPuanHafta4:F2}";
            IstatistiklerNew["GetiriPuanHafta5"] = $"{GetiriPuanHafta5:F2}";
            IstatistiklerNew["GetiriPuanBuGun"] = $"{GetiriPuanBuGun:F2}";
            IstatistiklerNew["GetiriPuanGun1"] = $"{GetiriPuanGun1:F2}";
            IstatistiklerNew["GetiriPuanGun2"] = $"{GetiriPuanGun2:F2}";
            IstatistiklerNew["GetiriPuanGun3"] = $"{GetiriPuanGun3:F2}";
            IstatistiklerNew["GetiriPuanGun4"] = $"{GetiriPuanGun4:F2}";
            IstatistiklerNew["GetiriPuanGun5"] = $"{GetiriPuanGun5:F2}";
            IstatistiklerNew["GetiriPuanBuSaat"] = $"{GetiriPuanBuSaat:F2}";
            IstatistiklerNew["GetiriPuanSaat1"] = $"{GetiriPuanSaat1:F2}";
            IstatistiklerNew["GetiriPuanSaat2"] = $"{GetiriPuanSaat2:F2}";
            IstatistiklerNew["GetiriPuanSaat3"] = $"{GetiriPuanSaat3:F2}";
            IstatistiklerNew["GetiriPuanSaat4"] = $"{GetiriPuanSaat4:F2}";
            IstatistiklerNew["GetiriPuanSaat5"] = $"{GetiriPuanSaat5:F2}";
        }

        #endregion
    }
}
