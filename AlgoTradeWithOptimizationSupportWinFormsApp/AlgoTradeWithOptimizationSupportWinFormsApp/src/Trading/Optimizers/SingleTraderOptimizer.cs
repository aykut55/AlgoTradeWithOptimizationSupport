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
using System.Reflection.PortableExecutable;
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
                    // Build header: CombNo + Parameters + OptimizationResult fields + OptimizationSummary fields
                    var headerParts = new List<string> { "CombNo" };

                    // Add parameter names
                    if (result.Parameters != null && result.Parameters.Count > 0)
                    {
                        headerParts.AddRange(result.Parameters.Keys);
                    }

                    // Add OptimizationResult section header
                    headerParts.Add("OptResult");

                    // Add OptimizationResult fields
                    headerParts.Add("OR_IlkBakFy");
                    headerParts.Add("OR_BakFiyat");
                    headerParts.Add("OR_GetFiyat");
                    headerParts.Add("OR_GetFyt%");
                    headerParts.Add("OR_KomFiyat");
                    headerParts.Add("OR_BakFyNet");
                    headerParts.Add("OR_GetFyNet");
                    headerParts.Add("OR_GetFy%N");
                    headerParts.Add("OR_KomFy%");
                    headerParts.Add("OR_MinBakFy");
                    headerParts.Add("OR_MaxBakFy");
                    headerParts.Add("OR_MinBak%");
                    headerParts.Add("OR_MaxBak%");
                    headerParts.Add("OR_MinBakNt");
                    headerParts.Add("OR_MaxBakNt");
                    headerParts.Add("OR_Islem");
                    headerParts.Add("OR_Alis");
                    headerParts.Add("OR_Satis");
                    headerParts.Add("OR_Flat");
                    headerParts.Add("OR_Pass");
                    headerParts.Add("OR_KarAl");
                    headerParts.Add("OR_ZararKes");
                    headerParts.Add("OR_KomIslem");
                    headerParts.Add("OR_Kazand");
                    headerParts.Add("OR_Kaybett");
                    headerParts.Add("OR_Notr");
                    headerParts.Add("OR_TopKarFy");
                    headerParts.Add("OR_TopZarFy");
                    headerParts.Add("OR_NetKarFy");
                    headerParts.Add("OR_MaxKarFy");
                    headerParts.Add("OR_MaxZarFy");
                    headerParts.Add("OR_KarliOra");
                    headerParts.Add("OR_MaxDD");
                    headerParts.Add("OR_MaxDDDt");
                    headerParts.Add("OR_MaxKayip");
                    headerParts.Add("OR_ProfFact");
                    headerParts.Add("OR_NetProf");
                    headerParts.Add("OR_WinRate");
                    headerParts.Add("OR_MaxDD2");
                    headerParts.Add("OR_Sharpe");
                    headerParts.Add("OR_TotTrade");
                    headerParts.Add("OR_WinTrade");
                    headerParts.Add("OR_LosTrade");
                    headerParts.Add("OR_TotProf");
                    headerParts.Add("OR_TotLoss");
                    headerParts.Add("OR_AvgWin");
                    headerParts.Add("OR_AvgLoss");
                    headerParts.Add("OR_MaxDD%");

                    headerParts.Add("OR_TraderId");
                    headerParts.Add("OR_TrdrName");
                    headerParts.Add("OR_Symbol");
                    headerParts.Add("OR_SymPer");
                    headerParts.Add("OR_SysId");
                    headerParts.Add("OR_SysName");
                    headerParts.Add("OR_StratId");
                    headerParts.Add("OR_StratNam");
                    headerParts.Add("OR_BarCnt");
                    headerParts.Add("OR_IlkBarDT");
                    headerParts.Add("OR_IlkBarD");
                    headerParts.Add("OR_IlkBarT");
                    headerParts.Add("OR_SonBarDT");
                    headerParts.Add("OR_SonBarD");
                    headerParts.Add("OR_SonBarT");

                    // Add OptimizationSummary section header
                    headerParts.Add("OptSummary");

                    // Add OptimizationSummary fields
                    headerParts.Add("TraderId");
                    headerParts.Add("TraderName");
                    headerParts.Add("SymbolName");
                    headerParts.Add("SymbolPer");
                    headerParts.Add("SystemId");
                    headerParts.Add("SystemName");
                    headerParts.Add("StrategyId");
                    headerParts.Add("StrategyName");
                    headerParts.Add("LastExecId");
                    headerParts.Add("LastExecTime");
                    headerParts.Add("ExecStart");
                    headerParts.Add("ExecStop");
                    headerParts.Add("ExecMs");
                    headerParts.Add("ResetTime");
                    headerParts.Add("StatsTime");
                    headerParts.Add("BarCnt");
                    headerParts.Add("SelBarNo");
                    headerParts.Add("SelBarDT");
                    headerParts.Add("SelBarD");
                    headerParts.Add("SelBarT");
                    headerParts.Add("IlkBarDT");
                    headerParts.Add("IlkBarD");
                    headerParts.Add("IlkBarT");
                    headerParts.Add("SonBarDT");
                    headerParts.Add("SonBarD");
                    headerParts.Add("SonBarT");
                    headerParts.Add("IlkBarIdx");
                    headerParts.Add("SonBarIdx");
                    headerParts.Add("SonBarOp");
                    headerParts.Add("SonBarHi");
                    headerParts.Add("SonBarLo");
                    headerParts.Add("SonBarCl");
                    headerParts.Add("Months");
                    headerParts.Add("Days");
                    headerParts.Add("Hours");
                    headerParts.Add("Mins");
                    headerParts.Add("AvgMoTrd");
                    headerParts.Add("AvgWeekTr");
                    headerParts.Add("AvgDayTr");
                    headerParts.Add("AvgHrTrd");
                    headerParts.Add("IlkBakFyt");
                    headerParts.Add("IlkBakPua");
                    headerParts.Add("BakFiyat");
                    headerParts.Add("BakPuan");
                    headerParts.Add("GetFiyat");
                    headerParts.Add("GetPuan");
                    headerParts.Add("GetFyt%");
                    headerParts.Add("GetPua%");
                    headerParts.Add("BakFytNet");
                    headerParts.Add("BakPuaNet");
                    headerParts.Add("GetFytNet");
                    headerParts.Add("GetPuaNet");
                    headerParts.Add("GetFyt%N");
                    headerParts.Add("GetPua%N");
                    headerParts.Add("GetKz");
                    headerParts.Add("GetKzNet");
                    headerParts.Add("GetKzSis");
                    headerParts.Add("GetKzSis%");
                    headerParts.Add("GetKzNetS");
                    headerParts.Add("GetKzNtS%");
                    headerParts.Add("MinBakFyt");
                    headerParts.Add("MaxBakFyt");
                    headerParts.Add("MinBakPua");
                    headerParts.Add("MaxBakPua");
                    headerParts.Add("MinBakF%");
                    headerParts.Add("MaxBakF%");
                    headerParts.Add("MinBakIdx");
                    headerParts.Add("MaxBakIdx");
                    headerParts.Add("MinBakNet");
                    headerParts.Add("MaxBakNet");
                    headerParts.Add("Islem");
                    headerParts.Add("Alis");
                    headerParts.Add("Satis");
                    headerParts.Add("Flat");
                    headerParts.Add("Pass");
                    headerParts.Add("KarAl");
                    headerParts.Add("ZararKes");
                    headerParts.Add("Kazand");
                    headerParts.Add("Kaybett");
                    headerParts.Add("Notr");
                    headerParts.Add("KazAlis");
                    headerParts.Add("KayAlis");
                    headerParts.Add("NotAlis");
                    headerParts.Add("KazSatis");
                    headerParts.Add("KaySatis");
                    headerParts.Add("NotSatis");
                    headerParts.Add("AlKomut");
                    headerParts.Add("SatKomut");
                    headerParts.Add("PasKomut");
                    headerParts.Add("KarAlKom");
                    headerParts.Add("ZarKesKom");
                    headerParts.Add("FlatKom");
                    headerParts.Add("KomIslem");
                    headerParts.Add("KomVarAd");
                    headerParts.Add("KomVarMic");
                    headerParts.Add("KomCarpa");
                    headerParts.Add("KomFiyat");
                    headerParts.Add("KomFyt%");
                    headerParts.Add("KomDahil");
                    headerParts.Add("KZFiyat");
                    headerParts.Add("KZFiyat%");
                    headerParts.Add("KZPuan");
                    headerParts.Add("TopKarFyt");
                    headerParts.Add("TopZarFyt");
                    headerParts.Add("NetKarFyt");
                    headerParts.Add("TopKarPua");
                    headerParts.Add("TopZarPua");
                    headerParts.Add("NetKarPua");
                    headerParts.Add("MaxKarFyt");
                    headerParts.Add("MaxZarFyt");
                    headerParts.Add("MaxKarPua");
                    headerParts.Add("MaxZarPua");
                    headerParts.Add("KarBar");
                    headerParts.Add("ZarBar");
                    headerParts.Add("KarliOran");
                    headerParts.Add("MaxDD");
                    headerParts.Add("MaxDDDate");
                    headerParts.Add("MaxKayip");
                    headerParts.Add("ProfitFac");
                    headerParts.Add("ProfFacSis");
                    headerParts.Add("Sinyal");
                    headerParts.Add("SonYon");
                    headerParts.Add("PrevYon");
                    headerParts.Add("SonFyt");
                    headerParts.Add("SonAFyt");
                    headerParts.Add("SonSFyt");
                    headerParts.Add("SonFFyt");
                    headerParts.Add("SonPFyt");
                    headerParts.Add("PrevFyt");
                    headerParts.Add("SonBarNo");
                    headerParts.Add("SonABarNo");
                    headerParts.Add("SonSBarNo");
                    headerParts.Add("EmirKomut");
                    headerParts.Add("EmirStatus");
                    headerParts.Add("HisseSayisi");
                    headerParts.Add("KontratSayisi");
                    headerParts.Add("VarlikAdCarp");
                    headerParts.Add("VarlikAded");
                    headerParts.Add("VarlikAdMic");
                    headerParts.Add("KaymaMikt");
                    headerParts.Add("KaymaDahil");
                    headerParts.Add("MicroLot");
                    headerParts.Add("Pyramiding");
                    headerParts.Add("MaxPosSize");
                    headerParts.Add("MaxPosFiyat");
                    headerParts.Add("MaxPosMicro");
                    headerParts.Add("GetFytBuAy");
                    headerParts.Add("GetFytAy1");
                    headerParts.Add("GetFytBuHaf");
                    headerParts.Add("GetFytHaf1");
                    headerParts.Add("GetFytBuGun");
                    headerParts.Add("GetFytGun1");
                    headerParts.Add("GetFytBuSa");
                    headerParts.Add("GetFytSa1");
                    headerParts.Add("GetPuaBuAy");
                    headerParts.Add("GetPuaAy1");
                    headerParts.Add("GetPuaBuHaf");
                    headerParts.Add("GetPuaHaf1");
                    headerParts.Add("GetPuaBuGun");
                    headerParts.Add("GetPuaGun1");
                    headerParts.Add("GetPuaBuSa");
                    headerParts.Add("GetPuaSa1");

                    sw.WriteLine(string.Join(";", headerParts));
                }

                // Write data row: CombNo + Parameter values + OptimizationResult values + OptimizationSummary values
                var dataParts = new List<string> { currentCombination.ToString() };

                // Add parameter values
                if (result.Parameters != null && result.Parameters.Count > 0)
                {
                    dataParts.AddRange(result.Parameters.Values.Select(v => v?.ToString() ?? ""));
                }

                // Add OptimizationResult section placeholder
                dataParts.Add("");

                // Add OptimizationResult values
                dataParts.Add(result.IlkBakiyeFiyat.ToString("F2"));
                dataParts.Add(result.BakiyeFiyat.ToString("F2"));
                dataParts.Add(result.GetiriFiyat.ToString("F2"));
                dataParts.Add(result.GetiriFiyatYuzde.ToString("F2"));
                dataParts.Add(result.KomisyonFiyat.ToString("F2"));
                dataParts.Add(result.BakiyeFiyatNet.ToString("F2"));
                dataParts.Add(result.GetiriFiyatNet.ToString("F2"));
                dataParts.Add(result.GetiriFiyatYuzdeNet.ToString("F2"));
                dataParts.Add(result.KomisyonFiyatYuzde.ToString("F4"));
                dataParts.Add(result.MinBakiyeFiyat.ToString("F2"));
                dataParts.Add(result.MaxBakiyeFiyat.ToString("F2"));
                dataParts.Add(result.MinBakiyeFiyatYuzde.ToString("F2"));
                dataParts.Add(result.MaxBakiyeFiyatYuzde.ToString("F2"));
                dataParts.Add(result.MinBakiyeFiyatNet.ToString("F2"));
                dataParts.Add(result.MaxBakiyeFiyatNet.ToString("F2"));
                dataParts.Add(result.IslemSayisi.ToString());
                dataParts.Add(result.AlisSayisi.ToString());
                dataParts.Add(result.SatisSayisi.ToString());
                dataParts.Add(result.FlatSayisi.ToString());
                dataParts.Add(result.PassSayisi.ToString());
                dataParts.Add(result.KarAlSayisi.ToString());
                dataParts.Add(result.ZararKesSayisi.ToString());
                dataParts.Add(result.KomisyonIslemSayisi.ToString());
                dataParts.Add(result.KazandiranIslemSayisi.ToString());
                dataParts.Add(result.KaybettirenIslemSayisi.ToString());
                dataParts.Add(result.NotrIslemSayisi.ToString());
                dataParts.Add(result.ToplamKarFiyat.ToString("F2"));
                dataParts.Add(result.ToplamZararFiyat.ToString("F2"));
                dataParts.Add(result.NetKarFiyat.ToString("F2"));
                dataParts.Add(result.MaxKarFiyat.ToString("F2"));
                dataParts.Add(result.MaxZararFiyat.ToString("F2"));
                dataParts.Add(result.KarliIslemOrani.ToString("F2"));
                dataParts.Add(result.GetiriMaxDD.ToString("F2"));
                dataParts.Add(result.GetiriMaxDDTarih.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(result.GetiriMaxKayip.ToString("F2"));
                dataParts.Add(result.ProfitFactor.ToString("F2"));
                dataParts.Add(result.NetProfit.ToString("F2"));
                dataParts.Add(result.WinRate.ToString("F2"));
                dataParts.Add(result.MaxDrawdown.ToString("F2"));
                dataParts.Add(result.SharpeRatio.ToString("F2"));
                dataParts.Add(result.TotalTrades.ToString());
                dataParts.Add(result.WinningTrades.ToString());
                dataParts.Add(result.LosingTrades.ToString());
                dataParts.Add(result.TotalProfit.ToString("F2"));
                dataParts.Add(result.TotalLoss.ToString("F2"));
                dataParts.Add(result.AverageWin.ToString("F2"));
                dataParts.Add(result.AverageLoss.ToString("F2"));
                dataParts.Add(result.MaxDrawdownPct.ToString("F2"));

                dataParts.Add(result.TraderId.ToString());
                dataParts.Add(result.TraderName);
                dataParts.Add(result.SymbolName);
                dataParts.Add(result.SymbolPeriod);
                dataParts.Add(result.SystemId.ToString());
                dataParts.Add(result.SystemName);
                dataParts.Add(result.StrategyId.ToString());
                dataParts.Add(result.StrategyName);
                dataParts.Add(result.ToplamBarSayisi.ToString());
                dataParts.Add(result.IlkBarTarihSaati.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(result.IlkBarTarihi.ToString("yyyy.MM.dd"));
                dataParts.Add(result.IlkBarSaati.ToString());
                dataParts.Add(result.SonBarTarihSaati.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(result.SonBarTarihi.ToString("yyyy.MM.dd"));
                dataParts.Add(result.SonBarSaati.ToString());

                // Add OptimizationSummary section placeholder
                dataParts.Add("");

                // Add OptimizationSummary values
                dataParts.Add(optSummary.TraderId.ToString());
                dataParts.Add(optSummary.TraderName);
                dataParts.Add(optSummary.SymbolName);
                dataParts.Add(optSummary.SymbolPeriod);
                dataParts.Add(optSummary.SystemId.ToString());
                dataParts.Add(optSummary.SystemName);
                dataParts.Add(optSummary.StrategyId.ToString());
                dataParts.Add(optSummary.StrategyName);
                dataParts.Add(optSummary.LastExecutionId.ToString());
                dataParts.Add(optSummary.LastExecutionTime.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(optSummary.LastExecutionTimeStart.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(optSummary.LastExecutionTimeStop.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(optSummary.LastExecutionTimeInMSec.ToString());
                dataParts.Add(optSummary.LastResetTime.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(optSummary.LastStatisticsCalculationTime.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(optSummary.ToplamBarSayisi.ToString());
                dataParts.Add(optSummary.SecilenBarNumarasi.ToString());
                dataParts.Add(optSummary.SecilenBarTarihSaati.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(optSummary.SecilenBarTarihi.ToString("yyyy.MM.dd"));
                dataParts.Add(optSummary.SecilenBarSaati.ToString());
                dataParts.Add(optSummary.IlkBarTarihSaati.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(optSummary.IlkBarTarihi.ToString("yyyy.MM.dd"));
                dataParts.Add(optSummary.IlkBarSaati.ToString());
                dataParts.Add(optSummary.SonBarTarihSaati.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(optSummary.SonBarTarihi.ToString("yyyy.MM.dd"));
                dataParts.Add(optSummary.SonBarSaati.ToString());
                dataParts.Add(optSummary.IlkBarIndex.ToString());
                dataParts.Add(optSummary.SonBarIndex.ToString());
                dataParts.Add(optSummary.SonBarAcilisFiyati.ToString("F4"));
                dataParts.Add(optSummary.SonBarYuksekFiyati.ToString("F4"));
                dataParts.Add(optSummary.SonBarDusukFiyati.ToString("F4"));
                dataParts.Add(optSummary.SonBarKapanisFiyati.ToString("F4"));
                dataParts.Add(optSummary.ToplamGecenSureAy.ToString("F1"));
                dataParts.Add(optSummary.ToplamGecenSureGun.ToString());
                dataParts.Add(optSummary.ToplamGecenSureSaat.ToString());
                dataParts.Add(optSummary.ToplamGecenSureDakika.ToString());
                dataParts.Add(optSummary.OrtAylikIslemSayisi.ToString("F2"));
                dataParts.Add(optSummary.OrtHaftalikIslemSayisi.ToString("F2"));
                dataParts.Add(optSummary.OrtGunlukIslemSayisi.ToString("F2"));
                dataParts.Add(optSummary.OrtSaatlikIslemSayisi.ToString("F2"));
                dataParts.Add(optSummary.IlkBakiyeFiyat.ToString("F2"));
                dataParts.Add(optSummary.IlkBakiyePuan.ToString("F2"));
                dataParts.Add(optSummary.BakiyeFiyat.ToString("F2"));
                dataParts.Add(optSummary.BakiyePuan.ToString("F2"));
                dataParts.Add(optSummary.GetiriFiyat.ToString("F2"));
                dataParts.Add(optSummary.GetiriPuan.ToString("F4"));
                dataParts.Add(optSummary.GetiriFiyatYuzde.ToString("F2"));
                dataParts.Add(optSummary.GetiriPuanYuzde.ToString("F2"));
                dataParts.Add(optSummary.BakiyeFiyatNet.ToString("F2"));
                dataParts.Add(optSummary.BakiyePuanNet.ToString("F2"));
                dataParts.Add(optSummary.GetiriFiyatNet.ToString("F2"));
                dataParts.Add(optSummary.GetiriPuanNet.ToString("F4"));
                dataParts.Add(optSummary.GetiriFiyatYuzdeNet.ToString("F2"));
                dataParts.Add(optSummary.GetiriPuanYuzdeNet.ToString("F2"));
                dataParts.Add(optSummary.GetiriKz.ToString("F4"));
                dataParts.Add(optSummary.GetiriKzNet.ToString("F4"));
                dataParts.Add(optSummary.GetiriKzSistem.ToString("F4"));
                dataParts.Add(optSummary.GetiriKzSistemYuzde.ToString("F2"));
                dataParts.Add(optSummary.GetiriKzNetSistem.ToString("F4"));
                dataParts.Add(optSummary.GetiriKzNetSistemYuzde.ToString("F2"));
                dataParts.Add(optSummary.MinBakiyeFiyat.ToString("F2"));
                dataParts.Add(optSummary.MaxBakiyeFiyat.ToString("F2"));
                dataParts.Add(optSummary.MinBakiyePuan.ToString("F2"));
                dataParts.Add(optSummary.MaxBakiyePuan.ToString("F2"));
                dataParts.Add(optSummary.MinBakiyeFiyatYuzde.ToString("F2"));
                dataParts.Add(optSummary.MaxBakiyeFiyatYuzde.ToString("F2"));
                dataParts.Add(optSummary.MinBakiyeFiyatIndex.ToString());
                dataParts.Add(optSummary.MaxBakiyeFiyatIndex.ToString());
                dataParts.Add(optSummary.MinBakiyeFiyatNet.ToString("F2"));
                dataParts.Add(optSummary.MaxBakiyeFiyatNet.ToString("F2"));
                dataParts.Add(optSummary.IslemSayisi.ToString());
                dataParts.Add(optSummary.AlisSayisi.ToString());
                dataParts.Add(optSummary.SatisSayisi.ToString());
                dataParts.Add(optSummary.FlatSayisi.ToString());
                dataParts.Add(optSummary.PassSayisi.ToString());
                dataParts.Add(optSummary.KarAlSayisi.ToString());
                dataParts.Add(optSummary.ZararKesSayisi.ToString());
                dataParts.Add(optSummary.KazandiranIslemSayisi.ToString());
                dataParts.Add(optSummary.KaybettirenIslemSayisi.ToString());
                dataParts.Add(optSummary.NotrIslemSayisi.ToString());
                dataParts.Add(optSummary.KazandiranAlisSayisi.ToString());
                dataParts.Add(optSummary.KaybettirenAlisSayisi.ToString());
                dataParts.Add(optSummary.NotrAlisSayisi.ToString());
                dataParts.Add(optSummary.KazandiranSatisSayisi.ToString());
                dataParts.Add(optSummary.KaybettirenSatisSayisi.ToString());
                dataParts.Add(optSummary.NotrSatisSayisi.ToString());
                dataParts.Add(optSummary.AlKomutSayisi.ToString());
                dataParts.Add(optSummary.SatKomutSayisi.ToString());
                dataParts.Add(optSummary.PasGecKomutSayisi.ToString());
                dataParts.Add(optSummary.KarAlKomutSayisi.ToString());
                dataParts.Add(optSummary.ZararKesKomutSayisi.ToString());
                dataParts.Add(optSummary.FlatOlKomutSayisi.ToString());
                dataParts.Add(optSummary.KomisyonIslemSayisi.ToString());
                dataParts.Add(optSummary.KomisyonVarlikAdedSayisi.ToString("F2"));
                dataParts.Add(optSummary.KomisyonVarlikAdedSayisiMicro.ToString("F4"));
                dataParts.Add(optSummary.KomisyonCarpan.ToString("F4"));
                dataParts.Add(optSummary.KomisyonFiyat.ToString("F2"));
                dataParts.Add(optSummary.KomisyonFiyatYuzde.ToString("F4"));
                dataParts.Add(optSummary.KomisyonuDahilEt.ToString());
                dataParts.Add(optSummary.KarZararFiyat.ToString("F2"));
                dataParts.Add(optSummary.KarZararFiyatYuzde.ToString("F2"));
                dataParts.Add(optSummary.KarZararPuan.ToString("F4"));
                dataParts.Add(optSummary.ToplamKarFiyat.ToString("F2"));
                dataParts.Add(optSummary.ToplamZararFiyat.ToString("F2"));
                dataParts.Add(optSummary.NetKarFiyat.ToString("F2"));
                dataParts.Add(optSummary.ToplamKarPuan.ToString("F4"));
                dataParts.Add(optSummary.ToplamZararPuan.ToString("F4"));
                dataParts.Add(optSummary.NetKarPuan.ToString("F4"));
                dataParts.Add(optSummary.MaxKarFiyat.ToString("F2"));
                dataParts.Add(optSummary.MaxZararFiyat.ToString("F2"));
                dataParts.Add(optSummary.MaxKarPuan.ToString("F4"));
                dataParts.Add(optSummary.MaxZararPuan.ToString("F4"));
                dataParts.Add(optSummary.KardaBarSayisi.ToString());
                dataParts.Add(optSummary.ZarardaBarSayisi.ToString());
                dataParts.Add(optSummary.KarliIslemOrani.ToString("F2"));
                dataParts.Add(optSummary.GetiriMaxDD.ToString("F2"));
                dataParts.Add(optSummary.GetiriMaxDDTarih.ToString("yyyy.MM.dd HH:mm:ss"));
                dataParts.Add(optSummary.GetiriMaxKayip.ToString("F2"));
                dataParts.Add(optSummary.ProfitFactor.ToString("F2"));
                dataParts.Add(optSummary.ProfitFactorSistem.ToString("F2"));
                dataParts.Add(optSummary.Sinyal);
                dataParts.Add(optSummary.SonYon);
                dataParts.Add(optSummary.PrevYon);
                dataParts.Add(optSummary.SonFiyat.ToString("F4"));
                dataParts.Add(optSummary.SonAFiyat.ToString("F4"));
                dataParts.Add(optSummary.SonSFiyat.ToString("F4"));
                dataParts.Add(optSummary.SonFFiyat.ToString("F4"));
                dataParts.Add(optSummary.SonPFiyat.ToString("F4"));
                dataParts.Add(optSummary.PrevFiyat.ToString("F4"));
                dataParts.Add(optSummary.SonBarNo.ToString());
                dataParts.Add(optSummary.SonABarNo.ToString());
                dataParts.Add(optSummary.SonSBarNo.ToString());
                dataParts.Add(optSummary.EmirKomut);
                dataParts.Add(optSummary.EmirStatus);
                dataParts.Add(optSummary.HisseSayisi.ToString("F2"));
                dataParts.Add(optSummary.KontratSayisi.ToString("F2"));
                dataParts.Add(optSummary.VarlikAdedCarpani.ToString("F2"));
                dataParts.Add(optSummary.VarlikAdedSayisi.ToString("F2"));
                dataParts.Add(optSummary.VarlikAdedSayisiMicro.ToString("F4"));
                dataParts.Add(optSummary.KaymaMiktari.ToString("F4"));
                dataParts.Add(optSummary.KaymayiDahilEt.ToString());
                dataParts.Add(optSummary.MicroLotSizeEnabled.ToString());
                dataParts.Add(optSummary.PyramidingEnabled.ToString());
                dataParts.Add(optSummary.MaxPositionSizeEnabled.ToString());
                dataParts.Add(optSummary.MaxPositionSize.ToString("F4"));
                dataParts.Add(optSummary.MaxPositionSizeMicro.ToString("F4"));
                dataParts.Add(optSummary.GetiriFiyatBuAy.ToString("F2"));
                dataParts.Add(optSummary.GetiriFiyatAy1.ToString("F2"));
                dataParts.Add(optSummary.GetiriFiyatBuHafta.ToString("F2"));
                dataParts.Add(optSummary.GetiriFiyatHafta1.ToString("F2"));
                dataParts.Add(optSummary.GetiriFiyatBuGun.ToString("F2"));
                dataParts.Add(optSummary.GetiriFiyatGun1.ToString("F2"));
                dataParts.Add(optSummary.GetiriFiyatBuSaat.ToString("F2"));
                dataParts.Add(optSummary.GetiriFiyatSaat1.ToString("F2"));
                dataParts.Add(optSummary.GetiriPuanBuAy.ToString("F4"));
                dataParts.Add(optSummary.GetiriPuanAy1.ToString("F4"));
                dataParts.Add(optSummary.GetiriPuanBuHafta.ToString("F4"));
                dataParts.Add(optSummary.GetiriPuanHafta1.ToString("F4"));
                dataParts.Add(optSummary.GetiriPuanBuGun.ToString("F4"));
                dataParts.Add(optSummary.GetiriPuanGun1.ToString("F4"));
                dataParts.Add(optSummary.GetiriPuanBuSaat.ToString("F4"));
                dataParts.Add(optSummary.GetiriPuanSaat1.ToString("F4"));

                sw.WriteLine(string.Join(";", dataParts));
                sw.Flush();
            }
        }

        /// <summary>
        /// Append single OptimizationSummary to TXT file (Tabular format like SaveListsToTxt)
        /// </summary>
        private void AppendSingleOptSummaryToTxt(
            OptimizationResult optResult,
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
                    if (optResult.Parameters != null && optResult.Parameters.Count > 0)
                    {
                        foreach (var paramName in optResult.Parameters.Keys)
                        {
                            headerBuilder.Append($"{paramName,12} | ");
                        }
                    }

                    // Add OptimizationResult section header
                    headerBuilder.Append($"{"OptResult",20} | ");

                    // Add OptimizationResult fields
                    headerBuilder.Append(
                        $"{"OR_IlkBakFy",11} | " +
                        $"{"OR_BakFiyat",11} | " +
                        $"{"OR_GetFiyat",11} | " +
                        $"{"OR_GetFyt%",9} | " +
                        $"{"OR_KomFiyat",11} | " +
                        $"{"OR_BakFyNet",11} | " +
                        $"{"OR_GetFyNet",11} | " +
                        $"{"OR_GetFy%N",9} | " +
                        $"{"OR_KomFy%",9} | " +
                        $"{"OR_MinBakFy",11} | " +
                        $"{"OR_MaxBakFy",11} | " +
                        $"{"OR_MinBak%",10} | " +
                        $"{"OR_MaxBak%",10} | " +
                        $"{"OR_MinBakNt",11} | " +
                        $"{"OR_MaxBakNt",11} | " +
                        $"{"OR_Islem",8} | " +
                        $"{"OR_Alis",8} | " +
                        $"{"OR_Satis",8} | " +
                        $"{"OR_Flat",8} | " +
                        $"{"OR_Pass",8} | " +
                        $"{"OR_KarAl",8} | " +
                        $"{"OR_ZararKes",10} | " +
                        $"{"OR_KomIslem",10} | " +
                        $"{"OR_Kazand",8} | " +
                        $"{"OR_Kaybett",8} | " +
                        $"{"OR_Notr",8} | " +
                        $"{"OR_TopKarFy",11} | " +
                        $"{"OR_TopZarFy",11} | " +
                        $"{"OR_NetKarFy",11} | " +
                        $"{"OR_MaxKarFy",11} | " +
                        $"{"OR_MaxZarFy",11} | " +
                        $"{"OR_KarliOra",10} | " +
                        $"{"OR_MaxDD",10} | " +
                        $"{"OR_MaxDDDt",19} | " +
                        $"{"OR_MaxKayip",10} | " +
                        $"{"OR_ProfFact",10} | " +
                        $"{"OR_NetProf",11} | " +
                        $"{"OR_WinRate",10} | " +
                        $"{"OR_MaxDD2",10} | " +
                        $"{"OR_Sharpe",10} | " +
                        $"{"OR_TotTrade",10} | " +
                        $"{"OR_WinTrade",10} | " +
                        $"{"OR_LosTrade",10} | " +
                        $"{"OR_TotProf",11} | " +
                        $"{"OR_TotLoss",11} | " +
                        $"{"OR_AvgWin",11} | " +
                        $"{"OR_AvgLoss",11} | " +
                        $"{"OR_MaxDD%",10} | " +

                        $"{"OR_TraderId",10} | " +
                        $"{"OR_TrdrName",20} | " +
                        $"{"OR_Symbol",10} | " +
                        $"{"OR_SymPer",9} | " +
                        $"{"OR_SysId",10} | " +
                        $"{"OR_SysName",20} | " +
                        $"{"OR_StratId",10} | " +
                        $"{"OR_StratNam",20} | " +
                        $"{"OR_BarCnt",10} | " +
                        $"{"OR_IlkBarDT",19} | " +
                        $"{"OR_IlkBarD",10} | " +
                        $"{"OR_IlkBarT",10} | " +
                        $"{"OR_SonBarDT",19} | " +
                        $"{"OR_SonBarD",10} | " +
                        $"{"OR_SonBarT",10} | "
                    );

                    // Add OptimizationSummary section header
                    headerBuilder.Append($"{"OptSummary",20} | ");

                    // Add OptimizationSummary fields
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
                if (optResult.Parameters != null && optResult.Parameters.Count > 0)
                {
                    foreach (var paramValue in optResult.Parameters.Values)
                    {
                        dataBuilder.Append($"{paramValue,12} | ");
                    }
                }

                // OptimizationResult headerBuilder deki sırayla...
                dataBuilder.Append(
                    $"{ "", 20} | " +
                    $"{optResult.IlkBakiyeFiyat,11:F2} | " +
                    $"{optResult.BakiyeFiyat,11:F2} | " +
                    $"{optResult.GetiriFiyat,11:F2} | " +
                    $"{optResult.GetiriFiyatYuzde,9:F2} | " +
                    $"{optResult.KomisyonFiyat,11:F2} | " +
                    $"{optResult.BakiyeFiyatNet,11:F2} | " +
                    $"{optResult.GetiriFiyatNet,11:F2} | " +
                    $"{optResult.GetiriFiyatYuzdeNet,9:F2} | " +
                    $"{optResult.KomisyonFiyatYuzde,9:F4} | " +
                    $"{optResult.MinBakiyeFiyat,11:F2} | " +
                    $"{optResult.MaxBakiyeFiyat,11:F2} | " +
                    $"{optResult.MinBakiyeFiyatYuzde,10:F2} | " +
                    $"{optResult.MaxBakiyeFiyatYuzde,10:F2} | " +
                    $"{optResult.MinBakiyeFiyatNet,11:F2} | " +
                    $"{optResult.MaxBakiyeFiyatNet,11:F2} | " +
                    $"{optResult.IslemSayisi,8} | " +
                    $"{optResult.AlisSayisi,8} | " +
                    $"{optResult.SatisSayisi,8} | " +
                    $"{optResult.FlatSayisi,8} | " +
                    $"{optResult.PassSayisi,8} | " +
                    $"{optResult.KarAlSayisi,8} | " +
                    $"{optResult.ZararKesSayisi,10} | " +
                    $"{optResult.KomisyonIslemSayisi,10} | " +
                    $"{optResult.KazandiranIslemSayisi,8} | " +
                    $"{optResult.KaybettirenIslemSayisi,8} | " +
                    $"{optResult.NotrIslemSayisi,8} | " +
                    $"{optResult.ToplamKarFiyat,11:F2} | " +
                    $"{optResult.ToplamZararFiyat,11:F2} | " +
                    $"{optResult.NetKarFiyat,11:F2} | " +
                    $"{optResult.MaxKarFiyat,11:F2} | " +
                    $"{optResult.MaxZararFiyat,11:F2} | " +
                    $"{optResult.KarliIslemOrani,10:F2} | " +
                    $"{optResult.GetiriMaxDD,10:F2} | " +
                    $"{optResult.GetiriMaxDDTarih,19:yyyy.MM.dd HH:mm:ss} | " +
                    $"{optResult.GetiriMaxKayip,10:F2} | " +
                    $"{optResult.ProfitFactor,10:F2} | " +
                    $"{optResult.NetProfit,11:F2} | " +
                    $"{optResult.WinRate,10:F2} | " +
                    $"{optResult.MaxDrawdown,10:F2} | " +
                    $"{optResult.SharpeRatio,10:F2} | " +
                    $"{optResult.TotalTrades,10} | " +
                    $"{optResult.WinningTrades,10} | " +
                    $"{optResult.LosingTrades,10} | " +
                    $"{optResult.TotalProfit,11:F2} | " +
                    $"{optResult.TotalLoss,11:F2} | " +
                    $"{optResult.AverageWin,11:F2} | " +
                    $"{optResult.AverageLoss,11:F2} | " +
                    $"{optResult.MaxDrawdownPct,10:F2} | " +

                    $"{optResult.TraderId,10} | " +
                    $"{optResult.TraderName,20} | " +
                    $"{optResult.SymbolName,10} | " +
                    $"{optResult.SymbolPeriod,9} | " +
                    $"{optResult.SystemId,10} | " +
                    $"{optResult.SystemName,20} | " +
                    $"{optResult.StrategyId,10} | " +
                    $"{optResult.StrategyName,20} | " +
                    $"{optResult.ToplamBarSayisi,10} | " +
                    $"{optResult.IlkBarTarihSaati,19:yyyy.MM.dd HH:mm:ss} | " +
                    $"{optResult.IlkBarTarihi,10:yyyy.MM.dd} | " +
                    $"{optResult.IlkBarSaati,10} | " +
                    $"{optResult.SonBarTarihSaati,19:yyyy.MM.dd HH:mm:ss} | " +
                    $"{optResult.SonBarTarihi,10:yyyy.MM.dd} | " +
                    $"{optResult.SonBarSaati,10} | " +
                    $"{ "", 20} | "
                );

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
