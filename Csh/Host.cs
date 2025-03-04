using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.Slog;
using Interop;
using KeraLuaEx;


// Entry.
var app = new Host.Host();
app.Dispose();


namespace Host
{
    /// <summary>A typical application.</summary>
    public class Host : IDisposable
    {
        #region Fields
        /// <summary>Host logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("Host");

        /// <summary>For interop.</summary>
        Lua _l = new();
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public Host()
        {
            // Where are we?
            var thisDir = MiscUtils.GetSourcePath();
            var lbotDir = Path.Combine(thisDir, @"..\LBOT");

            // Setup logging.
            LogManager.MinLevelFile = LogLevel.Trace;
            LogManager.MinLevelNotif = LogLevel.Info;
            LogManager.Run(Path.Combine(thisDir, "log.txt"), 50000);
            LogManager.LogMessage += (object? sender, LogMessageEventArgs e) => Console.WriteLine(e.ShortMessage);

            // TODOF Support reload. Unload current modules so reload will be minty fresh. This may fail safely

            try
            {
                Interop.Interop _interop = new(_l);
                _l.SetLuaPath([thisDir, lbotDir]);
                LuaStatus lstat = _l.LoadFile(Path.Combine(thisDir, "script_example.lua"));

                // Run it.
                _l.PCall(0, Lua.LUA_MULTRET, 0);
                // Reset stack.
                _l.SetTop(0);

                LuaType t = _l.GetGlobal("thing1");
                var i = _l.ToInteger(-1);

                // Execute script functions.
                List<int> lint = [34, 608, 999];
                TableEx t1 = new(lint);
                var res1 = _interop.MyLuaFunc("abcdef", 74747, t1);

                var res2 = _interop.MyLuaFunc2(true);

                var res3 = _interop.NoArgsFunc();

                var res4 = _interop.OptionalFunc();
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


    }
}
