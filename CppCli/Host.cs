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

        /// <summary>Script logger.</summary>
        readonly Logger _loggerScript = LogManager.CreateLogger("Script");

        /// <summary>The interop.</summary>
        protected HostInterop _interop = new();
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public Host()
        {
            // Where are we?
            var appDir = MiscUtils.GetHostDataDir("LuaSharp", "Ephemera");
            var thisDir = MiscUtils.GetSourcePath();
            var lbotDir = $@"{thisDir}\LuaBagOfTricks";

            // Setup logging.
            LogManager.MinLevelFile = LogLevel.Trace;
            LogManager.MinLevelNotif = LogLevel.Info;
            LogManager.Run(Path.Combine(appDir, "log.txt"), 50000);
            LogManager.LogMessage += (object? sender, LogMessageEventArgs e) => Console.WriteLine(e.ShortMessage);

            // TODO1? Support reload. Unload current modules so reload will be minty fresh. This may fail safely
            // if no script loaded yet.
            //string ret = _interop.NebCommand("unload_all", "no arg");

            try
            {
                // Check args.
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length != 2) throw new ArgumentException($"Invalid command line");
                var scriptFn = args[1];
                if (!Path.Exists(scriptFn)) throw new ArgumentException($"Invalid script file: {scriptFn}");

                // Hook script callbacks.
                HostInterop.Log += Interop_Log;
                HostInterop.Notification += Interop_Notification;

                // Load script using specific lua script paths.
                List<string> lpath = [thisDir, lbotDir];
                _interop.Run(scriptFn, lpath);

                // Execute script functions.
                int res = _interop.Setup(12345);
                for (int i = 0; i < res; i++)
                {
                    var cmdResp = _interop.DoCommand("mult", i.ToString());
                    _logger.Info($"mult {i} gave me {cmdResp}");
                }
            }
            catch (InteropException ex)
            {
                _loggerScript.Exception(ex);
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
            _interop.Dispose();
        }
        #endregion

        #region Script Event Handlers
        /// <summary>
        /// Log something from script.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Interop_Log(object? sender, LogArgs args) 
        {
            var level = MathUtils.Constrain(args.level, (int)LogLevel.Trace, (int)LogLevel.Error);
            _loggerScript.Log((LogLevel)level, args.msg);
        }

        /// <summary>
        /// Log something from script.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Interop_Notification(object? sender, NotificationArgs args) 
        {
            _loggerScript.Log(LogLevel.Info, "Hello there");
        }
        #endregion
    }
}
