using System;
using System.Drawing;
using System.Windows.Forms;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Logging.Sinks
{
    /// <summary>
    /// RichTextBox sink - Renkli, formatlı GUI log
    /// </summary>
    public class RichTextBoxSink : ILogSink
    {
        private RichTextBox? _richTextBox;
        private readonly object _lock = new object();
        private bool _isDisposed;

        public string Name => "RichTextBox";
        public LogSinks SinkType => LogSinks.RichTextBox;
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Max satır sayısı (performans için)
        /// </summary>
        public int MaxLines { get; set; } = 1000;

        public RichTextBoxSink(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
        }

        public void Write(LogEntry entry)
        {
            if (_isDisposed || !IsEnabled || _richTextBox == null)
                return;

            if (_richTextBox.InvokeRequired)
            {
                _richTextBox.BeginInvoke(new Action(() => WriteToRichTextBox(entry)));
            }
            else
            {
                WriteToRichTextBox(entry);
            }
        }

        private void WriteToRichTextBox(LogEntry entry)
        {
            lock (_lock)
            {
                if (_richTextBox == null)
                    return;

                try
                {
                    // Max satır kontrolü
                    if (_richTextBox.Lines.Length > MaxLines)
                    {
                        var lines = _richTextBox.Lines;
                        var keepLines = lines.Skip(lines.Length - MaxLines / 2).ToArray();
                        _richTextBox.Lines = keepLines;
                    }

                    // Renkli yazma
                    _richTextBox.SelectionStart = _richTextBox.TextLength;
                    _richTextBox.SelectionLength = 0;
                    _richTextBox.SelectionColor = GetColor(entry.Level);
                    _richTextBox.AppendText(entry.ToString("medium") + Environment.NewLine);
                    _richTextBox.SelectionColor = _richTextBox.ForeColor;

                    // Auto-scroll
                    _richTextBox.SelectionStart = _richTextBox.TextLength;
                    _richTextBox.ScrollToCaret();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"RichTextBoxSink error: {ex.Message}");
                }
            }
        }

        private Color GetColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => Color.LightGray,
                LogLevel.Debug => Color.Gray,
                LogLevel.Info => Color.Blue,
                LogLevel.Warning => Color.Orange,
                LogLevel.Error => Color.Red,
                LogLevel.Fatal => Color.DarkRed,
                _ => Color.Black
            };
        }

        public void Clear()
        {
            if (_richTextBox == null)
                return;

            if (_richTextBox.InvokeRequired)
            {
                _richTextBox.Invoke(new Action(() => _richTextBox.Clear()));
            }
            else
            {
                _richTextBox.Clear();
            }
        }

        public void Flush()
        {
            // RichTextBox direkt yazıyor, flush gerekmez
        }

        public void Dispose()
        {
            _isDisposed = true;
            _richTextBox = null;
        }
    }
}
