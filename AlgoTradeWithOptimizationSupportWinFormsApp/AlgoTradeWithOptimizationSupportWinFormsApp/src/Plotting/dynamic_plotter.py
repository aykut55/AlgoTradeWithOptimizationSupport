"""
Dinamik panel plotting sistemi
Esnek panel yapısı - her panelde birden fazla seri olabilir
"""

import matplotlib.pyplot as plt


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
                dates = series['dates']
                values = series['values']
                label = series.get('label', '')
                color = series.get('color', 'blue')
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
            y_min = min([min(s['values']) for s in series_list])
            y_max = max([max(s['values']) for s in series_list])
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
