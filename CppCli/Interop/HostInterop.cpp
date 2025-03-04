///// Warning - this file is created by gen_interop.lua, do not edit. /////

#include <windows.h>
#include "luainterop.h"
#include "HostInterop.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace Interop;

//============= C# => C functions .cpp =============//

//--------------------------------------------------------//
int HostInterop::Setup(int opt)
{
    LOCK();
    int ret = luainterop_Setup(_l, opt);
    _EvalLuaInteropStatus("Setup()");
    return ret;
}

//--------------------------------------------------------//
String^ HostInterop::DoCommand(String^ cmd, String^ arg)
{
    LOCK();
    String^ ret = ToManagedString(luainterop_DoCommand(_l, ToCString(cmd), ToCString(arg)));
    _EvalLuaInteropStatus("DoCommand()");
    return ret;
}


//============= C => C# callback functions .cpp =============//


//--------------------------------------------------------//

int luainteropcb_Log(lua_State* l, int level, const char* msg)
{
    LOCK();
    LogArgs^ args = gcnew LogArgs(level, msg);
    HostInterop::Notify(args);
    return 0;
}


//--------------------------------------------------------//

int luainteropcb_Notification(lua_State* l, int num, const char* text)
{
    LOCK();
    NotificationArgs^ args = gcnew NotificationArgs(num, text);
    HostInterop::Notify(args);
    return 0;
}


//============= Infrastructure .cpp =============//

//--------------------------------------------------------//
void HostInterop::Run(String^ scriptFn, List<String^>^ luaPath)
{
    InitLua(luaPath);
    // Load C host funcs into lua space.
    luainterop_Load(_l);
    // Clean up stack.
    lua_pop(_l, 1);
    OpenScript(scriptFn);
}
