using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders
{
    /// <summary>
    /// Multiple trader - manages and runs multiple SingleTraders in parallel
    /// Collects signals from all traders and creates a consensus signal for mainTrader
    /// </summary>
    public class MultipleTrader
    {
        #region Properties

        public int Id { get; private set; }
        public List<StockData> Data { get; private set; }
        public IndicatorManager Indicators { get; private set; }
        public IAlgoTraderLogger? Logger { get; private set; }

        public List<SingleTrader> Traders { get; private set; }
        public bool IsInitialized { get; private set; }
        public int CurrentIndex { get; private set; }

        private SingleTrader _mainTrader;

        public Action<MultipleTrader, int, int>? OnProgress { get; set; }

        #endregion

        #region Constructor

        public MultipleTrader()
        {
            Traders = new List<SingleTrader>();
            IsInitialized = false;
        }

        /// <summary>
        /// Parametreli constructor - mainTrader ile birlikte oluşturulur
        /// </summary>
        public MultipleTrader(int id, List<StockData> data, IndicatorManager indicators, IAlgoTraderLogger? logger)
        {
            Id = id;
            Data = data;
            Indicators = indicators;
            Logger = logger;

            Traders = new List<SingleTrader>();

            // Create mainTrader with ID = -1 to distinguish from other traders
            _mainTrader = new SingleTrader(-1, data, indicators, logger);

            IsInitialized = true;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize with market data
        /// </summary>
        public void Initialize(List<StockData> data)
        {
            if (data == null || data.Count == 0)
                throw new ArgumentException("Data cannot be null or empty");

            Data = data;
            CurrentIndex = 0;
            IsInitialized = true;
        }

        /// <summary>
        /// Add a trader
        /// </summary>
        public void AddTrader(SingleTrader trader)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("MultipleTrader not initialized");

            Traders.Add(trader);

            // Initialize trader with same data
            if (!trader.IsInitialized)
            {
                trader.SetData(Data);
            }
        }

        /// <summary>
        /// Reset all traders
        /// </summary>
        public void Reset()
        {
            CurrentIndex = 0;
            foreach (var trader in Traders)
            {

            }
        }
        public void Init()
        {
            CurrentIndex = 0;
            foreach (var trader in Traders)
            {

            }
        }
        public void Initialize()
        {
            CurrentIndex = 0;
            foreach (var trader in Traders)
            {
                trader.Initialize();
            }
        }

        /// <summary>
        /// Build consensus signal based on position status and trader signals
        /// Gruplar:
        /// - Pozisyon AÇIK ise: (StopLoss, TakeProfit, Flat) sinyallerine bak
        /// - Pozisyon KAPALI ise: (Buy, Sell) sinyallerine bak
        /// </summary>
        public TradeSignals buildConsensusSignal(
            bool isPozisyonAcik,
            int noneSignalCount,
            int alSignalCount,
            int satSignalCount,
            int flatOlSignalCount,
            int passGecSignalCount,
            int karAlSignalCount,
            int zararKesSignalCount)
        {
            TradeSignals consensusSignal = TradeSignals.None;

            // =========================================================
            // GRUP 1: POZİSYON AÇIK - Kapatma sinyallerini değerlendir
            // =========================================================
            if (isPozisyonAcik)
            {
                // Priority 1: StopLoss - risk yönetimi mutlak öncelik
                if (zararKesSignalCount > 0)
                {
                    consensusSignal = TradeSignals.StopLoss;
                }
                // Priority 2: TakeProfit - kar koruma
                else if (karAlSignalCount > 0)
                {
                    consensusSignal = TradeSignals.TakeProfit;
                }
                // Majority: Flat (pozisyonu kapat ama kar/zarar değil)
                else if (flatOlSignalCount > 0)
                {
                    // Eğer Flat çoğunluktaysa uygula
                    // Aksi halde pozisyonu koru (None döndür)
                    int totalVotes = flatOlSignalCount + alSignalCount + satSignalCount + passGecSignalCount;
                    if (flatOlSignalCount > totalVotes / 2)
                    {
                        consensusSignal = TradeSignals.Flat;
                    }
                    else
                    {
                        // Çoğunluk yok, pozisyonu koru
                        consensusSignal = TradeSignals.None;
                    }
                }
                else
                {
                    // Hiç kapatma sinyali yok, pozisyonu koru
                    consensusSignal = TradeSignals.None;
                }
            }
            // =========================================================
            // GRUP 2: POZİSYON KAPALI - Açma sinyallerini değerlendir
            // =========================================================
            else
            {
                // Buy vs Sell çoğunluk oylaması
                if (alSignalCount > satSignalCount && alSignalCount > 0)
                {
                    // Buy çoğunlukta
                    consensusSignal = TradeSignals.Buy;
                }
                else if (satSignalCount > alSignalCount && satSignalCount > 0)
                {
                    // Sell çoğunlukta
                    consensusSignal = TradeSignals.Sell;
                }
                else if (alSignalCount == satSignalCount && alSignalCount > 0)
                {
                    // Eşitlik - karar verilemedi
                    consensusSignal = TradeSignals.None;
                }
                else
                {
                    // Hiç açma sinyali yok
                    // Skip çoğunluktaysa Skip, yoksa None
                    if (passGecSignalCount > 0)
                    {
                        consensusSignal = TradeSignals.Skip;
                    }
                    else
                    {
                        consensusSignal = TradeSignals.None;
                    }
                }
            }

            return consensusSignal;
        }

        public void Run(int i)
        {
            int noneSignalCount = 0;
            int alSignalCount = 0;
            int satSignalCount = 0;
            int flatOlSignalCount = 0;
            int passGecSignalCount = 0;
            int karAlSignalCount = 0;
            int zararKesSignalCount = 0;

            // -----------------------------------------------------------
            foreach (var trader in Traders)
            {
                trader.Run(i);

                TradeSignals signal = trader.StrategySignal;

                if (signal == TradeSignals.None)
                {
                    noneSignalCount++;
                }

                if (signal == TradeSignals.Buy)
                {
                    alSignalCount++;
                }

                if (signal == TradeSignals.Sell)
                {
                    satSignalCount++;
                }

                if (signal == TradeSignals.TakeProfit)
                {
                    karAlSignalCount++;
                }

                if (signal == TradeSignals.StopLoss)
                {
                    zararKesSignalCount++;
                }

                if (signal == TradeSignals.Flat)
                {
                    flatOlSignalCount++;
                }

                if (signal == TradeSignals.Skip)
                {
                    passGecSignalCount++;
                }
            }

            // -------------- Consensus Logic ---------------
            var isSonYonA = _mainTrader.is_son_yon_a();

            var isSonYonS = _mainTrader.is_son_yon_s();

            var isSonYonF = _mainTrader.is_son_yon_f();

            // MainTrader'ın pozisyon durumunu kontrol et            
            bool isPozisyonAcik = isSonYonA || isSonYonS;   //bool isPozisyonAcik = _mainTrader.signals.PozAcildiAlis || _mainTrader.signals.PozAcildiSatis;

            // Pozisyon durumuna göre consensus sinyal oluştur
            TradeSignals consensusSignal = buildConsensusSignal(
                isPozisyonAcik,
                noneSignalCount,
                alSignalCount,
                satSignalCount,
                flatOlSignalCount,
                passGecSignalCount,
                karAlSignalCount,
                zararKesSignalCount);

            // -----------------------------------------------------------
            _mainTrader.OnRun?.Invoke(_mainTrader, 0);

            _mainTrader.emirleri_resetle(i);

            _mainTrader.emir_oncesi_dongu_foksiyonlarini_calistir(i);

            if (i < 1)
                return;

            // Set consensus signal to mainTrader
            _mainTrader.StrategySignal = consensusSignal;

            _mainTrader.emirleri_setle(i, _mainTrader.StrategySignal);

            int filterMode = 3;
            bool isTradeEnabled = false;
            bool isPozKapatEnabled = false;
            int checkResult = 0;
            _mainTrader.islem_zaman_filtresi_uygula(i, filterMode, ref isTradeEnabled, ref isPozKapatEnabled, ref checkResult);

            _mainTrader.emir_sonrasi_dongu_foksiyonlarini_calistir(i);

            _mainTrader.OnRun?.Invoke(_mainTrader, 1);


        }
        public void Finalize()
        {
            CurrentIndex = 0;
            foreach (var trader in Traders)
            {
                trader.Finalize();
            }
        }

        #endregion

        #region Trading Methods

        /// <summary>
        /// Execute one step for all traders
        /// </summary>
        public void Step()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("MultipleTrader not initialized");

            if (CurrentIndex >= Data.Count)
                return;

            // TODO: Step all traders
            foreach (var trader in Traders)
            {
                trader.Step();
            }

            CurrentIndex++;
        }

        /// <summary>
        /// Run all traders
        /// </summary>
        public void Run()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("MultipleTrader not initialized");

            Reset();

            // TODO: Run all traders
            foreach (var trader in Traders)
            {
                trader.Run(0);
            }
        }

        #endregion

        #region Statistics Methods

        /// <summary>
        /// Get statistics from all traders
        /// </summary>
        public Dictionary<string, string> GetAllStatistics()
        {
            var stats = new Dictionary<string, string>();

            for (int i = 0; i < Traders.Count; i++)
            {
                stats[$"Trader_{i}"] = Traders[i].GetStatisticsSummary();
            }

            return stats;
        }

        /// <summary>
        /// Get best trader by net profit
        /// </summary>
        public SingleTrader GetBestTrader()
        {
            if (Traders.Count == 0)
                return null;

            // TODO: Find trader with highest net profit
            //return Traders.OrderByDescending(t => tsStatistics.NetProfit).FirstOrDefault();
            return null;
        }

        #endregion

        #region Main Trader Methods

        /// <summary>
        /// Get the main trader that will execute consensus signals
        /// MainTrader has ID = -1
        /// </summary>
        public SingleTrader GetMainTrader()
        {
            return _mainTrader;
        }

        /// <summary>
        /// Set callbacks for mainTrader and all traders in the list
        /// Callbacks receive SingleTrader instance, so you can check trader.Id to distinguish
        /// MainTrader: trader.Id == -1
        /// Other traders: trader.Id >= 0
        /// </summary>
        public void SetCallbacks(
            Action<SingleTrader, int>? onReset = null,
            Action<SingleTrader, int>? onInit = null,
            Action<SingleTrader, int>? onRun = null,
            Action<SingleTrader, int>? onFinal = null,
            Action<SingleTrader, int>? onBeforeOrders = null,
            Action<SingleTrader, string, int>? onNotifySignal = null,
            Action<SingleTrader, int>? onAfterOrders = null,
            Action<SingleTrader, int, int>? onProgress = null,
            Action<SingleTrader>? onApplyUserFlags = null)
        {
            // Set callbacks for mainTrader
            _mainTrader.SetCallbacks(onReset, onInit, onRun, onFinal, onBeforeOrders, onNotifySignal, onAfterOrders, onProgress, onApplyUserFlags);

            // Set callbacks for all traders in the list
            foreach (var trader in Traders)
            {
                trader.SetCallbacks(onReset, onInit, onRun, onFinal, onBeforeOrders, onNotifySignal, onAfterOrders, onProgress, onApplyUserFlags);
            }
        }

        /// <summary>
        /// Dispose mainTrader and all traders
        /// </summary>
        public void Dispose()
        {
            // Dispose mainTrader
            _mainTrader?.Dispose();
            _mainTrader = null;

            // Dispose all traders
            foreach (var trader in Traders)
            {
                trader?.Dispose();
            }
            Traders.Clear();
        }

        #endregion
    }
}
