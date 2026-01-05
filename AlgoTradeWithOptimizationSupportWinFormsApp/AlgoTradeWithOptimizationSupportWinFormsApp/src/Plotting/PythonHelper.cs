using Python.Runtime;
using System;
using System.IO;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Plotting
{
    /// <summary>
    /// Basit Python helper class - Test için minimal implementation
    /// </summary>
    public class PythonHelper : IDisposable
    {
        private bool _isInitialized = false;
        private readonly string _pythonDllPath;
        private readonly string _scriptDirectory;

        /// <summary>
        /// Constructor - Python DLL ve script dizinini ayarlar
        /// </summary>
        /// <param name="pythonDllPath">Python DLL yolu (null = otomatik algıla)</param>
        public PythonHelper(string pythonDllPath = null)
        {
            // Python DLL path - varsayılan Python 3.9, 3.10, 3.11 yollarını dene
            _pythonDllPath = pythonDllPath ?? FindPythonDll();

            // Script directory - exe dizini altında src/Plotting
            _scriptDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "src",
                "Plotting"
            );
        }

        /// <summary>
        /// Python DLL'i otomatik olarak bul
        /// </summary>
        private string FindPythonDll()
        {
            // Yaygın Python kurulum yolları (en yeni versiyonlar önce)
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
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python311\python311.dll",
                // Python 3.10
                @"C:\Program Files\Python310\python310.dll",
                @"C:\Python310\python310.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python310\python310.dll",
                // Python 3.9
                @"C:\Program Files\Python39\python39.dll",
                @"C:\Python39\python39.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python39\python39.dll"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    // Bulunan DLL'i log'la (debug için)
                    System.Diagnostics.Debug.WriteLine($"Python DLL found: {path}");
                    return path;
                }
            }

            throw new FileNotFoundException(
                "Python DLL bulunamadı! Lütfen Python 3.9+ kurun.\n" +
                "İndirme: https://www.python.org/downloads/\n\n" +
                "Kontrol edilen yollar:\n" +
                string.Join("\n", possiblePaths.Take(6))
            );
        }

        /// <summary>
        /// Python runtime'ı başlat
        /// </summary>
        public void Initialize()
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

                // Script dizinini kontrol et/oluştur
                if (!Directory.Exists(_scriptDirectory))
                {
                    Directory.CreateDirectory(_scriptDirectory);
                }

                // Python runtime'ı başlat
                Runtime.PythonDLL = _pythonDllPath;
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads(); // Multi-threading desteği

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Python initialization failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Basit test - Python'dan "Hello World" mesajı alır
        /// </summary>
        public string TestHelloWorld(string name = "AlgoTrade")
        {
            if (!_isInitialized)
                Initialize();

            using (Py.GIL())
            {
                try
                {
                    // sys.path'e script dizinini ekle
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_scriptDirectory);

                    // Test modülünü import et
                    dynamic testModule = Py.Import("test_simple");

                    // Python fonksiyonunu çağır
                    dynamic result = testModule.hello_from_python(name);

                    return result.ToString();
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Python error: {pyEx.Message}\n{pyEx.StackTrace}", pyEx);
                }
            }
        }

        /// <summary>
        /// Basit test - İki sayıyı Python'da toplar
        /// </summary>
        public int TestAddNumbers(int a, int b)
        {
            if (!_isInitialized)
                Initialize();

            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_scriptDirectory);

                    dynamic testModule = Py.Import("test_simple");
                    dynamic result = testModule.add_numbers(a, b);

                    return (int)result;
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Python error: {pyEx.Message}", pyEx);
                }
            }
        }

        /// <summary>
        /// Matplotlib test - Basit sin dalgası çizer
        /// </summary>
        public bool TestMatplotlibSineWave()
        {
            if (!_isInitialized)
                Initialize();

            using (Py.GIL())
            {
                try
                {
                    // sys.path'e script dizinini ekle
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_scriptDirectory);

                    // matplotlib modülünü kontrol et
                    try
                    {
                        Py.Import("matplotlib");
                        Py.Import("numpy");
                    }
                    catch (PythonException)
                    {
                        throw new Exception(
                            "Matplotlib veya NumPy yüklü değil!\n\n" +
                            "Lütfen şu komutu çalıştırın:\n" +
                            "pip install matplotlib numpy"
                        );
                    }

                    // Test modülünü import et
                    dynamic testModule = Py.Import("test_matplotlib");

                    // Sin dalgası çiz
                    dynamic result = testModule.plot_simple_sine_wave();

                    return (bool)result;
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Matplotlib test hatası: {pyEx.Message}\n{pyEx.StackTrace}", pyEx);
                }
            }
        }

        /// <summary>
        /// Matplotlib test - Trading simülasyonu çizer
        /// </summary>
        public bool TestMatplotlibTradingSimulation()
        {
            if (!_isInitialized)
                Initialize();

            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_scriptDirectory);

                    // matplotlib kontrol
                    try
                    {
                        Py.Import("matplotlib");
                        Py.Import("numpy");
                    }
                    catch (PythonException)
                    {
                        throw new Exception(
                            "Matplotlib veya NumPy yüklü değil!\n\n" +
                            "Lütfen şu komutu çalıştırın:\n" +
                            "pip install matplotlib numpy"
                        );
                    }

                    dynamic testModule = Py.Import("test_matplotlib");
                    dynamic result = testModule.plot_trading_simulation();

                    return (bool)result;
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Trading simulation test hatası: {pyEx.Message}", pyEx);
                }
            }
        }

        /// <summary>
        /// Basit Close plot testi - Sadece Close değerlerini çizer
        /// </summary>
        public bool TestPlotSimpleClose(System.Collections.Generic.List<System.DateTime> dates, System.Collections.Generic.List<double> closes)
        {
            if (!_isInitialized)
                Initialize();

            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_scriptDirectory);

                    // matplotlib kontrol
                    try
                    {
                        Py.Import("matplotlib");
                    }
                    catch (PythonException)
                    {
                        throw new Exception(
                            "Matplotlib yüklü değil!\n\n" +
                            "Lütfen şu komutu çalıştırın:\n" +
                            "pip install matplotlib"
                        );
                    }

                    // Test modülünü import et
                    dynamic testModule = Py.Import("test_simple_plot");

                    // Tarihleri string'e çevir
                    var dateStrings = new System.Collections.Generic.List<string>();
                    foreach (var dt in dates)
                    {
                        dateStrings.Add(dt.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                    // Python listelerine çevir
                    using (var pyDates = new PyList())
                    using (var pyCloses = new PyList())
                    {
                        foreach (var date in dateStrings)
                            pyDates.Append(new PyString(date));

                        foreach (var close in closes)
                            pyCloses.Append(new PyFloat(close));

                        // Python fonksiyonunu çağır
                        dynamic result = testModule.plot_close_only(pyDates, pyCloses);

                        return (bool)result;
                    }
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Simple plot test hatası: {pyEx.Message}\n{pyEx.StackTrace}", pyEx);
                }
            }
        }

        /// <summary>
        /// 6+ panelli grafik - TÜM trading listeleri
        /// </summary>
        public bool TestPlot6Panels(
            System.Collections.Generic.List<System.DateTime> dates,
            System.Collections.Generic.List<double> closes,
            System.Collections.Generic.List<double> volumes,
            System.Collections.Generic.List<double> sinyalList,
            System.Collections.Generic.List<double> karZararFiyatList,
            System.Collections.Generic.List<double> karZararFiyatYuzdeList,
            System.Collections.Generic.List<double> bakiyeFiyatList,
            System.Collections.Generic.List<double> getiriFiyatList,
            System.Collections.Generic.List<double> getiriFiyatYuzdeList,
            System.Collections.Generic.List<double> komisyonFiyatList,
            System.Collections.Generic.List<double> getiriFiyatNetList,
            System.Collections.Generic.List<double> bakiyeFiyatNetList,
            System.Collections.Generic.List<double> getiriFiyatYuzdeNetList)
        {
            if (!_isInitialized)
                Initialize();

            using (Py.GIL())
            {
                try
                {
                    dynamic sys = Py.Import("sys");
                    sys.path.append(_scriptDirectory);

                    // matplotlib kontrol
                    try
                    {
                        Py.Import("matplotlib");
                    }
                    catch (PythonException)
                    {
                        throw new Exception(
                            "Matplotlib yüklü değil!\n\n" +
                            "Lütfen şu komutu çalıştırın:\n" +
                            "pip install matplotlib"
                        );
                    }

                    // Test modülünü import et
                    dynamic testModule = Py.Import("test_simple_plot");

                    // Tarihleri string'e çevir
                    var dateStrings = new System.Collections.Generic.List<string>();
                    foreach (var dt in dates)
                    {
                        dateStrings.Add(dt.ToString("yyyy-MM-dd HH:mm:ss"));
                    }

                    // Python listelerine çevir - TÜM listeleri
                    using (var pyDates = new PyList())
                    using (var pyCloses = new PyList())
                    using (var pyVolumes = new PyList())
                    using (var pySinyal = new PyList())
                    using (var pyKarZararFiyat = new PyList())
                    using (var pyKarZararFiyatYuzde = new PyList())
                    using (var pyBakiyeFiyat = new PyList())
                    using (var pyGetiriFiyat = new PyList())
                    using (var pyGetiriFiyatYuzde = new PyList())
                    using (var pyKomisyonFiyat = new PyList())
                    using (var pyGetiriFiyatNet = new PyList())
                    using (var pyBakiyeFiyatNet = new PyList())
                    using (var pyGetiriFiyatYuzdeNet = new PyList())
                    {
                        foreach (var date in dateStrings)
                            pyDates.Append(new PyString(date));

                        foreach (var close in closes)
                            pyCloses.Append(new PyFloat(close));

                        foreach (var vol in volumes)
                            pyVolumes.Append(new PyFloat(vol));

                        foreach (var sinyal in sinyalList)
                            pySinyal.Append(new PyFloat(sinyal));

                        foreach (var kz in karZararFiyatList)
                            pyKarZararFiyat.Append(new PyFloat(kz));

                        foreach (var kzy in karZararFiyatYuzdeList)
                            pyKarZararFiyatYuzde.Append(new PyFloat(kzy));

                        foreach (var bakiye in bakiyeFiyatList)
                            pyBakiyeFiyat.Append(new PyFloat(bakiye));

                        foreach (var getiri in getiriFiyatList)
                            pyGetiriFiyat.Append(new PyFloat(getiri));

                        foreach (var getiriy in getiriFiyatYuzdeList)
                            pyGetiriFiyatYuzde.Append(new PyFloat(getiriy));

                        foreach (var kom in komisyonFiyatList)
                            pyKomisyonFiyat.Append(new PyFloat(kom));

                        foreach (var getirin in getiriFiyatNetList)
                            pyGetiriFiyatNet.Append(new PyFloat(getirin));

                        foreach (var bakiyen in bakiyeFiyatNetList)
                            pyBakiyeFiyatNet.Append(new PyFloat(bakiyen));

                        foreach (var getiriyn in getiriFiyatYuzdeNetList)
                            pyGetiriFiyatYuzdeNet.Append(new PyFloat(getiriyn));

                        // Python fonksiyonunu çağır - TÜM parametreleri gönder
                        dynamic result = testModule.plot_6_panels(
                            pyDates,
                            pyCloses,
                            pyVolumes,
                            pySinyal,
                            pyKarZararFiyat,
                            pyKarZararFiyatYuzde,
                            pyBakiyeFiyat,
                            pyGetiriFiyat,
                            pyGetiriFiyatYuzde,
                            pyKomisyonFiyat,
                            pyGetiriFiyatNet,
                            pyBakiyeFiyatNet,
                            pyGetiriFiyatYuzdeNet
                        );

                        return (bool)result;
                    }
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"6 panel plot hatası: {pyEx.Message}\n{pyEx.StackTrace}", pyEx);
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
                    // Shutdown hatalarını yoksay
                }
                _isInitialized = false;
            }
        }
    }
}
