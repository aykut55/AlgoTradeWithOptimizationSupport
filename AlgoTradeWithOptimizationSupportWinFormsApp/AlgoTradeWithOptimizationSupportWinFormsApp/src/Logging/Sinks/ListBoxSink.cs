using System;
using System.Windows.Forms;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks
{
    /// <summary>
    /// ListBox sink - Liste halinde GUI log
    /// </summary>
    public class ListBoxSink : ILogSink
    {
        private ListBox? _listBox;
        private readonly object _lock = new object();
        private bool _isDisposed;

        public string Name => "ListBox";
        public LogSinks SinkType => LogSinks.ListBox;
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Max item sayısı (performans için)
        /// </summary>
        public int MaxItems { get; set; } = 1000;

        public ListBoxSink(ListBox listBox)
        {
            _listBox = listBox;
        }

        public void Write(LogEntry entry)
        {
            if (_isDisposed || !IsEnabled || _listBox == null)
                return;

            if (_listBox.InvokeRequired)
            {
                _listBox.BeginInvoke(new Action(() => WriteToListBox(entry)));
            }
            else
            {
                WriteToListBox(entry);
            }
        }

        private void WriteToListBox(LogEntry entry)
        {
            lock (_lock)
            {
                if (_listBox == null)
                    return;

                try
                {
                    // Max item kontrolü
                    if (_listBox.Items.Count > MaxItems)
                    {
                        // Eski itemları sil
                        int removeCount = _listBox.Items.Count - MaxItems / 2;
                        for (int i = 0; i < removeCount; i++)
                        {
                            _listBox.Items.RemoveAt(0);
                        }
                    }

                    // Log ekle
                    _listBox.Items.Add(entry.ToString("short"));

                    // Auto-scroll (en son itema)
                    _listBox.TopIndex = _listBox.Items.Count - 1;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ListBoxSink error: {ex.Message}");
                }
            }
        }

        public void Clear()
        {
            if (_listBox == null)
                return;

            if (_listBox.InvokeRequired)
            {
                _listBox.Invoke(new Action(() => _listBox.Items.Clear()));
            }
            else
            {
                _listBox.Items.Clear();
            }
        }

        public void Flush()
        {
            // ListBox direkt yazıyor, flush gerekmez
        }

        public void Dispose()
        {
            _isDisposed = true;
            _listBox = null;
        }
    }
}
