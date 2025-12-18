namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Take profit and stop loss management
    /// </summary>
    public class KarZarar
    {
        public double TakeProfitPrice { get; set; }
        public double StopLossPrice { get; set; }
        public double TakeProfitPct { get; set; }
        public double StopLossPct { get; set; }
        public bool IsEnabled { get; set; }

        public KarZarar()
        {
            IsEnabled = false;
            TakeProfitPrice = 0;
            StopLossPrice = 0;
            TakeProfitPct = 0;
            StopLossPct = 0;
        }

        /// <summary>
        /// Set take profit and stop loss by percentage
        /// </summary>
        public void SetByPercentage(double takeProfitPct, double stopLossPct)
        {
            TakeProfitPct = takeProfitPct;
            StopLossPct = stopLossPct;
            IsEnabled = true;
        }

        /// <summary>
        /// Set take profit and stop loss by price
        /// </summary>
        public void SetByPrice(double takeProfitPrice, double stopLossPrice)
        {
            TakeProfitPrice = takeProfitPrice;
            StopLossPrice = stopLossPrice;
            IsEnabled = true;
        }

        /// <summary>
        /// Calculate TP/SL prices from entry price
        /// </summary>
        public void CalculatePrices(double entryPrice, PositionType positionType)
        {
            if (!IsEnabled) return;

            if (positionType == PositionType.Long)
            {
                if (TakeProfitPct > 0)
                    TakeProfitPrice = entryPrice * (1 + TakeProfitPct / 100.0);
                if (StopLossPct > 0)
                    StopLossPrice = entryPrice * (1 - StopLossPct / 100.0);
            }
            else if (positionType == PositionType.Short)
            {
                if (TakeProfitPct > 0)
                    TakeProfitPrice = entryPrice * (1 - TakeProfitPct / 100.0);
                if (StopLossPct > 0)
                    StopLossPrice = entryPrice * (1 + StopLossPct / 100.0);
            }
        }

        /// <summary>
        /// Check if take profit is hit
        /// </summary>
        public bool CheckKarAl(double currentPrice, PositionType positionType)
        {
            if (!IsEnabled || TakeProfitPrice == 0) return false;

            if (positionType == PositionType.Long)
                return currentPrice >= TakeProfitPrice;
            else if (positionType == PositionType.Short)
                return currentPrice <= TakeProfitPrice;

            return false;
        }

        /// <summary>
        /// Check if stop loss is hit
        /// </summary>
        public bool CheckZararKes(double currentPrice, PositionType positionType)
        {
            if (!IsEnabled || StopLossPrice == 0) return false;

            if (positionType == PositionType.Long)
                return currentPrice <= StopLossPrice;
            else if (positionType == PositionType.Short)
                return currentPrice >= StopLossPrice;

            return false;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            TakeProfitPrice = 0;
            StopLossPrice = 0;
        }
    }
}
