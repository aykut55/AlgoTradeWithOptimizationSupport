namespace AlgoTradeWithOptimizationSupportWinFormsApp.Indicators.Volatility.Results
{
    /// <summary>
    /// Result container for Bollinger Bands indicator
    /// </summary>
    public class BollingerBandsResult
    {
        /// <summary>Upper band values</summary>
        public double[] Upper { get; set; } = new double[0];

        /// <summary>Middle band (SMA) values</summary>
        public double[] Middle { get; set; } = new double[0];

        /// <summary>Lower band values</summary>
        public double[] Lower { get; set; } = new double[0];

        /// <summary>Bandwidth (Upper - Lower)</summary>
        public double[] Bandwidth { get; set; } = new double[0];

        /// <summary>%B indicator ((Price - Lower) / (Upper - Lower))</summary>
        public double[] PercentB { get; set; } = new double[0];

        /// <summary>Current upper band value</summary>
        public double CurrentUpper => Upper.Length > 0 ? Upper[^1] : double.NaN;

        /// <summary>Current middle band value</summary>
        public double CurrentMiddle => Middle.Length > 0 ? Middle[^1] : double.NaN;

        /// <summary>Current lower band value</summary>
        public double CurrentLower => Lower.Length > 0 ? Lower[^1] : double.NaN;

        /// <summary>Current bandwidth</summary>
        public double CurrentBandwidth => Bandwidth.Length > 0 ? Bandwidth[^1] : double.NaN;

        /// <summary>Current %B value</summary>
        public double CurrentPercentB => PercentB.Length > 0 ? PercentB[^1] : double.NaN;

        /// <summary>Number of data points</summary>
        public int Length => Upper.Length;
    }
}
