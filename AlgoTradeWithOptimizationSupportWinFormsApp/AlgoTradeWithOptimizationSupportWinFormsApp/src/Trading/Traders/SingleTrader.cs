using System;
using System.Collections.Generic;
using AlgoTradeWithOptimizationSupportWinFormsApp.DataProvider;
using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics;
using System.Linq.Expressions;

// Import IAlgoTraderLogger from Trading namespace
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading;

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
        public KarZarar KarZarar { get; private set; }
        public TradeStatistics Statistics { get; private set; }
        public int CurrentIndex { get; private set; }

        private bool _isInitialized = false;
        public override bool IsInitialized => _isInitialized;

        // Logger
        private IAlgoTraderLogger? _logger;

        public TradeSignals Signal { get; private set; }
        public bool NoneSignal { get; private set; }
        public bool BuySignal { get; private set; }
        public bool SellSignal { get; private set; }
        public bool TakeProfitSignal { get; private set; }
        public bool StopLossSignal { get; private set; }
        public bool FlatSignal { get; private set; }
        public bool SkipSignal { get; private set; }
        public bool BuySignalEnabled { get; set; }
        public bool SellSignalEnabled { get; set; }
        public bool TakeProfitSignalEnabled { get; set; }
        public bool StopLossSignalEnabled { get; set; }
        public bool FlatSignalEnabled { get; set; }
        public bool SkipSignalEnabled { get; set; }

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



        #endregion

        #region Constructor

        // Parametresiz constructor (eski kullanımlar için)
        public SingleTrader()
        {
            Position = new Position();
            Bakiye = new Bakiye();
            KarZarar = new KarZarar();
            Statistics = new TradeStatistics();
            _isInitialized = false;
        }

        // Parametreli constructor (yeni kullanım)
        public SingleTrader(List<StockData> data, IndicatorManager indicators, BaseStrategy strategy)
        {
            Position = new Position();
            Bakiye = new Bakiye();
            KarZarar = new KarZarar();
            Statistics = new TradeStatistics();

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
            // Create or reinitialize modules
            Position = new Position();
            Bakiye = new Bakiye();
            KarZarar = new KarZarar();
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
            DeleteModules();

            this.BuySignalEnabled = false;
            this.SellSignalEnabled = false;
            this.TakeProfitSignalEnabled = false;
            this.StopLossSignalEnabled = false;
            this.FlatSignalEnabled = false;
            this.SkipSignalEnabled = false;

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
            Position.Close();
            Bakiye.Reset();
            KarZarar.Reset();
            Statistics.Reset();
            Strategy?.Reset();
        }
        public void Init()
        {
            CurrentIndex = 0;
            Position.Close();
            Bakiye.Reset();
            KarZarar.Reset();
            Statistics.Reset();
            Strategy?.Reset();
        }

        public void Initialize(int i)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");
        }

        public void Run(int i)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");

            if (Strategy == null)
                throw new InvalidOperationException("Strategy not set");

            if (i >= Data.Count)
                return;

            // --------------------------------------------------------------------------------------------------------------------------------------------
            this.NoneSignal = this.BuySignal = this.SellSignal = this.TakeProfitSignal = this.StopLossSignal = this.FlatSignal = this.SkipSignal = false;

            // --------------------------------------------------------------------------------------------------------------------------------------------
            this.Signal = this.Strategy.OnStep(i);

            // --------------------------------------------------------------------------------------------------------------------------------------------
            if (this.Signal == TradeSignals.None) 
            {
                this.NoneSignal = true;
            }

            if (this.Signal == TradeSignals.Buy)
            {
                if (this.BuySignalEnabled)
                    this.BuySignal = true;
            }

            if (this.Signal == TradeSignals.Sell)
            {
                if (this.SellSignalEnabled)
                    this.SellSignal = true;
            }

            if (this.Signal == TradeSignals.TakeProfit)
            {
                if (this.TakeProfitSignalEnabled)
                    this.TakeProfitSignal = true;
            }

            if (this.Signal == TradeSignals.StopLoss)
            {
                if (this.StopLossSignalEnabled)
                    this.StopLossSignal = true;
            }

            if (this.Signal == TradeSignals.Flat)
            {
                if (this.FlatSignalEnabled)
                    this.FlatSignal = true;
            }

            if (this.Signal == TradeSignals.Skip)
            {
                if (this.SkipSignalEnabled)
                    this.SkipSignal = true;
            }

            // --------------------------------------------------------------------------------------------------------------------------------------------
            if (this.NoneSignal == true)
            {
      
            }            
            else if (this.BuySignal == true)
            {
                Log($"BuySignal received at {i} bar...");
            }
            else if (this.SellSignal == true)
            {
                Log($"SellSignal received at {i} bar...");
            }
            else if (this.TakeProfitSignal == true)
            {
                Log($"TakeProfitSignal received at {i} bar...");
            }
            else if (this.StopLossSignal == true)
            {
                Log($"StopLossSignal received at {i} bar...");
            }
            else if (this.FlatSignal == true)
            {
                Log($"FlatSignal received at {i} bar...");
            }
            else if (this.SkipSignal == true)
            {
                Log($"SkipSignal received at {i} bar...");
            }
            // --------------------------------------------------------------------------------------------------------------------------------------------
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
