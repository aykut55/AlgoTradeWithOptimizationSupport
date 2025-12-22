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

        // Time Filter Properties
        public string StartDateTimeStr { get; set; }
        public string StopDateTimeStr { get; set; }
        public string StartDateStr { get; set; }
        public string StopDateStr { get; set; }
        public string StartTimeStr { get; set; }
        public string StopTimeStr { get; set; }

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
        public AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics.Statistics statistics { get; private set; }

        // Execution Time Tracking
        public string LastExecutionTime { get; set; }
        public string LastExecutionTimeStart { get; set; }
        public string LastExecutionTimeStop { get; set; }
        public int ExecutionTimeInMSec { get; set; }
        public string LastResetTime { get; set; }
        public string LastStatisticsCalculationTime { get; set; }

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
            timeUtils.SetTrader(this);
            karZarar = new KarZarar(this);
            karAlZararKes = new KarAlZararKes();
            komisyon = new Komisyon();
            komisyon.SetTrader(this);
            Bakiye = new Bakiye();
            Bakiye.SetTrader(this);
            bakiye = new Bakiye();
            bakiye.SetTrader(this);
            pozisyonBuyuklugu = new PozisyonBuyuklugu();
            Position = new Position();
            statistics = new AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics.Statistics();
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
            statistics.Reset();

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
            karZarar.Init(this);

            karAlZararKes.Init();
            komisyon.Init();

            Bakiye.Init();
            pozisyonBuyuklugu.Init();

            //Position.Init();
            statistics.Init(this);
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
            this.status.KomisyonVarlikAdedSayisi = this.pozisyonBuyuklugu.KomisyonVarlikAdedSayisi;
            this.status.KomisyonVarlikAdedSayisiMicro = this.pozisyonBuyuklugu.KomisyonVarlikAdedSayisiMicro;
            this.status.KomisyonCarpan = this.pozisyonBuyuklugu.KomisyonCarpan;
            this.flags.KomisyonuDahilEt = this.pozisyonBuyuklugu.KomisyonuDahilEt;
            this.status.KaymaMiktari = this.pozisyonBuyuklugu.KaymaMiktari;
            this.flags.KaymayiDahilEt = this.pozisyonBuyuklugu.KaymayiDahilEt;
            this.status.VarlikAdedSayisi = this.pozisyonBuyuklugu.VarlikAdedSayisi;
            this.status.VarlikAdedSayisiMicro = this.pozisyonBuyuklugu.VarlikAdedSayisiMicro;
            this.status.VarlikAdedCarpani = this.pozisyonBuyuklugu.VarlikAdedCarpani;
            this.status.KontratSayisi = this.pozisyonBuyuklugu.KontratSayisi;
            this.status.HisseSayisi = this.pozisyonBuyuklugu.HisseSayisi;
            this.status.IlkBakiyeFiyat = this.pozisyonBuyuklugu.IlkBakiyeFiyat;
            this.status.IlkBakiyePuan = this.pozisyonBuyuklugu.IlkBakiyePuan;
            this.status.GetiriFiyatTipi = this.pozisyonBuyuklugu.GetiriFiyatTipi;
            this.MicroLotSizeEnabled = this.pozisyonBuyuklugu.MicroLotSizeEnabled;
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

        public int islem_zaman_filtresi_uygula(int BarIndex, int FilterMode, ref bool IsTradeEnabled, ref bool IsPozKapatEnabled, ref int CheckResult)
        {
            int i = BarIndex;
            DateTime BarDateTime = this.Data[i].DateTime;
            string startDateTime = this.StartDateTimeStr ?? "";
            string stopDateTime = this.StopDateTimeStr ?? "";
            string startDate = this.StartDateStr ?? "";
            string stopDate = this.StopDateStr ?? "";
            string startTime = this.StartTimeStr ?? "";
            string stopTime = this.StopTimeStr ?? "";

            DateTime now = DateTime.Now;
            string nowDateTime = now.ToString("dd.MM.yyyy HH:mm:ss");
            string nowDate = now.ToString("dd.MM.yyyy");
            string nowTime = now.ToString("HH:mm:ss");

            bool useTimeFiltering = this.signals.TimeFilteringEnabled;

            if (useTimeFiltering)
            {
                if (i == this.Data.Count - 1)
                {
                    string s = "";
                    s += $"  {startDateTime}\n";
                    s += $"  {stopDateTime}\n";
                    s += $"  {startDate}\n";
                    s += $"  {stopDate}\n";
                    s += $"  {startTime}\n";
                    s += $"  {stopTime}\n";
                    s += $"  {nowDateTime}\n";
                    s += $"  {nowDate}\n";
                    s += $"  {nowTime}\n";
                    s += $"  FilterMode = {FilterMode}\n";
                    s += "  CTrader::IslemZamanFiltresiUygula\n";
                    // Log if needed
                }

                if (FilterMode == 0)
                {
                    IsTradeEnabled = true;
                    CheckResult = 0;
                }
                else if (FilterMode == 1)
                {
                    if (this.timeUtils.check_bar_time_with(i, startTime) >= 0 && this.timeUtils.check_bar_time_with(i, stopTime) < 0)
                    {
                        IsTradeEnabled = true;
                        CheckResult = 0;
                    }
                    else if (this.timeUtils.check_bar_time_with(i, startTime) < 0)
                    {
                        if (!this.is_son_yon_f())
                        {
                            IsPozKapatEnabled = true;
                        }
                        CheckResult = -1;
                    }
                    else if (this.timeUtils.check_bar_time_with(i, stopTime) >= 0)
                    {
                        if (!this.is_son_yon_f())
                        {
                            IsPozKapatEnabled = true;
                        }
                        CheckResult = 1;
                    }
                }
                else if (FilterMode == 2)
                {
                    if (this.timeUtils.check_bar_date_with(i, startDate) >= 0 && this.timeUtils.check_bar_date_with(i, stopDate) < 0)
                    {
                        IsTradeEnabled = true;
                        CheckResult = 0;
                    }
                    else if (this.timeUtils.check_bar_date_with(i, startDate) < 0)
                    {
                        if (!this.is_son_yon_f())
                        {
                            IsPozKapatEnabled = true;
                        }
                        CheckResult = -1;
                    }
                    else if (this.timeUtils.check_bar_date_with(i, stopDate) >= 0)
                    {
                        if (!this.is_son_yon_f())
                        {
                            IsPozKapatEnabled = true;
                        }
                        CheckResult = 1;
                    }
                }
                else if (FilterMode == 3)
                {
                    if (this.timeUtils.check_bar_date_time_with(i, startDateTime) >= 0 && this.timeUtils.check_bar_date_time_with(i, stopDateTime) < 0)
                    {
                        IsTradeEnabled = true;
                        CheckResult = 0;
                    }
                    else if (this.timeUtils.check_bar_date_time_with(i, startDateTime) < 0)
                    {
                        if (!this.is_son_yon_f())
                        {
                            IsPozKapatEnabled = true;
                        }
                        CheckResult = -1;
                    }
                    else if (this.timeUtils.check_bar_date_time_with(i, stopDateTime) >= 0)
                    {
                        if (!this.is_son_yon_f())
                        {
                            IsPozKapatEnabled = true;
                        }
                        CheckResult = 1;
                    }
                }
                else if (FilterMode == 4)
                {
                    if (this.timeUtils.check_bar_time_with(i, startTime) >= 0)
                    {
                        IsTradeEnabled = true;
                        CheckResult = 0;
                    }
                    else if (this.timeUtils.check_bar_time_with(i, startTime) < 0)
                    {
                        if (!this.is_son_yon_f())
                        {
                            IsPozKapatEnabled = true;
                        }
                        CheckResult = -1;
                    }
                }
                else if (FilterMode == 5)
                {
                    if (this.timeUtils.check_bar_date_with(i, startDate) >= 0)
                    {
                        IsTradeEnabled = true;
                        CheckResult = 0;
                    }
                    else if (this.timeUtils.check_bar_date_with(i, startDate) < 0)
                    {
                        if (!this.is_son_yon_f())
                        {
                            IsPozKapatEnabled = true;
                        }
                        CheckResult = -1;
                    }
                }
                else if (FilterMode == 6)
                {
                    if (this.timeUtils.check_bar_date_time_with(i, startDateTime) >= 0)
                    {
                        IsTradeEnabled = true;
                        CheckResult = 0;
                    }
                    else if (this.timeUtils.check_bar_date_time_with(i, startDateTime) < 0)
                    {
                        if (!this.is_son_yon_f())
                        {
                            IsPozKapatEnabled = true;
                        }
                        CheckResult = -1;
                    }
                }
            }

            return 0;
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
            this.Bakiye.Hesapla(i);
            if (this.flags.BakiyeGuncelle)
                this.flags.BakiyeGuncelle = false;
        }
        public void dongu_sonu_degiskenleri_setle(int i)
        {

        }
        public bool gun_sonu_poz_kapat(int i, bool gunSonuPozKapatEnabled = true)
        {
            bool gunSonuPozKapatildi = false;

            int barCount = GetDataCount();

            var dates = GetDates();

            if (gunSonuPozKapatEnabled)
            {
                if (i < (barCount-1) && dates[i] != dates[i + 1])
                {
                    this.signals.FlatOl = true;
                    gunSonuPozKapatildi = true;
                }
            }
            return gunSonuPozKapatildi;
        }

        public bool gun_sonu_poz_kapat2(int i, bool gunSonuPozKapatEnabled = true, int hour = 18, int minute= 0)
        {
            bool gunSonuPozKapatildi = false;

            int barCount = GetDataCount();

            var dates = GetDateTimes();

            if (gunSonuPozKapatEnabled)
                {
                if (dates[i].Hour == hour && dates[i].Minute >= minute)
                {
                    this.signals.FlatOl = true;
                    gunSonuPozKapatildi = true;
                }
            }
            return gunSonuPozKapatildi;
        }
        public void emir_sonrasi_dongu_foksiyonlarini_calistir(int i)
        {
            // TODO : Cok yavas calisiyor...
            // this.signals.GunSonuPozKapatildi = this.gun_sonu_poz_kapat(i, this.signals.GunSonuPozKapatEnabled);            

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
        public int emirleri_uygula(int BarIndex)
        {
            int result = 0;
            int i = BarIndex;

            this.flags.AGerceklesti = false;
            this.flags.SGerceklesti = false;
            this.flags.FGerceklesti = false;
            this.flags.PGerceklesti = false;

            double AnlikKapanisFiyati = this.Data[i].Close;
            double AnlikYuksekFiyati = this.Data[i].High;
            double AnlikDusukFiyati = this.Data[i].Low;

            // Set EmirKomut based on signals
            if (this.signals.None)
            {

            }
            if (this.signals.Al)
            {
                this.signals.Sinyal = "A";
                this.signals.EmirKomut = 1;
                this.status.AlKomutSayisi += 1;
            }
            if (this.signals.Sat)
            {
                this.signals.Sinyal = "S";
                this.signals.EmirKomut = 2;
                this.status.SatKomutSayisi += 1;
            }
            if (this.signals.PasGec)
            {
                this.signals.Sinyal = "P";
                this.signals.EmirKomut = 3;
                this.status.PasGecKomutSayisi += 1;
            }
            if (this.signals.KarAl)
            {
                this.signals.Sinyal = "F";
                this.signals.EmirKomut = 4;
                this.status.KarAlKomutSayisi += 1;
            }
            if (this.signals.ZararKes)
            {
                this.signals.Sinyal = "F";
                this.signals.EmirKomut = 5;
                this.status.ZararKesKomutSayisi += 1;
            }
            if (this.signals.FlatOl)
            {
                this.signals.Sinyal = "F";
                this.signals.EmirKomut = 6;
                this.status.FlatOlKomutSayisi += 1;
            }

            this.status.KarAlSayisi = this.status.KarAlKomutSayisi;
            this.status.ZararKesSayisi = this.status.ZararKesKomutSayisi;

            // Process "A" (Al/Buy) signal
            if (this.signals.Sinyal == "A" && this.signals.SonYon != "A")
            {
                this.signals.PrevAFiyat = this.signals.SonAFiyat;
                this.signals.PrevABarNo = this.signals.SonABarNo;
                this.signals.PrevYon = this.signals.SonYon;
                this.signals.PrevFiyat = this.signals.SonFiyat;
                this.signals.PrevBarNo = this.signals.SonBarNo;

                if (this.signals.PrevYon == "F")
                {
                    // pass
                }
                if (this.signals.PrevYon == "S")
                {
                    // pass
                }

                this.lists.YonList[i] = "A";
                this.signals.SonYon = this.lists.YonList[i];
                this.signals.SonFiyat = AnlikKapanisFiyati;

                if (this.flags.KaymayiDahilEt)
                {
                    this.signals.SonFiyat = AnlikYuksekFiyati;
                }

                this.lists.SeviyeList[i] = this.signals.SonFiyat;
                this.signals.SonBarNo = i;
                this.signals.SonAFiyat = this.signals.SonFiyat;
                this.signals.SonABarNo = this.signals.SonBarNo;

                if (this.signals.PrevYon == "F")
                {
                    this.status.KomisyonIslemSayisi += 1;
                    this.signals.EmirStatus = 1;
                }
                if (this.signals.PrevYon == "S")
                {
                    double fark = this.signals.SonFiyat - this.signals.SonSFiyat;
                    if (fark < 0)
                    {
                        this.status.KazandiranSatisSayisi += 1;
                    }
                    else if (fark > 0)
                    {
                        this.status.KaybettirenSatisSayisi += 1;
                    }
                    else
                    {
                        this.status.NotrSatisSayisi += 1;
                    }
                    this.status.KomisyonIslemSayisi += 2;
                    this.signals.EmirStatus = 2;
                }

                this.flags.BakiyeGuncelle = true;
                this.flags.KomisyonGuncelle = true;
                this.flags.DonguSonuIstatistikGuncelle = true;
                this.status.IslemSayisi += 1;
                this.status.AlisSayisi += 1;
                this.flags.AGerceklesti = true;
            }
            // Process "S" (Sat/Sell) signal
            else if (this.signals.Sinyal == "S" && this.signals.SonYon != "S")
            {
                this.signals.PrevSFiyat = this.signals.SonSFiyat;
                this.signals.PrevSBarNo = this.signals.SonSBarNo;
                this.signals.PrevYon = this.signals.SonYon;
                this.signals.PrevFiyat = this.signals.SonFiyat;
                this.signals.PrevBarNo = this.signals.SonBarNo;

                if (this.signals.PrevYon == "F")
                {
                    // pass
                }
                if (this.signals.PrevYon == "A")
                {
                    // pass
                }

                this.lists.YonList[i] = "S";
                this.signals.SonYon = this.lists.YonList[i];
                this.signals.SonFiyat = AnlikKapanisFiyati;

                if (this.flags.KaymayiDahilEt)
                {
                    this.signals.SonFiyat = AnlikDusukFiyati;
                }

                this.lists.SeviyeList[i] = this.signals.SonFiyat;
                this.signals.SonBarNo = i;
                this.signals.SonSFiyat = this.signals.SonFiyat;
                this.signals.SonSBarNo = this.signals.SonSBarNo;

                if (this.signals.PrevYon == "F")
                {
                    this.status.KomisyonIslemSayisi += 1;
                    this.signals.EmirStatus = 3;
                }
                if (this.signals.PrevYon == "A")
                {
                    double fark = this.signals.SonFiyat - this.signals.SonAFiyat;
                    if (fark > 0)
                    {
                        this.status.KazandiranAlisSayisi += 1;
                    }
                    else if (fark < 0)
                    {
                        this.status.KaybettirenAlisSayisi += 1;
                    }
                    else
                    {
                        this.status.NotrAlisSayisi += 1;
                    }
                    this.status.KomisyonIslemSayisi += 2;
                    this.signals.EmirStatus = 4;
                }

                this.flags.BakiyeGuncelle = true;
                this.flags.KomisyonGuncelle = true;
                this.flags.DonguSonuIstatistikGuncelle = true;
                this.status.IslemSayisi += 1;
                this.status.SatisSayisi += 1;
                this.flags.SGerceklesti = true;
            }
            // Process "F" (Flat) signal
            else if (this.signals.Sinyal == "F" && this.signals.SonYon != "F")
            {
                this.signals.PrevFFiyat = this.signals.SonFFiyat;
                this.signals.PrevFBarNo = this.signals.SonFBarNo;
                this.signals.PrevYon = this.signals.SonYon;
                this.signals.PrevFiyat = this.signals.SonFiyat;
                this.signals.PrevBarNo = this.signals.SonBarNo;

                if (this.signals.PrevYon == "A")
                {
                    // pass
                }
                if (this.signals.PrevYon == "S")
                {
                    // pass
                }

                this.lists.YonList[i] = "F";
                this.signals.SonYon = this.lists.YonList[i];
                this.signals.SonFiyat = AnlikKapanisFiyati;

                if (this.flags.KaymayiDahilEt)
                {
                    if (this.signals.PrevYon == "A")
                    {
                        this.signals.SonFiyat = AnlikDusukFiyati;
                    }
                    if (this.signals.PrevYon == "S")
                    {
                        this.signals.SonFiyat = AnlikYuksekFiyati;
                    }
                }

                this.lists.SeviyeList[i] = this.signals.SonFiyat;
                this.signals.SonBarNo = i;
                this.signals.SonFFiyat = this.signals.SonFiyat;
                this.signals.SonFBarNo = this.signals.SonFBarNo;

                if (this.signals.PrevYon == "A")
                {
                    double fark = this.signals.SonFiyat - this.signals.SonAFiyat;
                    if (fark > 0)
                    {
                        this.status.KazandiranAlisSayisi += 1;
                    }
                    else if (fark < 0)
                    {
                        this.status.KaybettirenAlisSayisi += 1;
                    }
                    else
                    {
                        this.status.NotrAlisSayisi += 1;
                    }
                    this.status.KomisyonIslemSayisi += 1;
                    this.signals.EmirStatus = 5;
                }
                if (this.signals.PrevYon == "S")
                {
                    double fark = this.signals.SonFiyat - this.signals.SonSFiyat;
                    if (fark < 0)
                    {
                        this.status.KazandiranSatisSayisi += 1;
                    }
                    else if (fark > 0)
                    {
                        this.status.KaybettirenSatisSayisi += 1;
                    }
                    else
                    {
                        this.status.NotrSatisSayisi += 1;
                    }
                    this.status.KomisyonIslemSayisi += 1;
                    this.signals.EmirStatus = 6;
                }

                this.flags.BakiyeGuncelle = true;
                this.flags.KomisyonGuncelle = true;
                this.flags.DonguSonuIstatistikGuncelle = true;
                this.status.IslemSayisi += 1;
                this.status.FlatSayisi += 1;
                this.flags.FGerceklesti = true;
            }
            // Process "P" (PasGec/Skip) or empty signal
            else if (this.signals.Sinyal == "P" || this.signals.Sinyal == "")
            {
                this.signals.PrevPFiyat = this.signals.SonPFiyat;
                this.signals.PrevPBarNo = this.signals.SonPBarNo;
                this.signals.SonPFiyat = AnlikKapanisFiyati;
                this.signals.SonPBarNo = i;

                if (this.signals.SonYon == "A")
                {
                    this.signals.EmirStatus = 7;
                }
                if (this.signals.SonYon == "S")
                {
                    this.signals.EmirStatus = 8;
                }
                if (this.signals.SonYon == "F")
                {
                    this.signals.EmirStatus = 9;
                }

                this.flags.BakiyeGuncelle = true;
                this.flags.KomisyonGuncelle = true;
                this.flags.DonguSonuIstatistikGuncelle = true;
                this.status.PassSayisi += 1;
                this.flags.PGerceklesti = true;
            }

            // Update totals
            this.status.KazandiranIslemSayisi = this.status.KazandiranAlisSayisi + this.status.KazandiranSatisSayisi;
            this.status.KaybettirenIslemSayisi = this.status.KaybettirenAlisSayisi + this.status.KaybettirenSatisSayisi;
            this.status.NotrIslemSayisi = this.status.NotrAlisSayisi + this.status.NotrSatisSayisi;

            // Reset bar counters if trade happened
            if (this.flags.AGerceklesti || this.flags.SGerceklesti || this.flags.FGerceklesti)
            {
                this.status.KardaBarSayisi = 0;
                this.status.ZarardaBarSayisi = 0;
            }

            // Enable calculations if trades exist
            if (this.status.IslemSayisi > 0)
            {
                this.flags.AnlikKarZararHesaplaEnabled = true;
                this.flags.KarAlYuzdeHesaplaEnabled = true;
                this.flags.IzleyenStopYuzdeHesaplaEnabled = true;
                this.flags.ZararKesYuzdeHesaplaEnabled = true;
                this.flags.KarAlSeviyeHesaplaEnabled = true;
                this.flags.ZararKesSeviyeHesaplaEnabled = true;
            }

            // Reset flags
            this.flags.AGerceklesti = false;
            this.flags.SGerceklesti = false;
            this.flags.FGerceklesti = false;
            this.flags.PGerceklesti = false;

            // Update lists
            this.lists.EmirKomutList[i] = this.signals.EmirKomut;
            this.lists.EmirStatusList[i] = this.signals.EmirStatus;

            return result;
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

            var isSonYonA = is_son_yon_a();

            var isSonYonS = is_son_yon_s();

            var isSonYonF = is_son_yon_f();

            // --------------------------------------------------------------------------------------------------------------------------------------------
            emirleri_setle(i, this.StrategySignal);

            int filterMode = 3;
            bool isTradeEnabled = false; 
            bool isPozKapatEnabled = false;
            int checkResult = 0;
            islem_zaman_filtresi_uygula(i, filterMode, ref isTradeEnabled, ref isPozKapatEnabled, ref checkResult);

            emir_sonrasi_dongu_foksiyonlarini_calistir(i);
        }

        public void Finalize(int i)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");

            istatistikleri_hesapla();
        }

        public void istatistikleri_hesapla()
        {
            int lastBarIndex = GetLastBarIndex();
            statistics.Hesapla(lastBarIndex);
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
            return ""; // Statistics.GetSummary();
        }

        /// <summary>
        /// Check if last direction is Flat (F)
        /// </summary>
        public bool is_son_yon_f()
        {
            return this.signals.SonYon == "F";
        }

        /// <summary>
        /// Check if last direction is Buy (A)
        /// </summary>
        public bool is_son_yon_a()
        {
            return this.signals.SonYon == "A";
        }

        /// <summary>
        /// Check if last direction is Sell (S)
        /// </summary>
        public bool is_son_yon_s()
        {
            return this.signals.SonYon == "S";
        }

        #endregion
    }
}
