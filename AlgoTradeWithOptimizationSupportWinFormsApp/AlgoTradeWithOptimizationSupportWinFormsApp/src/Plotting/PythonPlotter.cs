using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Plotting
{
    /// <summary>
    /// Python matplotlib ile gerçek trading grafikleri çizimi için wrapper class
    /// Python.NET kullanarak doğrudan bellek üzerinden veri aktarımı yapar
    /// </summary>
    public class PythonPlotter : IDisposable
    {
        private bool _isInitialized = false;
        private dynamic? _plotModule;
        private readonly string _pythonDllPath;
        private readonly string _scriptDirectory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pythonDllPath">Python DLL path (null = otomatik algıla)</param>
        /// <param name="scriptDirectory">Python script dizini (null = varsayılan)</param>
        public PythonPlotter(string? pythonDllPath = null, string? scriptDirectory = null)
        {
            // Python DLL path
            _pythonDllPath = pythonDllPath ?? FindPythonDll();

            // Script directory - varsayılan: exe dizini altında src/Plotting
            _scriptDirectory = scriptDirectory ?? Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "src",
                "Plotting"
            );

            InitializePython();
        }

        /// <summary>
        /// Python DLL'i otomatik bul
        /// </summary>
        private string FindPythonDll()
        {
            string[] possiblePaths = new[]
            {
                // Python 3.13
                @"C:\Program Files\Python313\python313.dll",
                @"C:\Python313\python313.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python313\python313.dll",
                // Python 3.12
                @"C:\Program Files\Python312\python312.dll",
                @"C:\Python312\python312.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python312\python312.dll",
                // Python 3.11
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

        /// <summary>
        /// Python runtime'ı başlat
        /// </summary>
        private void InitializePython()
        {
            if (_isInitialized) return;

            try
            {
                // Python DLL kontrolü
                if (!File.Exists(_pythonDllPath))
                {
                    throw new FileNotFoundException(
                        $"Python DLL bulunamadı: {_pythonDllPath}\n" +
                        "Lütfen Python kurulumunu kontrol edin."
                    );
                }

                // Script dizinini kontrol et
                if (!Directory.Exists(_scriptDirectory))
                {
                    Directory.CreateDirectory(_scriptDirectory);
                    throw new DirectoryNotFoundException(
                        $"Python script dizini oluşturuldu ancak boş: {_scriptDirectory}\n" +
                        "Lütfen plot_results.py dosyasını bu dizine kopyalayın."
                    );
                }

                // Python runtime'ı başlat
                Runtime.PythonDLL = _pythonDllPath;
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads(); // Multi-threading desteği

                using (Py.GIL())
                {
                    // sys.path'e script dizinini ekle
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_scriptDirectory);

                    // Gerekli modülleri kontrol et
                    try
                    {
                        Py.Import("matplotlib");
                        Py.Import("numpy");
                    }
                    catch (PythonException)
                    {
                        throw new Exception(
                            "Python modülleri eksik! Lütfen şu komutları çalıştırın:\n" +
                            "pip install matplotlib numpy"
                        );
                    }

                    // Plot modülünü import et
                    _plotModule = Py.Import("plot_results");
                }

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Python initialization failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Trading sonuçlarını görselleştirir
        /// </summary>
        public void PlotTradingResults(
            List<DateTime> dates,
            List<double> prices,
            List<(int index, double price)> buySignals,
            List<(int index, double price)> sellSignals,
            List<double> balance,
            List<double> pnl,
            List<double>? emaFast = null,
            List<double>? emaSlow = null,
            int fastPeriod = 10,
            int slowPeriod = 20,
            string? savePath = null)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Python not initialized");

            if (dates == null || dates.Count == 0)
                throw new ArgumentException("dates listesi boş olamaz");

            using (Py.GIL())
            {
                try
                {
                    // Python dictionary oluştur
                    using (var pyDict = new PyDict())
                    {
                        // Tarihleri ISO format string'e çevir
                        var dateStrings = dates.Select(d => d.ToString("yyyy-MM-ddTHH:mm:ss")).ToArray();
                        pyDict["dates"] = ToPythonList(dateStrings);

                        // Fiyat verileri
                        pyDict["prices"] = ToPythonList(prices);
                        pyDict["balance"] = ToPythonList(balance);
                        pyDict["pnl"] = ToPythonList(pnl);

                        // EMA verileri (varsa)
                        if (emaFast != null && emaFast.Count > 0)
                            pyDict["ema_fast"] = ToPythonList(emaFast);

                        if (emaSlow != null && emaSlow.Count > 0)
                            pyDict["ema_slow"] = ToPythonList(emaSlow);

                        pyDict["fast_period"] = new PyInt(fastPeriod);
                        pyDict["slow_period"] = new PyInt(slowPeriod);

                        // Al/Sat sinyallerini tuple list olarak aktar
                        if (buySignals != null && buySignals.Count > 0)
                        {
                            using (var buyList = new PyList())
                            {
                                foreach (var signal in buySignals)
                                {
                                    using (var tuple = new PyTuple(new PyObject[]
                                    {
                                        new PyInt(signal.index),
                                        new PyFloat(signal.price)
                                    }))
                                    {
                                        buyList.Append(tuple);
                                    }
                                }
                                pyDict["buy_signals"] = buyList;
                            }
                        }

                        if (sellSignals != null && sellSignals.Count > 0)
                        {
                            using (var sellList = new PyList())
                            {
                                foreach (var signal in sellSignals)
                                {
                                    using (var tuple = new PyTuple(new PyObject[]
                                    {
                                        new PyInt(signal.index),
                                        new PyFloat(signal.price)
                                    }))
                                    {
                                        sellList.Append(tuple);
                                    }
                                }
                                pyDict["sell_signals"] = sellList;
                            }
                        }

                        // Dosya kayıt path (opsiyonel)
                        if (!string.IsNullOrEmpty(savePath))
                        {
                            pyDict["save_path"] = new PyString(savePath);
                        }

                        // Python fonksiyonunu çağır
                        _plotModule!.plot_trading_results(pyDict);
                    }
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Python plotting error: {pyEx.Message}\n{pyEx.StackTrace}", pyEx);
                }
            }
        }

        /// <summary>
        /// Equity curve ve drawdown çizer
        /// </summary>
        public void PlotEquityCurveWithDrawdown(List<double> balance, List<DateTime> dates)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Python not initialized");

            using (Py.GIL())
            {
                try
                {
                    var dateStrings = dates.Select(d => d.ToString("yyyy-MM-ddTHH:mm:ss")).ToArray();

                    _plotModule!.plot_equity_curve_with_drawdown(
                        ToPythonList(balance),
                        ToPythonList(dateStrings)
                    );
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Python plotting error: {pyEx.Message}", pyEx);
                }
            }
        }

        /// <summary>
        /// C# List'i Python list'e çevirir (optimize edilmiş)
        /// </summary>
        private PyObject ToPythonList<T>(IEnumerable<T> list)
        {
            var pyList = new PyList();
            foreach (var item in list)
            {
                if (item is int intVal)
                    pyList.Append(new PyInt(intVal));
                else if (item is double doubleVal)
                    pyList.Append(new PyFloat(doubleVal));
                else if (item is float floatVal)
                    pyList.Append(new PyFloat(floatVal));
                else if (item is string strVal)
                    pyList.Append(new PyString(strVal));
                else
                    pyList.Append(item!.ToPython());
            }
            return pyList;
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
                    // Shutdown hatalarını yoksay
                }
                _isInitialized = false;
            }
        }
    }
}
