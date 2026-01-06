using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Plotting
{
    /// <summary>
    /// ImGui/ImPlot tabanlı plotting sistemi
    /// AlgoTradeWithPythonWithGemini projesindeki plotDataImgBundle yapısına benzer
    ///
    /// Kullanım:
    ///   using (var plotter = new ImGuiPlotter())
    ///   {
    ///       plotter.PlotDataBundle(dates, opens, highs, lows, closes, volumes, lots,
    ///                              sinyalList, karZararList, bakiyeList, ...);
    ///   }
    /// </summary>
    public class ImGuiPlotter : IDisposable
    {
        private bool _isInitialized = false;
        private readonly string _pythonDllPath;
        private readonly string _scriptDirectory;

        // Global Python initialization state (singleton pattern)
        private static bool _globalPythonInitialized = false;
        private static readonly object _pythonInitLock = new object();

        public ImGuiPlotter(string pythonDllPath = null, string scriptDirectory = null)
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
            // Check local instance state first (fast path)
            if (_isInitialized) return;

            // Thread-safe global initialization (only once per application lifetime)
            lock (_pythonInitLock)
            {
                // Double-check pattern: another thread might have initialized while we were waiting
                if (_globalPythonInitialized)
                {
                    _isInitialized = true;
                    return;
                }

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

                    // Initialize Python engine ONLY ONCE per application lifetime
                    Runtime.PythonDLL = _pythonDllPath;
                    PythonEngine.Initialize();
                    PythonEngine.BeginAllowThreads();

                    // AlgoTradeWithPythonWithGemini projesinin venv'ini kullan (eğer varsa)
                    using (Py.GIL())
                    {
                        dynamic sys = Py.Import("sys");

                        // 1. Venv site-packages yolu ekle
                        string[] venvPaths = new[]
                        {
                            @"D:\Aykut\Projects\AlgoTradeWithPaythonWithGemini\venv\Lib\site-packages",
                            @"D:\Aykut\Projects\AlgoTradeWithPaythonWithGemini\.venv\Lib\site-packages",
                            @"D:\Aykut\Projects\AlgoTradeWithPaythonWithGemini\Aykut\venv\Lib\site-packages",
                        };

                        foreach (var venvPath in venvPaths)
                        {
                            if (Directory.Exists(venvPath))
                            {
                                sys.path.insert(0, venvPath);
                                System.Diagnostics.Debug.WriteLine($"✓ Venv site-packages eklendi: {venvPath}");
                                break;
                            }
                        }

                        // 2. AlgoTradeWithPythonWithGemini/src yolu ekle (DataPlotterImgBundle.py için)
                        string srcPath = @"D:\Aykut\Projects\AlgoTradeWithPaythonWithGemini\src";
                        if (Directory.Exists(srcPath))
                        {
                            sys.path.insert(0, srcPath);
                            System.Diagnostics.Debug.WriteLine($"✓ AlgoTradeWithPythonWithGemini/src eklendi: {srcPath}");
                        }
                    }

                    // Mark as globally initialized
                    _globalPythonInitialized = true;
                    _isInitialized = true;

                    System.Diagnostics.Debug.WriteLine("✓ Python Engine initialized successfully (global singleton)");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Python initialization failed: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// ImGui/ImPlot ile 5 panelli grafik çizdirir
        /// AlgoTradeWithPythonWithGemini'deki plotDataImgBundle yapısına benzer
        /// </summary>
        /// <param name="dates">Tarih listesi</param>
        /// <param name="opens">Open fiyatları</param>
        /// <param name="highs">High fiyatları</param>
        /// <param name="lows">Low fiyatları</param>
        /// <param name="closes">Close fiyatları</param>
        /// <param name="volumes">Volume listesi</param>
        /// <param name="lots">Lot listesi</param>
        /// <param name="sinyalList">Trading sinyalleri (-1, 0, 1)</param>
        /// <param name="karZararFiyatList">Kar/Zarar fiyat listesi</param>
        /// <param name="bakiyeFiyatList">Bakiye listesi</param>
        /// <param name="getiriFiyatList">Brüt getiri listesi</param>
        /// <param name="getiriFiyatNetList">Net getiri listesi</param>
        /// <param name="title">Grafik başlığı (opsiyonel)</param>
        /// <param name="periyot">Periyot bilgisi (opsiyonel)</param>
        /// <returns>True if successful</returns>
        public bool PlotDataBundle(
            List<DateTime> dates,
            List<double> opens,
            List<double> highs,
            List<double> lows,
            List<double> closes,
            List<long> volumes,
            List<long> lots,
            List<double> sinyalList,
            List<double> karZararFiyatList,
            List<double> bakiyeFiyatList,
            List<double> getiriFiyatList,
            List<double> getiriFiyatNetList,
            string title = "AlgoTrade",
            string periyot = "1H")
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Python not initialized");

            // Veri kontrolü
            if (dates == null || closes == null || dates.Count == 0)
                throw new ArgumentException("dates ve closes verileri gerekli!");

            int dataCount = dates.Count;
            if (opens.Count != dataCount || highs.Count != dataCount ||
                lows.Count != dataCount || closes.Count != dataCount)
            {
                throw new ArgumentException($"OHLC verileri aynı boyutta olmalı! dates: {dataCount}");
            }

            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_scriptDirectory);

                    // imgui_bundle kontrolü
                    try
                    {
                        Py.Import("imgui_bundle");
                    }
                    catch (PythonException)
                    {
                        throw new Exception(
                            "imgui_bundle yüklü değil!\n\n" +
                            "Lütfen şu komutu çalıştırın:\n" +
                            "pip install imgui-bundle"
                        );
                    }

                    // plotDataImgBundleNew modülünü import et
                    dynamic plotModule = Py.Import("plotDataImgBundleNew");

                    // Tarihleri string'e çevir ("YYYY.MM.DD HH:MM:SS" formatı)
                    var dateStrings = new List<string>();
                    foreach (var dt in dates)
                    {
                        dateStrings.Add(dt.ToString("yyyy.MM.dd HH:mm:ss"));
                    }

                    // Python listelerine çevir
                    using (var pyDates = new PyList())
                    using (var pyOpens = new PyList())
                    using (var pyHighs = new PyList())
                    using (var pyLows = new PyList())
                    using (var pyCloses = new PyList())
                    using (var pyVolumes = new PyList())
                    using (var pyLots = new PyList())
                    using (var pySinyal = new PyList())
                    using (var pyKarZarar = new PyList())
                    using (var pyBakiye = new PyList())
                    using (var pyGetiri = new PyList())
                    using (var pyGetiriNet = new PyList())
                    {
                        // Tarihleri ekle
                        foreach (var date in dateStrings)
                            pyDates.Append(new PyString(date));

                        // OHLC verileri
                        foreach (var val in opens) pyOpens.Append(new PyFloat(val));
                        foreach (var val in highs) pyHighs.Append(new PyFloat(val));
                        foreach (var val in lows) pyLows.Append(new PyFloat(val));
                        foreach (var val in closes) pyCloses.Append(new PyFloat(val));

                        // Volume/Lot
                        foreach (var val in volumes) pyVolumes.Append(new PyFloat(val));
                        foreach (var val in lots) pyLots.Append(new PyFloat(val));

                        // Trading verileri
                        foreach (var val in sinyalList) pySinyal.Append(new PyFloat(val));
                        foreach (var val in karZararFiyatList) pyKarZarar.Append(new PyFloat(val));
                        foreach (var val in bakiyeFiyatList) pyBakiye.Append(new PyFloat(val));
                        foreach (var val in getiriFiyatList) pyGetiri.Append(new PyFloat(val));
                        foreach (var val in getiriFiyatNetList) pyGetiriNet.Append(new PyFloat(val));

                        // Python stdout'u yakala
                        dynamic io = Py.Import("io");
                        dynamic stdout = io.StringIO();
                        dynamic sys_module = sys;
                        dynamic old_stdout = sys_module.stdout;
                        sys_module.stdout = stdout;

                        try
                        {
                            // Python fonksiyonunu çağır
                            dynamic result = plotModule.plot_data_img_bundle_new(
                                pyDates,
                                pyOpens,
                                pyHighs,
                                pyLows,
                                pyCloses,
                                pyVolumes,
                                pyLots,
                                pySinyal,
                                pyKarZarar,
                                pyBakiye,
                                pyGetiri,
                                pyGetiriNet,
                                bakiye_fiyat_net_list: null,      // Optional
                                kar_zarar_fiyat_yuzde_list: null, // Optional
                                getiri_fiyat_yuzde_list: null,    // Optional
                                komisyon_fiyat_list: null,        // Optional
                                getiri_fiyat_yuzde_net_list: null,// Optional
                                title: title,
                                periyot: periyot
                            );

                            // Python stdout'u al
                            string pythonOutput = stdout.getvalue().ToString();
                            if (!string.IsNullOrEmpty(pythonOutput))
                            {
                                System.Diagnostics.Debug.WriteLine("=== PYTHON OUTPUT ===");
                                System.Diagnostics.Debug.WriteLine(pythonOutput);
                                System.Diagnostics.Debug.WriteLine("=== END PYTHON OUTPUT ===");
                            }

                            return (bool)result;
                        }
                        finally
                        {
                            // stdout'u geri yükle
                            sys_module.stdout = old_stdout;
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
            // IMPORTANT: Do NOT call PythonEngine.Shutdown() here!
            // Python engine is managed as a singleton and should remain initialized
            // for the application lifetime. Multiple ImGuiPlotter instances can be created
            // and disposed, but the Python engine should only be initialized once.

            // Simply mark this instance as disposed
            if (_isInitialized)
            {
                _isInitialized = false;
                System.Diagnostics.Debug.WriteLine("ImGuiPlotter instance disposed (Python engine remains active)");
            }
        }
    }
}
