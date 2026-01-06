"""
plotDataImgBundleNew.py
-----------------------
C# tarafından gelen verileri ImGui/ImPlot ile çizdirir.
AlgoTradeWithPythonWithGemini'deki DataPlotterImgBundle.py'yi kullanır.

IMPORTANT:
    - pip install imgui-bundle
    - AlgoTradeWithPythonWithGemini venv'i kullanılır (sys.path'te olmalı)

Kullanım (C# tarafından):
    plot_data_img_bundle_new(dates, opens, highs, lows, closes, volumes, ...)
"""

import sys
import numpy as np

# AlgoTradeWithPythonWithGemini'deki DataPlotterImgBundle'ı import et
try:
    # sys.path zaten C# tarafında eklendi
    from DataPlotterImgBundleNew import DataPlotterImgBundleNew, DataType
    from imgui_bundle import immapp
    IMPORTS_OK = True
except ImportError as e:
    print(f"❌ Import error: {e}")
    print("sys.path:", sys.path)
    IMPORTS_OK = False


def plot_data_img_bundle_new(
    dates, opens, highs, lows, closes, volumes, lots,
    sinyal_list, kar_zarar_fiyat_list, bakiye_fiyat_list,
    getiri_fiyat_list, getiri_fiyat_net_list,
    bakiye_fiyat_net_list=None,
    kar_zarar_fiyat_yuzde_list=None,
    getiri_fiyat_yuzde_list=None,
    komisyon_fiyat_list=None,
    getiri_fiyat_yuzde_net_list=None,
    title="BTCUSDT",
    periyot="1H"
):
    """
    C# tarafından gelen verileri ImGui/ImPlot ile çizdirir.
    AlgoTradeWithPythonWithGemini'deki DataPlotterImgBundle sınıfını kullanır.

    Parameters:
    -----------
    dates : list of str
        Tarih listesi ["YYYY.MM.DD HH:MM:SS", ...]
    opens, highs, lows, closes : list of float
        OHLC verileri
    volumes : list of long
        Volume verileri
    lots : list of long
        Lot verileri
    sinyal_list : list of double
        Trading sinyalleri (-1, 0, 1)
    kar_zarar_fiyat_list : list of double
        Kar/Zarar fiyat listesi
    bakiye_fiyat_list : list of double
        Bakiye listesi
    getiri_fiyat_list : list of double
        Brüt getiri listesi
    getiri_fiyat_net_list : list of double
        Net getiri listesi
    title : str
        Grafik başlığı
    periyot : str
        Periyot bilgisi

    Returns:
    --------
    bool : True if successful
    """

    if not IMPORTS_OK:
        print("❌ DataPlotterImgBundle import edilemedi!")
        print("AlgoTradeWithPythonWithGemini venv'inin sys.path'te olduğundan emin olun.")
        return False

    print("=== plot_data_img_bundle_new BAŞLADI ===")
    print(f"Grafik: {title} {periyot}")
    print(f"Bar sayısı: {len(dates)}")

    try:
        # Numpy array'e çevir
        opens = np.array(opens, dtype=np.float64)
        highs = np.array(highs, dtype=np.float64)
        lows = np.array(lows, dtype=np.float64)
        closes = np.array(closes, dtype=np.float64)
        volumes = np.array(volumes, dtype=np.float64)
        sinyal_list = np.array(sinyal_list, dtype=np.float64)
        kar_zarar_fiyat_list = np.array(kar_zarar_fiyat_list, dtype=np.float64)
        bakiye_fiyat_list = np.array(bakiye_fiyat_list, dtype=np.float64)
        getiri_fiyat_list = np.array(getiri_fiyat_list, dtype=np.float64)
        getiri_fiyat_net_list = np.array(getiri_fiyat_net_list, dtype=np.float64)

        # OHLC array oluştur (N, 4)
        ohlc = np.column_stack([opens, highs, lows, closes])

        # Time data (bar indices)
        n_bars = len(dates)
        time_data = np.arange(n_bars, dtype=np.float64)

        # DataPlotterImgBundle oluştur
        plotter = DataPlotterImgBundleNew()
        print(f"✓ DataPlotterImgBundleNew created successfully")

        # Temel verileri ayarla
        plotter.setTimeData(time_data)
        plotter.setOHLCData(ohlc)  # OHLC array'i gönder
        plotter.setVolumeData(volumes)
        plotter.setLotData(np.array(lots, dtype=np.float64))
        plotter.setDateTimeLabels(dates)
        plotter.setTradeSignals(sinyal_list)
        plotter.setWindowTitle(f"{title} {periyot} - Multi Panel Chart")

        # Window özellikleri
        plotter.setEnableVerticalScrollBar(False)
        plotter.setEnableSharedCrossHair(True)
        plotter.setEnableSharedXAxis(True)
        plotter.setShowInfoOnAllPanels(True)
        plotter.setShowTradeSignals(True)
        plotter.setEnableRangeSlider(True)

        # Height ratios (AlgoTrader.py'den)
        HeightRatioList = [1.5, 0.7, 0.7, 0.7, 1.0]

        # ==============================================================================
        # Panel 0: Price Chart (OHLC + Indicators)
        # ==============================================================================
        panel0 = plotter.AddPanel(0)
        panel0.setTitle("Price Chart")
        panel0.setYAxisLabel("Price")
        panel0.setHeightRatio(HeightRatioList[0])
        panel0.setOHLCData(plotter.getOHLCData())  # OHLC verilerini panel'e ekle
        panel0.setInfoPanelPosition(100, 2)
        panel0.setInfoPanelOffsets(label_dx=5, value_dx=80)

        # Indicators eklenebilir (MA5, MA21, etc.)
        # panel0.setData(0, DataType.Line, ma5, "MA(5)", (1.0, 0.5, 0.0, 1.0))

        # ==============================================================================
        # Panel 1: Trade Signals
        # ==============================================================================
        panel1 = plotter.AddPanel(1)
        panel1.setTitle("Trade Signals")
        panel1.setYAxisLabel("Signals")
        panel1.setHeightRatio(HeightRatioList[1])
        panel1.setInfoPanelPosition(120, 2)
        panel1.setInfoPanelOffsets(label_dx=5, value_dx=80)

        panel1.setData(0, DataType.Stairs, sinyal_list, "Signals", (0.2, 0.8, 1.0, 1.0))  # Cyan

        # Padding (autoscale hack)
        padding_min = np.full(n_bars, -2.0, dtype=np.float64)
        padding_max = np.full(n_bars, +2.0, dtype=np.float64)
        panel1.setData(998, DataType.Line, padding_min, "##pad_min", (1, 1, 1, 0))
        panel1.setData(999, DataType.Line, padding_max, "##pad_max", (1, 1, 1, 0))

        # ==============================================================================
        # Panel 2: PnL (Kar/Zarar)
        # ==============================================================================
        panel2 = plotter.AddPanel(2)
        panel2.setTitle("PnL")
        panel2.setYAxisLabel("Kar/Zarar Fiyat")
        panel2.setHeightRatio(HeightRatioList[2])
        panel2.setInfoPanelPosition(100, 2)
        panel2.setInfoPanelOffsets(label_dx=5, value_dx=80)

        panel2.setData(0, DataType.Line, kar_zarar_fiyat_list, "PnL", (1.0, 1.0, 0.0, 1.0))  # Sarı

        # ==============================================================================
        # Panel 3: Balance (Bakiye/Getiri)
        # ==============================================================================
        panel3 = plotter.AddPanel(3)
        panel3.setTitle("Balance")
        panel3.setYAxisLabel("Getiri")
        panel3.setHeightRatio(HeightRatioList[3])
        panel3.setInfoPanelPosition(100, 2)
        panel3.setInfoPanelOffsets(label_dx=5, value_dx=80)

        panel3.setData(0, DataType.Line, getiri_fiyat_list, "Balance", (0.0, 0.5, 1.0, 1.0))  # Mavi
        panel3.setData(1, DataType.Line, getiri_fiyat_net_list, "Net Balance", (1.0, 1.0, 0.0, 1.0))  # Sarı

        # ==============================================================================
        # Panel 4: Volume
        # ==============================================================================
        # panel4 = plotter.AddPanel(4)
        # panel4.setTitle("MOST (21,0.5)")
        # panel4.setYAxisLabel("MOST")
        # panel4.setHeightRatio(HeightRatioList[4])

        # # Info panel positioning for Panel 3
        # panel4.setInfoPanelPosition(100, 2)
        # panel4.setInfoPanelOffsets(label_dx=5, value_dx=80)
        # panel4.setData(0, DataType.Line, most, "MOST", (0.6, 0.6, 0.0, 1.0))
        # panel4.setData(1, DataType.Line, exMov, "EMA", (0.5, 0.2, 0.8, 1.0))

        # ==============================================================================
        # Y-axis sync (optional)
        # ==============================================================================
        groupId = 0
        plotter.RegisterYSyncGroup(groupId, panel0)
        # plotter.RegisterYSyncGroup(groupId, panel2)

        # ==============================================================================
        # Grafiği göster
        print(f"\n✓ {len(plotter.panels)} panel oluşturuldu")
        for idx in sorted(plotter.panels.keys()):
            panel = plotter.panels[idx]
            print(f"  Panel {idx}: {panel.title} ({len(panel.data_items)} data series)")

        print("\n🚀 ImGui window açılıyor...")
        print(f"📊 Window title: {title} {periyot} - Multi Panel Chart")
        print(f"📊 Window size: 1600x1200")
        print(f"📊 ImPlot enabled: True")

        try:
            # ImGui window'u aç
            immapp.run(plotter.Plot, with_implot=True, window_size=(1600, 1200))
            print("✓ immapp.run() completed successfully")
        except Exception as e:
            print(f"❌ immapp.run() error: {e}")
            import traceback
            traceback.print_exc()
            return False

        print("✓ plot_data_img_bundle_new TAMAMLANDI")
        return True

    except Exception as e:
        print(f"❌ ImGui plotting error: {e}")
        import traceback
        traceback.print_exc()
        return False


if __name__ == "__main__":
    # Standalone test
    print("plotDataImgBundleNew.py loaded successfully!")
    print("Available functions:")
    print("  - plot_data_img_bundle_new(...)")
    print("\nREQUIRED:")
    print("  - pip install imgui-bundle")
    print("  - AlgoTradeWithPythonWithGemini venv in sys.path")
