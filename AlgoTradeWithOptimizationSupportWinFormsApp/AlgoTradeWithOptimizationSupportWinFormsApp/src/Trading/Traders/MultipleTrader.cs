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
    ///
    /// Pozisyon Büyüklüğü Modları:
    /// - Sabit Lot (DynamicPositionSizeEnabled=false): mainTrader sabit pozisyon büyüklüğü kullanır (varsayılan)
    /// - Dinamik Lot (DynamicPositionSizeEnabled=true): Consensus sinyalinden gelen lot büyüklüğü kullanılır
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

        // State flags (similar to SingleTrader)
        public bool IsStarted { get; internal set; }
        public bool IsRunning { get; internal set; }
        public bool IsStopped { get; internal set; }
        public bool IsStopRequested { get; internal set; }

        private SingleTrader _mainTrader;

        /// <summary>
        /// Dinamik pozisyon büyüklüğü desteği
        /// true: Consensus sinyalinden gelen lot büyüklüğü kullanılır (her pozisyon farklı büyüklükte olabilir)
        /// false: mainTrader'ın sabit pozisyon büyüklüğü kullanılır (varsayılan)
        /// </summary>
        public bool DynamicPositionSizeEnabled { get; set; } = false;

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
            _mainTrader = new SingleTrader(-1, "mainTrader", data, indicators, logger);

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

            // Reset state flags
            IsStarted = false;
            IsRunning = false;
            IsStopped = false;
            IsStopRequested = false;
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

        public TradeSignals buildConsensusSignal(ref double varlikAdedSayisiFinal)
        {
            TradeSignals consensusSignal = TradeSignals.None;

            double sonYonACount = 0;
            double sonYonSCount = 0;
            double sonYonFCount = 0;

            foreach (var trader in Traders)
            {
                double varlikAdedSayisi = 0.0;
                var isSonYonA = trader.is_son_yon_a();
                var isSonYonS = trader.is_son_yon_s();
                var isSonYonF = trader.is_son_yon_f();

                varlikAdedSayisi = trader.pozisyonBuyuklugu.VarlikAdedSayisi;
                if (trader.pozisyonBuyuklugu.MicroLotSizeEnabled)
                    varlikAdedSayisi = trader.pozisyonBuyuklugu.VarlikAdedSayisiMicro;

                if (isSonYonA)
                    sonYonACount += varlikAdedSayisi;   //0.5

                if (isSonYonS)
                    sonYonSCount += varlikAdedSayisi;   // 0.8

                if (isSonYonF)
                    sonYonFCount += varlikAdedSayisi;   // 0.1
            }

            double sonYonCountFinal = (sonYonACount * 1) + (sonYonSCount * -1) + (sonYonFCount * 0);    // -0.3

            if (sonYonCountFinal > 0.0)
            {
                consensusSignal = TradeSignals.Buy;
                varlikAdedSayisiFinal = Math.Abs(sonYonCountFinal);
            }
            else if (sonYonCountFinal < 0.0)
            {
                consensusSignal = TradeSignals.Sell;
                varlikAdedSayisiFinal = Math.Abs(sonYonCountFinal);
            }
            else
            {
                consensusSignal = TradeSignals.Flat;
                varlikAdedSayisiFinal = Math.Abs(sonYonCountFinal);
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

            double varlikAdedSayisiFinal = 0.0;
            TradeSignals consensusSignal = buildConsensusSignal(ref varlikAdedSayisiFinal);

            // varlikAdedSayisiFinal: Bilgi amaçlı (net exposure)
            // İsterseniz loglayabilirsiniz: Logger?.Log($"Net Exposure: {varlikAdedSayisiFinal:F2} lot")

            if (this.DynamicPositionSizeEnabled)
            {
                // DİNAMİK POZİSYON BÜYÜKLÜĞÜ MODU
                // Consensus sinyalinden gelen lot büyüklüğü kullanılır
                // Her pozisyon farklı büyüklükte olabilir (örn: Bar 1: 8 lot, Bar 2: 4 lot, Bar 3: 7 lot)
                if (_mainTrader.pozisyonBuyuklugu.MicroLotSizeEnabled)
                    _mainTrader.pozisyonBuyuklugu.VarlikAdedSayisiMicro = varlikAdedSayisiFinal;
                else
                    _mainTrader.pozisyonBuyuklugu.VarlikAdedSayisi = varlikAdedSayisiFinal;
            }
            else
            {
                // SABİT POZİSYON BÜYÜKLÜĞÜ MODU (Varsayılan)
                // mainTrader'ın başlangıçta ayarlanan sabit pozisyon büyüklüğü kullanılır
                // varlikAdedSayisiFinal sadece bilgi amaçlı hesaplanır (net exposure göstergesi)
                // Hiçbir güncelleme yapılmaz, pozisyon büyüklüğü sabittir
            }

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
            // TODO : islem zaman filtresine göre burası güncellenecek....Unutma

            _mainTrader.emir_sonrasi_dongu_foksiyonlarini_calistir(i);

            _mainTrader.OnRun?.Invoke(_mainTrader, 1);


        }
        public void Finalize(bool saveStatisticsToFile = true)
        {
            CurrentIndex = 0;
            foreach (var trader in Traders)
            {
                trader.Finalize(false);
            }

            // TODO : Bu kısım kontrol edilecek...

            if (!IsInitialized)
                throw new InvalidOperationException("Trader not initialized");

            _mainTrader.OnFinal?.Invoke(_mainTrader, 0);

            _mainTrader.istatistikleri_hesapla();

            // Write MultipleTrader lists to file (both TXT and CSV formats)
            if (saveStatisticsToFile)
                WriteMultipleTraderListsToFiles();

            if (saveStatisticsToFile)
                _mainTrader.istatistikleri_dosyaya_yaz();

            _mainTrader.OnFinal?.Invoke(_mainTrader, 1);
        }

        #endregion

        #region MultipleTrader Lists Export

        /// <summary>
        /// Write MultipleTrader lists to both TXT and CSV files
        /// </summary>
        private void WriteMultipleTraderListsToFiles(bool saveListsToFileTxt = true, bool saveListsToFileCsv = true)
        {
            if (saveListsToFileTxt)
                WriteMultipleTraderListsToTxt();
            if (saveListsToFileCsv)
                WriteMultipleTraderListsToCsv();
        }

        /// <summary>
        /// Write MultipleTrader lists to TXT file (tabular format with fixed-width columns)
        /// </summary>
        private void WriteMultipleTraderListsToTxt()
        {
            if (_mainTrader == null || Data == null || Data.Count == 0)
                return;

            var logDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!System.IO.Directory.Exists(logDir))
                System.IO.Directory.CreateDirectory(logDir);

            var filePath = System.IO.Path.Combine(logDir, "MultipleTraderLists.txt");

            using (var writer = new System.IO.StreamWriter(filePath, append: false, System.Text.Encoding.UTF8))
            {
                // Title
                writer.WriteLine($"MULTIPLE TRADER BAR-BY-BAR DATA");
                writer.WriteLine($"Generated: {DateTime.Now:yyyy.MM.dd HH:mm:ss}");
                writer.WriteLine("".PadRight(300, '='));

                // Header
                WriteHeaderTxt(writer);

                // Data rows
                for (int i = 0; i < Data.Count; i++)
                {
                    WriteBarDataTxt(writer, i);
                }

                writer.WriteLine("".PadRight(300, '='));
            }

            Logger?.Log($"MultipleTraderLists.txt written to: {filePath}");
        }

        /// <summary>
        /// Write header row for TXT file
        /// </summary>
        private void WriteHeaderTxt(System.IO.StreamWriter writer)
        {
            var header = $"{"BarNo",7} | " +
                        $"{"Date",10} | " +
                        $"{"Time",8} | " +
                        $"{"Open",10} | " +
                        $"{"High",10} | " +
                        $"{"Low",10} | " +
                        $"{"Close",10} | " +
                        $"{"Volume",10}";

            // MainTrader columns
            header += $" | {"MainYon",7} | {"MainSvy",10} | {"MainSny",7}";

            // Other trader columns
            for (int t = 0; t < Traders.Count; t++)
            {
                header += $" | {$"T{t}Yon",7} | {$"T{t}Svy",10} | {$"T{t}Sny",7}";
            }

            writer.WriteLine(header);
        }

        /// <summary>
        /// Write bar data row for TXT file
        /// </summary>
        private void WriteBarDataTxt(System.IO.StreamWriter writer, int barIndex)
        {
            var bar = Data[barIndex];

            // Format: BarNo | Date | Time | OHLC | Volume
            var line = $"{barIndex,7} | " +
                      $"{bar.Date:yyyy.MM.dd} | " +
                      $"{bar.DateTime:HH:mm:ss} | " +
                      $"{bar.Open,10:F2} | " +
                      $"{bar.High,10:F2} | " +
                      $"{bar.Low,10:F2} | " +
                      $"{bar.Close,10:F2} | " +
                      $"{bar.Volume,10:F0}";

            // MainTrader data
            line += $" | {GetYon(_mainTrader, barIndex),7} | " +
                   $"{GetSeviye(_mainTrader, barIndex),10:F2} | " +
                   $"{GetSinyal(_mainTrader, barIndex),7:F2}";

            // Other traders data
            foreach (var trader in Traders)
            {
                line += $" | {GetYon(trader, barIndex),7} | " +
                       $"{GetSeviye(trader, barIndex),10:F2} | " +
                       $"{GetSinyal(trader, barIndex),7:F2}";
            }

            writer.WriteLine(line);
        }

        /// <summary>
        /// Write MultipleTrader lists to CSV file (semicolon separated)
        /// </summary>
        private void WriteMultipleTraderListsToCsv()
        {
            if (_mainTrader == null || Data == null || Data.Count == 0)
                return;

            var logDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!System.IO.Directory.Exists(logDir))
                System.IO.Directory.CreateDirectory(logDir);

            var filePath = System.IO.Path.Combine(logDir, "MultipleTraderLists.csv");

            using (var writer = new System.IO.StreamWriter(filePath, append: false, System.Text.Encoding.UTF8))
            {
                // Header
                WriteHeaderCsv(writer);

                // Data rows
                for (int i = 0; i < Data.Count; i++)
                {
                    WriteBarDataCsv(writer, i);
                }
            }

            Logger?.Log($"MultipleTraderLists.csv written to: {filePath}");
        }

        /// <summary>
        /// Write header row for CSV file
        /// </summary>
        private void WriteHeaderCsv(System.IO.StreamWriter writer)
        {
            var header = "BarNo;Date;Time;Open;High;Low;Close;Volume";

            // MainTrader columns
            header += ";MainTrader_Yon;MainTrader_Seviye;MainTrader_Sinyal";

            // Other trader columns
            for (int t = 0; t < Traders.Count; t++)
            {
                header += $";Trader{t}_Yon;Trader{t}_Seviye;Trader{t}_Sinyal";
            }

            writer.WriteLine(header);
        }

        /// <summary>
        /// Write bar data row for CSV file
        /// </summary>
        private void WriteBarDataCsv(System.IO.StreamWriter writer, int barIndex)
        {
            var bar = Data[barIndex];

            // Format: BarNo;Date;Time;OHLC;Volume
            var line = $"{barIndex};" +
                      $"{bar.Date:yyyy.MM.dd};" +
                      $"{bar.DateTime:HH:mm:ss};" +
                      $"{bar.Open:F2};" +
                      $"{bar.High:F2};" +
                      $"{bar.Low:F2};" +
                      $"{bar.Close:F2};" +
                      $"{bar.Volume:F0}";

            // MainTrader data
            line += $";{GetYon(_mainTrader, barIndex)};" +
                   $"{GetSeviye(_mainTrader, barIndex):F2};" +
                   $"{GetSinyal(_mainTrader, barIndex):F2}";

            // Other traders data
            foreach (var trader in Traders)
            {
                line += $";{GetYon(trader, barIndex)};" +
                       $"{GetSeviye(trader, barIndex):F2};" +
                       $"{GetSinyal(trader, barIndex):F2}";
            }

            writer.WriteLine(line);
        }

        /// <summary>
        /// Get Yon (direction) for a trader at a specific bar index
        /// </summary>
        private string GetYon(SingleTrader trader, int barIndex)
        {
            if (trader == null || trader.lists == null || trader.lists.YonList == null)
                return "";

            if (barIndex < 0 || barIndex >= trader.lists.YonList.Count)
                return "";

            return trader.lists.YonList[barIndex] ?? "";
        }

        /// <summary>
        /// Get Seviye (level/price) for a trader at a specific bar index
        /// </summary>
        private double GetSeviye(SingleTrader trader, int barIndex)
        {
            if (trader == null || trader.lists == null || trader.lists.SeviyeList == null)
                return 0.0;

            if (barIndex < 0 || barIndex >= trader.lists.SeviyeList.Count)
                return 0.0;

            return trader.lists.SeviyeList[barIndex];
        }

        /// <summary>
        /// Get Sinyal (signal) for a trader at a specific bar index
        /// </summary>
        private double GetSinyal(SingleTrader trader, int barIndex)
        {
            if (trader == null || trader.lists == null || trader.lists.SinyalList == null)
                return 0.0;

            if (barIndex < 0 || barIndex >= trader.lists.SinyalList.Count)
                return 0.0;

            return trader.lists.SinyalList[barIndex];
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
        /// Request stop for currently running MultipleTrader
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                IsStopRequested = true;
                Logger?.Log($"Stop requested for MultipleTrader (Id: {Id})");
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
