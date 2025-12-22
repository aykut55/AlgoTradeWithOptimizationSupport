using System.Collections.Generic;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core
{
    /// <summary>
    /// Trading lists - Stores historical trading data for analysis and statistics
    /// Converted from Python backtesting system
    /// </summary>
    public class Lists
    {
        #region General

        public int BarCount { get; set; }
        public List<int> BarIndexList { get; set; }

        #endregion

        #region Signal Data

        public List<string> YonList { get; set; }
        public List<double> SeviyeList { get; set; }
        public List<double> SinyalList { get; set; }

        #endregion

        #region Profit/Loss Data

        public List<double> KarZararPuanList { get; set; }
        public List<double> KarZararFiyatList { get; set; }
        public List<double> KarZararPuanYuzdeList { get; set; }
        public List<double> KarZararFiyatYuzdeList { get; set; }

        #endregion

        #region Trading Actions

        public List<bool> KarAlList { get; set; }
        public List<double> IzleyenStopList { get; set; }

        #endregion

        #region Trade Counts

        public List<int> IslemSayisiList { get; set; }
        public List<int> AlisSayisiList { get; set; }
        public List<int> SatisSayisiList { get; set; }
        public List<int> FlatSayisiList { get; set; }
        public List<int> PassSayisiList { get; set; }

        #endregion

        #region Position Size

        public List<double> KontratSayisiList { get; set; }
        public List<double> VarlikAdedSayisiList { get; set; }

        #endregion

        #region Commission

        public List<double> KomisyonVarlikAdedSayisiList { get; set; }
        public List<int> KomisyonIslemSayisiList { get; set; }
        public List<double> KomisyonFiyatList { get; set; }

        #endregion

        #region Bar Counts in Profit/Loss

        public List<int> KardaBarSayisiList { get; set; }
        public List<int> ZarardaBarSayisiList { get; set; }

        #endregion

        #region Balance Data

        public List<double> BakiyePuanList { get; set; }
        public List<double> BakiyeFiyatList { get; set; }

        #endregion

        #region Return Data (Getiri)

        public List<double> GetiriPuanList { get; set; }
        public List<double> GetiriFiyatList { get; set; }
        public List<double> GetiriPuanYuzdeList { get; set; }
        public List<double> GetiriFiyatYuzdeList { get; set; }

        #endregion

        #region Net Balance Data

        public List<double> BakiyePuanNetList { get; set; }
        public List<double> BakiyeFiyatNetList { get; set; }

        #endregion

        #region Net Return Data

        public List<double> GetiriPuanNetList { get; set; }
        public List<double> GetiriFiyatNetList { get; set; }
        public List<double> GetiriPuanYuzdeNetList { get; set; }
        public List<double> GetiriFiyatYuzdeNetList { get; set; }

        #endregion

        #region KZ System Data

        public List<double> GetiriKz { get; set; }
        public List<double> GetiriKzNet { get; set; }
        public List<double> GetiriKzSistem { get; set; }
        public List<double> GetiriKzNetSistem { get; set; }

        #endregion

        #region Order Commands

        public List<double> EmirKomutList { get; set; }
        public List<double> EmirStatusList { get; set; }

        #endregion

        #region Constructor

        public Lists()
        {
            BarCount = 0;

            // Initialize all lists
            BarIndexList = new List<int>();
            YonList = new List<string>();
            SeviyeList = new List<double>();
            SinyalList = new List<double>();

            KarZararPuanList = new List<double>();
            KarZararFiyatList = new List<double>();
            KarZararPuanYuzdeList = new List<double>();
            KarZararFiyatYuzdeList = new List<double>();

            KarAlList = new List<bool>();
            IzleyenStopList = new List<double>();

            IslemSayisiList = new List<int>();
            AlisSayisiList = new List<int>();
            SatisSayisiList = new List<int>();
            FlatSayisiList = new List<int>();
            PassSayisiList = new List<int>();

            KontratSayisiList = new List<double>();
            VarlikAdedSayisiList = new List<double>();

            KomisyonVarlikAdedSayisiList = new List<double>();
            KomisyonIslemSayisiList = new List<int>();
            KomisyonFiyatList = new List<double>();

            KardaBarSayisiList = new List<int>();
            ZarardaBarSayisiList = new List<int>();

            BakiyePuanList = new List<double>();
            BakiyeFiyatList = new List<double>();

            GetiriPuanList = new List<double>();
            GetiriFiyatList = new List<double>();
            GetiriPuanYuzdeList = new List<double>();
            GetiriFiyatYuzdeList = new List<double>();

            BakiyePuanNetList = new List<double>();
            BakiyeFiyatNetList = new List<double>();

            GetiriPuanNetList = new List<double>();
            GetiriFiyatNetList = new List<double>();
            GetiriPuanYuzdeNetList = new List<double>();
            GetiriFiyatYuzdeNetList = new List<double>();

            GetiriKz = new List<double>();
            GetiriKzNet = new List<double>();
            GetiriKzSistem = new List<double>();
            GetiriKzNetSistem = new List<double>();

            EmirKomutList = new List<double>();
            EmirStatusList = new List<double>();
        }

        #endregion

        #region Methods

        public Lists Reset()
        {
            // Clear all existing lists before recreating
            ClearAllLists();

            return this;
        }

        private void ClearAllLists()
        {
            BarCount = 0;

            BarIndexList?.Clear();
            YonList?.Clear();
            SeviyeList?.Clear();
            SinyalList?.Clear();

            KarZararPuanList?.Clear();
            KarZararFiyatList?.Clear();
            KarZararPuanYuzdeList?.Clear();
            KarZararFiyatYuzdeList?.Clear();

            KarAlList?.Clear();
            IzleyenStopList?.Clear();

            IslemSayisiList?.Clear();
            AlisSayisiList?.Clear();
            SatisSayisiList?.Clear();
            FlatSayisiList?.Clear();
            PassSayisiList?.Clear();

            KontratSayisiList?.Clear();
            VarlikAdedSayisiList?.Clear();

            KomisyonVarlikAdedSayisiList?.Clear();
            KomisyonIslemSayisiList?.Clear();
            KomisyonFiyatList?.Clear();

            KardaBarSayisiList?.Clear();
            ZarardaBarSayisiList?.Clear();

            BakiyePuanList?.Clear();
            BakiyeFiyatList?.Clear();

            GetiriPuanList?.Clear();
            GetiriFiyatList?.Clear();
            GetiriPuanYuzdeList?.Clear();
            GetiriFiyatYuzdeList?.Clear();

            BakiyePuanNetList?.Clear();
            BakiyeFiyatNetList?.Clear();

            GetiriPuanNetList?.Clear();
            GetiriFiyatNetList?.Clear();
            GetiriPuanYuzdeNetList?.Clear();
            GetiriFiyatYuzdeNetList?.Clear();

            GetiriKz?.Clear();
            GetiriKzNet?.Clear();
            GetiriKzSistem?.Clear();
            GetiriKzNetSistem?.Clear();

            EmirKomutList?.Clear();
            EmirStatusList?.Clear();
        }

        public Lists Init(int barCount)
        {
            CreateLists(barCount);
            return this;
        }
        public void CreateLists(int barCount)
        {
            BarCount = barCount;

            if (barCount > 0)
            {
                // Create lists with default values (Python: [0] * BarCount, [""] * BarCount)
                BarIndexList = new List<int>(new int[barCount]);
                YonList = new List<string>(new string[barCount]);
                SeviyeList = new List<double>(new double[barCount]);
                SinyalList = new List<double>(new double[barCount]);

                KarZararPuanList = new List<double>(new double[barCount]);
                KarZararFiyatList = new List<double>(new double[barCount]);
                KarZararPuanYuzdeList = new List<double>(new double[barCount]);
                KarZararFiyatYuzdeList = new List<double>(new double[barCount]);

                KarAlList = new List<bool>(new bool[barCount]);
                IzleyenStopList = new List<double>(new double[barCount]);

                IslemSayisiList = new List<int>(new int[barCount]);
                AlisSayisiList = new List<int>(new int[barCount]);
                SatisSayisiList = new List<int>(new int[barCount]);
                FlatSayisiList = new List<int>(new int[barCount]);
                PassSayisiList = new List<int>(new int[barCount]);

                KontratSayisiList = new List<double>(new double[barCount]);
                VarlikAdedSayisiList = new List<double>(new double[barCount]);

                KomisyonVarlikAdedSayisiList = new List<double>(new double[barCount]);
                KomisyonIslemSayisiList = new List<int>(new int[barCount]);
                KomisyonFiyatList = new List<double>(new double[barCount]);

                KardaBarSayisiList = new List<int>(new int[barCount]);
                ZarardaBarSayisiList = new List<int>(new int[barCount]);

                BakiyePuanList = new List<double>(new double[barCount]);
                BakiyeFiyatList = new List<double>(new double[barCount]);

                GetiriPuanList = new List<double>(new double[barCount]);
                GetiriFiyatList = new List<double>(new double[barCount]);
                GetiriPuanYuzdeList = new List<double>(new double[barCount]);
                GetiriFiyatYuzdeList = new List<double>(new double[barCount]);

                BakiyePuanNetList = new List<double>(new double[barCount]);
                BakiyeFiyatNetList = new List<double>(new double[barCount]);

                GetiriPuanNetList = new List<double>(new double[barCount]);
                GetiriFiyatNetList = new List<double>(new double[barCount]);
                GetiriPuanYuzdeNetList = new List<double>(new double[barCount]);
                GetiriFiyatYuzdeNetList = new List<double>(new double[barCount]);

                GetiriKz = new List<double>(new double[barCount]);
                GetiriKzNet = new List<double>(new double[barCount]);
                GetiriKzSistem = new List<double>(new double[barCount]);
                GetiriKzNetSistem = new List<double>(new double[barCount]);

                EmirKomutList = new List<double>(new double[barCount]);
                EmirStatusList = new List<double>(new double[barCount]);

                // Initialize string arrays with empty strings (null -> "")
                for (int i = 0; i < barCount; i++)
                {
                    YonList[i] = "";
                    SinyalList[i] = 0.0;
                }
            }
        }

        #endregion
    }
}
