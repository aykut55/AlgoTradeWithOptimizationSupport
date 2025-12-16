using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.ConsoleManagement
{
    /// <summary>
    /// Console Manager - Birden fazla console ekranı yönetir
    ///
    /// KULLANIM:
    ///
    /// // 1. Ana console'u aç (index 0)
    /// ConsoleManager.Instance.OpenConsole();
    /// ConsoleManager.WriteLine("Ana console mesajı");
    ///
    /// // 2. Ek console'lar aç
    /// int idx1 = ConsoleManager.Instance.OpenConsole("Debug Console");
    /// int idx2 = ConsoleManager.Instance.OpenConsole("Trading Console");
    ///
    /// // 3. Belirli console'a yaz
    /// ConsoleManager.WriteLine("Debug mesajı", idx1);
    /// ConsoleManager.WriteLine("Trade mesajı", idx2);
    ///
    /// // 4. Console'u kapat
    /// ConsoleManager.Instance.CloseConsole(idx1);
    /// ConsoleManager.Instance.CloseAllConsoles();
    /// </summary>
    public class ConsoleManager : IDisposable
    {
        #region Singleton Pattern

        private static ConsoleManager? _instance;
        private static readonly object _instanceLock = new object();

        public static ConsoleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConsoleManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private ConsoleManager() { }

        #endregion

        #region Windows API

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleTitle(string title);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate? handlerRoutine, bool add);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int SW_SHOWNORMAL = 1;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;

        // Console control event types
        private const int CTRL_C_EVENT = 0;
        private const int CTRL_BREAK_EVENT = 1;
        private const int CTRL_CLOSE_EVENT = 2;
        private const int CTRL_LOGOFF_EVENT = 5;
        private const int CTRL_SHUTDOWN_EVENT = 6;

        private delegate bool ConsoleCtrlDelegate(int ctrlType);

        #endregion

        #region Fields

        private readonly ConcurrentDictionary<int, ConsoleInfo> _consoles = new ConcurrentDictionary<int, ConsoleInfo>();
        private readonly object _mainConsoleLock = new object();
        private int _nextConsoleIndex = 1; // 0 is reserved for main console
        private bool _mainConsoleAllocated = false;
        private bool _isDisposed = false;
        private ConsoleCtrlDelegate? _consoleCtrlHandler;

        #endregion

        #region Console Info Class

        private class ConsoleInfo
        {
            public int Index { get; set; }
            public string Title { get; set; } = string.Empty;
            public IntPtr Handle { get; set; }
            public Process? Process { get; set; }
            public StreamWriter? Writer { get; set; }
            public bool IsMainConsole { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        #endregion

        #region Public API - Open/Close Console

        /// <summary>
        /// Ana console'u aç (index 0) - AllocConsole ile, kapatma korumalı
        /// </summary>
        public bool OpenMainConsole(string title = "Main Console")
        {
            lock (_mainConsoleLock)
            {
                if (_mainConsoleAllocated)
                    return true; // Already open

                try
                {
                    // AllocConsole ile gerçek console oluştur
                    if (AllocConsole())
                    {
                        _mainConsoleAllocated = true;
                        var handle = GetConsoleWindow();
                        SetConsoleTitle(title);

                        // Console encoding ayarla
                        System.Console.OutputEncoding = Encoding.UTF8;

                        // Console close event handler'ı ekle (console kapatılınca app kapanmasın)
                        _consoleCtrlHandler = new ConsoleCtrlDelegate(ConsoleCtrlCheck);
                        SetConsoleCtrlHandler(_consoleCtrlHandler, true);

                        // NOT: Close butonu aktif - kullanıcı kapatabilir ama app kapanmaz

                        // Console'u görünür yap ve öne getir
                        ShowWindow(handle, SW_SHOWNORMAL);
                        SetForegroundWindow(handle);

                        var consoleInfo = new ConsoleInfo
                        {
                            Index = 0,
                            Title = title,
                            Handle = handle,
                            IsMainConsole = true,
                            CreatedAt = DateTime.Now
                        };

                        _consoles.TryAdd(0, consoleInfo);

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ConsoleManager: Failed to open main console: {ex.Message}");
                }

                return false;
            }
        }

        /// <summary>
        /// Console kapatma event'ini handle et
        /// </summary>
        private bool ConsoleCtrlCheck(int ctrlType)
        {
            // X'e basınca uygulamayı kapat
            switch (ctrlType)
            {
                case CTRL_CLOSE_EVENT:
                    // Console kapatılıyor - uygulamayı da kapat
                    return false; // Event not handled, app will close

                case CTRL_C_EVENT:
                case CTRL_BREAK_EVENT:
                    // Ctrl+C veya Ctrl+Break - izin ver
                    return false;

                default:
                    return false;
            }
        }


        /// <summary>
        /// Console penceresini konumlandır
        /// </summary>
        private void PositionConsoleWindow(Process process, int index)
        {
            try
            {
                var handle = process.MainWindowHandle;
                if (handle == IntPtr.Zero)
                    return;

                // Main Console'un pozisyonunu al
                if (_consoles.TryGetValue(0, out var mainConsole) && mainConsole.Handle != IntPtr.Zero)
                {
                    RECT mainRect;
                    if (GetWindowRect(mainConsole.Handle, out mainRect))
                    {
                        // Main Console'un sağına yerleştir
                        int width = mainRect.Right - mainRect.Left;
                        int newX = mainRect.Right + 10; // 10 pixel boşluk
                        int newY = mainRect.Top + (index - 1) * 50; // Her console biraz aşağıda

                        SetWindowPos(handle, IntPtr.Zero, newX, newY, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConsoleManager: Failed to position console: {ex.Message}");
            }
        }

        /// <summary>
        /// Yeni bir console ekranı aç (cmd.exe process olarak)
        /// </summary>
        public int OpenConsole(string title)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(ConsoleManager));

            int newIndex = _nextConsoleIndex++;

            try
            {
                // Yeni bir cmd.exe process başlat
                var processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/k title {title}",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                var process = Process.Start(processInfo);

                if (process != null)
                {
                    // Process'in başlamasını bekle
                    System.Threading.Thread.Sleep(100);

                    // Process handle'ı alabilmek için biraz daha bekle
                    process.WaitForInputIdle();

                    // Pozisyonu ayarla - Main Console'un yanına koy
                    PositionConsoleWindow(process, newIndex);

                    var consoleInfo = new ConsoleInfo
                    {
                        Index = newIndex,
                        Title = title,
                        Process = process,
                        Writer = process.StandardInput,
                        IsMainConsole = false,
                        CreatedAt = DateTime.Now
                    };

                    _consoles.TryAdd(newIndex, consoleInfo);

                    return newIndex;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConsoleManager: Failed to open console {newIndex}: {ex.Message}");
            }

            return -1; // Failed
        }

        /// <summary>
        /// Console'u kapat
        /// </summary>
        public bool CloseConsole(int index)
        {
            if (!_consoles.TryRemove(index, out var consoleInfo))
                return false;

            try
            {
                if (consoleInfo.IsMainConsole)
                {
                    // Ana console - FreeConsole ile kapat
                    lock (_mainConsoleLock)
                    {
                        if (_mainConsoleAllocated)
                        {
                            // Handler'ı kaldır
                            if (_consoleCtrlHandler != null)
                            {
                                SetConsoleCtrlHandler(_consoleCtrlHandler, false);
                                _consoleCtrlHandler = null;
                            }

                            FreeConsole();
                            _mainConsoleAllocated = false;
                        }
                    }
                }
                else
                {
                    // External process console - cmd.exe'yi kapat
                    consoleInfo.Writer?.Close();

                    if (consoleInfo.Process != null && !consoleInfo.Process.HasExited)
                    {
                        consoleInfo.Process.CloseMainWindow();
                        if (!consoleInfo.Process.WaitForExit(50))
                        {
                            consoleInfo.Process.Kill();
                        }
                        consoleInfo.Process.Dispose();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConsoleManager: Failed to close console {index}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Tüm console'ları kapat
        /// </summary>
        public void CloseAllConsoles()
        {
            var indexes = _consoles.Keys.ToArray();
            foreach (var index in indexes)
            {
                CloseConsole(index);
            }
        }

        #endregion

        #region Public API - Write to Console

        /// <summary>
        /// Ana console'a yaz (index belirtilmezse)
        /// </summary>
        public static void WriteLine(string message, int consoleIndex = 0)
        {
            Instance.WriteLineToConsole(message, consoleIndex);
        }

        /// <summary>
        /// Ana console'a yaz (index belirtilmezse)
        /// </summary>
        public static void Write(string message, int consoleIndex = 0)
        {
            Instance.WriteToConsole(message, consoleIndex);
        }

        /// <summary>
        /// Renkli yazma
        /// </summary>
        public static void WriteLine(string message, ConsoleColor color, int consoleIndex = 0)
        {
            Instance.WriteLineToConsole(message, color, consoleIndex);
        }

        /// <summary>
        /// Console'a satır yaz
        /// </summary>
        private void WriteLineToConsole(string message, int consoleIndex = 0)
        {
            WriteToConsole(message + Environment.NewLine, consoleIndex);
        }

        /// <summary>
        /// Console'a renkli satır yaz
        /// </summary>
        private void WriteLineToConsole(string message, ConsoleColor color, int consoleIndex = 0)
        {
            if (!_consoles.TryGetValue(consoleIndex, out var consoleInfo))
                return;

            try
            {
                if (consoleInfo.IsMainConsole)
                {
                    // Ana console - AllocConsole ile oluşturuldu, System.Console kullan
                    var originalColor = System.Console.ForegroundColor;
                    System.Console.ForegroundColor = color;
                    System.Console.WriteLine(message);
                    System.Console.ForegroundColor = originalColor;
                }
                else
                {
                    // External process console - cmd.exe
                    consoleInfo.Writer?.WriteLine($"echo {message}");
                    consoleInfo.Writer?.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConsoleManager: Failed to write to console {consoleIndex}: {ex.Message}");
            }
        }

        /// <summary>
        /// Console'a yaz
        /// </summary>
        private void WriteToConsole(string message, int consoleIndex = 0)
        {
            if (!_consoles.TryGetValue(consoleIndex, out var consoleInfo))
                return;

            try
            {
                if (consoleInfo.IsMainConsole)
                {
                    // Ana console - AllocConsole ile oluşturuldu, System.Console kullan
                    System.Console.Write(message);
                }
                else
                {
                    // External process console - cmd.exe
                    if (message.Contains(Environment.NewLine) || message.EndsWith("\n"))
                    {
                        consoleInfo.Writer?.Write($"echo {message.Replace(Environment.NewLine, "")}");
                        consoleInfo.Writer?.WriteLine();
                    }
                    else
                    {
                        consoleInfo.Writer?.WriteLine($"set /p=\"{message}\" <nul");
                    }
                    consoleInfo.Writer?.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConsoleManager: Failed to write to console {consoleIndex}: {ex.Message}");
            }
        }

        #endregion

        #region Public API - Console Management

        /// <summary>
        /// Console var mı kontrol et
        /// </summary>
        public bool HasConsole(int index)
        {
            return _consoles.ContainsKey(index);
        }

        /// <summary>
        /// Console'u göster/gizle (cmd.exe process için sınırlı destek)
        /// </summary>
        public void ShowConsole(int index, bool show)
        {
            if (!_consoles.TryGetValue(index, out var consoleInfo))
                return;

            try
            {
                // Process window handle'ını al ve göster/gizle
                if (consoleInfo.Process != null && !consoleInfo.Process.HasExited)
                {
                    var handle = consoleInfo.Process.MainWindowHandle;
                    if (handle != IntPtr.Zero)
                    {
                        ShowWindow(handle, show ? SW_SHOW : SW_HIDE);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConsoleManager: Failed to show/hide console {index}: {ex.Message}");
            }
        }

        /// <summary>
        /// Console'u temizle
        /// </summary>
        public void ClearConsole(int index = 0)
        {
            if (!_consoles.TryGetValue(index, out var consoleInfo))
                return;

            try
            {
                if (consoleInfo.IsMainConsole)
                {
                    // Ana console - System.Console.Clear()
                    System.Console.Clear();
                }
                else
                {
                    // External process console - cls komutu
                    consoleInfo.Writer?.WriteLine("cls");
                    consoleInfo.Writer?.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConsoleManager: Failed to clear console {index}: {ex.Message}");
            }
        }

        /// <summary>
        /// Tüm console'ların listesini al
        /// </summary>
        public List<(int Index, string Title, bool IsMain)> GetConsoleList()
        {
            var list = new List<(int Index, string Title, bool IsMain)>();

            foreach (var kvp in _consoles)
            {
                list.Add((kvp.Key, kvp.Value.Title, kvp.Value.IsMainConsole));
            }

            return list.OrderBy(x => x.Index).ToList();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            CloseAllConsoles();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
