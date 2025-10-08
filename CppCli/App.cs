using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using Ephemera.NBagOfTricks;


namespace CppCli //TODO1 delete this project
{
    /// <summary>A typical application using interop and debugex.</summary>
    public class App : Form
    {
        #region Fields
        readonly RichTextBox rtbOut;
        // readonly Button btnGo;

        /// <summary>The interop.</summary>
        protected Interop _interop = new();
        #endregion

        #region Lifecycle
        /// <summary>Constructor.</summary>
        public App()
        {
            #region InitializeComponent();
            ClientSize = new Size(800, 450);
            Name = "Form";
            Text = "Test";

            rtbOut = new RichTextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(12, 44),
                Name = "rtbOut",
                Size = new Size(776, 394),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new("Cascadia Code", 10)
            };
            Controls.Add(rtbOut);

            // do math
            var btn = new Button { Location = new Point(20, 8), Name = "do_math", Size = new Size(100, 28), Text = "do math" };
            btn.Click += (object? sender, EventArgs e) =>
            {
                var res = _interop.DoCommand("do_math", 55);
                Log("SCR_RET", $"do_math gave me {res}");
            };
            Controls.Add(btn);

            // do dbg()
            btn = new Button { Location = new Point(140, 8), Name = "do_dbg", Size = new Size(100, 28), Text = "do dbg()" };
            btn.Click += (object? sender, EventArgs e) =>
            {
                var res = _interop.DoCommand("do_dbg", 9999);
                Log("SCR_RET", $"do_dbg gave me {res}");
            };
            Controls.Add(btn);

            // boom dbg()
            btn = new Button { Location = new Point(260, 8), Name = "boom_dbg", Size = new Size(100, 28), Text = "boom dbg()" };
            btn.Click += (object? sender, EventArgs e) =>
            {
                var res = _interop.DoCommand("boom_dbg", 9999);
                Log("SCR_RET", $"boom_dbg gave me {res}");
            };
            Controls.Add(btn);

            // boom exception
            btn = new Button { Location = new Point(380, 8), Name = "boom_exc", Size = new Size(100, 28), Text = "boom exc" };
            btn.Click += (object? sender, EventArgs e) =>
            {
                try
                {
                    var res = _interop.DoCommand("boom_exc", 9999);
                    Log("SCR_RET", $"boom_exc gave me {res}");
                }
                catch (Exception ex)
                {
                    Log("SCR_EXC", $"boom {ex.Message}");
                }
            };
            Controls.Add(btn);
            #endregion

            // Where are we?
            var srcDir = MiscUtils.GetSourcePath();

            try
            {
                // Hook script callbacks.
                Interop.Log += (object? sender, LogArgs args) => { Log("SCR_LOG", args.msg); };
                Interop.Notification += (object? sender, NotificationArgs args) => { Log("SCR_NOT", $"{args.text}({args.num})"); };

                // Load script.
                var scriptFn = Path.Combine(srcDir, "script_test.lua");
                var luaPath = $"{srcDir}\\..\\LBOT\\?.lua;{srcDir}\\lua\\?.lua;;";
                _interop.RunScript(scriptFn, luaPath);

                // Execute script functions.
                var res = _interop.Setup(12345);
                Log("SCR_RET", $"setup gave me {res}");
            }
            catch (LuaException ex)
            {
                Log("LUA_EXC", $"{ex.Message}");
            }
            catch (Exception ex)
            {
                Log("SYS_EXC", $"{ex.Message}");
            }
        }

        /// <summary>Resources.</summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _interop.Dispose();
            base.Dispose(disposing);
        }
        #endregion

        /// <summary>Logging.</summary>
        /// <param name="cat"></param>
        /// <param name="s"></param>
        void Log(string cat, string s)
        {
            rtbOut.AppendText($"{cat} {s}{Environment.NewLine}");
        }
    }

    /// <summary>Start here.</summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new App());
        }
    }
}
