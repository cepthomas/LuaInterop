//#define _DUMP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;

// TODO1 LBOT\csrc\cliex.h/cpp probably should live somewhere in this project. Or where?



namespace Test
{
    #region Infrastructure
    public class Common
    {
        // Where are we?
        static public string SrcDir { get; set; }
        static public string LuaPath { get; set; }
        static public Interop Interop { get; set; } = new();

        // Hook script callbacks. Capture payload.
        static public LogArgs? LogArgs { get; set; } = null;
        static public NotificationArgs? NotifArgs { get; set; } = null;

        static Common()
        {
            SrcDir = MiscUtils.GetSourcePath().Replace("\\", "/");
            LuaPath = $"{SrcDir}/?.lua;{SrcDir}/LBOT/?.lua;;";

            Interop.Log += (sender, args) => { LogArgs = args; };
            Interop.Notification += (sender, args) => { NotifArgs = args; NotifArgs.ret = 987; };
        }

        static public void Reset()
        {
            LogArgs = null;
            NotifArgs = null;
        }

        static void Main()
        {   
            TestRunner runner = new(OutputFormat.Readable);
            var cases = new[] { "INTEROP" }; // INTEROP_SCRIPT_FILE_ERROR
            runner.RunSuites(cases);
            File.WriteAllLines(Path.Combine(SrcDir, "_test_out.txt"), runner.Context.OutputLines);

            Interop.Dispose();
        }

        public static void Dump(LuaException e, [CallerLineNumber] int lineNumber = -1)
        {
            #if _DUMP
            List<string> ls = [];
            ls.Add($"LuaException => {new StackTrace().GetFrame(1)!.GetMethod()!.ReflectedType!.Name}({lineNumber})");
            ls.Add($"error:[{e.Error}]");
            ls.Add($"context:[{e.Context}]");
            ls.Add($"message:[{e.Message}]");
            ls.Add($"");
            Console.WriteLine(string.Join(Environment.NewLine, ls));
            #endif
        }
    }
    #endregion

    #region Successful operation
    /// <summary>All good.</summary>
    public class INTEROP_HAPPY : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code.
                string s = @"--line 1
                    local li  = require('luainterop')
                    li.log('Loading script_test.lua')
                    nret = li.notif(33, 'Notification from script_test', true, 123.45)
                    li.log('nret:' .. nret)
                    function setup(arg) return arg + 1111 end
                    function do_command(cmd, arg) return 'do_command('..cmd..', '..arg..')' end";
                Common.Interop.RunChunk(s, "INTEROP_HAPPY", Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);
                UT_EQUAL(resi, 2345);
            }
            catch (LuaException)
            {
                UT_FAIL("Should not throw");
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }

            UT_NOT_NULL(Common.LogArgs);
            UT_NOT_NULL(Common.NotifArgs);
            UT_EQUAL(Common.LogArgs!.msg, "nret:987");
            UT_EQUAL(Common.NotifArgs!.arg_N, 123.45);
        }
    }
    #endregion

    #region Host => script chunk lua func
    /// <summary>host => script lua func</summary>
    public class INTEROP_MISSING_REQ_FUNC : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code.
                string s = @"--line 1
                    local li = require('luainterop')
                    -- missing function setup(arg) return 111 end
                    function do_command(cmd, arg) return 'aaa' end";
                Common.Interop.RunChunk(s, "INTEROP_MISSING_REQ_FUNC", Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                Common.Dump(e);
                UT_EQUAL(e.Message, "Script does not implement function setup()");
                // UT_STRING_CONTAINS(e.Info, "Script does not implement function setup()");
                // UT_EQUAL(e.Context, "");
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }
            UT_NULL(Common.LogArgs);
            UT_NULL(Common.NotifArgs);
        }
    }

    /// <summary>host => script lua func</summary>
    public class INTEROP_EXPLICIT_ERROR : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code.
                string s = @"--line 1
                    local li = require('luainterop')
                    function setup(arg) error('boom!!!') end";
                Common.Interop.RunChunk(s, "INTEROP_EXPLICIT_ERROR", Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                Common.Dump(e);
                UT_EQUAL(e.Message, "[string \"INTEROP_EXPLICIT_ERROR\"]:3: boom!!!");
                // UT_STRING_CONTAINS(e.Info, "Script function setup() error");
                // UT_STRING_CONTAINS(e.Context, ":3: boom!!!");
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }

            UT_NULL(Common.LogArgs);
            UT_NULL(Common.NotifArgs);
        }
    }

    /// <summary>host => script lua func</summary>
    public class INTEROP_SCRIPT_ERROR : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code.
                string s = @"--line 1
                    local li = require('luainterop')
                    local function boomer2(tt) return 'boom'..nil end
                    local function boomer1(tt) v = boomer2(tt) return #v end
                    function setup(arg) boomer1('shakalaka') end";
                Common.Interop.RunChunk(s, "INTEROP_SCRIPT_ERROR", Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                Common.Dump(e);
                UT_EQUAL(e.Message, "[string \"INTEROP_SCRIPT_ERROR\"]:3: attempt to concatenate a nil value");
                // UT_STRING_CONTAINS(e.Info, "Script function setup() error");
                // UT_STRING_CONTAINS(e.Context, ":3: attempt to concatenate a nil value");
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }

            UT_NULL(Common.LogArgs);
            UT_NULL(Common.NotifArgs);
        }
    }

    /// <summary>host => script lua func</summary>
    public class INTEROP_SCRIPT_ERROR_TAIL_CALLS : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code. Has tail calls.
                string s = @"--line 1
                    local li = require('luainterop')
                    local function boomer2(tt) return 'boom'..nil end
                    local function boomer1(tt) return boomer2(tt) end
                    function setup(arg) boomer1('shakalaka') end";
                Common.Interop.RunChunk(s, "INTEROP_SCRIPT_ERROR_TAIL_CALLS", Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                Common.Dump(e);
                UT_EQUAL(e.Message, "[string \"INTEROP_SCRIPT_ERROR_TAIL_CALLS\"]:3: attempt to concatenate a nil value");
                // UT_STRING_CONTAINS(e.Info, "Script function setup() error");
                // UT_STRING_CONTAINS(e.Context, ":3: attempt to concatenate a nil value");
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }

            UT_NULL(Common.LogArgs);
            UT_NULL(Common.NotifArgs);
        }
    }
    #endregion

    #region Script chunk => host callback func
    /// <summary>host => script lua func</summary>
    public class INTEROP_SYNTAX_ERROR : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code.
                string s = @"--line 1
                    local li = require('luainterop')
                    syntax error
                    function setup(arg) return 111 end
                    function do_command(cmd, arg) return 'aaa' end";
                Common.Interop.RunChunk(s, "INTEROP_SYNTAX_ERROR", Common.LuaPath);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                Common.Dump(e);
                UT_EQUAL(e.Message, "[string \"INTEROP_SYNTAX_ERROR\"]:3: syntax error near 'error'");
                // UT_STRING_CONTAINS(e.Info, "Load chunk failed.");
                // UT_STRING_CONTAINS(e.Context, ":3: syntax error near 'error'");
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }
            UT_NULL(Common.LogArgs);
            UT_NULL(Common.NotifArgs);
        }
    }

    /// <summary>script => host callback func</summary>
    public class INTEROP_INVALID_FUNC : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code.
                string s = @"--line 1
                    local li  = require('luainterop')
                    li.invalid_func(444)";
                Common.Interop.RunChunk(s, "INTEROP_INVALID_FUNC", Common.LuaPath);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                Common.Dump(e);
                UT_EQUAL(e.Message, "[string \"INTEROP_INVALID_FUNC\"]:3: attempt to call a nil value (field 'invalid_func')");
                // UT_STRING_CONTAINS(e.Info, "Execute chunk failed.");
                // UT_STRING_CONTAINS(e.Context, ":3: attempt to call a nil value (field 'invalid_func'");
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }

            UT_NULL(Common.LogArgs);
            UT_NULL(Common.NotifArgs);
        }
    }

    /// <summary>script => host callback func</summary>
    public class INTEROP_ARG_TYPE_WRONG : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code.
                string s = @"
                    local li  = require('luainterop')
                    nret = li.notif('bad arg', true, 123.45)";
                Common.Interop.RunChunk(s, "INTEROP_ARG_TYPE_WRONG", Common.LuaPath);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                Common.Dump(e);
                UT_EQUAL(e.Message, "[string \"INTEROP_ARG_TYPE_WRONG\"]:3: Invalid arg type for arg_I");
                // UT_STRING_CONTAINS(e.Info, "Execute chunk failed");
                // UT_STRING_CONTAINS(e.Context, ":3: Invalid arg type for arg_I");
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }

            UT_NULL(Common.LogArgs);
            UT_NULL(Common.NotifArgs);
        }
    }
    #endregion

    #region Host => script file lua func
    /// <summary>host => script file lua func</summary>
    public class INTEROP_SCRIPT_FILE_ERROR : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code.
                var fn = Path.Combine(Common.SrcDir, "test1.lua");
                Common.Interop.RunScript(fn, Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);
                UT_EQUAL(resi, 2345);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                Common.Dump(e);
                UT_EQUAL(e.Message, "C:/Dev/Libs/LuaInterop/Test/CppCli/test2.lua:14: attempt to add a 'string' with a 'nil'");
                // UT_STRING_CONTAINS(e.Info, "Script function setup() error");
                // UT_STRING_CONTAINS(e.Context, "attempt to add a 'string' with a 'nil'");
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }
            UT_NOT_NULL(Common.LogArgs);
            UT_NOT_NULL(Common.NotifArgs);
            UT_EQUAL(Common.LogArgs!.msg, "Loading test1.lua");
            UT_EQUAL(Common.NotifArgs!.arg_I, 111);
        }
    }
    #endregion

    #region Misc tests
    /// <summary>host => script lua func</summary>
    public class DEBUGGER : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();
            UT_STOP_ON_FAIL(true);

            try
            {
                // Load test code.
                string s = @"
                    local li = require('luainterop')
                    local dbg = require('debugex')
                    dbg.init() -- local cli
                    local function not_boomer(tt) dbg() end
                    function setup(arg) not_boomer('shakalaka') end";
                Common.Interop.RunChunk(s, "DEBUGGER", Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);
                UT_EQUAL(resi, 2345);
            }
            catch (LuaException e)
            {
                Common.Dump(e);
            }
            catch (Exception e)
            {
                UT_FAIL($"Unexpected exception {e.GetType().Name} {e}");
            }
        }
    }
    #endregion
}
