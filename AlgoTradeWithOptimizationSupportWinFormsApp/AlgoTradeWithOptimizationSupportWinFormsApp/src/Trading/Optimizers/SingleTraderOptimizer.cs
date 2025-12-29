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
        public Dictionary<string, object> Parameters { get; set; }
        public double NetProfit { get; set; }
        public double WinRate { get; set; }
        public double ProfitFactor { get; set; }
        public double MaxDrawdown { get; set; }
        public double SharpeRatio { get; set; }

        public OptimizationResult()
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

                Logger?.Log($"Testing combination {currentCombination}/{totalCombinations} ({progressPercent:F1}%) [Effective: {effectiveCombinationCount}]: {paramsStr}");

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

                OptimizationSummary optSummary = singleTrader.statistics.GetOptimizationSummary();








                // Store result
                var result = new OptimizationResult
                {
                    NetProfit = singleTrader.status.GetiriFiyatNet,
                    WinRate = singleTrader.statistics.KazandiranIslemSayisi > 0
                        ? (double)singleTrader.statistics.KazandiranIslemSayisi / (singleTrader.statistics.KazandiranIslemSayisi + singleTrader.statistics.KaybettirenIslemSayisi) * 100.0
                        : 0.0,
                    ProfitFactor = singleTrader.status.ToplamZararFiyat != 0
                        ? Math.Abs(singleTrader.status.ToplamKarFiyat / singleTrader.status.ToplamZararFiyat)
                        : 0.0,
                    MaxDrawdown = singleTrader.statistics.GetiriMaxDD
                };

                // Add all parameters to result (generic)
                foreach (var kvp in paramCombo)
                {
                    result.Parameters[kvp.Key] = kvp.Value;
                }

                Results.Add(result);

                Logger?.Log($"  → NetProfit: {result.NetProfit:F2}, WinRate: {result.WinRate:F2}%, PF: {result.ProfitFactor:F2}");











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
