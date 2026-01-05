"""
AlgoTrade Backtest Sonuçları Görselleştirme
Gerçek trading verilerini matplotlib ile çizdirir
"""

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
            'ema_fast': list of float (optional),
            'ema_slow': list of float (optional),
            'fast_period': int (optional),
            'slow_period': int (optional),
            'save_path': str (optional)
        }

    Returns:
    --------
    bool: True if successful
    """
    try:
        # Tarihleri parse et
        dates = [datetime.fromisoformat(d) for d in data['dates']]

        # 3 panel oluştur
        fig, (ax1, ax2, ax3) = plt.subplots(3, 1, figsize=(16, 12))
        fig.suptitle('AlgoTrade Backtest Sonuçları', fontsize=16, fontweight='bold')

        # ============================================================
        # PANEL 1: Fiyat + EMA'lar + Al/Sat Sinyalleri
        # ============================================================
        ax1.plot(dates, data['prices'], label='Fiyat', color='black', linewidth=1.5, alpha=0.8)

        # EMA'ları çiz (varsa)
        if data.get('ema_fast') and len(data['ema_fast']) > 0:
            ax1.plot(dates, data['ema_fast'],
                    label=f"EMA({data.get('fast_period', 'N/A')})",
                    color='blue', linewidth=1.2, alpha=0.7)

        if data.get('ema_slow') and len(data['ema_slow']) > 0:
            ax1.plot(dates, data['ema_slow'],
                    label=f"EMA({data.get('slow_period', 'N/A')})",
                    color='red', linewidth=1.2, alpha=0.7)

        # AL sinyalleri (yeşil üçgen yukarı)
        if data.get('buy_signals') and len(data['buy_signals']) > 0:
            buy_indices = [sig[0] for sig in data['buy_signals']]
            buy_prices = [sig[1] for sig in data['buy_signals']]
            buy_dates = [dates[i] for i in buy_indices]
            ax1.scatter(buy_dates, buy_prices,
                       marker='^', color='green', s=150,
                       label=f"AL ({len(buy_indices)})",
                       zorder=5, edgecolors='darkgreen', linewidths=1.5)

        # SAT sinyalleri (kırmızı üçgen aşağı)
        if data.get('sell_signals') and len(data['sell_signals']) > 0:
            sell_indices = [sig[0] for sig in data['sell_signals']]
            sell_prices = [sig[1] for sig in data['sell_signals']]
            sell_dates = [dates[i] for i in sell_indices]
            ax1.scatter(sell_dates, sell_prices,
                       marker='v', color='red', s=150,
                       label=f"SAT ({len(sell_indices)})",
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

    except Exception as e:
        print(f"Plotting error: {e}")
        import traceback
        traceback.print_exc()
        return False


def plot_equity_curve_with_drawdown(balance_list, dates):
    """
    Bakiye eğrisi ve drawdown'u ayrı panellerde gösterir

    Parameters:
    -----------
    balance_list : list of float
    dates : list of str (ISO format)

    Returns:
    --------
    bool: True if successful
    """
    try:
        dates_parsed = [datetime.fromisoformat(d) for d in dates]
        fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(14, 10), sharex=True)

        balance = np.array(balance_list)
        peak = np.maximum.accumulate(balance)
        drawdown = ((balance - peak) / peak) * 100  # %

        # Equity curve
        ax1.plot(dates_parsed, balance, label='Bakiye', color='darkgreen', linewidth=2)
        ax1.plot(dates_parsed, peak, label='Peak', color='blue', linestyle='--', alpha=0.5)
        ax1.fill_between(dates_parsed, balance, peak, color='red', alpha=0.2)
        ax1.set_ylabel('Bakiye (₺)', fontsize=11)
        ax1.set_title('Equity Curve ve Drawdown', fontsize=14, fontweight='bold')
        ax1.legend()
        ax1.grid(True, alpha=0.3)

        # Drawdown
        ax2.fill_between(dates_parsed, drawdown, 0, color='red', alpha=0.4)
        ax2.plot(dates_parsed, drawdown, color='darkred', linewidth=1.5)
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

    except Exception as e:
        print(f"Drawdown plotting error: {e}")
        import traceback
        traceback.print_exc()
        return False


if __name__ == "__main__":
    # Standalone test
    print("plot_results.py loaded successfully!")
    print("Available functions:")
    print("  - plot_trading_results(data)")
    print("  - plot_equity_curve_with_drawdown(balance_list, dates)")
