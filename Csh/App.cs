using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.Slog;
using KeraLuaEx;


// Entry.
var app = new Csh.App();
app.Dispose();


namespace Csh
{
    /// <summary>A typical application.</summary>
    public partial class App : IDisposable
    {
        #region Fields
        /// <summary>App logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("App");
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public App()
        {
            // Where are we? New applications will have to supply their own paths.
            var thisDir = MiscUtils.GetSourcePath();
            var lbotDir = Path.Combine(thisDir, "..", "LBOT");
            var scriptFn = Path.Combine(thisDir, "script_example.lua");

            // Setup logging.
            LogManager.MinLevelFile = LogLevel.Trace;
            LogManager.MinLevelNotif = LogLevel.Info;
            LogManager.Run(Path.Combine(thisDir, "log.txt"), 50000);
            LogManager.LogMessage += (object? sender, LogMessageEventArgs e) => Console.WriteLine(e.ShortMessage);

            try
            {
                // Load luainterop lib.
                LoadInterop();

                LoadScript(scriptFn, [thisDir, lbotDir]);

                // Execute script functions.
                List<int> lint = [34, 608, 999];
                TableEx t1 = new(lint);

                var res1 = MyLuaFunc("abcdef", 74747, t1);
                _logger.Info($"MyLuaFunc() returned {res1}");

                var res2 = MyLuaFunc2(true);
                _logger.Info($"MyLuaFunc2() returned {res2}");

                var res3 = NoArgsFunc();
                _logger.Info($"NoArgsFunc() returned {res3}");

                var res4 = OptionalFunc();
                _logger.Info($"OptionalFunc() returned {res4}");
            }
            catch (SyntaxException ex)
            {
                _logger.Exception(ex);
            }
            catch (LuaException ex)
            {
                _logger.Exception(ex);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex);
            }

            LogManager.Stop();
        }

        /// <summary>
        /// Clean up resources. https://stackoverflow.com/a/4935448
        /// </summary>
        public void Dispose()
        {
           _l.Dispose();
        }
        #endregion

        #region Lua callback functions
        /// <summary>
        /// Bound lua callback work function.
        /// </summary>
        /// <returns></returns>
        int LogCb(int? level, string? msg)
        {
            _logger.Log((LogLevel)level!, $"SCRIPT LOGS {msg ?? "null"}");
            return 0;
        }
        
        /// <summary>
        /// Bound lua callback work function.
        /// </summary>
        /// <returns>answer</returns>
        string GetTimeCb(int? tzone)
        {
            return $"{DateTime.Now} Zone:{tzone}";
        }
        #endregion
    }
}
