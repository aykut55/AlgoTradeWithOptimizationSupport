using System;
using System.Windows.Forms;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks
{
    /// <summary>
    /// TextBox sink - Basit text GUI log
    /// </summary>
    public class TextBoxSink : ILogSink
    {
        private TextBox? _textBox;
        private readonly object _lock = new object();
        private bool _isDisposed;

        public string Name => "TextBox";
        public LogSinks SinkType => LogSinks.TextBox;
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Max satır sayısı (performans için)
        /// </summary>
        public int MaxLines { get; set; } = 1000;

        public TextBoxSink(TextBox textBox)
        {
            _textBox = textBox;
            if (_textBox != null)
            {
                _textBox.Multiline = true;
                _textBox.ScrollBars = ScrollBars.Vertical;
            }
        }

        public void Write(LogEntry entry)
        {
            if (_isDisposed || !IsEnabled || _textBox == null)
                return;

            if (_textBox.InvokeRequired)
            {
                _textBox.BeginInvoke(new Action(() => WriteToTextBox(entry)));
            }
            else
            {
                WriteToTextBox(entry);
            }
        }

        private void WriteToTextBox(LogEntry entry)
        {
            lock (_lock)
            {
                if (_textBox == null)
                    return;

                try
                {
                    // Max satır kontrolü
                    var lines = _textBox.Lines;
                    if (lines.Length > MaxLines)
                    {
                        var keepLines = lines.Skip(lines.Length - MaxLines / 2).ToArray();
                        _textBox.Lines = keepLines;
                    }

                    // Append log
                    _textBox.AppendText(entry.ToString("medium") + Environment.NewLine);

                    // Auto-scroll
                    _textBox.SelectionStart = _textBox.Text.Length;
                    _textBox.ScrollToCaret();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"TextBoxSink error: {ex.Message}");
                }
            }
        }

        public void Clear()
        {
            if (_textBox == null)
                return;

            if (_textBox.InvokeRequired)
            {
                _textBox.Invoke(new Action(() => _textBox.Clear()));
            }
            else
            {
                _textBox.Clear();
            }
        }

        public void Flush()
        {
            // TextBox direkt yazıyor, flush gerekmez
        }

        public void Dispose()
        {
            _isDisposed = true;
            _textBox = null;
        }
    }
}
