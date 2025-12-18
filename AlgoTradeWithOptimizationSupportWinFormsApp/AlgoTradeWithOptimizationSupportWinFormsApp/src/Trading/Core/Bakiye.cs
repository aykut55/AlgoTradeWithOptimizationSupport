namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Account balance and equity management
    /// </summary>
    public class Bakiye
    {
        public double InitialBalance { get; set; }
        public double CurrentBalance { get; set; }
        public double AvailableBalance { get; set; }
        public double Equity { get; set; }
        public double Margin { get; set; }

        public Bakiye(double initialBalance = 100000)
        {
            InitialBalance = initialBalance;
            CurrentBalance = initialBalance;
            AvailableBalance = initialBalance;
            Equity = initialBalance;
            Margin = 0;
        }

        /// <summary>
        /// Total profit/loss
        /// </summary>
        public double TotalPnL => CurrentBalance - InitialBalance;

        /// <summary>
        /// Total profit/loss percentage
        /// </summary>
        public double TotalPnLPct => InitialBalance > 0 ? (TotalPnL / InitialBalance) * 100.0 : 0;

        /// <summary>
        /// Update balance after trade
        /// </summary>
        public void UpdateBalance(double pnl)
        {
            CurrentBalance += pnl;
            AvailableBalance = CurrentBalance - Margin;
            Equity = CurrentBalance;
        }

        /// <summary>
        /// Reserve margin
        /// </summary>
        public void ReserveMargin(double amount)
        {
            Margin += amount;
            AvailableBalance = CurrentBalance - Margin;
        }

        /// <summary>
        /// Release margin
        /// </summary>
        public void ReleaseMargin(double amount)
        {
            Margin -= amount;
            if (Margin < 0) Margin = 0;
            AvailableBalance = CurrentBalance - Margin;
        }

        /// <summary>
        /// Reset to initial state
        /// </summary>
        public void Reset()
        {
            CurrentBalance = InitialBalance;
            AvailableBalance = InitialBalance;
            Equity = InitialBalance;
            Margin = 0;
        }
    }
}
