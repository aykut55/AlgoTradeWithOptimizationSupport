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
            // Yaygın Python kurulum yolları
            string[] possiblePaths = new[]
            {
                @"C:\Python311\python311.dll",
                @"C:\Python310\python310.dll",
                @"C:\Python39\python39.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python311\python311.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python310\python310.dll",
                @"C:\Users\" + Environment.UserName + @"\AppData\Local\Programs\Python\Python39\python39.dll"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                    return path;
            }

            throw new FileNotFoundException(
                "Python DLL bulunamadı! Lütfen Python 3.9+ kurun.\n" +
                "İndirme: https://www.python.org/downloads/"
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
