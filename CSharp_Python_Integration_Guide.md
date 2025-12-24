# C# - Python Entegrasyonu Kılavuzu

## İçindekiler
1. [Genel Bakış](#genel-bakış)
2. [Yöntem Karşılaştırması](#yöntem-karşılaştırması)
3. [Python.NET ile Doğrudan Entegrasyon (ÖNERİLEN)](#pythonnet-ile-doğrudan-entegrasyon)
4. [Process.Start() ile Script Çalıştırma](#processstart-ile-script-çalıştırma)
5. [JSON ile Veri Aktarımı (Yedek Yöntem)](#json-ile-veri-aktarımı)
6. [Trading Sonuçları Plotting Uygulaması](#trading-sonuçları-plotting-uygulaması)
7. [Performans İpuçları](#performans-ipuçları)
8. [Sorun Giderme](#sorun-giderme)

---

## Genel Bakış

AlgoTrade projesinde backtest sonuçlarını (Al/Sat sinyalleri, Kar/Zarar, Bakiye, EMA'lar) Python ile görselleştirmek için üç ana yöntem vardır:

### Yöntem Özeti

| Yöntem | Hız | Karmaşıklık | Veri Boyutu Limiti | Kullanım Senaryosu |
|--------|-----|-------------|-------------------|-------------------|
| **Python.NET** | ⚡⚡⚡ Çok Hızlı | Orta | Sınırsız (Bellek) | 2M+ data, real-time plotting |
| **Process + JSON** | 🐢 Yavaş | Basit | ~100K satır | Basit, bağımsız scriptler |
| **Process + Binary** | ⚡⚡ Hızlı | Orta-Yüksek | Büyük data | Dosya bazlı aktarım gerekirse |

**2M data için önerilen:** Python.NET (bellek üzerinden doğrudan veri aktarımı)

---

## Yöntem Karşılaştırması

### 1. Python.NET (Önerilen)
**Artıları:**
- ✅ Doğrudan bellek üzerinden veri aktarımı (JSON serialize/deserialize yok)
- ✅ Python kütüphanelerine native erişim (matplotlib, pandas, numpy)
- ✅ Çok büyük data setleri için ideal (2M+ satır)
- ✅ Hata yönetimi C# exception handling ile entegre
- ✅ Real-time plotting yapılabilir

**Eksileri:**
- ❌ Python kurulumu gerekli (Runtime.PythonDLL ayarı)
- ❌ NuGet paketi dependency
- ❌ İlk setup biraz karmaşık

### 2. Process.Start() + JSON
**Artıları:**
- ✅ Basit, anlaşılır kod
- ✅ Python ve C# tamamen bağımsız
- ✅ Debugging kolay

**Eksileri:**
- ❌ 2M data için JSON serialize çok yavaş (5-10 saniye)
- ❌ Büyük dosya boyutu (disk I/O)
- ❌ Bellek tüketimi yüksek

### 3. Process.Start() + Binary (Pickle/NumPy)
**Artıları:**
- ✅ JSON'dan çok daha hızlı
- ✅ Dosya boyutu küçük

**Eksileri:**
- ❌ Binary format uyumsuzluk riski
- ❌ Debugging zor

---

## Python.NET ile Doğrudan Entegrasyon

### Adım 1: Kurulum

#### NuGet Paketi
```bash
dotnet add package Python.Runtime
```

#### Python Gereksinimleri
```bash
pip install matplotlib numpy pandas
```

### Adım 2: Python Script (plot_results.py)

**Dosya Yeri:** `AlgoTradeWithOptimizationSupportWinFormsApp/src/Plotting/plot_results.py`

```python
import matplotlib.pyplot as plt
import numpy as np
from datetime import datetime

def plot_trading_results(data):
    """
    AlgoTrade backtest sonuçlarını görselleştirir

    Parameters:
    -----------
    data : dict
        {
            'dates': list of str (ISO format),
            'prices': list of float,
            'buy_signals': list of (index, price) tuples,
            'sell_signals': list of (index, price) tuples,
            'balance': list of float,
            'pnl': list of float (cumulative),
            'ema_fast': list of float,
            'ema_slow': list of float,
            'fast_period': int,
            'slow_period': int,
            'save_path': str (optional)
        }
    """

    # Tarihleri parse et
    dates = [datetime.fromisoformat(d) for d in data['dates']]

    # 3 panel oluştur
    fig, (ax1, ax2, ax3) = plt.subplots(3, 1, figsize=(16, 12))
    fig.suptitle('AlgoTrade Backtest Sonuçları', fontsize=16, fontweight='bold')

    # ============================================================
    # PANEL 1: Fiyat + EMA'lar + Al/Sat Sinyalleri
    # ============================================================
    ax1.plot(dates, data['prices'], label='Fiyat', color='black', linewidth=1.5, alpha=0.8)

    if data.get('ema_fast'):
        ax1.plot(dates, data['ema_fast'],
                label=f"EMA({data.get('fast_period', 'N/A')})",
                color='blue', linewidth=1.2, alpha=0.7)

    if data.get('ema_slow'):
        ax1.plot(dates, data['ema_slow'],
                label=f"EMA({data.get('slow_period', 'N/A')})",
                color='red', linewidth=1.2, alpha=0.7)

    # AL sinyalleri (yeşil üçgen yukarı)
    if data.get('buy_signals') and len(data['buy_signals']) > 0:
        buy_indices, buy_prices = zip(*data['buy_signals'])
        buy_dates = [dates[i] for i in buy_indices]
        ax1.scatter(buy_dates, buy_prices,
                   marker='^', color='green', s=150,
                   label=f"AL ({len(buy_signals)})",
                   zorder=5, edgecolors='darkgreen', linewidths=1.5)

    # SAT sinyalleri (kırmızı üçgen aşağı)
    if data.get('sell_signals') and len(data['sell_signals']) > 0:
        sell_indices, sell_prices = zip(*data['sell_signals'])
        sell_dates = [dates[i] for i in sell_indices]
        ax1.scatter(sell_dates, sell_prices,
                   marker='v', color='red', s=150,
                   label=f"SAT ({len(sell_signals)})",
                   zorder=5, edgecolors='darkred', linewidths=1.5)

    ax1.set_title('Fiyat Hareketi ve İşlem Sinyalleri', fontsize=12, fontweight='bold')
    ax1.set_ylabel('Fiyat', fontsize=11)
    ax1.legend(loc='upper left', fontsize=10)
    ax1.grid(True, alpha=0.3, linestyle='--')

    # ============================================================
    # PANEL 2: Bakiye Eğrisi
    # ============================================================
    balance = data['balance']
    initial_balance = balance[0] if balance else 100000

    ax2.plot(dates, balance, label='Bakiye', color='darkgreen', linewidth=2)
    ax2.axhline(y=initial_balance, color='gray', linestyle='--',
                linewidth=1, label=f'İlk Bakiye ({initial_balance:,.0f})')

    # Bakiye değişimi (%)
    if balance:
        final_balance = balance[-1]
        balance_change_pct = ((final_balance - initial_balance) / initial_balance) * 100
        color = 'green' if balance_change_pct >= 0 else 'red'
        ax2.text(0.02, 0.98, f'Bakiye Değişimi: {balance_change_pct:+.2f}%',
                transform=ax2.transAxes, fontsize=11, verticalalignment='top',
                bbox=dict(boxstyle='round', facecolor=color, alpha=0.3))

    ax2.set_title('Bakiye Değişimi', fontsize=12, fontweight='bold')
    ax2.set_ylabel('Bakiye (₺)', fontsize=11)
    ax2.legend(loc='upper left', fontsize=10)
    ax2.grid(True, alpha=0.3, linestyle='--')
    ax2.ticklabel_format(style='plain', axis='y')

    # ============================================================
    # PANEL 3: Kümülatif Kar/Zarar
    # ============================================================
    pnl = data['pnl']
    pnl_array = np.array(pnl)

    ax3.plot(dates, pnl, label='Kümülatif Kar/Zarar', color='steelblue', linewidth=2)
    ax3.axhline(y=0, color='black', linestyle='-', linewidth=1.5, alpha=0.5)

    # Kar/Zarar alanlarını doldur
    ax3.fill_between(dates, pnl, 0,
                     where=(pnl_array >= 0),
                     color='green', alpha=0.2, label='Kar Bölgesi', interpolate=True)
    ax3.fill_between(dates, pnl, 0,
                     where=(pnl_array < 0),
                     color='red', alpha=0.2, label='Zarar Bölgesi', interpolate=True)

    # İstatistikler
    if pnl:
        max_profit = max(pnl)
        max_loss = min(pnl)
        final_pnl = pnl[-1]

        stats_text = f'Son Kar/Zarar: {final_pnl:+,.2f}\n'
        stats_text += f'Max Kar: {max_profit:+,.2f}\n'
        stats_text += f'Max Zarar: {max_loss:+,.2f}'

        ax3.text(0.02, 0.98, stats_text,
                transform=ax3.transAxes, fontsize=10, verticalalignment='top',
                fontfamily='monospace',
                bbox=dict(boxstyle='round', facecolor='wheat', alpha=0.5))

    ax3.set_title('Kümülatif Kar/Zarar', fontsize=12, fontweight='bold')
    ax3.set_xlabel('Tarih', fontsize=11)
    ax3.set_ylabel('Kar/Zarar (₺)', fontsize=11)
    ax3.legend(loc='upper left', fontsize=10)
    ax3.grid(True, alpha=0.3, linestyle='--')

    # Format x-axis tarihleri
    for ax in [ax1, ax2, ax3]:
        ax.tick_params(axis='x', rotation=45)

    plt.tight_layout()

    # Kaydet veya göster
    if data.get('save_path'):
        plt.savefig(data['save_path'], dpi=150, bbox_inches='tight')
        print(f"✓ Grafik kaydedildi: {data['save_path']}")

    plt.show()
    return True


def plot_optimization_results(results):
    """
    Optimizasyon sonuçlarını 3D surface plot olarak gösterir

    Parameters:
    -----------
    results : list of dict
        [{
            'fast_period': int,
            'slow_period': int,
            'total_return': float,
            'max_drawdown': float,
            'sharpe_ratio': float,
            'win_rate': float
        }, ...]
    """

    if not results:
        print("Optimizasyon sonucu yok!")
        return False

    # Veriyi numpy array'e çevir
    fast_periods = np.array([r['fast_period'] for r in results])
    slow_periods = np.array([r['slow_period'] for r in results])
    total_returns = np.array([r['total_return'] for r in results])
    max_drawdowns = np.array([r['max_drawdown'] for r in results])

    # 2x2 subplot oluştur
    fig = plt.figure(figsize=(16, 12))

    # ============================================================
    # 1. Total Return - 3D Surface
    # ============================================================
    from mpl_toolkits.mplot3d import Axes3D
    from scipy.interpolate import griddata

    ax1 = fig.add_subplot(2, 2, 1, projection='3d')

    # Grid oluştur
    fast_unique = np.unique(fast_periods)
    slow_unique = np.unique(slow_periods)
    grid_x, grid_y = np.meshgrid(fast_unique, slow_unique)

    points = np.column_stack((fast_periods, slow_periods))
    grid_z = griddata(points, total_returns, (grid_x, grid_y), method='cubic')

    surf = ax1.plot_surface(grid_x, grid_y, grid_z, cmap='RdYlGn', alpha=0.8, edgecolor='none')
    ax1.set_xlabel('Fast Period', fontsize=10)
    ax1.set_ylabel('Slow Period', fontsize=10)
    ax1.set_zlabel('Total Return (%)', fontsize=10)
    ax1.set_title('Total Return (3D Surface)', fontsize=12, fontweight='bold')
    fig.colorbar(surf, ax=ax1, shrink=0.5, aspect=5)

    # ============================================================
    # 2. Max Drawdown - Heatmap
    # ============================================================
    ax2 = fig.add_subplot(2, 2, 2)

    grid_dd = griddata(points, max_drawdowns, (grid_x, grid_y), method='cubic')

    im = ax2.contourf(grid_x, grid_y, grid_dd, levels=20, cmap='RdYlGn_r')
    ax2.set_xlabel('Fast Period', fontsize=10)
    ax2.set_ylabel('Slow Period', fontsize=10)
    ax2.set_title('Max Drawdown (%) - Heatmap', fontsize=12, fontweight='bold')
    fig.colorbar(im, ax=ax2)

    # En iyi noktayı işaretle
    best_idx = np.argmax(total_returns)
    ax2.scatter(fast_periods[best_idx], slow_periods[best_idx],
               marker='*', s=500, color='gold', edgecolors='black', linewidths=2,
               label=f'Best: ({fast_periods[best_idx]}, {slow_periods[best_idx]})')
    ax2.legend(fontsize=9)

    # ============================================================
    # 3. Scatter: Return vs Drawdown
    # ============================================================
    ax3 = fig.add_subplot(2, 2, 3)

    scatter = ax3.scatter(max_drawdowns, total_returns,
                         c=total_returns, cmap='RdYlGn',
                         s=100, alpha=0.6, edgecolors='black', linewidths=0.5)

    ax3.set_xlabel('Max Drawdown (%)', fontsize=10)
    ax3.set_ylabel('Total Return (%)', fontsize=10)
    ax3.set_title('Return vs Drawdown', fontsize=12, fontweight='bold')
    ax3.grid(True, alpha=0.3)
    fig.colorbar(scatter, ax=ax3, label='Return')

    # En iyi noktayı işaretle
    ax3.scatter(max_drawdowns[best_idx], total_returns[best_idx],
               marker='*', s=500, color='gold', edgecolors='black', linewidths=2,
               label='Best')
    ax3.legend(fontsize=9)

    # ============================================================
    # 4. Sharpe Ratio Histogram
    # ============================================================
    ax4 = fig.add_subplot(2, 2, 4)

    sharpe_ratios = np.array([r.get('sharpe_ratio', 0) for r in results])

    ax4.hist(sharpe_ratios, bins=30, color='steelblue', alpha=0.7, edgecolor='black')
    ax4.axvline(sharpe_ratios.mean(), color='red', linestyle='--',
                linewidth=2, label=f'Mean: {sharpe_ratios.mean():.2f}')
    ax4.axvline(sharpe_ratios.median(), color='green', linestyle='--',
                linewidth=2, label=f'Median: {np.median(sharpe_ratios):.2f}')

    ax4.set_xlabel('Sharpe Ratio', fontsize=10)
    ax4.set_ylabel('Frequency', fontsize=10)
    ax4.set_title('Sharpe Ratio Distribution', fontsize=12, fontweight='bold')
    ax4.legend(fontsize=9)
    ax4.grid(True, alpha=0.3, axis='y')

    plt.tight_layout()
    plt.show()

    return True


def plot_equity_curve_with_drawdown(balance_list, dates):
    """
    Bakiye eğrisi ve drawdown'u ayrı panellerde gösterir
    """
    fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(14, 10), sharex=True)

    balance = np.array(balance_list)
    peak = np.maximum.accumulate(balance)
    drawdown = ((balance - peak) / peak) * 100  # %

    # Equity curve
    ax1.plot(dates, balance, label='Bakiye', color='darkgreen', linewidth=2)
    ax1.plot(dates, peak, label='Peak', color='blue', linestyle='--', alpha=0.5)
    ax1.fill_between(dates, balance, peak, color='red', alpha=0.2)
    ax1.set_ylabel('Bakiye (₺)', fontsize=11)
    ax1.set_title('Equity Curve ve Drawdown', fontsize=14, fontweight='bold')
    ax1.legend()
    ax1.grid(True, alpha=0.3)

    # Drawdown
    ax2.fill_between(dates, drawdown, 0, color='red', alpha=0.4)
    ax2.plot(dates, drawdown, color='darkred', linewidth=1.5)
    ax2.set_ylabel('Drawdown (%)', fontsize=11)
    ax2.set_xlabel('Tarih', fontsize=11)
    ax2.grid(True, alpha=0.3)

    max_dd = drawdown.min()
    ax2.text(0.02, 0.98, f'Max Drawdown: {max_dd:.2f}%',
            transform=ax2.transAxes, fontsize=12, verticalalignment='top',
            bbox=dict(boxstyle='round', facecolor='red', alpha=0.3))

    plt.tight_layout()
    plt.show()
    return True
```

### Adım 3: C# Helper Class (PythonPlotter.cs)

**Dosya Yeri:** `AlgoTradeWithOptimizationSupportWinFormsApp/src/Plotting/PythonPlotter.cs`

```csharp
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Plotting
{
    /// <summary>
    /// Python matplotlib ile grafik çizimi için helper class
    /// Python.NET kullanarak doğrudan bellek üzerinden veri aktarımı yapar
    /// </summary>
    public class PythonPlotter : IDisposable
    {
        private bool _isInitialized = false;
        private dynamic _plotModule;
        private readonly string _pythonDllPath;
        private readonly string _scriptDirectory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pythonDllPath">Python DLL path (örn: C:\Python39\python39.dll)</param>
        /// <param name="scriptDirectory">Python script'lerin bulunduğu dizin (opsiyonel)</param>
        public PythonPlotter(string pythonDllPath = null, string scriptDirectory = null)
        {
            // Python DLL path (varsayılan: Python39)
            _pythonDllPath = pythonDllPath ?? @"C:\Python39\python39.dll";

            // Script directory (varsayılan: exe dizini altında src/Plotting)
            _scriptDirectory = scriptDirectory ?? Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "src",
                "Plotting"
            );

            InitializePython();
        }

        private void InitializePython()
        {
            if (_isInitialized) return;

            try
            {
                // Python DLL'i kontrol et
                if (!File.Exists(_pythonDllPath))
                {
                    throw new FileNotFoundException(
                        $"Python DLL bulunamadı: {_pythonDllPath}\n" +
                        "Lütfen doğru Python kurulumu yaptığınızdan emin olun."
                    );
                }

                // Script dizinini kontrol et/oluştur
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
                            "pip install matplotlib numpy scipy"
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
            List<double> emaFast,
            List<double> emaSlow,
            int fastPeriod = 10,
            int slowPeriod = 20,
            string savePath = null)
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
                        var dateStrings = dates.Select(d => d.ToString("yyyy-MM-dd HH:mm:ss")).ToArray();
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
                        _plotModule.plot_trading_results(pyDict);
                    }
                }
                catch (PythonException pyEx)
                {
                    throw new Exception($"Python plotting error: {pyEx.Message}\n{pyEx.StackTrace}", pyEx);
                }
            }
        }

        /// <summary>
        /// Optimizasyon sonuçlarını görselleştirir (3D surface plot)
        /// </summary>
        public void PlotOptimizationResults(List<OptimizationResult> results)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Python not initialized");

            if (results == null || results.Count == 0)
                throw new ArgumentException("results listesi boş olamaz");

            using (Py.GIL())
            {
                try
                {
                    using (var pyList = new PyList())
                    {
                        foreach (var result in results)
                        {
                            using (var pyDict = new PyDict())
                            {
                                pyDict["fast_period"] = new PyInt(result.FastPeriod);
                                pyDict["slow_period"] = new PyInt(result.SlowPeriod);
                                pyDict["total_return"] = new PyFloat(result.TotalReturn);
                                pyDict["max_drawdown"] = new PyFloat(result.MaxDrawdown);
                                pyDict["sharpe_ratio"] = new PyFloat(result.SharpeRatio);
                                pyDict["win_rate"] = new PyFloat(result.WinRate);

                                pyList.Append(pyDict);
                            }
                        }

                        _plotModule.plot_optimization_results(pyList);
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
                    var dateStrings = dates.Select(d => d.ToString("yyyy-MM-dd HH:mm:ss")).ToArray();

                    _plotModule.plot_equity_curve_with_drawdown(
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
            using (var pyList = new PyList())
            {
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
                        pyList.Append(item.ToPython());
                }
                return pyList;
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
```

### Adım 4: AlgoTrader.cs'ye Ekleme (Helper Method)

```csharp
// AlgoTrader.cs
using AlgoTradeWithOptimizationSupportWinFormsApp.Plotting;

public class AlgoTrader : MarketDataProvider
{
    // ... mevcut kodlar ...

    /// <summary>
    /// SingleTrader sonuçlarını Python ile çizdirir
    /// </summary>
    public void PlotSingleTraderResults(string savePath = null)
    {
        if (singleTrader == null)
            throw new InvalidOperationException("SingleTrader henüz çalıştırılmadı");

        using (var plotter = new PythonPlotter())
        {
            // Verileri topla
            var dates = Data.Select(d => d.DateTime).ToList();
            var prices = Data.Select(d => d.Close).ToList();

            // Al/Sat sinyallerini topla
            var buySignals = new List<(int, double)>();
            var sellSignals = new List<(int, double)>();

            foreach (var trade in singleTrader.lists.IslemListesi)
            {
                if (trade.Yon == "AL")
                    buySignals.Add((trade.BarIndex, trade.Fiyat));
                else if (trade.Yon == "SAT")
                    sellSignals.Add((trade.BarIndex, trade.Fiyat));
            }

            // Bakiye ve PnL
            var balance = singleTrader.lists.BakiyeListesi.ToList();
            var pnl = singleTrader.lists.KumulatifKarZararListesi.ToList();

            // Strategy'den EMA'ları al
            var strategy = singleTrader.GetStrategy() as SimpleMAStrategy;
            var emaFast = strategy?.GetFastEMA()?.ToList() ?? new List<double>();
            var emaSlow = strategy?.GetSlowEMA()?.ToList() ?? new List<double>();

            // Çiz
            plotter.PlotTradingResults(
                dates: dates,
                prices: prices,
                buySignals: buySignals,
                sellSignals: sellSignals,
                balance: balance,
                pnl: pnl,
                emaFast: emaFast,
                emaSlow: emaSlow,
                fastPeriod: strategy?.FastPeriod ?? 10,
                slowPeriod: strategy?.SlowPeriod ?? 20,
                savePath: savePath
            );
        }
    }

    /// <summary>
    /// Optimizasyon sonuçlarını Python ile çizdirir
    /// </summary>
    public void PlotOptimizationResults(List<OptimizationResult> results)
    {
        if (results == null || results.Count == 0)
            throw new ArgumentException("Optimizasyon sonucu bulunamadı");

        using (var plotter = new PythonPlotter())
        {
            plotter.PlotOptimizationResults(results);
        }
    }
}
```

### Adım 5: Form1.AlgoTrader.cs'de Kullanım

```csharp
// Form1.AlgoTrader.cs

private async void btnTestSingleTrader_Click(object sender, EventArgs e)
{
    btnTestSingleTrader.Enabled = false;

    try
    {
        // ... mevcut initialization kodları ...

        // Run SingleTrader with progress (ASYNC)
        await algoTrader.RunSingleTraderWithProgressAsync(progress);

        _singleTraderLogger.Log("Backtest completed!");

        // Python ile çizdirme
        try
        {
            _singleTraderLogger.Log("Plotting results with Python...");

            algoTrader.PlotSingleTraderResults(
                savePath: @"D:\AlgoTradeResults\backtest_result.png" // opsiyonel
            );

            _singleTraderLogger.Log("✓ Plotting completed!");
        }
        catch (Exception plotEx)
        {
            _singleTraderLogger.LogError("Plotting error:", plotEx.Message);
            MessageBox.Show(
                $"Grafik çiziminde hata:\n{plotEx.Message}\n\n" +
                "Python kurulumunu kontrol edin (pip install matplotlib numpy scipy)",
                "Python Hatası",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }
    catch (Exception ex)
    {
        _singleTraderLogger?.LogError("AlgoTrader test hatası:", ex.Message, ex.StackTrace);
        MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnTestSingleTrader.Enabled = true;
    }
}

private async void btnStartSingleTraderOpt_Click(object sender, EventArgs e)
{
    btnStartSingleTraderOpt.Enabled = false;

    try
    {
        // ... mevcut optimization kodları ...

        // Run optimization
        await algoTrader.RunSingleTraderOptWithProgressAsync(progressOptimization, progressSingleTrader);

        _singleTraderLogger.Log("Optimization completed!");

        // Optimizasyon sonuçlarını çizdirme
        try
        {
            _singleTraderLogger.Log("Plotting optimization results with Python...");

            var optimizer = algoTrader.GetOptimizer(); // getter method eklemen gerekebilir
            algoTrader.PlotOptimizationResults(optimizer.Results);

            _singleTraderLogger.Log("✓ Optimization plotting completed!");
        }
        catch (Exception plotEx)
        {
            _singleTraderLogger.LogError("Plotting error:", plotEx.Message);
        }
    }
    catch (Exception ex)
    {
        _singleTraderLogger?.LogError("Optimization hatası:", ex.Message, ex.StackTrace);
    }
    finally
    {
        btnStartSingleTraderOpt.Enabled = true;
    }
}
```

---

## Process.Start() ile Script Çalıştırma

### Basit Async Method

```csharp
public async Task<string> RunPythonScriptAsync(string scriptPath, string arguments = "")
{
    ProcessStartInfo startInfo = new ProcessStartInfo
    {
        FileName = "python",  // veya tam path: @"C:\Python39\python.exe"
        Arguments = $"\"{scriptPath}\" {arguments}",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true,
        StandardOutputEncoding = System.Text.Encoding.UTF8,
        StandardErrorEncoding = System.Text.Encoding.UTF8
    };

    using (Process process = new Process { StartInfo = startInfo })
    {
        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Python Error (Exit Code {process.ExitCode}):\n{error}");
        }

        return output;
    }
}

// Kullanım
string result = await RunPythonScriptAsync(@"D:\path\to\script.py", "arg1 arg2");
Console.WriteLine(result);
```

---

## JSON ile Veri Aktarımı

**⚠️ UYARI:** 2M data için KULLANMAYIN (çok yavaş)

### C# Tarafı

```csharp
using System.Text.Json;

public async Task SaveAndPlotWithJson(string jsonPath, string scriptPath)
{
    // Verileri JSON'a serialize et
    var data = new
    {
        dates = Data.Select(d => d.DateTime.ToString("yyyy-MM-dd HH:mm:ss")).ToList(),
        prices = Data.Select(d => d.Close).ToList(),
        buy_signals = singleTrader.lists.IslemListesi
            .Where(t => t.Yon == "AL")
            .Select(t => new { index = t.BarIndex, price = t.Fiyat })
            .ToList(),
        sell_signals = singleTrader.lists.IslemListesi
            .Where(t => t.Yon == "SAT")
            .Select(t => new { index = t.BarIndex, price = t.Fiyat })
            .ToList(),
        balance = singleTrader.lists.BakiyeListesi,
        pnl = singleTrader.lists.KumulatifKarZararListesi
    };

    // JSON'a yaz (YAVAS!)
    var options = new JsonSerializerOptions { WriteIndented = false };
    string json = JsonSerializer.Serialize(data, options);
    await File.WriteAllTextAsync(jsonPath, json);

    // Python script çalıştır
    await RunPythonScriptAsync(scriptPath, $"\"{jsonPath}\"");
}
```

### Python Tarafı (plot_from_json.py)

```python
import json
import sys
from plot_results import plot_trading_results

json_path = sys.argv[1]

with open(json_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

# Tuple'ları çevir
if 'buy_signals' in data:
    data['buy_signals'] = [(s['index'], s['price']) for s in data['buy_signals']]
if 'sell_signals' in data:
    data['sell_signals'] = [(s['index'], s['price']) for s in data['sell_signals']]

plot_trading_results(data)
```

---

## Performans İpuçları

### 2M Data için Optimizasyon

1. **Python.NET kullanın** - Bellek üzerinden doğrudan aktarım
2. **Downsampling yapın** - Her bar'ı çizmek yerine her N bar'da bir
3. **Sadece önemli sinyalleri gösterin** - Tüm bar'ları değil
4. **Lazy loading** - Grafik çizimi async olsun, UI bloke etmesin

### C# Tarafı Downsampling

```csharp
public void PlotTradingResultsOptimized(int downsampleFactor = 10)
{
    // Her 10 bar'da bir al
    var dates = Data.Where((d, i) => i % downsampleFactor == 0)
                   .Select(d => d.DateTime).ToList();
    var prices = Data.Where((d, i) => i % downsampleFactor == 0)
                    .Select(d => d.Close).ToList();

    // Sinyaller aynen (zaten az sayıda)
    var buySignals = singleTrader.lists.IslemListesi
        .Where(t => t.Yon == "AL")
        .Select(t => (t.BarIndex / downsampleFactor, t.Fiyat))
        .ToList();

    // ... plotter.PlotTradingResults(...) ...
}
```

### Python Tarafı Downsampling

```python
def plot_trading_results_optimized(data, downsample=10):
    """
    Büyük data setleri için optimize edilmiş plotting
    """
    # Fiyat verileri downsample
    dates = data['dates'][::downsample]
    prices = data['prices'][::downsample]

    # Sinyaller aynen (index'leri ayarla)
    buy_signals = [(idx // downsample, price) for idx, price in data['buy_signals']]

    # ... normal plotting devam eder ...
```

---

## Sorun Giderme

### 1. "Python DLL not found"

**Çözüm:**
```csharp
// Python kurulum dizinini bulun
string pythonPath = @"C:\Users\{YourUser}\AppData\Local\Programs\Python\Python39";
string dllPath = Path.Combine(pythonPath, "python39.dll");

Runtime.PythonDLL = dllPath;
```

### 2. "Module 'matplotlib' not found"

**Çözüm:**
```bash
# Cmd/PowerShell'de
python -m pip install --upgrade pip
pip install matplotlib numpy scipy
```

### 3. "GIL Error: Thread state NULL"

**Çözüm:**
```csharp
// PythonEngine.Initialize() sonrasında ekle
PythonEngine.BeginAllowThreads();

// Her Python çağrısında Py.GIL() kullan
using (Py.GIL())
{
    // Python kodları
}
```

### 4. "Memory Leak" - Bellek Sızıntısı

**Çözüm:**
```csharp
// PyObject'leri using ile kullan
using (var pyDict = new PyDict())
{
    pyDict["key"] = new PyInt(123);
    // ...
}

// Veya manuel dispose
PyObject obj = new PyInt(123);
try
{
    // ...
}
finally
{
    obj.Dispose();
}
```

### 5. UI Thread Bloklama

**Çözüm:**
```csharp
// Plotting'i Task.Run() içinde yap
private async void btnPlot_Click(object sender, EventArgs e)
{
    await Task.Run(() =>
    {
        using (var plotter = new PythonPlotter())
        {
            plotter.PlotTradingResults(...);
        }
    });
}
```

---

## Örnek Kullanım Senaryoları

### Senaryo 1: Backtest Sonrası Otomatik Çizdirme

```csharp
private async void btnTestSingleTrader_Click(object sender, EventArgs e)
{
    await algoTrader.RunSingleTraderWithProgressAsync(progress);

    // Backtest bitti, hemen çiz
    await Task.Run(() => algoTrader.PlotSingleTraderResults());
}
```

### Senaryo 2: Optimizasyon Sonrası 3D Plot

```csharp
private async void btnStartSingleTraderOpt_Click(object sender, EventArgs e)
{
    await algoTrader.RunSingleTraderOptWithProgressAsync(...);

    var results = algoTrader.singleTraderOptimizer.Results;

    await Task.Run(() => algoTrader.PlotOptimizationResults(results));
}
```

### Senaryo 3: Dosyaya Kaydet, Gösterme

```csharp
// Grafik gösterme, sadece dosyaya kaydet
algoTrader.PlotSingleTraderResults(
    savePath: @"D:\Results\backtest_{DateTime.Now:yyyyMMdd_HHmmss}.png"
);

// Python tarafında plt.show()'u kaldır
```

---

## Python Script Dizin Yapısı

```
AlgoTradeWithOptimizationSupportWinFormsApp/
├── src/
│   ├── Plotting/
│   │   ├── plot_results.py         # Ana plotting modülü
│   │   ├── plot_from_json.py       # JSON yedek yöntem
│   │   └── __init__.py             # (opsiyonel, boş bırakılabilir)
│   ├── Trading/
│   │   ├── AlgoTrader.cs
│   │   └── ...
├── docs/
│   └── CSharp_Python_Integration_Guide.md  # Bu dosya
└── bin/Debug/net9.0/
    └── src/Plotting/                # Build sonrası kopyalanır
```

### .csproj'a Ekleme (Auto-copy)

```xml
<ItemGroup>
  <None Update="src\Plotting\**\*.py">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

---

## Sonuç ve Öneriler

### 2M Data için En İyi Yöntem

**Python.NET + Downsampling:**
```csharp
using (var plotter = new PythonPlotter())
{
    plotter.PlotTradingResultsOptimized(downsampleFactor: 100);
}
```

### Avantajlar:
- ✅ 100x daha hızlı (JSON yok)
- ✅ Bellek verimli
- ✅ Real-time update mümkün
- ✅ Matplotlib'in tüm gücünü kullanabilirsiniz

### Dezavantajlar:
- ❌ Python kurulumu gerekli
- ❌ İlk setup zaman alır

---

## Ek Kaynaklar

- **Python.NET Docs:** https://pythonnet.github.io/
- **Matplotlib Gallery:** https://matplotlib.org/stable/gallery/index.html
- **Trading Visualization:** https://github.com/matplotlib/mplfinance

---

**Son Güncelleme:** 2024-12-24
**Versiyon:** 1.0
**Yazar:** AlgoTrade Development Team
