using System;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Position type
    /// </summary>
    public enum PositionType
    {
        Flat = 0,   // No position
        Long = 1,   // Buy position
        Short = 2   // Sell position
    }

    /// <summary>
    /// Trading position
    /// </summary>
    public class Position
    {
        public PositionType Type { get; set; }
        public double Quantity { get; set; }
        public double EntryPrice { get; set; }
        public double CurrentPrice { get; set; }
        public DateTime EntryTime { get; set; }

        public Position()
        {
            Type = PositionType.Flat;
            Quantity = 0;
            EntryPrice = 0;
            CurrentPrice = 0;
        }

        /// <summary>
        /// Is position open?
        /// </summary>
        public bool IsOpen => Type != PositionType.Flat && Quantity > 0;

        /// <summary>
        /// Is position flat (closed)?
        /// </summary>
        public bool IsFlat => Type == PositionType.Flat || Quantity == 0;

        /// <summary>
        /// Unrealized profit/loss
        /// </summary>
        public double UnrealizedPnL
        {
            get
            {
                if (IsFlat) return 0;

                if (Type == PositionType.Long)
                    return (CurrentPrice - EntryPrice) * Quantity;
                else // Short
                    return (EntryPrice - CurrentPrice) * Quantity;
            }
        }

        /// <summary>
        /// Unrealized profit/loss percentage
        /// </summary>
        public double UnrealizedPnLPct
        {
            get
            {
                if (IsFlat || EntryPrice == 0) return 0;
                return (UnrealizedPnL / (EntryPrice * Quantity)) * 100.0;
            }
        }

        /// <summary>
        /// Open a long position
        /// </summary>
        public void OpenLong(double quantity, double price)
        {
            Type = PositionType.Long;
            Quantity = quantity;
            EntryPrice = price;
            CurrentPrice = price;
            EntryTime = DateTime.Now;
        }

        /// <summary>
        /// Open a short position
        /// </summary>
        public void OpenShort(double quantity, double price)
        {
            Type = PositionType.Short;
            Quantity = quantity;
            EntryPrice = price;
            CurrentPrice = price;
            EntryTime = DateTime.Now;
        }

        /// <summary>
        /// Close position
        /// </summary>
        public void Close()
        {
            Type = PositionType.Flat;
            Quantity = 0;
            EntryPrice = 0;
            CurrentPrice = 0;
        }

        /// <summary>
        /// Update current price
        /// </summary>
        public void UpdatePrice(double price)
        {
            CurrentPrice = price;
        }
    }
}
