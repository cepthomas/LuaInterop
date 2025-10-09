using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Test
{
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
            LuaPath = $"{SrcDir}/LBOT/?.lua;{SrcDir}/lua/?.lua;;";

            Interop.Log += (object? sender, LogArgs args) => { LogArgs = args; };
            Interop.Notification += (object? sender, NotificationArgs args) => { NotifArgs = args; NotifArgs.ret = 987; };
        }

        static public void Reset()
        {
            LogArgs = null;
            NotifArgs = null;
        }

        static void Main()
        {
            TestRunner runner = new(OutputFormat.Readable);
            var cases = new[] { "INTEROP" };
            runner.RunSuites(cases);
            File.WriteAllLines(Path.Combine(MiscUtils.GetSourcePath(), "_test_out.txt"), runner.Context.OutputLines);

            Interop.Dispose();
        }
    }

    /// <summary>All success operations.</summary>
    public class INTEROP_HAPPY : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();

            try
            {
                // Load test code.
                string s = @"
                    local li  = require('luainterop')
                    li.log('Loading script_test.lua')
                    nret = li.notif(33, 'Notification from script_test', true, 123.45)
                    li.log('nret:' .. nret)
                    function setup(arg) return arg + 1111 end
                    function do_command(cmd, arg) return 'do_command('..cmd..', '..arg..')' end";
                Common.Interop.RunChunk(s, Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);
                UT_EQUAL(resi, 2345);
            }
            catch (LuaException e)
            {
                UT_FAIL("Should not throw");
            }
            catch (Exception e)
            {
                UT_FAIL(e.GetType().Name);
            }

            UT_NOT_NULL(Common.LogArgs);
            UT_NOT_NULL(Common.NotifArgs);
            UT_EQUAL(Common.LogArgs.msg, "nret:987");
            UT_EQUAL(Common.NotifArgs.arg_N, 123.45);
        }
    }

    /// <summary>host => script lua func</summary>
    public class INTEROP_SYNTAX_ERROR : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();

            try
            {
                // Load test code.
                string  s = @"
                    local li = require('luainterop')
                    syntax error
                    function setup(arg) return 111 end
                    function do_command(cmd, arg) return 'aaa' end";
                Common.Interop.RunChunk(s, Common.LuaPath);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                UT_EQUAL(e.Status, LuaStatus.ERRSYNTAX);
                UT_STRING_CONTAINS(e.Info, "Load chunk failed.");
                UT_STRING_CONTAINS(e.Context, ":3: syntax error near 'error'");
            }
            catch (Exception e)
            {
                UT_FAIL(e.GetType().Name);
            }
            UT_NULL(Common.LogArgs);
            UT_NULL(Common.NotifArgs);
        }
    }

    /// <summary>host => script lua func</summary>
    public class INTEROP_MISSING_REQ_FUNC : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();

            try
            {
                // Load test code.
                string s = @"
                    local li = require('luainterop')
                    -- missing function setup(arg) return 111 end
                    function do_command(cmd, arg) return 'aaa' end";
                Common.Interop.RunChunk(s, Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                UT_EQUAL(e.Status, LuaStatus.ERRINTEROP);
                UT_STRING_CONTAINS(e.Info, "Setup() Script does not implement required function setup()");
                UT_STRING_CONTAINS(e.Context, "Context ????????");
            }
            catch (Exception e)
            {
                UT_FAIL(e.GetType().Name);
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

            try
            {
                // Load test code.
                string s = @"
                    local li = require('luainterop')
                    function setup(arg) error('setup() raises error()') end";
                Common.Interop.RunChunk(s, Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                UT_EQUAL(e.Status, LuaStatus.ERRINTEROP);
                UT_STRING_CONTAINS(e.Info, "Info ????????");
                UT_STRING_CONTAINS(e.Context, ":3: setup() raises error()");
            }
            catch (Exception e)
            {
                UT_FAIL(e.GetType().Name);
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

            try
            {
                // Load test code. Has tail calls.
                string s = @"
                    local li = require('luainterop')
                    local function boomer2(tt) return 'boom'..nil end
                    local function boomer1(tt) v = boomer2(tt) return #v end
                    function setup(arg) boomer1('shakalaka') end";
                Common.Interop.RunChunk(s, Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                UT_EQUAL(e.Status, LuaStatus.ERRINTEROP);
                UT_STRING_CONTAINS(e.Info, "Info ????????");
                UT_STRING_CONTAINS(e.Context, ":3: attempt to concatenate a nil value");
            }
            catch (Exception e)
            {
                UT_FAIL(e.GetType().Name);
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

            try
            {
                // Load test code. Has tail calls.
                string s = @"
                    local li = require('luainterop')
                    local function boomer2(tt) return 'boom'..nil end
                    local function boomer1(tt) return boomer2(tt) end
                    function setup(arg) boomer1('shakalaka') end";
                Common.Interop.RunChunk(s, Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                UT_EQUAL(e.Status, LuaStatus.ERRINTEROP);
                UT_STRING_CONTAINS(e.Info, "Info ????????");
                UT_STRING_CONTAINS(e.Context, ":3: attempt to concatenate a nil value");
            }
            catch (Exception e)
            {
                UT_FAIL(e.GetType().Name);
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

            try
            {
                // Load test code.
                string s = @"
                    local li  = require('luainterop')
                    li.invalid_func(444)";
                Common.Interop.RunChunk(s, Common.LuaPath);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                UT_EQUAL(e.Status, LuaStatus.ERRRUN);
                UT_STRING_CONTAINS(e.Info, "Info ????????");
                UT_STRING_CONTAINS(e.Context, ":3: attempt to call a nil value (field 'invalid_func'");
            }
            catch (Exception e)
            {
                UT_FAIL(e.GetType().Name);
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

            try
            {
                // Load test code.
                string s = @"
                    local li  = require('luainterop')
                    nret = li.notif('bad arg', true, 123.45)";
                Common.Interop.RunChunk(s, Common.LuaPath);

                UT_FAIL("Should throw");
            }
            catch (LuaException e)
            {
                UT_EQUAL(e.Status, LuaStatus.ERRRUN);
                UT_STRING_CONTAINS(e.Info, "Execute chunk failed");
                UT_STRING_CONTAINS(e.Context, ":3: Invalid arg type for arg_I");
            }
            catch (Exception e)
            {
                UT_FAIL(e.GetType().Name);
            }

            UT_NULL(Common.LogArgs);
            UT_NULL(Common.NotifArgs);
        }
    }

    /// <summary>host => script lua func</summary>
    public class DEBUGGER : TestSuite
    {
        public override void RunSuite()
        {
            Common.Reset();

            try
            {
                // Load test code.
                string s = @"
                    local li = require('luainterop')
                    local dbg = require('debugex')
                    dbg.init() -- local cli
                    local function not_boomer(tt) dbg() end
                    function setup(arg) not_boomer('shakalaka') end";
                Common.Interop.RunChunk(s, Common.LuaPath);

                // Execute script functions.
                var resi = Common.Interop.Setup(1234);
                UT_EQUAL(resi, 2345);

            }
            catch (LuaException e)
            {
                UT_EQUAL(e.Status, LuaStatus.ERRINTEROP);
                UT_STRING_CONTAINS(e.Info, "Info ????????");
                UT_STRING_CONTAINS(e.Context, "Context ????????");
            }
            catch (Exception e)
            {
                UT_FAIL(e.GetType().Name);
            }
        }
    }
}
