using System;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Order types
    /// </summary>
    public enum OrderType
    {
        Market,
        Limit,
        Stop,
        StopLimit
    }

    /// <summary>
    /// Order side
    /// </summary>
    public enum OrderSide
    {
        Buy,
        Sell
    }

    /// <summary>
    /// Order status
    /// </summary>
    public enum OrderStatus
    {
        Pending,
        Filled,
        PartiallyFilled,
        Cancelled,
        Rejected
    }

    /// <summary>
    /// Trading order
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public OrderType Type { get; set; }
        public OrderSide Side { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FilledAt { get; set; }
        public double FilledQuantity { get; set; }
        public double FilledPrice { get; set; }

        public Order()
        {
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.Now;
        }

        public bool IsFilled() => Status == OrderStatus.Filled;
        public bool IsPending() => Status == OrderStatus.Pending;
    }
}
