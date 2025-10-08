///// Warning - this file is created by gen_interop.lua - do not edit. /////

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using KeraLuaEx;
using System.Diagnostics;

namespace Csh
{
    public partial class App
    {
        #region ============= C# => KeraLuaEx functions =============

        /// <summary>Lua export function: Tell me something good.</summary>
        /// <param name="arg_one">some strings</param>
        /// <param name="arg_two">a nice integer</param>
        /// <param name="arg_three"></param>
        /// <returns>TableEx a returned thing></returns>
        public TableEx? MyLuaFunc(string arg_one, int arg_two, TableEx arg_three)
        {
            int numArgs = 0;
            int numRet = 1;

            // Get function.
            LuaType ltype = _l.GetGlobal("my_lua_func");
            if (ltype != LuaType.Function) { throw new SyntaxException("", -1, $"Invalid lua function my_lua_func"); }

            // Push arguments.
            _l.PushString(arg_one);
            numArgs++;
            _l.PushInteger(arg_two);
            numArgs++;
            _l.PushTableEx(arg_three);
            numArgs++;

            // Do the actual call.
            LuaStatus lstat = _l.DoCall(numArgs, numRet);
            if (lstat >= LuaStatus.ErrRun) { throw new LuaException("", -1, lstat, "DoCall() failed"); }

            // Get the results from the stack.
            TableEx? ret = _l.ToTableEx(-1);
            if (ret is null) { throw new SyntaxException("", -1, "MyLuaFunc return value is not a TableEx"); }
            _l.Pop(1);
            return ret;
        }

        /// <summary>Lua export function: wooga wooga</summary>
        /// <param name="arg_one">aaa bbb ccc</param>
        /// <returns>double a returned number></returns>
        public double? MyLuaFunc2(bool arg_one)
        {
            int numArgs = 0;
            int numRet = 1;

            // Get function.
            LuaType ltype = _l.GetGlobal("my_lua_func2");
            if (ltype != LuaType.Function) { throw new SyntaxException("", -1, $"Invalid lua function my_lua_func2"); }

            // Push arguments.
            _l.PushBoolean(arg_one);
            numArgs++;

            // Do the actual call.
            LuaStatus lstat = _l.DoCall(numArgs, numRet);
            if (lstat >= LuaStatus.ErrRun) { throw new LuaException("", -1, lstat, "DoCall() failed"); }

            // Get the results from the stack.
            double? ret = _l.ToNumber(-1);
            if (ret is null) { throw new SyntaxException("", -1, "MyLuaFunc2 return value is not a double"); }
            _l.Pop(1);
            return ret;
        }

        /// <summary>Lua export function: no_args</summary>
        /// <returns>double a returned number></returns>
        public double? NoArgsFunc()
        {
            int numArgs = 0;
            int numRet = 1;

            // Get function.
            LuaType ltype = _l.GetGlobal("no_args_func");
            if (ltype != LuaType.Function) { throw new SyntaxException("", -1, $"Invalid lua function no_args_func"); }

            // Push arguments.

            // Do the actual call.
            LuaStatus lstat = _l.DoCall(numArgs, numRet);
            if (lstat >= LuaStatus.ErrRun) { throw new LuaException("", -1, lstat, "DoCall() failed"); }

            // Get the results from the stack.
            double? ret = _l.ToNumber(-1);
            if (ret is null) { throw new SyntaxException("", -1, "NoArgsFunc return value is not a double"); }
            _l.Pop(1);
            return ret;
        }

        /// <summary>Lua export function: Function is optional.</summary>
        /// <returns>int Dummy return value.></returns>
        public int? OptionalFunc()
        {
            int numArgs = 0;
            int numRet = 1;

            // Get function.
            LuaType ltype = _l.GetGlobal("optional_func");
            if (ltype != LuaType.Function) { throw new SyntaxException("", -1, $"Invalid lua function optional_func"); }

            // Push arguments.

            // Do the actual call.
            LuaStatus lstat = _l.DoCall(numArgs, numRet);
            if (lstat >= LuaStatus.ErrRun) { throw new LuaException("", -1, lstat, "DoCall() failed"); }

            // Get the results from the stack.
            int? ret = _l.ToInteger(-1);
            if (ret is null) { throw new SyntaxException("", -1, "OptionalFunc return value is not a int"); }
            _l.Pop(1);
            return ret;
        }

        #endregion

        #region ============= KeraLuaEx => C# callback functions =============s
        
        /// <summary>Host export function: Script wants to log something.
        /// Lua arg: "level" Log level
        /// Lua arg: "msg" Log message
        /// Lua return: int Unused
        /// </summary>
        /// <param name="p">Internal lua state</param>
        /// <returns>Number of lua return values></returns>
        int Log(IntPtr p)
        {
            Lua l = Lua.FromIntPtr(p)!;

            // Get arguments
            int? level = null;
            if (l.IsInteger(1)) { level = l.ToInteger(1); }
            else { throw new SyntaxException("", -1, "Invalid arg type log(level)"); }
            string? msg = null;
            if (l.IsString(2)) { msg = l.ToString(2); }
            else { throw new SyntaxException("", -1, "Invalid arg type log(msg)"); }

            // Do the work. Always one result.
            int ret = LogCb(level, msg);
            l.PushInteger(ret);
            return 1;
        }

        /// <summary>Host export function: What time is it
        /// Lua arg: "tzone" Time zone
        /// Lua return: string The time
        /// </summary>
        /// <param name="p">Internal lua state</param>
        /// <returns>Number of lua return values></returns>
        int GetTime(IntPtr p)
        {
            Lua l = Lua.FromIntPtr(p)!;

            // Get arguments
            int? tzone = null;
            if (l.IsInteger(1)) { tzone = l.ToInteger(1); }
            else { throw new SyntaxException("", -1, "Invalid arg type get_time(tzone)"); }

            // Do the work. Always one result.
            string ret = GetTimeCb(tzone);
            l.PushString(ret);
            return 1;
        }

        #endregion

        #region ============= Infrastructure =============

        readonly List<LuaRegister> _libFuncs = new();
        readonly Lua _l = new ();

        int OpenInterop(IntPtr p)
        {
            var l = Lua.FromIntPtr(p)!;
            l.NewLib(_libFuncs.ToArray());
            return 1;
        }

        void LoadInterop()
        {
            _libFuncs.Add(new LuaRegister("log", Log));
            _libFuncs.Add(new LuaRegister("get_time", GetTime));
            _libFuncs.Add(new LuaRegister(null, null));
            _l.RequireF("luainterop", OpenInterop, true);
        }

        void LoadScript(string scriptFn, List<string> lbotDirs)
        {
            _l.SetLuaPath(lbotDirs);
            LuaStatus lstat = _l.LoadFile(scriptFn);
            if (lstat >= LuaStatus.ErrRun) { throw new LuaException("", -1, lstat, "LoadScript() failed"); }
            // Run it.
            _l.PCall(0, Lua.LUA_MULTRET, 0);
            // Reset stack.
            _l.SetTop(0);
        }
        #endregion
    }
}
