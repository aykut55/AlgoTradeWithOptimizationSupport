using AlgoTradeWithOptimizationSupportWinFormsApp.DataProvider;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
// Import IAlgoTraderLogger from Trading namespace
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using ScottPlot.TickGenerators.Financial;
using ScottPlot.TickGenerators.TimeUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms.VisualStyles;
using static SkiaSharp.HarfBuzz.SKShaper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders
{
    public enum MarketTypes
    {
        BistEndex = 0,
        BistHisse = 1,
        BistParite = 2,
        BistMetal = 3,

        ViopEndex = 4,
        ViopHisse = 5,
        ViopParite = 6,
        ViopMetal = 7,

        FxEndex = 8,
        FxHisse = 9,
        FxParite = 10,
        FxMetal = 11,

        FxCrypto = 12,
        Crypto = 13
    }

    /// <summary>
    /// Single trader - executes one strategy on market data
    /// </summary>
    public class SingleTrader : MarketDataProvider
    {
        #region Properties

        private List<StockData> _data = new List<StockData>();
        public override List<StockData> Data => _data;

        public IndicatorManager Indicators { get; private set; }
        public IStrategy Strategy { get; private set; }
        public Position Position { get; private set; }
        public Bakiye Bakiye { get; private set; }
        public TradeStatistics Statistics { get; private set; }
        public int CurrentIndex { get; private set; }

        private bool _isInitialized = false;
        public override bool IsInitialized => _isInitialized;

        // Logger
        private IAlgoTraderLogger? _logger;

        public TradeSignals StrategySignal { get; private set; }
        //public bool NoneSignal { get; private set; }
        //public bool BuySignal { get; private set; }
        //public bool SellSignal { get; private set; }
        //public bool TakeProfitSignal { get; private set; }
        //public bool StopLossSignal { get; private set; }
        //public bool FlatSignal { get; private set; }
        //public bool SkipSignal { get; private set; }
        //public bool BuySignalEnabled { get; set; }
        //public bool SellSignalEnabled { get; set; }
        //public bool TakeProfitSignalEnabled { get; set; }
        //public bool StopLossSignalEnabled { get; set; }
        //public bool FlatSignalEnabled { get; set; }
        //public bool SkipSignalEnabled { get; set; }

        public int KontratSayisi { get; set; }      // 1 Lot, 1000 Hisse, 10 Kontrat
        public int LotSayisi { get; set; }          // 1 Lot, 1000 Hisse, 10 Kontrat
        public int HisseSayisi { get; set; }        // 1 Lot, 1000 Hisse, 10 Kontra
        public int VarlikAdedCarpani { get; set; }        // 
        public int VarlikAdedSayisi { get; set; }   // 1 Lot, 1000 Hisse, 10 Kontrat
        public int KomisyonVarlikAdedSayisi { get; set; }   // 1 Lot, 1000 Hisse, 10 Kontrat
        public MarketTypes MarketType { get; set; }
        public bool MicroLotSizeEnabled { get; set; }
        public double VarlikAdedSayisiMicro { get; set; }   // 0.01 Lot
        public double KomisyonVarlikAdedSayisiMicro { get; set; }   // 
        public double KomisyonCarpan { get; set; }   // 
        public double KaymaMiktari { get; set; }   //
        public double IlkBakiyeFiyat { get; set; }   //
        public double SonBakiyeFiyat { get; set; }   //
        public double NetBakiyeFiyat { get; set; }   //

        public Signals signals { get; private set; }
        public Status status { get; private set; }
        public Flags flags { get; private set; }
        public Lists lists { get; private set; }
        public TimeUtils timeUtils { get; private set; }
        public TimeFilter timeFilter { get; private set; }
        public KarZarar karZarar { get; private set; }
        public KarAlZararKes karAlZararKes { get; private set; }
        public Komisyon komisyon { get; private set; }
        public Bakiye bakiye { get; private set; }
        public PozisyonBuyuklugu pozisyonBuyuklugu { get; private set; }
        public int ExecutionStepNumber { get; set; }
        public bool BakiyeInitialized { get; set; }

        #endregion

        #region Constructor

        // Parametresiz constructor (eski kullanımlar için)
        public SingleTrader()
        {
            _isInitialized = false;
        }

        // Parametreli constructor (yeni kullanım)
        public SingleTrader(List<StockData> data, IndicatorManager indicators, BaseStrategy strategy)
        {
            // Set data, indicators and strategy
            _data = data;
            Indicators = indicators;
            Strategy = strategy;
            CurrentIndex = 0;

            _isInitialized = true;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize with market data
        /// </summary>
        public void SetData(List<StockData> data)
        {
            if (data == null || data.Count == 0)
                throw new ArgumentException("Data cannot be null or empty");

            _data = data;
            CurrentIndex = 0;

            // Initialize indicators
            Indicators = new IndicatorManager(data);

            _isInitialized = true;
        }

        /// <summary>
        /// Set strategy
        /// </summary>
        public void SetStrategy(IStrategy strategy)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized. Call Initialize() first.");

            Strategy = strategy;

            if (strategy is BaseStrategy baseStrategy)
            {
                baseStrategy.Initialize(Data, Indicators);
            }
            else
            {
                //strategy.OnInit();
            }
        }

        /// <summary>
        /// Set logger for SingleTrader
        /// </summary>
        public void SetLogger(IAlgoTraderLogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Logging Methods

        /// <summary>
        /// Log a message
        /// </summary>
        private void Log(params object[] args)
        {
            _logger?.Log(args);
        }

        /// <summary>
        /// Log a warning
        /// </summary>
        private void LogWarning(params object[] args)
        {
            _logger?.LogWarning(args);
        }

        /// <summary>
        /// Log an error
        /// </summary>
        private void LogError(params object[] args)
        {
            _logger?.LogError(args);
        }

        #endregion

        #region Trading Methods

        /// <summary>
        /// Execute one step (process one bar)
        /// </summary>
        public void Step()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");

            if (Strategy == null)
                throw new InvalidOperationException("Strategy not set");

            if (CurrentIndex >= Data.Count)
                return;

            // TODO: Implement step logic
            // 1. Get signal from strategy
            // 2. Process signal (buy/sell/close)
            // 3. Update position
            // 4. Update balance
            // 5. Check kar/zarar
            // 6. Update statistics

            CurrentIndex++;
        }
        public void CreateModules()
        {
            signals = new Signals();
            status = new Status();
            flags = new Flags();
            lists = new Lists();
            timeUtils = new TimeUtils();
            karZarar = new KarZarar(this);
            karAlZararKes = new KarAlZararKes();
            komisyon = new Komisyon();
            Bakiye = new Bakiye();
            pozisyonBuyuklugu = new PozisyonBuyuklugu();
            Position = new Position();
            Statistics = new TradeStatistics();
        }
        public void InitModules()
        {

        }
        public void DeleteModules()
        {

        }

        public void Reset()
        {
            if (_data == null || _data.Count == 0)
                throw new ArgumentException("Data cannot be null or empty");

            signals.Reset();
            status.Reset();

            flags.Reset();
            lists.Reset();

            timeUtils.Reset();
            karZarar.Reset();

            karAlZararKes.Reset();
            komisyon.Reset();

            Bakiye.Reset();
            pozisyonBuyuklugu.Reset();

            Position.Close();
            Statistics.Reset();

            /*this.BuySignalEnabled = false;
            this.SellSignalEnabled = false;
            this.TakeProfitSignalEnabled = false;
            this.StopLossSignalEnabled = false;
            this.FlatSignalEnabled = false;
            this.SkipSignalEnabled = false;*/

            this.KontratSayisi = 1;
            this.LotSayisi = 1;
            this.HisseSayisi = 1000;
            this.VarlikAdedCarpani = 10;
            this.VarlikAdedSayisi = 1;
            this.KomisyonVarlikAdedSayisi = 1;
            this.MarketType = MarketTypes.ViopEndex;
            this.MicroLotSizeEnabled = false;
            this.VarlikAdedSayisiMicro = 0.0;
            this.KomisyonVarlikAdedSayisiMicro = 0.0;
            this.KomisyonCarpan = 0.0;
            this.KaymaMiktari = 0.0;
            this.IlkBakiyeFiyat = 100000.0;

            CurrentIndex = 0;

            //Position.Close();
            //Statistics.Reset();
            //Strategy?.Reset();
        }
        public void Init()
        {
            if (_data == null || _data.Count == 0)
                throw new ArgumentException("Data cannot be null or empty");

            signals.Init();
            status.Init();

            flags.Init();
            lists.Init(_data.Count);

            timeUtils.Init();
            //karZarar.Init();

            karAlZararKes.Init();
            komisyon.Init();

            //Bakiye.Init();
            pozisyonBuyuklugu.Init();

            //Position.Init();
            //Statistics.Init();
        }

        public void Initialize(int i)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");

            // Busekilde setlenmesi/resetlenmesi gerekiyor
            this.signals.AlEnabled = true;
            this.signals.SatEnabled = true;
            this.signals.FlatOlEnabled = true;
            this.signals.PasGecEnabled = true;
            this.signals.KarAlEnabled = true;
            this.signals.ZararKesEnabled = true;
            this.signals.GunSonuPozKapatEnabled = true;
            this.signals.TimeFilteringEnabled = true;
        }

        public void emirleri_resetle(int i)
        {
            //this.NoneSignal = this.BuySignal = this.SellSignal = this.TakeProfitSignal = this.StopLossSignal = this.FlatSignal = this.SkipSignal = false;
            this.signals.None = this.signals.Al = this.signals.Sat = this.signals.FlatOl = this.signals.PasGec = this.signals.KarAl = this.signals.ZararKes = false;
        }
        public void dongu_basi_degiskenleri_resetle(int i)
        {
            this.lists.BarIndexList[i] = i;
            this.lists.YonList[i] = "";
            this.lists.SeviyeList[i] = 0.0;
            this.lists.SinyalList[i] = 0.0;
            this.lists.KarZararPuanList[i] = 0.0;
            this.lists.KarZararFiyatList[i] = 0.0;
            this.lists.KarZararPuanYuzdeList[i] = 0.0;
            this.lists.KarZararFiyatYuzdeList[i] = 0.0;
            this.status.KarZararPuan = 0.0;
            this.status.KarZararFiyat = 0.0;
            this.status.KarZararPuanYuzde = 0.0;
            this.status.KarZararFiyatYuzde = 0.0;
            this.lists.KarAlList[i] = false;
            this.lists.IzleyenStopList[i] = 0.0;
            this.lists.IslemSayisiList[i] = 0;
            this.lists.AlisSayisiList[i] = 0;
            this.lists.SatisSayisiList[i] = 0;
            this.lists.FlatSayisiList[i] = 0;
            this.lists.PassSayisiList[i] = 0;
            this.lists.KontratSayisiList[i] = 0;
            this.lists.VarlikAdedSayisiList[i] = 0;
            this.lists.KomisyonVarlikAdedSayisiList[i] = 0;
            this.lists.KomisyonIslemSayisiList[i] = 0;
            this.lists.KomisyonFiyatList[i] = 0.0;
            this.lists.KardaBarSayisiList[i] = 0;
            this.lists.ZarardaBarSayisiList[i] = 0;
            this.lists.BakiyeFiyatList[i] = this.status.BakiyeFiyat;
            this.lists.GetiriFiyatList[i] = this.lists.BakiyeFiyatList[i] - this.status.BakiyeFiyat;
            this.lists.BakiyePuanList[i] = this.status.BakiyePuan;
            this.lists.GetiriPuanList[i] = this.lists.BakiyePuanList[i] - this.status.BakiyePuan;
            this.lists.EmirKomutList[i] = 0;
            this.lists.EmirStatusList[i] = 0;
            if (this.ExecutionStepNumber == 0)
                ;
            this.ExecutionStepNumber += 1;
        }
        public void dongu_basi_degiskenleri_guncelle(int i)
        {
            /*this.status.KomisyonVarlikAdedSayisi = this.VarlikManager.KomisyonVarlikAdedSayisi;
            this.status.KomisyonCarpan = this.VarlikManager.KomisyonCarpan;
            this.flags.KomisyonuDahilEt = this.VarlikManager.KomisyonuDahilEt;
            this.status.KaymaMiktari = this.VarlikManager.KaymaMiktari;
            this.flags.KaymayiDahilEt = this.VarlikManager.KaymayiDahilEt;
            this.status.VarlikAdedSayisi = this.VarlikManager.VarlikAdedSayisi;
            this.status.VarlikAdedCarpani = this.VarlikManager.VarlikAdedCarpani;
            this.status.KontratSayisi = this.VarlikManager.KontratSayisi;
            this.status.HisseSayisi = this.VarlikManager.HisseSayisi;
            this.status.IlkBakiyeFiyat = this.VarlikManager.IlkBakiyeFiyat;
            this.status.IlkBakiyePuan = sethislf.VarlikManager.IlkBakiyePuan;
            this.status.GetiriFiyatTipi = this.VarlikManager.GetiriFiyatTipi;*/
            if (this.BakiyeInitialized == false)
            {
                this.BakiyeInitialized = true;
                this.status.BakiyeFiyat = this.status.IlkBakiyeFiyat;
                this.status.BakiyePuan = this.status.IlkBakiyePuan;
                this.lists.BakiyeFiyatList[i] = this.status.BakiyeFiyat;
                this.lists.GetiriFiyatList[i] = this.lists.BakiyeFiyatList[i] - this.status.BakiyeFiyat;
                this.lists.BakiyePuanList[i] = this.status.BakiyePuan;
                this.lists.GetiriPuanList[i] = this.lists.BakiyePuanList[i] - this.status.BakiyePuan;
            }
        }
        public void anlik_kar_zarar_hesapla(int i, string Type= "C")
        {
            this.karZarar.AnlikKarZararHesapla(i, Type);
        }

        public void emir_oncesi_dongu_foksiyonlarini_calistir(int i)
        {
            dongu_basi_degiskenleri_resetle(i);

            dongu_basi_degiskenleri_guncelle(i);

            if (i < 1)
                return;

            anlik_kar_zarar_hesapla(i);

            emirleri_resetle(i);

            if (this.signals.GunSonuPozKapatildi)
                this.signals.GunSonuPozKapatildi = false;

            if (this.signals.KarAlindi || this.signals.ZararKesildi || this.signals.FlatOlundu)
            {
                this.signals.KarAlindi = false;
                this.signals.ZararKesildi = false;
                this.signals.FlatOlundu = false;
                this.signals.PozAcilabilir = false;
            }

            if (this.signals.PozAcilabilir == false)
            {

                this.signals.PozAcilabilir = true;
                this.signals.PozAcildi = false;
            }
        }
        public void emirleri_setle(int i, TradeSignals signal)
        {

            if (signal == TradeSignals.None)
            {
                this.signals.None = true;
            }

            if (signal == TradeSignals.Buy)
            {
                if (this.signals.AlEnabled)
                    this.signals.Al = true;
            }

            if (signal == TradeSignals.Sell)
            {
                if (this.signals.SatEnabled)
                    this.signals.Sat = true;
            }

            if (signal == TradeSignals.TakeProfit)
            {
                if (this.signals.KarAlEnabled)
                    this.signals.KarAl = true;
            }

            if (signal == TradeSignals.StopLoss)
            {
                if (this.signals.ZararKesEnabled)
                    this.signals.ZararKes = true;
            }

            if (signal == TradeSignals.Flat)
            {
                if (this.signals.FlatOlEnabled)
                    this.signals.FlatOl = true;
            }

            if (signal == TradeSignals.Skip)
            {
                if (this.signals.PasGecEnabled)
                    this.signals.PasGec = true;
            }
        }

        public void islem_zaman_filtresi_uygula(int i)
        {

        }
        public void sistem_yon_listesini_guncelle(int i)
        {
            //# BURASI YAPILACAK
            //# Sistem.Yon[i] = self.Lists.YonList[i]
        }
        public void sistem_seviye_listesini_guncelle(int i)
        {
            //# BURASI YAPILACAK
            //# Sistem.Seviye[i] = self.Lists.SeviyeList[i]
        }
        public void sinyal_listesini_guncelle(int i)
        {
            if (this.signals.SonYon == "A")
            {
                this.lists.SinyalList[i] = 1.0;
            }
            else if (this.signals.SonYon == "S")
            {
                this.lists.SinyalList[i] = -1.0;
            }
            else if (this.signals.SonYon == "F")
            {
                this.lists.SinyalList[i] = 0.0;
            }
        }
        public void islem_listesini_guncelle(int i)
        {
            this.lists.IslemSayisiList[i] = this.status.IslemSayisi;
            this.lists.AlisSayisiList[i] = this.status.AlisSayisi;
            this.lists.SatisSayisiList[i] = this.status.SatisSayisi;
            this.lists.FlatSayisiList[i] = this.status.FlatSayisi;
            this.lists.PassSayisiList[i] = this.status.PassSayisi;
            this.lists.VarlikAdedSayisiList[i] = this.status.VarlikAdedSayisi;
            this.lists.KontratSayisiList[i] = this.status.KontratSayisi;
            this.lists.KomisyonVarlikAdedSayisiList[i] = this.status.KomisyonVarlikAdedSayisi;
            this.lists.KomisyonIslemSayisiList[i] = this.status.KomisyonIslemSayisi;
            this.lists.KomisyonFiyatList[i] = this.status.KomisyonFiyat;
            this.lists.KardaBarSayisiList[i] = this.status.KardaBarSayisi;
            this.lists.ZarardaBarSayisiList[i] = this.status.ZarardaBarSayisi;
        }
        public void komisyon_listesini_guncelle(int i)
        {
            this.komisyon.Hesapla(i);
            if (this.flags.KomisyonGuncelle)
                this.flags.KomisyonGuncelle = false;

        }
        public void bakiye_listesini_guncelle(int i)
        {
            //this.Bakiye.Hesapla(i);
            if (this.flags.BakiyeGuncelle)
                this.flags.BakiyeGuncelle = false;
        }
        public void dongu_sonu_degiskenleri_setle(int i)
        {

        }
        public bool gun_sonu_poz_kapat(int i, bool gunSonuPozKapatEnabled = true)
        {
            bool gunSonuPozKapatildi = false;
            /*
            if (gunSonuPozKapatEnabled)
            {
                if (i < this.BarCount - 1 && this.Date[i] != this.Date[i + 1])
                {
                    this.signals.FlatOl = true;
                    gunSonuPozKapatildi = true;
                }
            }*/
            return gunSonuPozKapatildi;
        }

        public bool gun_sonu_poz_kapat2(int i, bool gunSonuPozKapatEnabled = true, int hour = 18, int minute= 0)
        {
            bool gunSonuPozKapatildi = false;
            /*
            if (gunSonuPozKapatEnabled)
                {
                if (this.Date[i].Hour == hour && this.Date[i].Minute >= minute)
                {
                    this.signals.FlatOl = true;
                    gunSonuPozKapatildi = true;
                }
            }*/
            return gunSonuPozKapatildi;
        }
        public void emir_sonrasi_dongu_foksiyonlarini_calistir(int i)
        {
            this.signals.GunSonuPozKapatildi = this.gun_sonu_poz_kapat(i, this.signals.GunSonuPozKapatEnabled);            

            emirleri_uygula(i);

            if (this.signals.KarAlindi == false && this.signals.KarAl)
                this.signals.KarAlindi = true;

            if (this.signals.ZararKesildi == false && this.signals.ZararKes)
                this.signals.ZararKesildi = true;

            if (this.signals.FlatOlundu == false && this.signals.FlatOl)
                this.signals.FlatOlundu = true;

            this.sistem_yon_listesini_guncelle(i);

            this.sistem_seviye_listesini_guncelle(i);

            this.sinyal_listesini_guncelle(i);

            this.islem_listesini_guncelle(i);

            this.komisyon_listesini_guncelle(i);

            this.bakiye_listesini_guncelle(i);

            this.dongu_sonu_degiskenleri_setle(i);

        }
        public void emirleri_uygula(int i)
        {
            if (this.signals.None == true)
            {

            }
            else if (this.signals.Al  == true)
            {
                //Log($"BuySignal received at {i} bar...");
            }
            else if (this.signals.Sat == true)
            {
                //Log($"SellSignal received at {i} bar...");
            }
            else if (this.signals.KarAl == true)
            {
                //Log($"TakeProfitSignal received at {i} bar...");
            }
            else if (this.signals.ZararKes == true)
            {
                //Log($"StopLossSignal received at {i} bar...");
            }
            else if (this.signals.FlatOl == true)
            {
                //Log($"FlatSignal received at {i} bar...");
            }
            else if (this.signals.PasGec == true)
            {
                //Log($"SkipSignal received at {i} bar...");
            }
        }

        public void Run(int i)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");

            if (Strategy == null)
                throw new InvalidOperationException("Strategy not set");

            if (i >= Data.Count)
                return;

            emirleri_resetle(i);

            emir_oncesi_dongu_foksiyonlarini_calistir(i);

            if (i < 1)
                return;

            // --------------------------------------------------------------------------------------------------------------------------------------------
            this.StrategySignal = this.Strategy.OnStep(i);

            //IsSonYonA = trader.is_son_yon_a()

            //IsSonYonS = trader.is_son_yon_s()

            //IsSonYonF = trader.is_son_yon_f()

            // --------------------------------------------------------------------------------------------------------------------------------------------
            emirleri_setle(i, this.StrategySignal);

            islem_zaman_filtresi_uygula(i);

            emir_sonrasi_dongu_foksiyonlarini_calistir(i);
        }
        public void Finalize(int i)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");
        }

#endregion

        #region Helper Methods

        /// <summary>
        /// Get current bar
        /// </summary>
        public StockData GetCurrentBar()
        {
            if (CurrentIndex < 0 || CurrentIndex >= Data.Count)
                throw new InvalidOperationException($"Invalid current index: {CurrentIndex}");

            return Data[CurrentIndex];
        }

        /// <summary>
        /// Get statistics summary
        /// </summary>
        public string GetStatisticsSummary()
        {
            return Statistics.GetSummary();
        }

        #endregion
    }
}
