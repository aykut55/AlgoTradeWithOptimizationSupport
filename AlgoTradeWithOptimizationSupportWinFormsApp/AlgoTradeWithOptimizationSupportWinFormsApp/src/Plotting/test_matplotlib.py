"""
Matplotlib test script - Basit grafik çizimi testi
C#'tan matplotlib'in çalıştığını doğrular
"""

import matplotlib.pyplot as plt
import numpy as np


def plot_simple_sine_wave():
    """
    Basit sin dalgası çizer
    Returns: True if successful
    """
    try:
        # Veri oluştur
        x = np.linspace(0, 2 * np.pi, 100)
        y = np.sin(x)

        # Grafik oluştur
        plt.figure(figsize=(10, 6))
        plt.plot(x, y, 'b-', linewidth=2, label='sin(x)')
        plt.grid(True, alpha=0.3)
        plt.xlabel('x', fontsize=12)
        plt.ylabel('sin(x)', fontsize=12)
        plt.title('Matplotlib Test - Sin Dalgası', fontsize=14, fontweight='bold')
        plt.legend()

        # Grafiği göster
        plt.show()

        return True
    except Exception as e:
        print(f"Error: {e}")
        return False


def plot_trading_simulation():
    """
    Basit trading grafiği simülasyonu
    Returns: True if successful
    """
    try:
        # Rastgele fiyat verisi oluştur
        np.random.seed(42)
        days = 100
        prices = 100 + np.cumsum(np.random.randn(days) * 2)

        # Al/Sat sinyalleri (rastgele)
        buy_indices = [10, 30, 60, 80]
        sell_indices = [20, 45, 70, 90]

        # 2 panel oluştur
        fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(12, 8))
        fig.suptitle('Trading Simülasyon Testi', fontsize=14, fontweight='bold')

        # Panel 1: Fiyat + Sinyaller
        ax1.plot(prices, 'k-', linewidth=1.5, label='Fiyat', alpha=0.8)

        # AL sinyalleri
        ax1.scatter([buy_indices], [prices[i] for i in buy_indices],
                   marker='^', color='green', s=150,
                   label='AL Sinyali', zorder=5, edgecolors='darkgreen', linewidths=1.5)

        # SAT sinyalleri
        ax1.scatter([sell_indices], [prices[i] for i in sell_indices],
                   marker='v', color='red', s=150,
                   label='SAT Sinyali', zorder=5, edgecolors='darkred', linewidths=1.5)

        ax1.set_title('Fiyat ve İşlem Sinyalleri', fontsize=12)
        ax1.set_ylabel('Fiyat', fontsize=11)
        ax1.legend(loc='upper left')
        ax1.grid(True, alpha=0.3, linestyle='--')

        # Panel 2: Bakiye (simüle edilmiş)
        balance = 10000 + np.cumsum(np.random.randn(days) * 50)
        ax2.plot(balance, 'g-', linewidth=2, label='Bakiye')
        ax2.axhline(y=10000, color='gray', linestyle='--', linewidth=1, label='İlk Bakiye')
        ax2.fill_between(range(days), balance, 10000,
                        where=(balance >= 10000), color='green', alpha=0.2)
        ax2.fill_between(range(days), balance, 10000,
                        where=(balance < 10000), color='red', alpha=0.2)

        ax2.set_title('Bakiye Değişimi', fontsize=12)
        ax2.set_xlabel('Gün', fontsize=11)
        ax2.set_ylabel('Bakiye (₺)', fontsize=11)
        ax2.legend(loc='upper left')
        ax2.grid(True, alpha=0.3, linestyle='--')

        plt.tight_layout()
        plt.show()

        return True
    except Exception as e:
        print(f"Error: {e}")
        return False


if __name__ == "__main__":
    print("Test 1: Sin dalgası...")
    plot_simple_sine_wave()

    print("\nTest 2: Trading simülasyonu...")
    plot_trading_simulation()
