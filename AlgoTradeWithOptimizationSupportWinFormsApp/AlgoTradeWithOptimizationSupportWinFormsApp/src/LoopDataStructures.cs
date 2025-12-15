using System;
using System.Collections.Generic;

namespace AlgoTradeWithOptimizationSupportWinFormsApp
{
    /// <summary>
    /// Main loop içinde kullanılan veri yapıları
    /// </summary>

    // ========================================================================
    // GUI'den okunan veriler
    // ========================================================================
    public class GuiState
    {
        public int ActiveTabIndex { get; set; }
        public string? ActiveTabName { get; set; }
        public bool IsStrategyRunning { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public DateTime LastReadTime { get; set; }
    }

    // ========================================================================
    // Network'ten gelen market data
    // ========================================================================
    public class MarketData
    {
        public string Symbol { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
    }

    // ========================================================================
    // Config ayarları
    // ========================================================================
    public class AppConfig
    {
        public int LoopDelayMs { get; set; } = 10;
        public string LogFilePath { get; set; } = "logs/app.log";
        public bool EnableNetworking { get; set; } = false;
        public bool EnableFileLogging { get; set; } = true;
        public Dictionary<string, object> StrategyParams { get; set; } = new Dictionary<string, object>();
    }

    // ========================================================================
    // Business logic çıktıları (Trading sinyalleri)
    // ========================================================================
    public class TradingSignal
    {
        public string Symbol { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public SignalType Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = "";
    }

    public enum SignalType
    {
        None,
        Buy,
        Sell,
        Hold
    }

    // ========================================================================
    // Network'e gönderilecek emirler
    // ========================================================================
    public class Order
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public string Symbol { get; set; } = "";
        public OrderSide Side { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum OrderSide
    {
        Buy,
        Sell
    }

    public enum OrderStatus
    {
        Pending,
        Sent,
        Filled,
        Rejected,
        Cancelled
    }

    // ========================================================================
    // NOT: LogEntry ve LogLevel artık Logging namespace'inden geliyor
    // AlgoTradeWithOptimizationSupportWinFormsApp.Logging.LogEntry
    // AlgoTradeWithOptimizationSupportWinFormsApp.Logging.LogLevel
    // ========================================================================

    // ========================================================================
    // GUI'ye gönderilecek update verisi
    // ========================================================================
    public class GuiUpdate
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string TargetControl { get; set; } = "";  // Hangi kontrolü güncelleyeceğiz?
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }

    // ========================================================================
    // Loop performans metrikleri
    // ========================================================================
    public class LoopMetrics
    {
        public long TotalIterations { get; set; }
        public long SuccessfulIterations { get; set; }
        public long FailedIterations { get; set; }
        public TimeSpan TotalRuntime { get; set; }
        public TimeSpan AverageIterationTime { get; set; }
        public TimeSpan LastIterationTime { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? LastIterationAt { get; set; }

        public double IterationsPerSecond => TotalRuntime.TotalSeconds > 0
            ? TotalIterations / TotalRuntime.TotalSeconds
            : 0;
    }
}
