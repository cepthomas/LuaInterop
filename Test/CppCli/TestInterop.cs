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


// var r = ExecuteLuaCode and ExecuteLuaCode -> TODO1 simplify/remove: new OpenScript/Chunk


// compile script:
//   - syntax error(s)


// host => script lua func:
//   - missing required func
//   - arg type wrong
//   - arg value out of range


// script => host callback func:
//   - missing required func
//   - arg type wrong
//   - arg value out of range



//res = _interop.DoCommand("do_math", 55);
//res = _interop.DoCommand("do_dbg", 9999);
//res = _interop.DoCommand("boom_dbg", 9999);
//try
//{
//    res = _interop.DoCommand("boom_exc", 9999);
//    Log("SCR_RET", $"boom_exc gave me {res}");
//}
//catch (Exception ex)
//{
//    Log("SCR_EXC", $"boom {ex.Message}");
//}

//--------------------------- all? errors ----------------------------------
//
// WRN INTEROP Setup() C:\Dev\Apps\Nebulua\LBOT\lbot_types.lua:109: Invalid integer:100000
// stack traceback:
//     [C]: in function 'error'
//     C:\Dev\Apps\Nebulua\LBOT\lbot_types.lua:109: in function 'lbot_types.val_integer'
//     C:\Dev\Apps\Nebulua\lua\step_types.lua:31: in function 'step_types.note'
//     C:\Dev\Apps\Nebulua\lua\script_api.lua:183: in function 'script_api.send_midi_note'
//     C:\Dev\Apps\Nebulua\examples\example.lua:91: in function 'setup'
// 
// 
// INF OpenScriptFile exception thread:1
// WRN ERRSYNTAX C:\Dev\Apps\Nebulua\examples\example.lua:31: syntax error near 'local'
// 
// 
// INF OpenScriptFile exception thread:1
// ERR ERRRUN C:\Dev\Apps\Nebulua\lua\script_api.lua:106: Invalid arg type for dev_name
// stack traceback:
//     [C]: in function 'luainterop.open_midi_output'
//     C:\Dev\Apps\Nebulua\lua\script_api.lua:106: in function 'script_api.open_midi_output'
//     C:\Dev\Apps\Nebulua\examples\example.lua:39: in main chunk
// 
// 
// INF OpenScriptFile exception thread:1
// WRN DEBUG MidiOutputDevice TOD-O1 just a test - delete me
// 
// 
// WRN INTEROP Setup() C:\Dev\Apps\Nebulua\lua\script_api.lua:178: attempt to perform arithmetic on a nil value (field '?')
// stack traceback:
//     C:\Dev\Apps\Nebulua\lua\script_api.lua:178: in function 'script_api.send_midi_note'
//     C:\Dev\Apps\Nebulua\examples\example.lua:92: in function 'setup'


namespace Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            TestRunner runner = new(OutputFormat.Readable);
            // "INTEROP"  "CLI"  "MISC"
            var cases = new[] { "INTEROP_HAPPY" };
            runner.RunSuites(cases);
            File.WriteAllLines(@"_test_out.txt", runner.Context.OutputLines);
        }
    }

    /// <summary>All success operations.</summary>
    public class INTEROP_HAPPY : TestSuite
    {
        public override void RunSuite()
        {
            //UT_STOP_ON_FAIL(true); // throws TestFailException

            // Where are we?
            var srcDir = MiscUtils.GetSourcePath().Replace("\\", "/");
            var luaPath = $"{srcDir}/LBOT/?.lua;{srcDir}/lua/?.lua;;";

            // Hook script callbacks. Capture payload.
            LogArgs? logArgs = null;
            NotificationArgs? notifArgs = null;
            Interop.Log += (object? sender, LogArgs args) => { logArgs = args; };
            Interop.Notification += (object? sender, NotificationArgs args) => { notifArgs = args; };

            try
            {
                using Interop interop = new();

                // Load chunk.
                var s = @"
                local li  = require('luainterop')
                li.log('Loading script_test.lua')
                nret = li.notif(33, 'Notification from script_test', true, 123.45)
                print('>>>', nret)
                function setup(arg) return arg + 1111 end
                function do_command(cmd, arg) return 'do_command('..cmd..', '..arg..')' end";
                interop.RunChunk(s, luaPath);

                // Execute script functions.
                var resi = interop.Setup(1234);
                UT_EQUAL(resi, 2345);

                UT_FAIL("did not throw");
            }
            catch (Exception e)
            {
                UT_FAIL("did throw");
                //UT_STRING_CONTAINS(e.Message, "syntax error near 'is'");
            }
        }
    }

    /// <summary>Test basic failure modes. TODO1 from nebulua</summary>
    public class INTEROP_FAIL : TestSuite
    {
        public override void RunSuite()
        {
            UT_STOP_ON_FAIL(true);

            // Set up runtime lua environment.
            var testDir = MiscUtils.GetSourcePath();
            var luaPath = $"{testDir}\\?.lua;{testDir}\\..\\LBOT\\?.lua;;";
            var scriptFn = Path.Join(testDir, "lua", "script_happy.lua");
            var testFn = "_test.lua";

            // General syntax error during load.
            {
                try
                {
                    using Interop interop = new();
                    File.WriteAllText(testFn,
                        @"local api = require(""luainterop"")
                    this is a bad statement
                    end");

                    interop.RunScript(testFn, luaPath);
                    UT_FAIL("did not throw");
                }
                catch (Exception e)
                {
                    UT_STRING_CONTAINS(e.Message, "syntax error near 'is'");
                }
            }

            // Bad L2C function
            {
                try
                {
                    using Interop interop = new();
                    File.WriteAllText(testFn,
                        @"local api = require(""luainterop"")
                        api.no_good(95)");

                    interop.RunScript(testFn, luaPath);
                    UT_FAIL("did not throw");
                }
                catch (Exception e)
                {
                    UT_STRING_CONTAINS(e.Message, "attempt to call a nil value (field 'no_good')");
                }
            }

            // General explicit error.
            {
                try
                {
                    using Interop interop = new();
                    File.WriteAllText(testFn,
                        @"local api = require(""luainterop"")
                        error(""setup() raises error()"")");

                    interop.RunScript(testFn, luaPath);
                    UT_FAIL("did not throw");
                }
                catch (Exception e)
                {
                    UT_STRING_CONTAINS(e.Message, " setup() raises error()");
                }
            }

            // Runtime error.
            {
                try
                {
                    using Interop interop = new();
                    File.WriteAllText(testFn,
                        @"local api = require(""luainterop"")
                        local bad = 123 + ng");

                    interop.RunScript(testFn, luaPath);
                    UT_FAIL("did not throw");
                }
                catch (Exception e)
                {
                    UT_STRING_CONTAINS(e.Message, "attempt to perform arithmetic on a nil value (global 'ng')");
                }
            }
        }
    }
}
