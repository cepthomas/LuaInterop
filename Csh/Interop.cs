///// Warning - this file is created by gen_interop.lua - do not edit. 2025-03-04 12:56:05 /////

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using KeraLuaEx;
using System.Diagnostics;

namespace Interop
{
    public partial class Interop
    {
        #region ============= C# => Lua functions =============

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
            if (ltype != LuaType.Function) { throw new SyntaxException($"Invalid lua function: my_lua_func"); }

            // Push arguments.
            _l.PushString(arg_one);
            numArgs++;
            _l.PushInteger(arg_two);
            numArgs++;
            _l.PushTableEx(arg_three);
            numArgs++;

            // Do the actual call.
            LuaStatus lstat = _l.DoCall(numArgs, numRet);
            if (lstat >= LuaStatus.ErrRun) { throw new LuaException("DoCall() failed"); }

            // Get the results from the stack.
            TableEx? ret = _l.ToTableEx(-1);
            if (ret is null) { throw new SyntaxException("Return value is not a TableEx"); }
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
            if (ltype != LuaType.Function) { throw new SyntaxException($"Invalid lua function: my_lua_func2"); }

            // Push arguments.
            _l.PushBoolean(arg_one);
            numArgs++;

            // Do the actual call.
            LuaStatus lstat = _l.DoCall(numArgs, numRet);
            if (lstat >= LuaStatus.ErrRun) { throw new LuaException("DoCall() failed"); }

            // Get the results from the stack.
            double? ret = _l.ToNumber(-1);
            if (ret is null) { throw new SyntaxException("Return value is not a double"); }
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
            if (ltype != LuaType.Function) { throw new SyntaxException($"Invalid lua function: no_args_func"); }

            // Push arguments.

            // Do the actual call.
            LuaStatus lstat = _l.DoCall(numArgs, numRet);
            if (lstat >= LuaStatus.ErrRun) { throw new LuaException("DoCall() failed"); }

            // Get the results from the stack.
            double? ret = _l.ToNumber(-1);
            if (ret is null) { throw new SyntaxException("Return value is not a double"); }
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
            if (ltype != LuaType.Function) { throw new SyntaxException($"Invalid lua function: optional_func"); }

            // Push arguments.

            // Do the actual call.
            LuaStatus lstat = _l.DoCall(numArgs, numRet);
            if (lstat >= LuaStatus.ErrRun) { throw new LuaException("DoCall() failed"); }

            // Get the results from the stack.
            int? ret = _l.ToInteger(-1);
            if (ret is null) { throw new SyntaxException("Return value is not a int"); }
            _l.Pop(1);
            return ret;
        }

        #endregion

        #region ============= Lua => C# callback functions =============s
        
        /// <summary>Host export function: What time is it
        /// Lua return: string The time>
        /// </summary>
        /// <param name="p">Internal lua state</param>
        /// <returns>Number of lua return values></returns>
        int GetTime(IntPtr p)
        {
            Lua l = Lua.FromIntPtr(p)!;

            // Get arguments

            // Do the work. One result.
            string ret = GetTimeCb();
            l.PushString(ret);
            return 1;
        }

        /// <summary>Host export function: Val1 is greater than val2? with no args
        /// Lua arg: "val_one">Val 1
        /// Lua arg: "val_two">Val 2
        /// Lua return: bool The answer>
        /// </summary>
        /// <param name="p">Internal lua state</param>
        /// <returns>Number of lua return values></returns>
        int CheckValue(IntPtr p)
        {
            Lua l = Lua.FromIntPtr(p)!;

            // Get arguments
            double? val_one = null;
            if (l.IsNumber(1)) { val_one = l.ToNumber(1); }
            else { throw new SyntaxException($"Invalid arg type for {val_one}"); }
            double? val_two = null;
            if (l.IsNumber(2)) { val_two = l.ToNumber(2); }
            else { throw new SyntaxException($"Invalid arg type for {val_two}"); }

            // Do the work. One result.
            bool ret = CheckValueCb(val_one, val_two);
            l.PushBoolean(ret);
            return 1;
        }

        #endregion

        #region ============= Infrastructure =============
        // Bind functions to static instance.
        static Interop? _instance;
        // Bound functions.
        static LuaFunction? _GetTime;
        static LuaFunction? _CheckValue;
        readonly List<LuaRegister> _libFuncs = new();

        int OpenInterop(IntPtr p)
        {
            var l = Lua.FromIntPtr(p)!;
            l.NewLib(_libFuncs.ToArray());
            return 1;
        }

        void LoadInterop()
        {
            _instance = this;
            _GetTime = _instance!.GetTime;
            _libFuncs.Add(new LuaRegister("get_time", _GetTime));
            _CheckValue = _instance!.CheckValue;
            _libFuncs.Add(new LuaRegister("check_value", _CheckValue));

            _libFuncs.Add(new LuaRegister(null, null));
            _l.RequireF("luainterop", OpenInterop, true);
        }
        #endregion
    }
}
