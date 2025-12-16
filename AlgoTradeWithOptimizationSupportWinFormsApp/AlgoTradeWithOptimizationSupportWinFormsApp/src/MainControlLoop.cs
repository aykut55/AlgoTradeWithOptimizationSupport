using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using AlgoTradeWithOptimizationSupportWinFormsApp.Logging;
using AlgoTradeWithOptimizationSupportWinFormsApp.src.timer;

namespace AlgoTradeWithOptimizationSupportWinFormsApp
{
    /// <summary>
    /// Ana kontrol döngüsü - Sürekli çalışan main loop
    /// </summary>
    public class MainControlLoop
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _mainLoopTask;
        private bool _isRunning;

        // Thread-safe veri yapıları
        private ConcurrentQueue<MarketData> _marketDataQueue = new ConcurrentQueue<MarketData>();
        private ConcurrentQueue<Order> _orderQueue = new ConcurrentQueue<Order>();
        private ConcurrentQueue<TradingSignal> _signalQueue = new ConcurrentQueue<TradingSignal>();
        private ConcurrentQueue<GuiUpdate> _guiUpdateQueue = new ConcurrentQueue<GuiUpdate>();

        private object _configLock = new object();
        private object _metricsLock = new object();

        // GUI reference (Form1'e erişmek için)
        private Form1 _mainForm;

        // Anlık state
        private GuiState _currentGuiState = new GuiState();
        private AppConfig _config = new AppConfig();
        private LoopMetrics _metrics = new LoopMetrics();
        private Stopwatch _loopStopwatch = new Stopwatch();
        private Stopwatch _iterationStopwatch = new Stopwatch();

        public MainControlLoop(Form1 mainForm)
        {
            _mainForm = mainForm;
            LoadConfig();
        }

        /// <summary>
        /// Config'i başlangıçta yükle
        /// </summary>
        private void LoadConfig()
        {
            lock (_configLock)
            {
                // Default config zaten AppConfig constructor'da set
                // İsteğe bağlı: Dosyadan yüklenebilir
            }
        }

        public bool IsRunning => _isRunning;
        public LoopMetrics GetMetrics()
        {
            lock (_metricsLock)
            {
                return _metrics;
            }
        }

        /// <summary>
        /// Main loop'u başlat
        /// </summary>
        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            // Metrikleri başlat
            lock (_metricsLock)
            {
                _metrics = new LoopMetrics { StartedAt = DateTime.Now };
                _loopStopwatch.Restart();
            }

            LogManager.LogInfo("MainLoop: Starting main control loop...");

            // Background thread'de main loop'u çalıştır
            _mainLoopTask = Task.Run(() => MainLoop(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Main loop'u durdur
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
                return;

            _cancellationTokenSource?.Cancel();

            // Kısa bir süre bekle, loop hemen çıkmalı
            _mainLoopTask?.Wait(50); // 500ms yeterli (cancellation-aware sleep sayesinde)
            _isRunning = false;
        }

        /// <summary>
        /// ANA DÖNGÜ - Sürekli yukarıdan aşağıya taranır
        /// </summary>
        private void MainLoop(CancellationToken cancellationToken)
        {
            // MainLoop başlangıç log mesajı
            TimeManager.Instance.ResetTimer("MainLoop_Total");
            TimeManager.Instance.StartTimer("MainLoop_Total");

            while (!cancellationToken.IsCancellationRequested)
            {
                _iterationStopwatch.Restart();

                // Her iterasyon için timer'ları sıfırla
                TimeManager.Instance.ResetTimer("MainLoop_Iteration");
                TimeManager.Instance.ResetTimer("ReadGuiItems");
                TimeManager.Instance.ResetTimer("ReadNetwork");
                TimeManager.Instance.ResetTimer("ReadConfig");
                TimeManager.Instance.ResetTimer("ExecuteBusinessLogic");
                TimeManager.Instance.ResetTimer("UpdateConfig");
                TimeManager.Instance.ResetTimer("SendNetwork");
                TimeManager.Instance.ResetTimer("WriteDataToFiles");
                TimeManager.Instance.ResetTimer("UpdateGui");

                TimeManager.Instance.StartTimer("MainLoop_Iteration");

                try
                {
                    // ============================================
                    // 1. GUI OKUMA
                    // ============================================
                    TimeManager.Instance.StartTimer("ReadGuiItems");
                    ReadGuiItems();
                    TimeManager.Instance.StopTimer("ReadGuiItems");

                    // ============================================
                    // 2. NETWORK OKUMA
                    // ============================================
                    TimeManager.Instance.StartTimer("ReadNetwork");
                    ReadNetwork();
                    TimeManager.Instance.StopTimer("ReadNetwork");

                    // ============================================
                    // 3. CONFIG OKUMA
                    // ============================================
                    TimeManager.Instance.StartTimer("ReadConfig");
                    ReadConfig();
                    TimeManager.Instance.StopTimer("ReadConfig");

                    // ============================================
                    // 4. İŞ MANTIK ÇALIŞIR (Execute/Run)
                    // ============================================
                    TimeManager.Instance.StartTimer("ExecuteBusinessLogic");
                    ExecuteBusinessLogic();
                    TimeManager.Instance.StopTimer("ExecuteBusinessLogic");

                    // ============================================
                    // 5. CONFIG GÜNCELLEME
                    // ============================================
                    TimeManager.Instance.StartTimer("UpdateConfig");
                    UpdateConfig();
                    TimeManager.Instance.StopTimer("UpdateConfig");

                    // ============================================
                    // 6. NETWORK GÖNDERME
                    // ============================================
                    TimeManager.Instance.StartTimer("SendNetwork");
                    SendNetwork();
                    TimeManager.Instance.StopTimer("SendNetwork");

                    // ============================================
                    // 7. DOSYAYA YAZMA
                    // ============================================
                    TimeManager.Instance.StartTimer("WriteDataToFiles");
                    WriteDataToFiles();
                    TimeManager.Instance.StopTimer("WriteDataToFiles");

                    // ============================================
                    // 8. GUI GÜNCELLEME
                    // ============================================
                    TimeManager.Instance.StartTimer("UpdateGui");
                    UpdateGui();
                    TimeManager.Instance.StopTimer("UpdateGui");

                    TimeManager.Instance.StopTimer("MainLoop_Iteration");

                    // Başarılı iterasyon
                    UpdateMetrics(success: true);

                    // Her 10 iterasyonda bir periyodik log mesajı
                    lock (_metricsLock)
                    {
                        if (_metrics.TotalIterations % 10 == 0)
                        {
                            LogManager.LogInfo($"MainLoop: Iteration {_metrics.TotalIterations} completed - " +
                                $"Runtime: {_metrics.TotalRuntime.TotalSeconds:F1}s, " +
                                $"Last iteration: {_metrics.LastIterationTime.TotalMilliseconds:F2}ms");

                            // Method bazında timing bilgilerini bulk olarak loga yaz
                            var timings = $"[TIMING] MainLoop Iteration #{_metrics.TotalIterations} - " +
                                $"Total: {TimeManager.Instance.GetElapsedTime("MainLoop_Iteration")}ms | " +
                                $"ReadGuiItems: {TimeManager.Instance.GetElapsedTime("ReadGuiItems")}ms | " +
                                $"ReadNetwork: {TimeManager.Instance.GetElapsedTime("ReadNetwork")}ms | " +
                                $"ReadConfig: {TimeManager.Instance.GetElapsedTime("ReadConfig")}ms | " +
                                $"ExecuteBusinessLogic: {TimeManager.Instance.GetElapsedTime("ExecuteBusinessLogic")}ms | " +
                                $"UpdateConfig: {TimeManager.Instance.GetElapsedTime("UpdateConfig")}ms | " +
                                $"SendNetwork: {TimeManager.Instance.GetElapsedTime("SendNetwork")}ms | " +
                                $"WriteDataToFiles: {TimeManager.Instance.GetElapsedTime("WriteDataToFiles")}ms | " +
                                $"UpdateGui: {TimeManager.Instance.GetElapsedTime("UpdateGui")}ms";
                            LogManager.LogDebug(timings);
                        }

                        // Her 100 iterasyonda bir LogManager buffer'ını temizle
                        if (_metrics.TotalIterations % 100 == 0)
                        {
                            LogManager.Instance.ClearBuffer();
                        }
                    }

                }
                catch (Exception ex)
                {
                    TimeManager.Instance.StopTimer("MainLoop_Iteration");
                    LogManager.LogError("MainLoop: Exception in main loop", ex);
                    UpdateMetrics(success: false);

                    // Hata durumunda döngü durmasın, devam etsin
                    // Cancellation-aware sleep
                    try { Task.Delay(100, cancellationToken).Wait(); } catch { }
                }

                // Döngü hızını ayarla (CPU'yu %100 kullanmaması için)
                int delayMs;
                lock (_configLock)
                {
                    delayMs = _config.LoopDelayMs;
                }

                // Cancellation-aware sleep - hemen çıkış yapabilir
                try
                {
                    Task.Delay(delayMs, cancellationToken).Wait();
                }
                catch (AggregateException)
                {
                    // Cancellation requested, loop will exit
                }
            }

            TimeManager.Instance.StopTimer("MainLoop_Total");
            LogManager.LogInfo($"MainLoop: stopped - Total runtime: {TimeManager.Instance.GetElapsedTime("MainLoop_Total")}ms");
        }

        /// <summary>
        /// Loop metriklerini güncelle
        /// </summary>
        private void UpdateMetrics(bool success)
        {
            _iterationStopwatch.Stop();

            lock (_metricsLock)
            {
                _metrics.TotalIterations++;
                if (success)
                    _metrics.SuccessfulIterations++;
                else
                    _metrics.FailedIterations++;

                _metrics.LastIterationTime = _iterationStopwatch.Elapsed;
                _metrics.LastIterationAt = DateTime.Now;
                _metrics.TotalRuntime = _loopStopwatch.Elapsed;

                // Ortalama hesapla
                if (_metrics.TotalIterations > 0)
                {
                    _metrics.AverageIterationTime = TimeSpan.FromMilliseconds(
                        _metrics.TotalRuntime.TotalMilliseconds / _metrics.TotalIterations
                    );
                }

                // Her 100 iterasyonda bir rapor (sadece Console'a)
                if (_metrics.TotalIterations % 100 == 0)
                {
                    LogManager.Log(
                        Logging.LogLevel.Debug,
                        LogSinks.Console,
                        $"Metrics: Iter={_metrics.TotalIterations}, " +
                        $"Success={_metrics.SuccessfulIterations}, " +
                        $"Failed={_metrics.FailedIterations}, " +
                        $"Avg={_metrics.AverageIterationTime.TotalMilliseconds:F2}ms, " +
                        $"Rate={_metrics.IterationsPerSecond:F1}/sec");
                }
            }
        }

        // ====================================================================
        // ALT METHODLAR
        // ====================================================================

        /// <summary>
        /// 1. GUI elemanlarını oku
        /// </summary>
        private void ReadGuiItems()
        {
            // GUI'ye main thread'den erişmeliyiz
            if (_mainForm.InvokeRequired)
            {
                // Background thread'deyiz, main thread'e marshal et
                // NOT: GUI okuma senkron olmalı, yoksa race condition olur
                _mainForm.Invoke(new Action(() =>
                {
                    try
                    {
                        // Aktif tab'ı oku
                        _currentGuiState.ActiveTabIndex = _mainForm.GetActiveTabIndex();
                        var activeTab = _mainForm.GetActiveTab();
                        _currentGuiState.ActiveTabName = activeTab?.Text ?? "None";
                        _currentGuiState.LastReadTime = DateTime.Now;

                        // İleride: Tab içeriğindeki parametreleri oku
                        // Örneğin: TextBox'lardan strateji parametreleri
                        // _currentGuiState.Parameters["MA_Period"] = maPerıodTextBox.Text;
                    }
                    catch (Exception ex)
                    {
                        LogManager.LogWarning("ReadGuiItems: Failed to read GUI", ex);
                    }
                }));
            }
        }

        /// <summary>
        /// 2. Network'ten veri oku (UDP/TCP)
        /// </summary>
        private void ReadNetwork()
        {
            // Queue'dan gelen market data'ları işle
            int processedCount = 0;
            while (_marketDataQueue.TryDequeue(out MarketData? data) && processedCount < 100)
            {
                // Her iterasyonda max 100 veri işle (backpressure)
                processedCount++;

                // TODO: Market data'yı işle
                // - Fiyat değişikliklerini takip et
                // - Order book'u güncelle
                // - Strateji için hazırla
            }

            if (processedCount > 0)
            {
                LogManager.LogDebug($"ReadNetwork: Processed {processedCount} market data packets");
            }

            // NOT: Asıl UDP listener ayrı bir thread'de çalışır
            // Bu method sadece queue'yu tüketir
        }

        /// <summary>
        /// 3. Config dosyasını/ayarları oku
        /// </summary>
        private void ReadConfig()
        {
            // Config her iterasyonda okunmayabilir, performans için
            // Örnek: Her 100 iterasyonda bir oku
            lock (_metricsLock)
            {
                if (_metrics.TotalIterations % 100 == 0)
                {
                    lock (_configLock)
                    {
                        // TODO: Config dosyasından oku
                        // if (File.Exists("config.json"))
                        // {
                        //     var json = File.ReadAllText("config.json");
                        //     _config = JsonSerializer.Deserialize<AppConfig>(json);
                        // }
                    }
                }
            }
        }

        /// <summary>
        /// 4. İş mantığını çalıştır (Trading Strategy)
        /// </summary>
        private void ExecuteBusinessLogic()
        {
            // Sadece strateji çalışıyorsa işle
            if (!_currentGuiState.IsStrategyRunning)
                return;

            // TODO: Gerçek trading mantığı
            // 1. Teknik göstergeleri hesapla (MA, RSI, MACD, vb.)
            // 2. Sinyal üret
            // 3. Risk yönetimi kontrolü
            // 4. Pozisyon yönetimi

            // Örnek: Basit bir sinyal üret
            // if (MACrossover())
            // {
            //     var signal = new TradingSignal
            //     {
            //         Symbol = "EURUSD",
            //         Timestamp = DateTime.Now,
            //         Type = SignalType.Buy,
            //         Price = 1.0850m,
            //         Quantity = 10000,
            //         Reason = "MA crossover detected"
            //     };
            //     _signalQueue.Enqueue(signal);
            // }
        }

        /// <summary>
        /// 5. Config'i güncelle
        /// </summary>
        private void UpdateConfig()
        {
            lock (_configLock)
            {
                // TODO: Runtime'da değişen ayarları kaydet
            }
        }

        /// <summary>
        /// 6. Network'e veri gönder (Emirler vs.)
        /// </summary>
        private void SendNetwork()
        {
            // Networking kapalıysa çık
            lock (_configLock)
            {
                if (!_config.EnableNetworking)
                    return;
            }

            // Order queue'dan gönderilecek emirleri al
            int sentCount = 0;
            while (_orderQueue.TryDequeue(out Order? order) && sentCount < 50)
            {
                sentCount++;

                try
                {
                    // TODO: UDP/TCP ile emir gönder
                    // SendOrderToExchange(order);

                    order.Status = OrderStatus.Sent;
                    LogManager.LogInfo($"SendNetwork: Order sent {order.OrderId} - {order.Side} {order.Quantity} {order.Symbol} @ {order.Price}");
                }
                catch (Exception ex)
                {
                    order.Status = OrderStatus.Rejected;
                    LogManager.LogError($"SendNetwork: Failed to send order {order.OrderId}", ex);
                }
            }
        }

        /// <summary>
        /// 7. Dosyaya veri yaz (Export, data files, vs.)
        /// NOT: Loglar artık LogManager tarafından handle ediliyor
        /// </summary>
        private void WriteDataToFiles()
        {
            // TODO: Export işlemleri, data dosyası yazma, vs.
            // Trading data export
            // Backtest sonuçları kaydetme
            // vb.
        }

        /// <summary>
        /// 8. GUI'yi güncelle
        /// </summary>
        private void UpdateGui()
        {
            // GUI güncellemesi main thread'de olmalı
            if (_mainForm.InvokeRequired)
            {
                // GUI update queue'dan güncellemeleri al
                var updates = new List<GuiUpdate>();
                while (_guiUpdateQueue.TryDequeue(out GuiUpdate? update) && updates.Count < 20)
                {
                    updates.Add(update);
                }

                // Metrikleri de ekle (her iterasyonda)
                LoopMetrics currentMetrics;
                lock (_metricsLock)
                {
                    currentMetrics = _metrics;
                }

                // Async invoke kullan (non-blocking)
                _mainForm.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        // StatusBar'ı güncelle
                        // _mainForm.UpdateStatusBar($"Loop: {currentMetrics.TotalIterations} @ {currentMetrics.IterationsPerSecond:F1}/s");

                        // Diğer güncellemeleri uygula
                        foreach (var update in updates)
                        {
                            // TODO: Update'leri uygula
                            // switch (update.TargetControl)
                            // {
                            //     case "ChartPanel":
                            //         UpdateChart(update.Data);
                            //         break;
                            //     case "PositionGrid":
                            //         UpdatePositions(update.Data);
                            //         break;
                            // }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.LogWarning("UpdateGui: Failed to update GUI", ex);
                    }
                }));
            }
        }

        // ====================================================================
        // PUBLIC API - External thread'ler bu metodları çağırabilir
        // ====================================================================

        /// <summary>
        /// Market data ekle (UDP listener'dan gelir)
        /// </summary>
        public void EnqueueMarketData(MarketData data)
        {
            _marketDataQueue.Enqueue(data);
        }

        /// <summary>
        /// Emir gönder (GUI'den veya strategy'den gelir)
        /// </summary>
        public void EnqueueOrder(Order order)
        {
            _orderQueue.Enqueue(order);
            LogManager.LogInfo($"API: Order queued {order.OrderId}");
        }

        /// <summary>
        /// GUI update ekle
        /// </summary>
        public void EnqueueGuiUpdate(GuiUpdate update)
        {
            _guiUpdateQueue.Enqueue(update);
        }

        /// <summary>
        /// Config'i runtime'da güncelle
        /// </summary>
        public void UpdateLoopDelay(int delayMs)
        {
            lock (_configLock)
            {
                _config.LoopDelayMs = Math.Max(1, Math.Min(delayMs, 1000)); // 1-1000ms arası
                LogManager.LogInfo($"API: Loop delay updated to {_config.LoopDelayMs}ms");
            }
        }

        /// <summary>
        /// Networking'i aç/kapat
        /// </summary>
        public void SetNetworkingEnabled(bool enabled)
        {
            lock (_configLock)
            {
                _config.EnableNetworking = enabled;
                LogManager.LogInfo($"API: Networking {(enabled ? "enabled" : "disabled")}");
            }
        }

        /// <summary>
        /// File logging'i aç/kapat
        /// </summary>
        public void SetFileLoggingEnabled(bool enabled)
        {
            lock (_configLock)
            {
                _config.EnableFileLogging = enabled;
                LogManager.LogInfo($"API: File logging {(enabled ? "enabled" : "disabled")}");
            }
        }

        /// <summary>
        /// Stratejiyi başlat/durdur
        /// </summary>
        public void SetStrategyRunning(bool running)
        {
            _currentGuiState.IsStrategyRunning = running;
            LogManager.LogInfo($"API: Strategy {(running ? "started" : "stopped")}");
        }
    }
}

