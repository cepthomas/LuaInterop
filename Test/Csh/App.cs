using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Ephemera.NBagOfTricks;
using KeraLuaEx;


// Entry.
var app = new Csh.App();
app.Dispose();


namespace Csh
{
    /// <summary>A typical application.</summary>
    public partial class App : IDisposable
    {
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
                Console.WriteLine($"MyLuaFunc() returned {res1}");

                var res2 = MyLuaFunc2(true);
                Console.WriteLine($"MyLuaFunc2() returned {res2}");

                var res3 = NoArgsFunc();
                Console.WriteLine($"NoArgsFunc() returned {res3}");

                var res4 = OptionalFunc();
                Console.WriteLine($"OptionalFunc() returned {res4}");
            }
            catch (SyntaxException ex)
            {
                Console.WriteLine(ex);
            }
            catch (LuaException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
            if (level >= (int)LogLevel.Trace && level <= (int)LogLevel.Error)
            {
                string s = $"SCRIPT LOGS {msg ?? "null"}";
                switch ((LogLevel)level)
                {
                    case LogLevel.Trace: Console.WriteLine(s); break;
                    case LogLevel.Debug: Console.WriteLine(s); break;
                    case LogLevel.Info:  Console.WriteLine(s); break;
                    case LogLevel.Warn:  Console.WriteLine(s); break;
                    case LogLevel.Error: Console.WriteLine(s); break;
                }
            }
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
