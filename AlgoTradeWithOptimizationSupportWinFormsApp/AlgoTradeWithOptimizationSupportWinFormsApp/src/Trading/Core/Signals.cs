namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Trading signals - Tracks current and previous trading signals and states
    /// Converted from Python backtesting system
    /// </summary>
    public class Signals
    {
        #region Signal Flags

        public bool Al { get; set; }
        public bool Sat { get; set; }
        public bool FlatOl { get; set; }
        public bool PasGec { get; set; }
        public bool KarAl { get; set; }
        public bool ZararKes { get; set; }

        #endregion

        #region Signal Info

        public string Sinyal { get; set; } = "";

        #endregion

        #region Direction

        public string SonYon { get; set; } = "F";
        public string PrevYon { get; set; } = "F";

        #endregion

        #region Current Prices (Son)

        public double SonFiyat { get; set; }
        public double SonAFiyat { get; set; }
        public double SonSFiyat { get; set; }
        public double SonFFiyat { get; set; }
        public double SonPFiyat { get; set; }

        #endregion

        #region Previous Prices (Prev)

        public double PrevFiyat { get; set; }
        public double PrevAFiyat { get; set; }
        public double PrevSFiyat { get; set; }
        public double PrevFFiyat { get; set; }
        public double PrevPFiyat { get; set; }

        #endregion

        #region Current Bar Numbers (Son)

        public int SonBarNo { get; set; }
        public int SonABarNo { get; set; }
        public int SonSBarNo { get; set; }
        public int SonFBarNo { get; set; }
        public int SonPBarNo { get; set; }

        #endregion

        #region Previous Bar Numbers (Prev)

        public int PrevBarNo { get; set; }
        public int PrevABarNo { get; set; }
        public int PrevSBarNo { get; set; }
        public int PrevFBarNo { get; set; }
        public int PrevPBarNo { get; set; }

        #endregion

        #region Order Status

        public double EmirKomut { get; set; }
        public double EmirStatus { get; set; }

        #endregion

        #region Constructor

        public Signals()
        {
            // Initialize with default values
            Reset();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset all signal values to defaults
        /// </summary>
        public Signals Reset()
        {
            Al = false;
            Sat = false;
            FlatOl = false;
            PasGec = false;
            KarAl = false;
            ZararKes = false;

            Sinyal = "";

            SonYon = "F";
            PrevYon = "F";

            SonFiyat = 0.0;
            SonAFiyat = 0.0;
            SonSFiyat = 0.0;
            SonFFiyat = 0.0;
            SonPFiyat = 0.0;

            PrevFiyat = 0.0;
            PrevAFiyat = 0.0;
            PrevSFiyat = 0.0;
            PrevFFiyat = 0.0;
            PrevPFiyat = 0.0;

            SonBarNo = 0;
            SonABarNo = 0;
            SonSBarNo = 0;
            SonFBarNo = 0;
            SonPBarNo = 0;

            PrevBarNo = 0;
            PrevABarNo = 0;
            PrevSBarNo = 0;
            PrevFBarNo = 0;
            PrevPBarNo = 0;

            EmirKomut = 0.0;
            EmirStatus = 0.0;

            return this;
        }

        /// <summary>
        /// Initialize signals - Does nothing currently but returns this for method chaining
        /// </summary>
        public Signals Init()
        {
            return this;
        }

        #endregion
    }
}
