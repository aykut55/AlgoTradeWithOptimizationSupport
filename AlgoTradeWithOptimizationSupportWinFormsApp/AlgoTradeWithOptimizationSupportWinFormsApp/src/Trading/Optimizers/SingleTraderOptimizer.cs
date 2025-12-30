using AlgoTradeWithOptimizationSupportWinFormsApp.Definitions;
using AlgoTradeWithOptimizationSupportWinFormsApp.Indicators;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategies;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy;
using AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders;
using Serilog.Core;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Tulip;
using static AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics.Statistics;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Optimizers
{
    /// <summary>
    /// Strategy factory delegate - creates strategy instances with given parameters
    /// </summary>
    public delegate IStrategy StrategyFactory(List<StockData> data, IndicatorManager indicators, Dictionary<string, object> parameters);

    /// <summary>
    /// Parameter range for optimization
    /// </summary>
    public class ParameterRange
    {
        public string Name { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }

        public ParameterRange(string name, double min, double max, double step)
        {
            Name = name;
            Min = min;
            Max = max;
            Step = step;
        }

        public List<double> GetValues()
        {
            var values = new List<double>();
            for (double v = Min; v <= Max; v += Step)
            {
                values.Add(v);
            }
            return values;
        }
    }

    /// <summary>
    /// Optimization result
    /// </summary>
    public class OptimizationResult
    {
        // Parameters
        public Dictionary<string, object> Parameters { get; set; }

        // Trader Information
        public int TraderId { get; set; }
        public string TraderName { get; set; }

        // Symbol Information
        public string SymbolName { get; set; }
        public string SymbolPeriod { get; set; }

        // System Information
        public int SystemId { get; set; }
        public string SystemName { get; set; }

        // Strategy Information
        public int StrategyId { get; set; }
        public string StrategyName { get; set; }

        // Bar Information
        public int ToplamBarSayisi { get; set; }
        public DateTime IlkBarTarihSaati { get; set; }
        public DateTime IlkBarTarihi { get; set; }
        public TimeSpan IlkBarSaati { get; set; }
        public DateTime SonBarTarihSaati { get; set; }
        public DateTime SonBarTarihi { get; set; }
        public TimeSpan SonBarSaati { get; set; }

        // Balance & Return Metrics
        public double IlkBakiyeFiyat { get; set; }
        public double BakiyeFiyat { get; set; }
        public double GetiriFiyat { get; set; }
        public double GetiriFiyatYuzde { get; set; }
        public double KomisyonFiyat { get; set; }
        public double BakiyeFiyatNet { get; set; }
        public double GetiriFiyatNet { get; set; }
        public double GetiriFiyatYuzdeNet { get; set; }
        public double KomisyonFiyatYuzde { get; set; }

        // Balance Min/Max
        public double MinBakiyeFiyat { get; set; }
        public double MaxBakiyeFiyat { get; set; }
        public double MinBakiyeFiyatYuzde { get; set; }
        public double MaxBakiyeFiyatYuzde { get; set; }
        public double MinBakiyeFiyatNet { get; set; }
        public double MaxBakiyeFiyatNet { get; set; }

        // Trade Counts
        public int IslemSayisi { get; set; }
        public int AlisSayisi { get; set; }
        public int SatisSayisi { get; set; }
        public int FlatSayisi { get; set; }
        public int PassSayisi { get; set; }
        public int KarAlSayisi { get; set; }
        public int ZararKesSayisi { get; set; }
        public int KomisyonIslemSayisi { get; set; }

        // Win/Loss Counts
        public int KazandiranIslemSayisi { get; set; }
        public int KaybettirenIslemSayisi { get; set; }
        public int NotrIslemSayisi { get; set; }

        // Profit & Loss
        public double ToplamKarFiyat { get; set; }
        public double ToplamZararFiyat { get; set; }
        public double NetKarFiyat { get; set; }
        public double MaxKarFiyat { get; set; }
        public double MaxZararFiyat { get; set; }

        // Win Rate
        public double KarliIslemOrani { get; set; }

        // Drawdown & Risk Metrics
        public double GetiriMaxDD { get; set; }
        public DateTime GetiriMaxDDTarih { get; set; }
        public double GetiriMaxKayip { get; set; }
        public double ProfitFactor { get; set; }

        // Standard Performance Metrics
        public double NetProfit { get; set; }
        public double WinRate { get; set; }
        public double MaxDrawdown { get; set; }
        public double SharpeRatio { get; set; }

        // Additional Trade Statistics
        public int TotalTrades { get; set; }
        public int WinningTrades { get; set; }
        public int LosingTrades { get; set; }
        public double TotalProfit { get; set; }
        public double TotalLoss { get; set; }
        public double AverageWin { get; set; }
        public double AverageLoss { get; set; }
        public double MaxDrawdownPct { get; set; }

        public OptimizationResult()
        {
            Parameters = new Dictionary<string, object>();
            TraderName = string.Empty;
            SymbolName = string.Empty;
            SymbolPeriod = string.Empty;
            SystemName = string.Empty;
            StrategyName = string.Empty;
        }
    }

    /// <summary>
    /// Optimization result
    /// </summary>
    public class OptimizationResultEskisi
    {
        public Dictionary<string, object> Parameters { get; set; }
        public double NetProfit { get; set; }
        public double WinRate { get; set; }
        public double ProfitFactor { get; set; }
        public double MaxDrawdown { get; set; }
        public double SharpeRatio { get; set; }

        public OptimizationResultEskisi()
        {
            Parameters = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Single trader optimizer - finds best parameters for a strategy
    /// </summary>
    public class SingleTraderOptimizer
    {
        #region Properties

        public int Id { get; private set; }
        public List<StockData> Data { get; private set; }
        public IndicatorManager Indicators { get; private set; }
        public Type StrategyType { get; private set; }
        public StrategyFactory StrategyFactoryMethod { get; private set; }
        public List<ParameterRange> ParameterRanges { get; private set; }
        public List<OptimizationResult> Results { get; private set; }
        public bool IsInitialized { get; private set; }

        private IAlgoTraderLogger? Logger { get; set; }

        // Progress callbacks
        public Action<int, int>? OnOptimizationProgress { get; set; }  // (currentCombination, totalCombinations)
        public Action<int, int>? OnSingleTraderProgressCallback { get; set; }  // (currentBar, totalBars)

        // Skip iteration support for resuming optimization
        public bool SkipIterationEnabled { get; set; }
        public int SkipIteration { get; set; }

        // Max iterations support for chunked optimization
        public bool MaxIterationsEnabled { get; set; }
        public int MaxIterations { get; set; }  // Kaç kombinasyon çalıştır (effective - skip hariç)

        // Save intermediate results
        public int SaveEveryN { get; set; }  // Her kaç kombinasyonda bir ara sonuç kaydet (0 = disable)
        public Action<List<OptimizationResult>, int>? OnSaveResults { get; set; }  // (results, currentCombination)

        // Optimization log file settings
        public bool CsvFileLoggingEnabled { get; set; }
        public string CsvFilePath { get; set; }
        public bool TxtFileLoggingEnabled { get; set; }
        public string TxtFilePath { get; set; }
        public bool AppendEnabled { get; set; }

        #endregion

        #region Constructor

        public SingleTraderOptimizer()
        {
            ParameterRanges = new List<ParameterRange>();
            Results = new List<OptimizationResult>();
            IsInitialized = false;
        }

        public SingleTraderOptimizer(int id, List<StockData> data, IndicatorManager indicators, IAlgoTraderLogger? logger)
        {
            Id = id;
            Data = data;
            Indicators = indicators;
            Logger = logger;
            ParameterRanges = new List<ParameterRange>();
            Results = new List<OptimizationResult>();
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
            IsInitialized = true;
        }

        /// <summary>
        /// Set strategy type to optimize
        /// </summary>
        public void SetStrategy(Type strategyType)
        {
            if (!typeof(IStrategy).IsAssignableFrom(strategyType))
                throw new ArgumentException("Strategy type must implement IStrategy");

            StrategyType = strategyType;
        }

        /// <summary>
        /// Add parameter range for optimization
        /// </summary>
        public void AddParameterRange(string name, double min, double max, double step)
        {
            ParameterRanges.Add(new ParameterRange(name, min, max, step));
        }

        /// <summary>
        /// Set strategy factory method for creating strategy instances with parameters
        /// </summary>
        public void SetStrategyFactory(StrategyFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            StrategyFactoryMethod = factory;
        }

        /// <summary>
        /// Set skip iteration settings for resuming optimization
        /// Uzun optimizasyonları parça parça yapmak için kullanılır
        /// </summary>
        /// <param name="enabled">Skip özelliği aktif mi?</param>
        /// <param name="skipCount">İlk kaç kombinasyon atlanacak?</param>
        public void SetSkipIterationSettings(bool enabled, int skipCount)
        {
            SkipIterationEnabled = enabled;
            SkipIteration = skipCount;

            if (enabled && skipCount > 0)
            {
                Logger?.Log($"Skip iteration settings: Enabled=true, SkipCount={skipCount}");
            }
            else
            {
                Logger?.Log($"Skip iteration settings: Disabled");
            }
        }

        /// <summary>
        /// Disable skip iteration (baştan başla)
        /// </summary>
        public void DisableSkipIteration()
        {
            SetSkipIterationSettings(false, 0);
        }

        /// <summary>
        /// Set max iterations settings for chunked optimization
        /// Optimizasyonu parçalara bölmek için kullanılır
        /// </summary>
        /// <param name="enabled">Max iterations özelliği aktif mi?</param>
        /// <param name="maxCount">Kaç kombinasyon çalıştırılacak? (effective - skip hariç)</param>
        public void SetMaxIterationsSettings(bool enabled, int maxCount)
        {
            MaxIterationsEnabled = enabled;
            MaxIterations = maxCount;

            if (enabled && maxCount > 0)
            {
                Logger?.Log($"Max iterations settings: Enabled=true, MaxCount={maxCount}");
            }
            else
            {
                Logger?.Log($"Max iterations settings: Disabled (run to completion)");
            }
        }

        /// <summary>
        /// Disable max iterations (sonuna kadar çalıştır)
        /// </summary>
        public void DisableMaxIterations()
        {
            SetMaxIterationsSettings(false, 0);
        }

        /// <summary>
        /// Set intermediate save settings
        /// Ara sonuçları kaydetmek için kullanılır
        /// </summary>
        /// <param name="saveEveryN">Her kaç kombinasyonda bir kaydet (0 = disable)</param>
        public void SetIntermediateSaveSettings(int saveEveryN)
        {
            SaveEveryN = saveEveryN;

            if (saveEveryN > 0)
            {
                Logger?.Log($"Intermediate save settings: SaveEveryN={saveEveryN}");
            }
            else
            {
                Logger?.Log($"Intermediate save settings: Disabled");
            }
        }

        /// <summary>
        /// Set optimization log file parameters
        /// Optimizasyon sonuçlarını CSV ve TXT dosyalarına kaydetmek için kullanılır
        /// </summary>
        /// <param name="csvFileLoggingEnabled">CSV dosyasına kayıt yapılsın mı?</param>
        /// <param name="csvFilePath">CSV dosya yolu</param>
        /// <param name="txtFileLoggingEnabled">TXT dosyasına kayıt yapılsın mı?</param>
        /// <param name="txtFilePath">TXT dosya yolu</param>
        /// <param name="appendEnabled">Append modda mı yoksa yeniden oluştur mu?</param>
        public void SetOptimizationLogFileParams(
            bool csvFileLoggingEnabled, string csvFilePath,
            bool txtFileLoggingEnabled, string txtFilePath,
            bool appendEnabled)
        {
            CsvFileLoggingEnabled = csvFileLoggingEnabled;
            CsvFilePath = csvFilePath;
            TxtFileLoggingEnabled = txtFileLoggingEnabled;
            TxtFilePath = txtFilePath;
            AppendEnabled = appendEnabled;

            Logger?.Log($"Optimization log file settings:");
            Logger?.Log($"  - CSV: Enabled={csvFileLoggingEnabled}, Path={csvFilePath}");
            Logger?.Log($"  - TXT: Enabled={txtFileLoggingEnabled}, Path={txtFilePath}");
            Logger?.Log($"  - Append: {appendEnabled}");
        }

        #endregion

        #region Optimization Methods

        public void Reset()
        {
        }
        public void Init()
        {
        }

        /// <summary>
        /// Run optimization
        /// </summary>
        public OptimizationResult Run()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Optimizer not initialized");

            int totalBars = Data.Count;

            // Indicators zaten constructor'dan geldi
            var indicators = this.Indicators ?? new IndicatorManager(this.Data);

            // ============================================================
            // STRATEGY CONFIGURATION - Change this section for different strategies
            // ============================================================

            StrategyFactoryMethod = null;

            // Clear previous settings
            ParameterRanges.Clear();
            this.AddParameterRange("period", 10, 50, 5);

            this.SetStrategyFactory((data, indicators, parameters) =>
            {
                int period = Convert.ToInt32(parameters["period"]);
                return new SimpleMostStrategy(data, indicators, period, percent: 1.0);
            });
            // Kombinasyon sayısı: 9 (10, 15, 20, 25, 30, 35, 40, 45, 50)


            // Clear previous settings
            ParameterRanges.Clear();

            // Setup parameters and strategy
            this.AddParameterRange("fastPeriod", 5, 20, 5);
            this.AddParameterRange("slowPeriod", 20, 100, 10);

            this.SetStrategyFactory((data, indicators, parameters) =>
            {
                int fast = Convert.ToInt32(parameters["fastPeriod"]);
                int slow = Convert.ToInt32(parameters["slowPeriod"]);
                return new SimpleMAStrategy(data, indicators, fast, slow);
            });
            // Kombinasyon sayısı: 4 × 9 = 36
            // (5,20), (5,30), ..., (20,100)


            // Clear previous settings
            ParameterRanges.Clear();

            // Setup parameters and strategy
            this.AddParameterRange("period", 10, 30, 10);      // 3 değer: 10, 20, 30
            this.AddParameterRange("percent", 0.5, 2.0, 0.5);  // 4 değer: 0.5, 1.0, 1.5, 2.0
            //this.AddParameterRange("multiplier", 1.0, 3.0, 1.0); // 3 değer: 1.0, 2.0, 3.0

            this.SetStrategyFactory((data, indicators, parameters) =>
            {
                int period = Convert.ToInt32(parameters["period"]);
                double percent = Convert.ToDouble(parameters["percent"]);
                return new SimpleMostStrategy(data, indicators, period, percent);
            });
            // Kombinasyon sayısı: 3 × 4 × 3 = 36

            // ============================================================
            // END STRATEGY CONFIGURATION
            // ============================================================

            var singleTrader = new SingleTrader(0, "singleTrader", this.Data, indicators, Logger);

            // Assign callbacks
            singleTrader.SetCallbacks(OnSingleTraderReset, OnSingleTraderInit, OnSingleTraderRun, OnSingleTraderFinal, OnSingleTraderBeforeOrder, OnSingleTraderNotifySignal, OnSingleTraderAfterOrder, OnSingleTraderProgressInternal, OnApplyUserFlags);

            // Setup (order is important)
            singleTrader.CreateModules();

            // Validate StrategyFactory is set
            if (StrategyFactoryMethod == null)
                throw new InvalidOperationException("StrategyFactory must be set before running optimization. Use SetStrategyFactory().");

            Results.Clear();

            // Generate all parameter combinations (generic)
            var allCombinations = GenerateParameterCombinations();
            int totalCombinations = allCombinations.Count;
            int currentCombination = 0;

            Logger?.Log($"Starting optimization: {totalCombinations} combinations to test");
            if (SkipIterationEnabled && SkipIteration > 0)
            {
                Logger?.Log($"Skip iteration enabled: Skipping first {SkipIteration} combinations");
            }
            if (MaxIterationsEnabled && MaxIterations > 0)
            {
                Logger?.Log($"Max iterations enabled: Will run {MaxIterations} combinations (effective)");
                int estimatedEnd = SkipIteration + MaxIterations;
                Logger?.Log($"Estimated range: {SkipIteration + 1} to {estimatedEnd}");
            }
            if (SaveEveryN > 0)
            {
                Logger?.Log($"Intermediate save enabled: Saving every {SaveEveryN} combinations");
            }
            Logger?.Log($"Parameter ranges:");
            foreach (var range in ParameterRanges)
            {
                Logger?.Log($"  - {range.Name}: {range.Min} to {range.Max} (step: {range.Step})");
            }

            // Effective combination count (skip sonrası çalıştırılan)
            int effectiveCombinationCount = 0;

            // Test all combinations (generic - no more nested loops!)
            foreach (var paramCombo in allCombinations)
            {
                currentCombination++;

                // Calculate progress percentage
                double progressPercent = (currentCombination / (double)totalCombinations) * 100.0;

                // Report optimization progress (her zaman raporla, atlanmış iterasyonlar için de)
                OnOptimizationProgress?.Invoke(currentCombination, totalCombinations);

                // Build parameter string for logging (generic)
                string paramsStr = string.Join(", ", paramCombo.Select(kvp => $"{kvp.Key}={kvp.Value}"));

                // Skip iteration check
                if (SkipIterationEnabled)
                {
                    if (currentCombination <= SkipIteration)
                    {
                        Logger?.Log($"Skipping combination {currentCombination}/{totalCombinations} ({progressPercent:F1}%): {paramsStr}");
                        continue;
                    }
                }

                // Effective combination count (skip sonrası çalıştırılan)
                effectiveCombinationCount++;

                // Max iterations check
                if (MaxIterationsEnabled && MaxIterations > 0)
                {
                    if (effectiveCombinationCount > MaxIterations)
                    {
                        Logger?.Log($"Max iterations reached ({MaxIterations}). Stopping at combination {currentCombination}/{totalCombinations}");
                        break;  // Exit loop
                    }
                }

                Logger?.Log($"    Testing combination {currentCombination}/{totalCombinations} ({progressPercent:F1}%) [Effective: {effectiveCombinationCount}]: {paramsStr}");

                // Create strategy instance using factory (generic!)
                var strategy = StrategyFactoryMethod(this.Data, indicators, paramCombo);
                strategy.OnInit();
                singleTrader.SetStrategy(strategy);

                // Reset
                singleTrader.Reset();

                // Configure position sizing
                singleTrader.pozisyonBuyuklugu.Reset()
                    .SetBakiyeParams(ilkBakiye: 100000.0)
                    .SetKontratParamsViopEndex(kontratSayisi: 1)
                    .SetKomisyonParams(komisyonCarpan: 3.0)
                    .SetKaymaParams(kaymaMiktari: 0.5);

                singleTrader.Init();

                // Initialize
                singleTrader.Initialize();

                // Run SingleTrader
                for (int i = 0; i < totalBars; i++)
                {
                    if (i % 1000 == 0)
                        OnSingleTraderProgressCallback?.Invoke(i, totalBars);
                    singleTrader.Run(i);
                }
                OnSingleTraderProgressCallback?.Invoke(totalBars, totalBars);

                // Collect statistics
                singleTrader.Finalize(false);

                // Get optimization summary
                var optSummary = singleTrader.statistics.GetOptimizationSummary();

                // Create result from optimization summary
                var optResult = CreateOptimizationResultFromSummary(optSummary, paramCombo);

                Results.Add(optResult);

                Logger?.Log($"  → NetProfit: {optResult.NetProfit:F2}, WinRate: {optResult.WinRate:F2}%, PF: {optResult.ProfitFactor:F2}");

                // Append to CSV and TXT files (if enabled)
                //AppendSingleResultToFiles(result, currentCombination);

                // Append to CSV and TXT files (if enabled)
                AppendSingleOptSummaryToFiles(optResult, optSummary, currentCombination);

                // strategy.Dispose();
                strategy = null;

                // Intermediate save check
                if (SaveEveryN > 0 && effectiveCombinationCount % SaveEveryN == 0)
                {
                    Logger?.Log($"Saving intermediate results at combination {currentCombination} (effective: {effectiveCombinationCount})...");
                    OnSaveResults?.Invoke(Results, currentCombination);
                }
            }

            singleTrader.Dispose();
            singleTrader = null;

            Logger?.Log("");
            Logger?.Log($"Optimization completed! Tested {effectiveCombinationCount} combinations (Total: {currentCombination}/{totalCombinations})");




            OptimizationResult bestResult = GetBestResult();

            bool writeBestResultToFileEnabled = false;
            if (writeBestResultToFileEnabled)
            {
                if (bestResult != null)
                {
                    Logger?.Log("");
                    Logger?.Log("=== BEST RESULT ===");

                    // Log all parameters (generic)
                    foreach (var kvp in bestResult.Parameters)
                    {
                        Logger?.Log($"{kvp.Key}: {kvp.Value}");
                    }

                    Logger?.Log($"NetProfit: {bestResult.NetProfit:F2}");
                    Logger?.Log($"WinRate: {bestResult.WinRate:F2}%");
                    Logger?.Log($"ProfitFactor: {bestResult.ProfitFactor:F2}");
                    Logger?.Log($"MaxDrawdown: {bestResult.MaxDrawdown:F2}");
                }

                // Save results to files (if enabled)
                SaveOptimizationResultsToFiles();
            }

            return bestResult;
        }

        /// <summary>
        /// Get best optimization result
        /// </summary>
        public OptimizationResult GetBestResult()
        {
            if (Results.Count == 0)
                return null;

            // TODO: Sort by fitness function (e.g., net profit, sharpe ratio)
            return Results.OrderByDescending(r => r.NetProfit).FirstOrDefault();
        }

        /// <summary>
        /// Get trader with best parameters
        /// </summary>
        public SingleTrader GetBestTrader()
        {
            var bestResult = GetBestResult();
            if (bestResult == null)
                return null;

            // TODO: Create trader with best parameters
            return null;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Save optimization results to CSV and/or TXT files
        /// </summary>
        private void SaveOptimizationResultsToFiles()
        {
            if (!CsvFileLoggingEnabled && !TxtFileLoggingEnabled)
                return;

            if (Results == null || Results.Count == 0)
            {
                Logger?.Log("No results to save to files.");
                return;
            }

            // Save to CSV
            if (CsvFileLoggingEnabled && !string.IsNullOrEmpty(CsvFilePath))
            {
                try
                {
                    SaveResultsToCsv(CsvFilePath, AppendEnabled);
                    Logger?.Log($"Optimization results saved to CSV: {CsvFilePath}");
                }
                catch (Exception ex)
                {
                    Logger?.Log($"Error saving to CSV: {ex.Message}");
                }
            }

            // Save to TXT
            if (TxtFileLoggingEnabled && !string.IsNullOrEmpty(TxtFilePath))
            {
                try
                {
                    SaveResultsToTxt(TxtFilePath, AppendEnabled);
                    Logger?.Log($"Optimization results saved to TXT: {TxtFilePath}");
                }
                catch (Exception ex)
                {
                    Logger?.Log($"Error saving to TXT: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Append single optimization result to CSV and TXT files (called after each combination)
        /// </summary>
        private void AppendSingleResultToFiles(OptimizationResult result, int currentCombination)
        {
            if (!CsvFileLoggingEnabled && !TxtFileLoggingEnabled)
                return;

            // Append to CSV
            if (CsvFileLoggingEnabled && !string.IsNullOrEmpty(CsvFilePath))
            {
                try
                {
                    AppendSingleResultToCsv(result, CsvFilePath, currentCombination);
                }
                catch (Exception ex)
                {
                    Logger?.Log($"Error appending to CSV: {ex.Message}");
                }
            }

            // Append to TXT
            if (TxtFileLoggingEnabled && !string.IsNullOrEmpty(TxtFilePath))
            {
                try
                {
                    AppendSingleResultToTxt(result, TxtFilePath, currentCombination);
                }
                catch (Exception ex)
                {
                    Logger?.Log($"Error appending to TXT: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Append single result to CSV file
        /// </summary>
        private void AppendSingleResultToCsv(OptimizationResult result, string filePath, int currentCombination)
        {
            bool fileExists = System.IO.File.Exists(filePath);
            bool writeHeader = false;

            // If file doesn't exist OR (not appending and first combination), write header
            if (!fileExists || (!AppendEnabled && currentCombination == 1))
            {
                writeHeader = true;
            }

            using (var fs = new System.IO.FileStream(
                filePath,
                (AppendEnabled && fileExists) ? System.IO.FileMode.Append : System.IO.FileMode.Create,
                System.IO.FileAccess.Write,
                System.IO.FileShare.ReadWrite))
            using (var sw = new System.IO.StreamWriter(fs))
            {
                // Write header if needed
                if (writeHeader)
                {
                    var paramNames = result.Parameters.Keys.ToList();
                    var header = string.Join(",", paramNames) + ",NetProfit,WinRate,ProfitFactor,MaxDrawdown,SharpeRatio";
                    sw.WriteLine(header);
                }

                // Write data
                var paramValues = result.Parameters.Values.Select(v => v.ToString()).ToList();
                var metrics = new[]
                {
                    result.NetProfit.ToString("F2"),
                    result.WinRate.ToString("F2"),
                    result.ProfitFactor.ToString("F2"),
                    result.MaxDrawdown.ToString("F2"),
                    result.SharpeRatio.ToString("F2")
                };
                var line = string.Join(",", paramValues.Concat(metrics));
                sw.WriteLine(line);
                sw.Flush();
            }
        }

        /// <summary>
        /// Append single result to TXT file
        /// </summary>
        private void AppendSingleResultToTxt(OptimizationResult result, string filePath, int currentCombination)
        {
            bool fileExists = System.IO.File.Exists(filePath);
            bool writeHeader = false;

            // If file doesn't exist OR (not appending and first combination), write header
            if (!fileExists || (!AppendEnabled && currentCombination == 1))
            {
                writeHeader = true;
            }

            using (var fs = new System.IO.FileStream(
                filePath,
                (AppendEnabled && fileExists) ? System.IO.FileMode.Append : System.IO.FileMode.Create,
                System.IO.FileAccess.Write,
                System.IO.FileShare.ReadWrite))
            using (var sw = new System.IO.StreamWriter(fs))
            {
                // Write header if needed
                if (writeHeader)
                {
                    sw.WriteLine($"Optimization Results - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sw.WriteLine("================================================================================");
                    sw.WriteLine();
                }

                // Write result
                sw.WriteLine($"Combination #{currentCombination}:");

                // Parameters
                foreach (var kvp in result.Parameters)
                {
                    sw.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }

                // Metrics
                sw.WriteLine($"  NetProfit: {result.NetProfit:F2}");
                sw.WriteLine($"  WinRate: {result.WinRate:F2}%");
                sw.WriteLine($"  ProfitFactor: {result.ProfitFactor:F2}");
                sw.WriteLine($"  MaxDrawdown: {result.MaxDrawdown:F2}");
                sw.WriteLine($"  SharpeRatio: {result.SharpeRatio:F2}");
                sw.WriteLine();

                sw.Flush();
            }
        }

        /// <summary>
        /// Append single OptimizationSummary to CSV and TXT files (called after each combination)
        /// </summary>
        private void AppendSingleOptSummaryToFiles(
            OptimizationResult result,
            AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics.Statistics.OptimizationSummary optSummary,
            int currentCombination)
        {
            if (!CsvFileLoggingEnabled && !TxtFileLoggingEnabled)
                return;

            // Append to CSV
            if (CsvFileLoggingEnabled && !string.IsNullOrEmpty(CsvFilePath))
            {
                try
                {
                    AppendSingleOptSummaryToCsv(result, optSummary, CsvFilePath, currentCombination);
                }
                catch (Exception ex)
                {
                    Logger?.Log($"Error appending OptSummary to CSV: {ex.Message}");
                }
            }

            // Append to TXT
            if (TxtFileLoggingEnabled && !string.IsNullOrEmpty(TxtFilePath))
            {
                try
                {
                    AppendSingleOptSummaryToTxt(result, optSummary, TxtFilePath, currentCombination);
                }
                catch (Exception ex)
                {
                    Logger?.Log($"Error appending OptSummary to TXT: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Append single OptimizationSummary to CSV file
        /// </summary>
        private void AppendSingleOptSummaryToCsv(
            OptimizationResult result,
            AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics.Statistics.OptimizationSummary optSummary,
            string filePath,
            int currentCombination)
        {
            bool fileExists = System.IO.File.Exists(filePath);
            bool writeHeader = false;

            // If file doesn't exist OR (not appending and first combination), write header
            if (!fileExists || (!AppendEnabled && currentCombination == 1))
            {
                writeHeader = true;
            }

            using (var fs = new System.IO.FileStream(
                filePath,
                (AppendEnabled && fileExists) ? System.IO.FileMode.Append : System.IO.FileMode.Create,
                System.IO.FileAccess.Write,
                System.IO.FileShare.ReadWrite))
            using (var sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                // Write header if needed
                if (writeHeader)
                {
                    // Build header: CombNo + Parameters + OptimizationSummary fields
                    var headerParts = new List<string> { "CombNo" };

                    // Add parameter names
                    if (result.Parameters != null && result.Parameters.Count > 0)
                    {
                        headerParts.AddRange(result.Parameters.Keys);
                    }

                    // Add OptimizationSummary header
                    headerParts.Add(AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics.Statistics.OptimizationSummary.GetCsvHeader());

                    sw.WriteLine(string.Join(";", headerParts));
                }

                // Write data row: CombNo + Parameter values + OptimizationSummary data
                var dataParts = new List<string> { currentCombination.ToString() };

                // Add parameter values
                if (result.Parameters != null && result.Parameters.Count > 0)
                {
                    dataParts.AddRange(result.Parameters.Values.Select(v => v.ToString()));
                }

                // Add OptimizationSummary data
                dataParts.Add(optSummary.ToCsvRow());

                sw.WriteLine(string.Join(";", dataParts));
                sw.Flush();
            }
        }

        /// <summary>
        /// Append single OptimizationSummary to TXT file (Tabular format like SaveListsToTxt)
        /// </summary>
        private void AppendSingleOptSummaryToTxt(
            OptimizationResult result,
            AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics.Statistics.OptimizationSummary optSummary,
            string filePath,
            int currentCombination)
        {
            bool fileExists = System.IO.File.Exists(filePath);
            bool writeHeader = false;

            // If file doesn't exist OR (not appending and first combination), write header
            if (!fileExists || (!AppendEnabled && currentCombination == 1))
            {
                writeHeader = true;
            }

            using (var fs = new System.IO.FileStream(
                filePath,
                (AppendEnabled && fileExists) ? System.IO.FileMode.Append : System.IO.FileMode.Create,
                System.IO.FileAccess.Write,
                System.IO.FileShare.ReadWrite))
            using (var sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                // Write header if needed
                if (writeHeader)
                {
                    sw.WriteLine($"OPTIMIZATION RESULTS - {DateTime.Now:yyyy.MM.dd HH:mm:ss}");
                    sw.WriteLine("".PadRight(3000, '='));

                    // Build header with dynamic parameters
                    var headerBuilder = new System.Text.StringBuilder();
                    headerBuilder.Append($"{"CombNo",7} | ");

                    // Add parameter columns dynamically
                    if (result.Parameters != null && result.Parameters.Count > 0)
                    {
                        foreach (var paramName in result.Parameters.Keys)
                        {
                            headerBuilder.Append($"{paramName,12} | ");
                        }
                    }

                    // Add rest of the fields
                    headerBuilder.Append(
                        $"{"TraderId",8} | " +
                        $"{"TraderName",20} | " +
                        $"{"SymbolName",10} | " +
                        $"{"SymbolPer",9} | " +
                        $"{"SystemId",10} | " +
                        $"{"SystemName",20} | " +
                        $"{"StrategyId",10} | " +
                        $"{"StrategyName",25} | " +
                        $"{"LastExecId",15} | " +
                        $"{"LastExecTime",20} | " +
                        $"{"ExecStart",20} | " +
                        $"{"ExecStop",20} | " +
                        $"{"ExecMs",10} | " +
                        $"{"ResetTime",20} | " +
                        $"{"StatsTime",20} | " +
                        $"{"BarCnt",7} | " +
                        $"{"SelBarNo",8} | " +
                        $"{"SelBarDT",19} | " +
                        $"{"SelBarD",10} | " +
                        $"{"SelBarT",8} | " +
                        $"{"IlkBarDT",19} | " +
                        $"{"IlkBarD",10} | " +
                        $"{"IlkBarT",8} | " +
                        $"{"SonBarDT",19} | " +
                        $"{"SonBarD",10} | " +
                        $"{"SonBarT",8} | " +
                        $"{"IlkBarIdx",9} | " +
                        $"{"SonBarIdx",9} | " +
                        $"{"SonBarOp",10} | " +
                        $"{"SonBarHi",10} | " +
                        $"{"SonBarLo",10} | " +
                        $"{"SonBarCl",10} | " +
                        $"{"Months",7} | " +
                        $"{"Days",7} | " +
                        $"{"Hours",7} | " +
                        $"{"Mins",7} | " +
                        $"{"AvgMoTrd",9} | " +
                        $"{"AvgWeekTr",10} | " +
                        $"{"AvgDayTr",9} | " +
                        $"{"AvgHrTrd",9} | " +
                        $"{"IlkBakFyt",11} | " +
                        $"{"IlkBakPua",11} | " +
                        $"{"BakFiyat",11} | " +
                        $"{"BakPuan",11} | " +
                        $"{"GetFiyat",11} | " +
                        $"{"GetPuan",11} | " +
                        $"{"GetFyt%",9} | " +
                        $"{"GetPua%",9} | " +
                        $"{"BakFytNet",11} | " +
                        $"{"BakPuaNet",11} | " +
                        $"{"GetFytNet",11} | " +
                        $"{"GetPuaNet",11} | " +
                        $"{"GetFyt%N",9} | " +
                        $"{"GetPua%N",9} | " +
                        $"{"GetKz",11} | " +
                        $"{"GetKzNet",11} | " +
                        $"{"GetKzSis",11} | " +
                        $"{"GetKzSis%",10} | " +
                        $"{"GetKzNetS",11} | " +
                        $"{"GetKzNtS%",11} | " +
                        $"{"MinBakFyt",11} | " +
                        $"{"MaxBakFyt",11} | " +
                        $"{"MinBakPua",11} | " +
                        $"{"MaxBakPua",11} | " +
                        $"{"MinBakF%",10} | " +
                        $"{"MaxBakF%",10} | " +
                        $"{"MinBakIdx",9} | " +
                        $"{"MaxBakIdx",9} | " +
                        $"{"MinBakNet",11} | " +
                        $"{"MaxBakNet",11} | " +
                        $"{"Islem",6} | " +
                        $"{"Alis",6} | " +
                        $"{"Satis",6} | " +
                        $"{"Flat",6} | " +
                        $"{"Pass",6} | " +
                        $"{"KarAl",6} | " +
                        $"{"ZararKes",8} | " +
                        $"{"Kazand",7} | " +
                        $"{"Kaybett",8} | " +
                        $"{"Notr",6} | " +
                        $"{"KazAlis",8} | " +
                        $"{"KayAlis",8} | " +
                        $"{"NotAlis",8} | " +
                        $"{"KazSatis",9} | " +
                        $"{"KaySatis",9} | " +
                        $"{"NotSatis",9} | " +
                        $"{"AlKomut",8} | " +
                        $"{"SatKomut",9} | " +
                        $"{"PasKomut",9} | " +
                        $"{"KarAlKom",9} | " +
                        $"{"ZarKesKom",10} | " +
                        $"{"FlatKom",8} | " +
                        $"{"KomIslem",9} | " +
                        $"{"KomVarAd",10} | " +
                        $"{"KomVarMic",10} | " +
                        $"{"KomCarpa",10} | " +
                        $"{"KomFiyat",10} | " +
                        $"{"KomFyt%",9} | " +
                        $"{"KomDahil",9} | " +
                        $"{"KZFiyat",10} | " +
                        $"{"KZFiyat%",10} | " +
                        $"{"KZPuan",10} | " +
                        $"{"TopKarFyt",11} | " +
                        $"{"TopZarFyt",11} | " +
                        $"{"NetKarFyt",11} | " +
                        $"{"TopKarPua",11} | " +
                        $"{"TopZarPua",11} | " +
                        $"{"NetKarPua",11} | " +
                        $"{"MaxKarFyt",11} | " +
                        $"{"MaxZarFyt",11} | " +
                        $"{"MaxKarPua",11} | " +
                        $"{"MaxZarPua",11} | " +
                        $"{"KarBar",7} | " +
                        $"{"ZarBar",7} | " +
                        $"{"KarliOran",10} | " +
                        $"{"MaxDD",10} | " +
                        $"{"MaxDDDate",19} | " +
                        $"{"MaxKayip",10} | " +
                        $"{"ProfitFac",10} | " +
                        $"{"ProfFacSis",11} | " +
                        $"{"Sinyal",8} | " +
                        $"{"SonYon",6} | " +
                        $"{"PrevYon",7} | " +
                        $"{"SonFyt",10} | " +
                        $"{"SonAFyt",10} | " +
                        $"{"SonSFyt",10} | " +
                        $"{"SonFFyt",10} | " +
                        $"{"SonPFyt",10} | " +
                        $"{"PrevFyt",10} | " +
                        $"{"SonBarNo",9} | " +
                        $"{"SonABarNo",10} | " +
                        $"{"SonSBarNo",10} | " +
                        $"{"EmirKmt",8} | " +
                        $"{"EmirSts",8} | " +
                        $"{"Hisse",10} | " +
                        $"{"Kontrat",10} | " +
                        $"{"VarCarpa",10} | " +
                        $"{"VarAded",10} | " +
                        $"{"VarAdMic",10} | " +
                        $"{"Kayma",10} | " +
                        $"{"KaymaDah",9} | " +
                        $"{"MicroEna",9} | " +
                        $"{"PyramEna",9} | " +
                        $"{"MaxPosEna",10} | " +
                        $"{"MaxPos",10} | " +
                        $"{"MaxPosMic",10} | " +
                        $"{"GetFBuAy",10} | " +
                        $"{"GetFAy1",10} | " +
                        $"{"GetFBuHft",10} | " +
                        $"{"GetFHft1",10} | " +
                        $"{"GetFBuGun",10} | " +
                        $"{"GetFGun1",10} | " +
                        $"{"GetFBuSat",10} | " +
                        $"{"GetFSat1",10} | " +
                        $"{"GetPBuAy",10} | " +
                        $"{"GetPAy1",10} | " +
                        $"{"GetPBuHft",10} | " +
                        $"{"GetPHft1",10} | " +
                        $"{"GetPBuGun",10} | " +
                        $"{"GetPGun1",10} | " +
                        $"{"GetPBuSat",10} | " +
                        $"{"GetPSat1",10}"
                    );

                    sw.WriteLine(headerBuilder.ToString());
                    sw.WriteLine("".PadRight(3000, '-'));
                }

                // Build data row with dynamic parameters
                var dataBuilder = new System.Text.StringBuilder();
                dataBuilder.Append($"{currentCombination,7} | ");

                // Add parameter values dynamically
                if (result.Parameters != null && result.Parameters.Count > 0)
                {
                    foreach (var paramValue in result.Parameters.Values)
                    {
                        dataBuilder.Append($"{paramValue,12} | ");
                    }
                }

                // Add rest of the data
                dataBuilder.Append(
                    $"{optSummary.TraderId,8} | " +
                    $"{optSummary.TraderName,20} | " +
                    $"{optSummary.SymbolName,10} | " +
                    $"{optSummary.SymbolPeriod,9} | " +
                    $"{optSummary.SystemId,10} | " +
                    $"{optSummary.SystemName,20} | " +
                    $"{optSummary.StrategyId,10} | " +
                    $"{optSummary.StrategyName,25} | " +
                    $"{optSummary.LastExecutionId,15} | " +
                    $"{optSummary.LastExecutionTime,20} | " +
                    $"{optSummary.LastExecutionTimeStart,20} | " +
                    $"{optSummary.LastExecutionTimeStop,20} | " +
                    $"{optSummary.LastExecutionTimeInMSec,10} | " +
                    $"{optSummary.LastResetTime,20} | " +
                    $"{optSummary.LastStatisticsCalculationTime,20} | " +
                    $"{optSummary.ToplamBarSayisi,7} | " +
                    $"{optSummary.SecilenBarNumarasi,8} | " +
                    $"{optSummary.SecilenBarTarihSaati,19} | " +
                    $"{optSummary.SecilenBarTarihi,10} | " +
                    $"{optSummary.SecilenBarSaati,8} | " +
                    $"{optSummary.IlkBarTarihSaati,19} | " +
                    $"{optSummary.IlkBarTarihi,10} | " +
                    $"{optSummary.IlkBarSaati,8} | " +
                    $"{optSummary.SonBarTarihSaati,19} | " +
                    $"{optSummary.SonBarTarihi,10} | " +
                    $"{optSummary.SonBarSaati,8} | " +
                    $"{optSummary.IlkBarIndex,9} | " +
                    $"{optSummary.SonBarIndex,9} | " +
                    $"{optSummary.SonBarAcilisFiyati,10:F4} | " +
                    $"{optSummary.SonBarYuksekFiyati,10:F4} | " +
                    $"{optSummary.SonBarDusukFiyati,10:F4} | " +
                    $"{optSummary.SonBarKapanisFiyati,10:F4} | " +
                    $"{optSummary.ToplamGecenSureAy,7:F1} | " +
                    $"{optSummary.ToplamGecenSureGun,7} | " +
                    $"{optSummary.ToplamGecenSureSaat,7} | " +
                    $"{optSummary.ToplamGecenSureDakika,7} | " +
                    $"{optSummary.OrtAylikIslemSayisi,9:F2} | " +
                    $"{optSummary.OrtHaftalikIslemSayisi,10:F2} | " +
                    $"{optSummary.OrtGunlukIslemSayisi,9:F2} | " +
                    $"{optSummary.OrtSaatlikIslemSayisi,9:F2} | " +
                    $"{optSummary.IlkBakiyeFiyat,11:F2} | " +
                    $"{optSummary.IlkBakiyePuan,11:F2} | " +
                    $"{optSummary.BakiyeFiyat,11:F2} | " +
                    $"{optSummary.BakiyePuan,11:F2} | " +
                    $"{optSummary.GetiriFiyat,11:F2} | " +
                    $"{optSummary.GetiriPuan,11:F4} | " +
                    $"{optSummary.GetiriFiyatYuzde,9:F2} | " +
                    $"{optSummary.GetiriPuanYuzde,9:F2} | " +
                    $"{optSummary.BakiyeFiyatNet,11:F2} | " +
                    $"{optSummary.BakiyePuanNet,11:F2} | " +
                    $"{optSummary.GetiriFiyatNet,11:F2} | " +
                    $"{optSummary.GetiriPuanNet,11:F4} | " +
                    $"{optSummary.GetiriFiyatYuzdeNet,9:F2} | " +
                    $"{optSummary.GetiriPuanYuzdeNet,9:F2} | " +
                    $"{optSummary.GetiriKz,11:F4} | " +
                    $"{optSummary.GetiriKzNet,11:F4} | " +
                    $"{optSummary.GetiriKzSistem,11:F4} | " +
                    $"{optSummary.GetiriKzSistemYuzde,10:F2} | " +
                    $"{optSummary.GetiriKzNetSistem,11:F4} | " +
                    $"{optSummary.GetiriKzNetSistemYuzde,11:F2} | " +
                    $"{optSummary.MinBakiyeFiyat,11:F2} | " +
                    $"{optSummary.MaxBakiyeFiyat,11:F2} | " +
                    $"{optSummary.MinBakiyePuan,11:F2} | " +
                    $"{optSummary.MaxBakiyePuan,11:F2} | " +
                    $"{optSummary.MinBakiyeFiyatYuzde,10:F2} | " +
                    $"{optSummary.MaxBakiyeFiyatYuzde,10:F2} | " +
                    $"{optSummary.MinBakiyeFiyatIndex,9} | " +
                    $"{optSummary.MaxBakiyeFiyatIndex,9} | " +
                    $"{optSummary.MinBakiyeFiyatNet,11:F2} | " +
                    $"{optSummary.MaxBakiyeFiyatNet,11:F2} | " +
                    $"{optSummary.IslemSayisi,6} | " +
                    $"{optSummary.AlisSayisi,6} | " +
                    $"{optSummary.SatisSayisi,6} | " +
                    $"{optSummary.FlatSayisi,6} | " +
                    $"{optSummary.PassSayisi,6} | " +
                    $"{optSummary.KarAlSayisi,6} | " +
                    $"{optSummary.ZararKesSayisi,8} | " +
                    $"{optSummary.KazandiranIslemSayisi,7} | " +
                    $"{optSummary.KaybettirenIslemSayisi,8} | " +
                    $"{optSummary.NotrIslemSayisi,6} | " +
                    $"{optSummary.KazandiranAlisSayisi,8} | " +
                    $"{optSummary.KaybettirenAlisSayisi,8} | " +
                    $"{optSummary.NotrAlisSayisi,8} | " +
                    $"{optSummary.KazandiranSatisSayisi,9} | " +
                    $"{optSummary.KaybettirenSatisSayisi,9} | " +
                    $"{optSummary.NotrSatisSayisi,9} | " +
                    $"{optSummary.AlKomutSayisi,8} | " +
                    $"{optSummary.SatKomutSayisi,9} | " +
                    $"{optSummary.PasGecKomutSayisi,9} | " +
                    $"{optSummary.KarAlKomutSayisi,9} | " +
                    $"{optSummary.ZararKesKomutSayisi,10} | " +
                    $"{optSummary.FlatOlKomutSayisi,8} | " +
                    $"{optSummary.KomisyonIslemSayisi,9} | " +
                    $"{optSummary.KomisyonVarlikAdedSayisi,10:F2} | " +
                    $"{optSummary.KomisyonVarlikAdedSayisiMicro,10:F4} | " +
                    $"{optSummary.KomisyonCarpan,10:F4} | " +
                    $"{optSummary.KomisyonFiyat,10:F2} | " +
                    $"{optSummary.KomisyonFiyatYuzde,9:F4} | " +
                    $"{optSummary.KomisyonuDahilEt,9} | " +
                    $"{optSummary.KarZararFiyat,10:F2} | " +
                    $"{optSummary.KarZararFiyatYuzde,10:F2} | " +
                    $"{optSummary.KarZararPuan,10:F4} | " +
                    $"{optSummary.ToplamKarFiyat,11:F2} | " +
                    $"{optSummary.ToplamZararFiyat,11:F2} | " +
                    $"{optSummary.NetKarFiyat,11:F2} | " +
                    $"{optSummary.ToplamKarPuan,11:F4} | " +
                    $"{optSummary.ToplamZararPuan,11:F4} | " +
                    $"{optSummary.NetKarPuan,11:F4} | " +
                    $"{optSummary.MaxKarFiyat,11:F2} | " +
                    $"{optSummary.MaxZararFiyat,11:F2} | " +
                    $"{optSummary.MaxKarPuan,11:F4} | " +
                    $"{optSummary.MaxZararPuan,11:F4} | " +
                    $"{optSummary.KardaBarSayisi,7} | " +
                    $"{optSummary.ZarardaBarSayisi,7} | " +
                    $"{optSummary.KarliIslemOrani,10:F2} | " +
                    $"{optSummary.GetiriMaxDD,10:F2} | " +
                    $"{optSummary.GetiriMaxDDTarih,19} | " +
                    $"{optSummary.GetiriMaxKayip,10:F2} | " +
                    $"{optSummary.ProfitFactor,10:F2} | " +
                    $"{optSummary.ProfitFactorSistem,11:F2} | " +
                    $"{optSummary.Sinyal,8} | " +
                    $"{optSummary.SonYon,6} | " +
                    $"{optSummary.PrevYon,7} | " +
                    $"{optSummary.SonFiyat,10:F4} | " +
                    $"{optSummary.SonAFiyat,10:F4} | " +
                    $"{optSummary.SonSFiyat,10:F4} | " +
                    $"{optSummary.SonFFiyat,10:F4} | " +
                    $"{optSummary.SonPFiyat,10:F4} | " +
                    $"{optSummary.PrevFiyat,10:F4} | " +
                    $"{optSummary.SonBarNo,9} | " +
                    $"{optSummary.SonABarNo,10} | " +
                    $"{optSummary.SonSBarNo,10} | " +
                    $"{optSummary.EmirKomut,8} | " +
                    $"{optSummary.EmirStatus,8} | " +
                    $"{optSummary.HisseSayisi,10:F2} | " +
                    $"{optSummary.KontratSayisi,10:F2} | " +
                    $"{optSummary.VarlikAdedCarpani,10:F2} | " +
                    $"{optSummary.VarlikAdedSayisi,10:F2} | " +
                    $"{optSummary.VarlikAdedSayisiMicro,10:F4} | " +
                    $"{optSummary.KaymaMiktari,10:F4} | " +
                    $"{optSummary.KaymayiDahilEt,9} | " +
                    $"{optSummary.MicroLotSizeEnabled,9} | " +
                    $"{optSummary.PyramidingEnabled,9} | " +
                    $"{optSummary.MaxPositionSizeEnabled,10} | " +
                    $"{optSummary.MaxPositionSize,10:F4} | " +
                    $"{optSummary.MaxPositionSizeMicro,10:F4} | " +
                    $"{optSummary.GetiriFiyatBuAy,10:F2} | " +
                    $"{optSummary.GetiriFiyatAy1,10:F2} | " +
                    $"{optSummary.GetiriFiyatBuHafta,10:F2} | " +
                    $"{optSummary.GetiriFiyatHafta1,10:F2} | " +
                    $"{optSummary.GetiriFiyatBuGun,10:F2} | " +
                    $"{optSummary.GetiriFiyatGun1,10:F2} | " +
                    $"{optSummary.GetiriFiyatBuSaat,10:F2} | " +
                    $"{optSummary.GetiriFiyatSaat1,10:F2} | " +
                    $"{optSummary.GetiriPuanBuAy,10:F4} | " +
                    $"{optSummary.GetiriPuanAy1,10:F4} | " +
                    $"{optSummary.GetiriPuanBuHafta,10:F4} | " +
                    $"{optSummary.GetiriPuanHafta1,10:F4} | " +
                    $"{optSummary.GetiriPuanBuGun,10:F4} | " +
                    $"{optSummary.GetiriPuanGun1,10:F4} | " +
                    $"{optSummary.GetiriPuanBuSaat,10:F4} | " +
                    $"{optSummary.GetiriPuanSaat1,10:F4}"
                );

                sw.WriteLine(dataBuilder.ToString());
                sw.Flush();
            }
        }

        /// <summary>
        /// Save results to CSV file
        /// </summary>
        private void SaveResultsToCsv(string filePath, bool append)
        {
            using (var fs = new System.IO.FileStream(
                filePath,
                append ? System.IO.FileMode.Append : System.IO.FileMode.Create,
                System.IO.FileAccess.Write,
                System.IO.FileShare.ReadWrite))
            using (var sw = new System.IO.StreamWriter(fs))
            {
                // Write header if not appending or file is new
                if (!append || fs.Length == 0)
                {
                    // Get all parameter names from first result
                    if (Results.Count > 0)
                    {
                        var paramNames = Results[0].Parameters.Keys.ToList();
                        var header = string.Join(",", paramNames) + ",NetProfit,WinRate,ProfitFactor,MaxDrawdown,SharpeRatio";
                        sw.WriteLine(header);
                    }
                }

                // Write data
                foreach (var result in Results)
                {
                    var paramValues = result.Parameters.Values.Select(v => v.ToString()).ToList();
                    var metrics = new[]
                    {
                        result.NetProfit.ToString("F2"),
                        result.WinRate.ToString("F2"),
                        result.ProfitFactor.ToString("F2"),
                        result.MaxDrawdown.ToString("F2"),
                        result.SharpeRatio.ToString("F2")
                    };
                    var line = string.Join(",", paramValues.Concat(metrics));
                    sw.WriteLine(line);
                }

                sw.Flush();
            }
        }

        /// <summary>
        /// Save results to TXT file
        /// </summary>
        private void SaveResultsToTxt(string filePath, bool append)
        {
            using (var fs = new System.IO.FileStream(
                filePath,
                append ? System.IO.FileMode.Append : System.IO.FileMode.Create,
                System.IO.FileAccess.Write,
                System.IO.FileShare.ReadWrite))
            using (var sw = new System.IO.StreamWriter(fs))
            {
                // Write separator for append mode
                if (append && fs.Length > 0)
                {
                    sw.WriteLine();
                    sw.WriteLine("================================================================================");
                    sw.WriteLine($"New optimization run: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sw.WriteLine("================================================================================");
                    sw.WriteLine();
                }

                // Write results
                sw.WriteLine($"Optimization Results - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sw.WriteLine($"Total combinations tested: {Results.Count}");
                sw.WriteLine();

                for (int i = 0; i < Results.Count; i++)
                {
                    var result = Results[i];
                    sw.WriteLine($"Result #{i + 1}:");

                    // Parameters
                    foreach (var kvp in result.Parameters)
                    {
                        sw.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }

                    // Metrics
                    sw.WriteLine($"  NetProfit: {result.NetProfit:F2}");
                    sw.WriteLine($"  WinRate: {result.WinRate:F2}%");
                    sw.WriteLine($"  ProfitFactor: {result.ProfitFactor:F2}");
                    sw.WriteLine($"  MaxDrawdown: {result.MaxDrawdown:F2}");
                    sw.WriteLine($"  SharpeRatio: {result.SharpeRatio:F2}");
                    sw.WriteLine();
                }

                // Best result
                var bestResult = GetBestResult();
                if (bestResult != null)
                {
                    sw.WriteLine("=== BEST RESULT ===");
                    foreach (var kvp in bestResult.Parameters)
                    {
                        sw.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }
                    sw.WriteLine($"  NetProfit: {bestResult.NetProfit:F2}");
                    sw.WriteLine($"  WinRate: {bestResult.WinRate:F2}%");
                    sw.WriteLine($"  ProfitFactor: {bestResult.ProfitFactor:F2}");
                    sw.WriteLine($"  MaxDrawdown: {bestResult.MaxDrawdown:F2}");
                    sw.WriteLine($"  SharpeRatio: {bestResult.SharpeRatio:F2}");
                }

                sw.Flush();
            }
        }

        /// <summary>
        /// Generate all parameter combinations (generic - recursive)
        /// </summary>
        private List<Dictionary<string, object>> GenerateParameterCombinations()
        {
            if (ParameterRanges == null || ParameterRanges.Count == 0)
                return new List<Dictionary<string, object>>();

            var results = new List<Dictionary<string, object>>();
            GenerateCombinationsRecursive(0, new Dictionary<string, object>(), results);
            return results;
        }

        /// <summary>
        /// Recursive helper for generating parameter combinations
        /// </summary>
        private void GenerateCombinationsRecursive(int paramIndex, Dictionary<string, object> current, List<Dictionary<string, object>> results)
        {
            // Base case: all parameters assigned
            if (paramIndex >= ParameterRanges.Count)
            {
                results.Add(new Dictionary<string, object>(current));
                return;
            }

            // Recursive case: try all values for current parameter
            var range = ParameterRanges[paramIndex];
            var values = range.GetValues();

            foreach (var value in values)
            {
                current[range.Name] = value;
                GenerateCombinationsRecursive(paramIndex + 1, current, results);
            }
        }

        /// <summary>
        /// Create OptimizationResult from OptimizationSummary
        /// </summary>
        private OptimizationResult CreateOptimizationResultFromSummary(
            AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics.Statistics.OptimizationSummaryMinimal optSummary,
            Dictionary<string, object> paramCombo)
        {
            var result = new OptimizationResult
            {
                NetProfit = optSummary.GetiriFiyatNet,
                WinRate = optSummary.KarliIslemOrani, // Already calculated as percentage
                ProfitFactor = optSummary.ProfitFactor,
                MaxDrawdown = optSummary.GetiriMaxDD,
                SharpeRatio = 0.0 // TODO: Calculate Sharpe Ratio if needed
            };

            // Add all parameters to result (generic)
            foreach (var kvp in paramCombo)
            {
                result.Parameters[kvp.Key] = kvp.Value;
            }

            return result;
        }


        /// <summary>
        /// Create OptimizationResult from OptimizationSummary
        /// </summary>
        private OptimizationResult CreateOptimizationResultFromSummary(            
            AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Statistics.Statistics.OptimizationSummary optSummary,
            Dictionary<string, object> paramCombo)
        {
            var result = new OptimizationResult
            {
                // Trader Information
                TraderId = optSummary.TraderId,
                TraderName = optSummary.TraderName,

                // Symbol Information
                SymbolName = optSummary.SymbolName,
                SymbolPeriod = optSummary.SymbolPeriod,

                // System Information
                SystemId = int.TryParse(optSummary.SystemId, out int sysId) ? sysId : 0,
                SystemName = optSummary.SystemName,

                // Strategy Information
                StrategyId = int.TryParse(optSummary.StrategyId, out int stratId) ? stratId : 0,
                StrategyName = optSummary.StrategyName,

                // Bar Information
                ToplamBarSayisi = optSummary.ToplamBarSayisi,
                IlkBarTarihSaati = DateTime.TryParse(optSummary.IlkBarTarihSaati, out DateTime ilkDt) ? ilkDt : DateTime.MinValue,
                IlkBarTarihi = DateTime.TryParse(optSummary.IlkBarTarihi, out DateTime ilkTarih) ? ilkTarih : DateTime.MinValue,
                IlkBarSaati = TimeSpan.TryParse(optSummary.IlkBarSaati, out TimeSpan ilkSaat) ? ilkSaat : TimeSpan.Zero,
                SonBarTarihSaati = DateTime.TryParse(optSummary.SonBarTarihSaati, out DateTime sonDt) ? sonDt : DateTime.MinValue,
                SonBarTarihi = DateTime.TryParse(optSummary.SonBarTarihi, out DateTime sonTarih) ? sonTarih : DateTime.MinValue,
                SonBarSaati = TimeSpan.TryParse(optSummary.SonBarSaati, out TimeSpan sonSaat) ? sonSaat : TimeSpan.Zero,

                // Balance & Return Metrics
                IlkBakiyeFiyat = optSummary.IlkBakiyeFiyat,
                BakiyeFiyat = optSummary.BakiyeFiyat,
                GetiriFiyat = optSummary.GetiriFiyat,
                GetiriFiyatYuzde = optSummary.GetiriFiyatYuzde,
                KomisyonFiyat = optSummary.KomisyonFiyat,
                BakiyeFiyatNet = optSummary.BakiyeFiyatNet,
                GetiriFiyatNet = optSummary.GetiriFiyatNet,
                GetiriFiyatYuzdeNet = optSummary.GetiriFiyatYuzdeNet,
                KomisyonFiyatYuzde = optSummary.KomisyonFiyatYuzde,

                // Balance Min/Max
                MinBakiyeFiyat = optSummary.MinBakiyeFiyat,
                MaxBakiyeFiyat = optSummary.MaxBakiyeFiyat,
                MinBakiyeFiyatYuzde = optSummary.MinBakiyeFiyatYuzde,
                MaxBakiyeFiyatYuzde = optSummary.MaxBakiyeFiyatYuzde,
                MinBakiyeFiyatNet = optSummary.MinBakiyeFiyatNet,
                MaxBakiyeFiyatNet = optSummary.MaxBakiyeFiyatNet,

                // Trade Counts
                IslemSayisi = optSummary.IslemSayisi,
                AlisSayisi = optSummary.AlisSayisi,
                SatisSayisi = optSummary.SatisSayisi,
                FlatSayisi = optSummary.FlatSayisi,
                PassSayisi = optSummary.PassSayisi,
                KarAlSayisi = optSummary.KarAlSayisi,
                ZararKesSayisi = optSummary.ZararKesSayisi,
                KomisyonIslemSayisi = optSummary.KomisyonIslemSayisi,

                // Win/Loss Counts
                KazandiranIslemSayisi = optSummary.KazandiranIslemSayisi,
                KaybettirenIslemSayisi = optSummary.KaybettirenIslemSayisi,
                NotrIslemSayisi = optSummary.NotrIslemSayisi,

                // Profit & Loss
                ToplamKarFiyat = optSummary.ToplamKarFiyat,
                ToplamZararFiyat = optSummary.ToplamZararFiyat,
                NetKarFiyat = optSummary.NetKarFiyat,
                MaxKarFiyat = optSummary.MaxKarFiyat,
                MaxZararFiyat = optSummary.MaxZararFiyat,

                // Win Rate
                KarliIslemOrani = optSummary.KarliIslemOrani,

                // Drawdown & Risk Metrics
                GetiriMaxDD = optSummary.GetiriMaxDD,
                GetiriMaxDDTarih = DateTime.TryParse(optSummary.GetiriMaxDDTarih, out DateTime maxDdDate) ? maxDdDate : DateTime.MinValue,
                GetiriMaxKayip = optSummary.GetiriMaxKayip,
                ProfitFactor = optSummary.ProfitFactor,

                // Standard Performance Metrics
                NetProfit = optSummary.GetiriFiyatNet,
                WinRate = optSummary.KarliIslemOrani,
                MaxDrawdown = optSummary.GetiriMaxDD,
                SharpeRatio = 0.0, // TODO: Calculate Sharpe Ratio if needed

                // Additional Trade Statistics
                TotalTrades = optSummary.IslemSayisi,
                WinningTrades = optSummary.KazandiranIslemSayisi,
                LosingTrades = optSummary.KaybettirenIslemSayisi,
                TotalProfit = optSummary.ToplamKarFiyat,
                TotalLoss = Math.Abs(optSummary.ToplamZararFiyat), // Make sure it's positive
                AverageWin = optSummary.KazandiranIslemSayisi > 0 ? optSummary.ToplamKarFiyat / optSummary.KazandiranIslemSayisi : 0,
                AverageLoss = optSummary.KaybettirenIslemSayisi > 0 ? Math.Abs(optSummary.ToplamZararFiyat) / optSummary.KaybettirenIslemSayisi : 0,
                MaxDrawdownPct = optSummary.GetiriMaxDD
            };

            // Add all parameters to result (generic)
            foreach (var kvp in paramCombo)
            {
                result.Parameters[kvp.Key] = kvp.Value;
            }

            return result;
        }


        private void OnSingleTraderReset(SingleTrader trader, int mode)
        {

        }

        private void OnSingleTraderInit(SingleTrader trader, int mode)
        {

        }

        private void OnSingleTraderRun(SingleTrader trader, int mode)
        {

        }

        private void OnSingleTraderFinal(SingleTrader trader, int mode)
        {

        }

        // Callback function to be assigned to SingleTrader.Callback
        // Runs right after emirleri_uygula(i) for each bar
        private void OnSingleTraderBeforeOrder(SingleTrader trader, int barIndex)
        {
            // Example: you can inspect last signal/direction here
            // Logger?.Log($"CB | Bar={barIndex} Yon={trader.signals.SonYon} EmirStatus={trader.signals.EmirStatus}");
            // No-op by default
        }

        // Notification when a concrete A/S/F sinyali gerçekleştiğinde tetiklenir
        private void OnSingleTraderNotifySignal(SingleTrader trader, string signal, int barIndex)
        {
            // Example: Logger?.Log($"SIG | Yon={trader.signals.SonYon} Sinyal={signal}");
            // No-op by default
        }

        // Callback function to be assigned to SingleTrader.Callback
        // Runs right after emirleri_uygula(i) for each bar
        private void OnSingleTraderAfterOrder(SingleTrader trader, int barIndex)
        {
            // Example: you can inspect last signal/direction here
            // Logger?.Log($"CB | Bar={barIndex} Yon={trader.signals.SonYon} EmirStatus={trader.signals.EmirStatus}");
            // No-op by default
        }

        private void OnSingleTraderProgressInternal(SingleTrader trader, int currentBar, int totalBars)
        {
            // Forward to external callback
            OnSingleTraderProgressCallback?.Invoke(currentBar, totalBars);
        }

        private void OnApplyUserFlags(SingleTrader trader)
        {
            // InitializeUserControlledFlags
            trader.ConfigureUserFlagsOnce();
        }

        private void SaveResultsToFile(List<OptimizationResult> results, string filename)
        {
            // JSON olarak kaydet
            var json = JsonSerializer.Serialize(results, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(filename, json);

            //_singleTraderLogger?.Log($"Results saved to {filename}");
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            // Cleanup
            Results?.Clear();
            ParameterRanges?.Clear();
        }

        #endregion
    }
}
