///// Generated cpp and h files that bind Cpp/CLI to C interop code.   /////
///// Warning - this file is created by gen_interop.lua - do not edit. /////

#include <windows.h>
#include "luainterop.h"
#include "Interop.h"

using namespace System;
using namespace System::Collections::Generic;


//============= Cpp/CLI => interop C functions =============//

//--------------------------------------------------------//
int Interop::Setup(int opt)
{
    SCOPE();
    int ret = luainterop_Setup(_l, opt);
    EvalInterop(luainterop_Info(), luainterop_Context());
    return ret; 
}

//--------------------------------------------------------//
String^ Interop::DoCommand(String^ cmd, bool arg_B, int arg_I, double arg_N, String^ arg_S)
{
    SCOPE();
    String^ ret = gcnew String(luainterop_DoCommand(_l, ToCString(cmd), arg_B, arg_I, arg_N, ToCString(arg_S)));
    EvalInterop(luainterop_Info(), luainterop_Context());
    return ret; 
}


//============= interop C => Cpp/CLI callback functions =============//


//--------------------------------------------------------//

int luainteropcb_Log(lua_State* l, const char* msg)
{
    SCOPE();
    LogArgs^ args = gcnew LogArgs(msg);
    Interop::Notify(args);
    return args->ret;
}


//--------------------------------------------------------//

int luainteropcb_Notification(lua_State* l, int arg_I, const char* arg_S, bool arg_B, double arg_N)
{
    SCOPE();
    NotificationArgs^ args = gcnew NotificationArgs(arg_I, arg_S, arg_B, arg_N);
    Interop::Notify(args);
    return args->ret;
}


//============= Infrastructure =============//

//--------------------------------------------------------//
void Interop::RunScript(String^ scriptFn, String^ luaPath)
{
    InitLua(luaPath);
    // Load C host funcs into lua space.
    luainterop_Load(_l);
    // Clean up stack.
    lua_pop(_l, 1);
    OpenScript(scriptFn);
}

//--------------------------------------------------------//
void Interop::RunChunk(String^ code, String^ luaPath)
{
    InitLua(luaPath);
    // Load C host funcs into lua space.
    luainterop_Load(_l);
    // Clean up stack.
    lua_pop(_l, 1);
    OpenChunk(code);
}
