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


namespace CppCli
{
    /// <summary>A typical application using interop and debugex.</summary>
    public class App : Form
    {
        #region Fields
        readonly RichTextBox rtbOut;
        readonly Button btnGo;

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

            btnGo = new Button
            {
                Location = new Point(12, 12),
                Name = "btnGo",
                Size = new Size(86, 26),
                Text = "Go"
            };
            btnGo.Click += Go_Click;
            Controls.Add(btnGo);
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
                _interop.Run(scriptFn, luaPath);

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

        /// <summary></summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Go_Click(object? sender, EventArgs e)
        {
            // Execute script functions.

            // setup()
            {
                var res = _interop.Setup(12345);
                Log("SCR_RET", $"setup gave me {res}");
            }

            // do_command: do_dbg
            {
                var res = _interop.DoCommand("do_dbg", 9999);
                Log("SCR_RET", $"do_dbg gave me {res}");
                //--> SCR_RET do_dbg gave me!!!dbg()
            }

            // do_command: do_math
            //for (int i = 0; i < res; i++)
            //{
            //    var res = _interop.DoCommand("do_math", i * 2);
            //    Log("SCR_RET", $"do_math {i} gave me {res}");
            //}

            //// do_command: boom
            //try
            //{
            //    var res = _interop.DoCommand("boom", 9999);
            //    Log("SCR_RET", $"boom gave me {res}");
            //}
            //catch (Exception ex)
            //{
            //    Log("SCR_EXC", $"boom {ex.Message}");
            //    //SCR_LOG boom() was called
            //    //SCR_EXC boom LuaInteropError: DoCommand() [C:\Dev\Libs\LbotImpl\CppCli\script_test.lua:47: attempt to concatenate a nil value
            //    //stack traceback:
            //    //	C:\Dev\Libs\LbotImpl\CppCli\script_test.lua:47: in upvalue 'boom'
            //    //	C:\Dev\Libs\LbotImpl\CppCli\script_test.lua:68: in function 'do_command']
            //}
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
