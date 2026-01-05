"""
Basit plot testi - Sadece Close değerlerini çizer
Adım adım ilerleme için minimal test
"""

import matplotlib.pyplot as plt


def plot_close_only(dates, closes):
    """
    Sadece Close fiyatlarını çizer

    Parameters:
    -----------
    dates : list of str (ISO format veya str)
    closes : list of float

    Returns:
    --------
    bool: True if successful
    """
    try:
        # Basit grafik oluştur
        plt.figure(figsize=(12, 6))
        plt.plot(closes, 'b-', linewidth=1.5, label='Close')

        plt.title('Close Fiyatları', fontsize=14, fontweight='bold')
        plt.xlabel('Bar Index', fontsize=11)
        plt.ylabel('Fiyat', fontsize=11)
        plt.legend(loc='upper left')
        plt.grid(True, alpha=0.3, linestyle='--')

        plt.tight_layout()
        plt.show()

        print(f"✓ {len(closes)} adet Close değeri çizdirildi")
        return True

    except Exception as e:
        print(f"Plotting error: {e}")
        import traceback
        traceback.print_exc()
        return False


def plot_6_panels(dates, closes, volumes, sinyal_list, kar_zarar_fiyat_list, kar_zarar_fiyat_yuzde_list,
                  bakiye_fiyat_list, getiri_fiyat_list, getiri_fiyat_yuzde_list, komisyon_fiyat_list,
                  getiri_fiyat_net_list, bakiye_fiyat_net_list, getiri_fiyat_yuzde_net_list):
    """
    6 panelli grafik - Trading verileri (TÜM listeler)

    Parameters:
    -----------
    dates : list of str
    closes : list of float
    volumes : list of float
    sinyal_list : list of float
    kar_zarar_fiyat_list : list of float
    kar_zarar_fiyat_yuzde_list : list of float
    bakiye_fiyat_list : list of float
    getiri_fiyat_list : list of float
    getiri_fiyat_yuzde_list : list of float
    komisyon_fiyat_list : list of float
    getiri_fiyat_net_list : list of float
    bakiye_fiyat_net_list : list of float
    getiri_fiyat_yuzde_net_list : list of float

    Returns:
    --------
    bool: True if successful
    """
    print("=== plot_6_panels BAŞLADI ===")
    print(f"Gelen veri boyutları:")
    print(f"  dates: {len(dates)}")
    print(f"  closes: {len(closes)}")
    print(f"  volumes: {len(volumes)}")
    print(f"  sinyal_list: {len(sinyal_list)}")
    print(f"  kar_zarar_fiyat_list: {len(kar_zarar_fiyat_list)}")
    print(f"  kar_zarar_fiyat_yuzde_list: {len(kar_zarar_fiyat_yuzde_list)}")
    print(f"  bakiye_fiyat_list: {len(bakiye_fiyat_list)}")
    print(f"  getiri_fiyat_list: {len(getiri_fiyat_list)}")
    print(f"  getiri_fiyat_yuzde_list: {len(getiri_fiyat_yuzde_list)}")
    print(f"  komisyon_fiyat_list: {len(komisyon_fiyat_list)}")
    print(f"  getiri_fiyat_net_list: {len(getiri_fiyat_net_list)}")
    print(f"  bakiye_fiyat_net_list: {len(bakiye_fiyat_net_list)}")
    print(f"  getiri_fiyat_yuzde_net_list: {len(getiri_fiyat_yuzde_net_list)}")

    try:
        # 6 panel oluştur
        fig, (ax1, ax2, ax3, ax4, ax5, ax6) = plt.subplots(6, 1, figsize=(16, 18))
        fig.suptitle('AlgoTrade - 6 Panelli Analiz', fontsize=16, fontweight='bold')

        # Bar index
        bar_indices = list(range(len(closes)))

        # ============================================================
        # PANEL 1: Close
        # ============================================================
        ax1.plot(bar_indices, closes, 'b-', linewidth=1.5, label='Close')
        ax1.set_title('Close Fiyatları', fontsize=12, fontweight='bold')
        ax1.set_ylabel('Fiyat', fontsize=11)
        ax1.legend(loc='upper left', fontsize=10)
        ax1.grid(True, alpha=0.3, linestyle='--')

        # ============================================================
        # PANEL 2: Volume
        # ============================================================
        ax2.bar(bar_indices, volumes, width=0.8, color='gray', alpha=0.6, label='Volume')
        ax2.set_title('Volume', fontsize=12, fontweight='bold')
        ax2.set_ylabel('Volume', fontsize=11)
        ax2.legend(loc='upper left', fontsize=10)
        ax2.grid(True, alpha=0.3, linestyle='--')
        ax2.ticklabel_format(style='plain', axis='y')

        # ============================================================
        # PANEL 3: SinyalList
        # ============================================================
        ax3.plot(bar_indices, sinyal_list, 'g-', linewidth=1.5, label='Sinyal', alpha=0.7)
        ax3.axhline(y=0, color='black', linestyle='-', linewidth=1, alpha=0.5)
        ax3.set_title('Sinyal Listesi', fontsize=12, fontweight='bold')
        ax3.set_ylabel('Sinyal', fontsize=11)
        ax3.legend(loc='upper left', fontsize=10)
        ax3.grid(True, alpha=0.3, linestyle='--')

        # ============================================================
        # PANEL 4: KarZararFiyatList
        # ============================================================
        ax4.plot(bar_indices, kar_zarar_fiyat_list, 'purple', linewidth=1.5, label='Kar/Zarar Fiyat', alpha=0.7)
        ax4.axhline(y=0, color='black', linestyle='-', linewidth=1, alpha=0.5)
        ax4.set_title('Kar/Zarar Fiyat Listesi', fontsize=12, fontweight='bold')
        ax4.set_ylabel('Kar/Zarar (₺)', fontsize=11)
        ax4.legend(loc='upper left', fontsize=10)
        ax4.grid(True, alpha=0.3, linestyle='--')

        # ============================================================
        # PANEL 5: KarZararFiyatYuzdeList
        # ============================================================
        ax5.plot(bar_indices, kar_zarar_fiyat_yuzde_list, 'orange', linewidth=1.5, label='Kar/Zarar %', alpha=0.7)
        ax5.axhline(y=0, color='black', linestyle='-', linewidth=1, alpha=0.5)
        ax5.set_title('Kar/Zarar Yüzde Listesi', fontsize=12, fontweight='bold')
        ax5.set_ylabel('Kar/Zarar (%)', fontsize=11)
        ax5.legend(loc='upper left', fontsize=10)
        ax5.grid(True, alpha=0.3, linestyle='--')

        # ============================================================
        # PANEL 6: GetiriFiyatNetList
        # ============================================================
        ax6.plot(bar_indices, getiri_fiyat_net_list, 'steelblue', linewidth=1.5, label='Getiri Fiyat Net', alpha=0.7)
        ax6.axhline(y=0, color='black', linestyle='-', linewidth=1, alpha=0.5)
        ax6.set_title('Getiri Fiyat Net Listesi', fontsize=12, fontweight='bold')
        ax6.set_xlabel('Bar Index', fontsize=11)
        ax6.set_ylabel('Getiri Net (₺)', fontsize=11)
        ax6.legend(loc='upper left', fontsize=10)
        ax6.grid(True, alpha=0.3, linestyle='--')

        plt.tight_layout()
        plt.show()

        print(f"✓ 6 panel çizdirildi - {len(closes)} bar")
        return True

    except Exception as e:
        print(f"6 panel plotting error: {e}")
        import traceback
        traceback.print_exc()
        return False


if __name__ == "__main__":
    # Standalone test
    import random

    # Test verisi oluştur
    test_closes = [100 + random.uniform(-5, 5) for _ in range(100)]
    test_dates = [f"2024-01-{i+1:02d}" for i in range(100)]

    print("Test: Close değerlerini çizdirme...")
    plot_close_only(test_dates, test_closes)
