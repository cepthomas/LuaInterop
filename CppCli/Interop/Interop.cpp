///// Warning - this file is created by gen_interop.lua - do not edit. /////

#include <windows.h>
#include "luainterop.h"
#include "Interop.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace CppCli;


//============= C# => C functions .cpp =============//

//--------------------------------------------------------//
int Interop::Setup(int opt)
{
    SCOPE();
    int ret = luainterop_Setup(_l, opt);
    EvalLuaInteropStatus(luainterop_Error(), "Setup()");
    return ret; 
}

//--------------------------------------------------------//
String^ Interop::DoCommand(String^ cmd, int arg)
{
    SCOPE();
    String^ ret = gcnew String(luainterop_DoCommand(_l, ToCString(cmd), arg));
    EvalLuaInteropStatus(luainterop_Error(), "DoCommand()");
    return ret; 
}


//============= C => C# callback functions .cpp =============//


//--------------------------------------------------------//

int luainteropcb_Log(lua_State* l, int level, const char* msg)
{
    SCOPE();
    LogArgs^ args = gcnew LogArgs(level, msg);
    Interop::Notify(args);
    return args->ret;
}


//--------------------------------------------------------//

int luainteropcb_Notification(lua_State* l, int num, const char* text)
{
    SCOPE();
    NotificationArgs^ args = gcnew NotificationArgs(num, text);
    Interop::Notify(args);
    return args->ret;
}


//============= Infrastructure .cpp =============//

//--------------------------------------------------------//
void Interop::Run(String^ scriptFn, String^ luaPath)
{
    InitLua(luaPath);
    // Load C host funcs into lua space.
    luainterop_Load(_l);
    // Clean up stack.
    lua_pop(_l, 1);
    OpenScript(scriptFn);
}
