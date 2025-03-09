using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using KeraLuaEx;


namespace Interop
{
    /*
    public class LogEventArgs : EventArgs
    {
        public int Level { get; set; } = 0;
        public string Msg { get; set; } = "???";
    };


    /// <summary></summary>
    public partial class Interop
    {
        /// <summary>Main execution lua state.</summary>
        readonly Lua _l;

        /// <summary>Callback.</summary>
        public event EventHandler<LogEventArgs>? LogEvent;

        #region Lifecycle
        /// <summary>
        /// Load the lua libs implemented in C#.
        /// </summary>
        /// <param name="l">Lua context.</param>
        public Interop(Lua l)
        {
            _l = l;

            // Load our lib stuff.
            LoadInterop();
        }
        #endregion

        #region Lua call Host functions
        /// <summary>
        /// Bound lua callback work function.
        /// </summary>
        /// <returns></returns>
        int LogCb(int? level, string msg)
        {
            LogEvent?.Invoke(this, new LogEventArgs() { Level = (int)level!, Msg = msg });
            return 0;
        }
        
        /// <summary>
        /// Bound lua callback work function.
        /// </summary>
        /// <returns>answer</returns>
        string GetTimeCb()
        {
            return DateTime.Now.ToString();
        }

        /// <summary>
        /// Bound lua work function.
        /// </summary>
        /// <returns>answer</returns>
        bool CheckValueCb(double? val1, double? val2)
        {
            return val1! > val2!;
        }
        #endregion
    }
    */
}
