using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace AlgoTradeWithOptimizationSupportWinFormsApp.Scripting
{
    /// <summary>
    /// Executes C# scripts with access to AlgoTrader and project classes using Roslyn
    /// </summary>
    public class ScriptExecutor
    {
        private readonly ScriptOptions _scriptOptions;
        private CancellationTokenSource? _cancellationTokenSource;

        public ScriptExecutor()
        {
            // Configure script options with all necessary references and imports
            _scriptOptions = ScriptOptions.Default
                // Add assembly references
                .AddReferences(
                    typeof(object).Assembly,                    // mscorlib/System.Private.CoreLib
                    typeof(Console).Assembly,                   // System.Console
                    typeof(Enumerable).Assembly,                // System.Linq
                    typeof(List<>).Assembly,                    // System.Collections
                    typeof(Task).Assembly,                      // System.Threading.Tasks
                    Assembly.GetExecutingAssembly()             // This project (all classes)
                )
                // Add default imports so scripts don't need using statements
                .AddImports(
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Threading.Tasks",
                    "AlgoTradeWithOptimizationSupportWinFormsApp",
                    "AlgoTradeWithOptimizationSupportWinFormsApp.Trading",
                    "AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Core",
                    "AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Traders",
                    "AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategies",
                    "AlgoTradeWithOptimizationSupportWinFormsApp.Trading.Strategy",
                    "AlgoTradeWithOptimizationSupportWinFormsApp.Definitions",
                    "AlgoTradeWithOptimizationSupportWinFormsApp.Indicators"
                );
        }

        /// <summary>
        /// Execute a C# script asynchronously
        /// </summary>
        public async Task<ScriptExecutionResult> ExecuteAsync(
            string code,
            ScriptGlobals globals,
            CancellationToken cancellationToken = default)
        {
            var result = new ScriptExecutionResult();
            var startTime = DateTime.Now;

            try
            {
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                // Create the script
                var script = CSharpScript.Create(
                    code,
                    _scriptOptions,
                    globalsType: typeof(ScriptGlobals)
                );

                // Compile first to catch syntax errors
                var diagnostics = script.Compile(_cancellationTokenSource.Token);

                if (diagnostics.Any(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))
                {
                    result.Success = false;
                    result.CompilationErrors = diagnostics
                        .Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                        .Select(d => d.ToString())
                        .ToList();
                    return result;
                }

                // Execute the script
                var scriptState = await script.RunAsync(globals, _cancellationTokenSource.Token);

                result.Success = true;
                result.ReturnValue = scriptState.ReturnValue;
                result.ExecutionTime = DateTime.Now - startTime;
            }
            catch (OperationCanceledException)
            {
                result.Success = false;
                result.Error = "Script execution was cancelled.";
            }
            catch (CompilationErrorException ex)
            {
                result.Success = false;
                result.CompilationErrors = ex.Diagnostics.Select(d => d.ToString()).ToList();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = $"{ex.GetType().Name}: {ex.Message}";
                result.StackTrace = ex.StackTrace;
            }
            finally
            {
                result.ExecutionTime = DateTime.Now - startTime;
            }

            return result;
        }

        /// <summary>
        /// Cancel any running script
        /// </summary>
        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }
    }

    /// <summary>
    /// Result of script execution
    /// </summary>
    public class ScriptExecutionResult
    {
        public bool Success { get; set; }
        public object? ReturnValue { get; set; }
        public string? Error { get; set; }
        public string? StackTrace { get; set; }
        public List<string>? CompilationErrors { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }
}
