using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Plotting
{
    /// <summary>
    /// Panel bazlı esnek plotting sistemi
    /// Kullanım:
    ///   plotter.AddPanel(0, dates, closes, "Close", "blue");
    ///   plotter.AddPanel(0, dates, ma50, "MA50", "red");
    ///   plotter.AddPanel(1, dates, volumes, "Volume", "gray");
    ///   plotter.Plot();
    /// </summary>
    public class DynamicPlotter : IDisposable
    {
        /// <summary>
        /// Panel içindeki bir seri verisi
        /// </summary>
        public class PanelSeries
        {
            public List<DateTime> Dates { get; set; }
            public List<double> Values { get; set; }
            public string Label { get; set; }
            public string Color { get; set; }
            public string LineStyle { get; set; }
            public double LineWidth { get; set; }

            public PanelSeries()
            {
                LineStyle = "-";  // Solid line
                LineWidth = 1.5;
            }
        }

        private bool _isInitialized = false;
        private readonly string _pythonDllPath;
        private readonly string _scriptDirectory;

        // Panel verilerini tutar: panelIndex -> List of Series
        private Dictionary<int, List<PanelSeries>> _panels = new Dictionary<int, List<PanelSeries>>();

        public DynamicPlotter(string pythonDllPath = null, string scriptDirectory = null)
        {
            _pythonDllPath = pythonDllPath ?? FindPythonDll();
            _scriptDirectory = scriptDirectory ?? Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "src",
                "Plotting"
            );

            InitializePython();
        }

        private string FindPythonDll()
        {
            string[] possiblePaths = new[]
            {
                @"C:\Program Files\Python313\python313.dll",
                @"C:\Python313\python313.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python313\python313.dll",
                @"C:\Program Files\Python312\python312.dll",
                @"C:\Python312\python312.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python312\python312.dll",
                @"C:\Program Files\Python311\python311.dll",
                @"C:\Python311\python311.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python311\python311.dll"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine($"Python DLL found: {path}");
                    return path;
                }
            }

            throw new FileNotFoundException(
                "Python DLL bulunamadı! Lütfen Python 3.11+ kurun.\n" +
                "İndirme: https://www.python.org/downloads/"
            );
        }

        private void InitializePython()
        {
            if (_isInitialized) return;

            try
            {
                if (!File.Exists(_pythonDllPath))
                {
                    throw new FileNotFoundException($"Python DLL bulunamadı: {_pythonDllPath}");
                }

                if (!Directory.Exists(_scriptDirectory))
                {
                    Directory.CreateDirectory(_scriptDirectory);
                }

                Runtime.PythonDLL = _pythonDllPath;
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Python initialization failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Belirtilen panele bir seri ekler
        /// </summary>
        /// <param name="panelIndex">Panel numarası (0'dan başlar)</param>
        /// <param name="dates">Tarih listesi</param>
        /// <param name="values">Değer listesi</param>
        /// <param name="label">Seri adı (legend için)</param>
        /// <param name="color">Renk (opsiyonel: blue, red, green, etc.)</param>
        /// <param name="lineStyle">Çizgi stili (opsiyonel: -, --, :, -.)</param>
        /// <param name="lineWidth">Çizgi kalınlığı (opsiyonel)</param>
        public void AddPanel(int panelIndex, List<DateTime> dates, List<double> values,
                            string label = "", string color = null, string lineStyle = "-", double lineWidth = 1.5)
        {
            if (dates == null || values == null)
                throw new ArgumentNullException("dates veya values null olamaz");

            if (dates.Count != values.Count)
                throw new ArgumentException($"dates ve values boyutları eşit olmalı! dates: {dates.Count}, values: {values.Count}");

            // Panel yoksa oluştur
            if (!_panels.ContainsKey(panelIndex))
                _panels[panelIndex] = new List<PanelSeries>();

            // Seriyi ekle
            _panels[panelIndex].Add(new PanelSeries
            {
                Dates = dates,
                Values = values,
                Label = label ?? "",
                Color = color ?? "blue",
                LineStyle = lineStyle ?? "-",
                LineWidth = lineWidth
            });
        }

        /// <summary>
        /// Tüm panelleri temizler
        /// </summary>
        public void Clear()
        {
            _panels.Clear();
        }

        /// <summary>
        /// Eklenen tüm panelleri çizdirir
        /// </summary>
        public bool Plot(string title = "AlgoTrade Analiz", string savePath = null)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Python not initialized");

            if (_panels.Count == 0)
                throw new InvalidOperationException("Hiç panel eklenmedi! AddPanel() kullanın.");

            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_scriptDirectory);

                    // Matplotlib kontrol
                    try
                    {
                        Py.Import("matplotlib");
                    }
                    catch (PythonException)
                    {
                        throw new Exception("Matplotlib yüklü değil!\npip install matplotlib");
                    }

                    // Plot modülünü import et
                    dynamic plotModule = Py.Import("dynamic_plotter");

                    // Panel verilerini Python dictionary'sine çevir
                    using (var panelDataDict = new PyDict())
                    {
                        foreach (var panelEntry in _panels)
                        {
                            int panelIndex = panelEntry.Key;
                            var seriesList = panelEntry.Value;

                            // Bu panel için series listesi oluştur
                            using (var pySeriesList = new PyList())
                            {
                                foreach (var series in seriesList)
                                {
                                    using (var seriesDict = new PyDict())
                                    {
                                        // Tarihleri string'e çevir
                                        using (var pyDates = new PyList())
                                        {
                                            foreach (var dt in series.Dates)
                                                pyDates.Append(new PyString(dt.ToString("yyyy-MM-dd HH:mm:ss")));

                                            seriesDict["dates"] = pyDates;
                                        }

                                        // Values
                                        using (var pyValues = new PyList())
                                        {
                                            foreach (var val in series.Values)
                                                pyValues.Append(new PyFloat(val));

                                            seriesDict["values"] = pyValues;
                                        }

                                        seriesDict["label"] = new PyString(series.Label);
                                        seriesDict["color"] = new PyString(series.Color);
                                        seriesDict["linestyle"] = new PyString(series.LineStyle);
                                        seriesDict["linewidth"] = new PyFloat(series.LineWidth);

                                        pySeriesList.Append(seriesDict);
                                    }
                                }

                                panelDataDict[new PyInt(panelIndex)] = pySeriesList;
                            }
                        }

                        // Title ve savePath ekle
                        using (var configDict = new PyDict())
                        {
                            configDict["title"] = new PyString(title);
                            if (!string.IsNullOrEmpty(savePath))
                                configDict["save_path"] = new PyString(savePath);

                            // Python fonksiyonunu çağır
                            dynamic result = plotModule.plot_dynamic_panels(panelDataDict, configDict);

                            return (bool)result;
                        }
                    }
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Python plotting error: {pyEx.Message}\n{pyEx.StackTrace}", pyEx);
                }
            }
        }

        public void Dispose()
        {
            if (_isInitialized)
            {
                try
                {
                    PythonEngine.Shutdown();
                }
                catch
                {
                    // Ignore shutdown errors
                }
                _isInitialized = false;
            }
        }
    }
}
