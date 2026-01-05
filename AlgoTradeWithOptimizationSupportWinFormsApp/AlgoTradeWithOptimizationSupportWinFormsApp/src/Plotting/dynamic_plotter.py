"""
Dinamik panel plotting sistemi
Esnek panel yapısı - her panelde birden fazla seri olabilir
Line ve OHLC (candlestick) desteği
"""

import matplotlib.pyplot as plt
from matplotlib.patches import Rectangle


def plot_candlestick(ax, bar_indices, opens, highs, lows, closes, color='black', alpha=0.8):
    """
    Candlestick (OHLC) çizimi

    Parameters:
    -----------
    ax : matplotlib axis
    bar_indices : list of int
    opens, highs, lows, closes : list of float
    color : str (default: 'black')
    alpha : float (default: 0.8)
    """
    width = 0.6

    for i, idx in enumerate(bar_indices):
        open_price = opens[i]
        high_price = highs[i]
        low_price = lows[i]
        close_price = closes[i]

        # Yükseliş (yeşil) veya düşüş (kırmızı)
        if close_price >= open_price:
            body_color = 'green'
            body_bottom = open_price
            body_height = close_price - open_price
        else:
            body_color = 'red'
            body_bottom = close_price
            body_height = open_price - close_price

        # High-Low çizgisi (fitil)
        ax.plot([idx, idx], [low_price, high_price],
               color=color, linewidth=0.8, alpha=alpha, zorder=1)

        # Body (gövde)
        if body_height > 0:
            rect = Rectangle((idx - width/2, body_bottom), width, body_height,
                           facecolor=body_color, edgecolor=color,
                           linewidth=0.8, alpha=alpha, zorder=2)
            ax.add_patch(rect)
        else:
            # Doji (açılış = kapanış)
            ax.plot([idx - width/2, idx + width/2], [open_price, open_price],
                   color=color, linewidth=1.2, alpha=alpha, zorder=2)


def plot_dynamic_panels(panel_data, config):
    """
    Dinamik panel sistemi ile grafik çizimi

    Parameters:
    -----------
    panel_data : dict
        {
            0: [  # Panel 0
                {
                    'dates': [...],
                    'values': [...],
                    'label': 'Close',
                    'color': 'blue',
                    'linestyle': '-',
                    'linewidth': 1.5
                },
                {
                    'dates': [...],
                    'values': [...],
                    'label': 'MA50',
                    'color': 'red',
                    'linestyle': '--',
                    'linewidth': 1.0
                }
            ],
            1: [  # Panel 1
                {
                    'dates': [...],
                    'values': [...],
                    'label': 'Volume',
                    'color': 'gray',
                    'linestyle': '-',
                    'linewidth': 1.5
                }
            ]
        }

    config : dict
        {
            'title': 'AlgoTrade Analiz',
            'save_path': 'path/to/save.png' (optional)
        }

    Returns:
    --------
    bool: True if successful
    """
    try:
        print("=== plot_dynamic_panels BAŞLADI ===")
        print(f"Panel sayısı: {len(panel_data)}")

        # Panel index'lerini sırala
        panel_indices = sorted(panel_data.keys())
        num_panels = len(panel_indices)

        print(f"Panel index'leri: {panel_indices}")

        # Her paneldeki seri sayısını göster
        for panel_idx in panel_indices:
            series_list = panel_data[panel_idx]
            print(f"  Panel {panel_idx}: {len(series_list)} seri")
            for i, series in enumerate(series_list):
                print(f"    Seri {i}: {series.get('label', 'No label')} - {len(series['values'])} değer")

        # Figure oluştur
        panel_height = 4  # Her panel için yükseklik (inch)
        fig, axes = plt.subplots(num_panels, 1, figsize=(16, panel_height * num_panels))

        # Eğer tek panel varsa axes bir liste değil, listeye çevir
        if num_panels == 1:
            axes = [axes]

        # Başlık
        title = config.get('title', 'AlgoTrade Analiz')
        fig.suptitle(title, fontsize=16, fontweight='bold')

        # Her paneli çiz
        for idx, panel_idx in enumerate(panel_indices):
            ax = axes[idx]
            series_list = panel_data[panel_idx]

            # Bu paneldeki tüm serileri çiz
            for series in series_list:
                series_type = series.get('type', 'line')
                label = series.get('label', '')
                color = series.get('color', 'blue')

                if series_type == 'line':
                    # Line grafiği
                    values = series['values']
                    linestyle = series.get('linestyle', '-')
                    linewidth = series.get('linewidth', 1.5)

                    # Bar index kullan (tarih yerine)
                    bar_indices = list(range(len(values)))

                    # Çiz
                    ax.plot(bar_indices, values,
                           label=label,
                           color=color,
                           linestyle=linestyle,
                           linewidth=linewidth,
                           alpha=0.8)

                elif series_type == 'ohlc':
                    # Candlestick grafiği
                    opens = series['opens']
                    highs = series['highs']
                    lows = series['lows']
                    closes = series['closes']

                    bar_indices = list(range(len(opens)))

                    # Candlestick çiz
                    plot_candlestick(ax, bar_indices, opens, highs, lows, closes,
                                   color=color, alpha=0.8)

                    # Legend için dummy line ekle
                    ax.plot([], [], label=label, color=color, linewidth=2)

            # Panel başlığı - ilk serinin label'ını kullan veya "Panel X"
            if series_list and series_list[0].get('label'):
                panel_title = f"Panel {panel_idx}: {series_list[0]['label']}"
                if len(series_list) > 1:
                    panel_title += f" + {len(series_list) - 1} seri"
            else:
                panel_title = f"Panel {panel_idx}"

            ax.set_title(panel_title, fontsize=12, fontweight='bold')
            ax.set_ylabel('Değer', fontsize=11)
            ax.legend(loc='upper left', fontsize=10)
            ax.grid(True, alpha=0.3, linestyle='--')

            # Y ekseninde sıfır çizgisi ekle (eğer veriler pozitif/negatif geçiş yapıyorsa)
            y_values = []
            for s in series_list:
                if s.get('type', 'line') == 'line':
                    y_values.extend(s['values'])
                elif s.get('type', 'line') == 'ohlc':
                    y_values.extend(s['lows'])
                    y_values.extend(s['highs'])

            if y_values:
                y_min = min(y_values)
                y_max = max(y_values)
                if y_min < 0 < y_max:
                    ax.axhline(y=0, color='black', linestyle='-', linewidth=1, alpha=0.5)

        # Son panelde X label
        axes[-1].set_xlabel('Bar Index', fontsize=11)

        plt.tight_layout()

        # Kaydet veya göster
        save_path = config.get('save_path')
        if save_path:
            plt.savefig(save_path, dpi=150, bbox_inches='tight')
            print(f"✓ Grafik kaydedildi: {save_path}")

        plt.show()

        print("✓ plot_dynamic_panels TAMAMLANDI")
        return True

    except Exception as e:
        print(f"❌ Plotting error: {e}")
        import traceback
        traceback.print_exc()
        return False


if __name__ == "__main__":
    # Standalone test
    print("dynamic_plotter.py loaded successfully!")
    print("Available functions:")
    print("  - plot_dynamic_panels(panel_data, config)")
