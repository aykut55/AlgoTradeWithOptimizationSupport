using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlgoTradeWithOptimizationSupportWinFormsApp.Scripting;

namespace AlgoTradeWithOptimizationSupportWinFormsApp
{
    /// <summary>
    /// Form1 - Script Editor Tab functionality
    /// </summary>
    public partial class Form1
    {
        private ScriptExecutor? _scriptExecutor;
        private CancellationTokenSource? _scriptCancellationToken;
        private bool _isScriptRunning = false;

        /// <summary>
        /// Initialize script editor components.
        /// Called from Form1 constructor after InitializeComponent()
        /// </summary>
        private void InitializeScriptEditor()
        {
            _scriptExecutor = new ScriptExecutor();

            // Add keyboard shortcut for F5
            richTextBoxScriptInput.KeyDown += RichTextBoxScriptInput_KeyDown;

            LogToScriptOutput("=== C# Script Editor Ready ===");
            LogToScriptOutput("Available objects: algoTrader, stockData");
            LogToScriptOutput("Available functions: Log(message), ClearOutput()");
            LogToScriptOutput("Press F5 or click Execute to run script");
            LogToScriptOutput("");
        }

        /// <summary>
        /// Handle F5 key press to execute script
        /// </summary>
        private void RichTextBoxScriptInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                btnExecuteScript_Click(sender, e);
            }
        }

        /// <summary>
        /// Execute button click handler
        /// </summary>
        private async void btnExecuteScript_Click(object? sender, EventArgs e)
        {
            if (_isScriptRunning)
            {
                LogToScriptOutput("[WARNING] A script is already running.");
                return;
            }

            var code = richTextBoxScriptInput.Text;
            if (string.IsNullOrWhiteSpace(code))
            {
                LogToScriptOutput("[WARNING] No script code to execute.");
                return;
            }

            await ExecuteScriptAsync(code);
        }

        /// <summary>
        /// Execute script asynchronously
        /// </summary>
        private async Task ExecuteScriptAsync(string code)
        {
            _isScriptRunning = true;
            _scriptCancellationToken = new CancellationTokenSource();

            UpdateScriptUI(isRunning: true);
            LogToScriptOutput("========== Script Execution Started ==========");

            try
            {
                // Create globals object with access to algoTrader and data
                var globals = new ScriptGlobals(
                    algoTrader,
                    stockDataList,
                    message => LogToScriptOutput(message)
                );

                // Execute the script
                var result = await _scriptExecutor!.ExecuteAsync(
                    code,
                    globals,
                    _scriptCancellationToken.Token
                );

                // Display results
                if (result.Success)
                {
                    LogToScriptOutput($"[SUCCESS] Script completed in {result.ExecutionTime.TotalMilliseconds:F2}ms", Color.LightGreen);

                    if (result.ReturnValue != null)
                    {
                        LogToScriptOutput($"[RETURN] {result.ReturnValue}", Color.Cyan);
                    }
                }
                else
                {
                    if (result.CompilationErrors?.Count > 0)
                    {
                        LogToScriptOutput("[COMPILATION ERRORS]", Color.Red);
                        foreach (var error in result.CompilationErrors)
                        {
                            LogToScriptOutput($"  {error}", Color.Red);
                        }
                    }
                    else if (!string.IsNullOrEmpty(result.Error))
                    {
                        LogToScriptOutput($"[RUNTIME ERROR] {result.Error}", Color.Red);
                        if (!string.IsNullOrEmpty(result.StackTrace))
                        {
                            LogToScriptOutput($"[STACK TRACE]\n{result.StackTrace}", Color.Orange);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogToScriptOutput($"[EXCEPTION] {ex.Message}", Color.Red);
            }
            finally
            {
                _isScriptRunning = false;
                _scriptCancellationToken?.Dispose();
                _scriptCancellationToken = null;
                UpdateScriptUI(isRunning: false);
                LogToScriptOutput("========== Script Execution Ended ==========\n");
            }
        }

        /// <summary>
        /// Clear output button click handler
        /// </summary>
        private void btnClearScript_Click(object? sender, EventArgs e)
        {
            if (richTextBoxScriptOutput.InvokeRequired)
            {
                richTextBoxScriptOutput.Invoke(() => richTextBoxScriptOutput.Clear());
            }
            else
            {
                richTextBoxScriptOutput.Clear();
            }
        }

        /// <summary>
        /// Stop script button click handler
        /// </summary>
        private void btnStopScript_Click(object? sender, EventArgs e)
        {
            if (_scriptCancellationToken != null && !_scriptCancellationToken.IsCancellationRequested)
            {
                _scriptCancellationToken.Cancel();
                _scriptExecutor?.Cancel();
                LogToScriptOutput("[INFO] Script cancellation requested...", Color.Yellow);
            }
        }

        /// <summary>
        /// Log message to script output RichTextBox (thread-safe)
        /// </summary>
        private void LogToScriptOutput(string message, Color? color = null)
        {
            if (richTextBoxScriptOutput.InvokeRequired)
            {
                richTextBoxScriptOutput.BeginInvoke(() => LogToScriptOutputInternal(message, color));
            }
            else
            {
                LogToScriptOutputInternal(message, color);
            }
        }

        private void LogToScriptOutputInternal(string message, Color? color)
        {
            // Handle clear command
            if (message == "[CLEAR]")
            {
                richTextBoxScriptOutput.Clear();
                return;
            }

            richTextBoxScriptOutput.SelectionStart = richTextBoxScriptOutput.TextLength;
            richTextBoxScriptOutput.SelectionLength = 0;
            richTextBoxScriptOutput.SelectionColor = color ?? Color.LightGreen;
            richTextBoxScriptOutput.AppendText(message + Environment.NewLine);
            richTextBoxScriptOutput.SelectionColor = richTextBoxScriptOutput.ForeColor;
            richTextBoxScriptOutput.ScrollToCaret();
        }

        /// <summary>
        /// Update UI elements based on script running state
        /// </summary>
        private void UpdateScriptUI(bool isRunning)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateScriptUI(isRunning));
                return;
            }

            btnExecuteScript.Enabled = !isRunning;
            btnStopScript.Enabled = isRunning;
            lblScriptStatus.Text = isRunning ? "Running..." : "Ready";
            lblScriptStatus.ForeColor = isRunning ? Color.Orange : Color.Green;
        }
    }
}
