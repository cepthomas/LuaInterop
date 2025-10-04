///// Warning - this file is created by gen_interop.lua - do not edit. /////

#include "luainterop.h"

#if defined(_MSC_VER)
// Ignore some generated code warnings
#pragma warning( disable : 6001 4244 4703 4090 )
#endif

static const char* _error;

//============= C => Lua functions .c =============//

//--------------------------------------------------------//
int luainterop_Setup(lua_State* l, int opt)
{
    _error = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    int ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "setup");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Invalid function name setup()";
        return ret;
    }

    // Push arguments. No error checking required.
    lua_pushinteger(l, opt);
    num_args++;

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isinteger(l, -1)) { ret = lua_tointeger(l, -1); }
        else { _error = "Invalid return type for setup() should be integer"; }
    }
    else { _error = lua_tostring(l, -1); }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

//--------------------------------------------------------//
const char* luainterop_DoCommand(lua_State* l, const char* cmd, int arg)
{
    _error = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    const char* ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "do_command");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Invalid function name do_command()";
        return ret;
    }

    // Push arguments. No error checking required.
    lua_pushstring(l, cmd);
    num_args++;
    lua_pushinteger(l, arg);
    num_args++;

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isstring(l, -1)) { ret = lua_tostring(l, -1); }
        else { _error = "Invalid return type for do_command() should be string"; }
    }
    else { _error = lua_tostring(l, -1); }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}


//============= Lua => C callback functions .c =============//

//--------------------------------------------------------//
// Script wants to log something.
// @param[in] l Internal lua state.
// @return Number of lua return values.
// Lua arg: level Log level
// Lua arg: msg Log message
// Lua return: int Unused
static int luainterop_Log(lua_State* l)
{
    // Get arguments
    int level;
    if (lua_isinteger(l, 1)) { level = lua_tointeger(l, 1); }
    else { luaL_error(l, "Invalid arg type for level"); }
    const char* msg;
    if (lua_isstring(l, 2)) { msg = lua_tostring(l, 2); }
    else { luaL_error(l, "Invalid arg type for msg"); }

    // Do the work. One result.
    int ret = luainteropcb_Log(l, level, msg);
    lua_pushinteger(l, ret);
    return 1;
}

//--------------------------------------------------------//
// Script wants to say something.
// @param[in] l Internal lua state.
// @return Number of lua return values.
// Lua arg: num A number
// Lua arg: text Some text
// Lua return: int Unused
static int luainterop_Notification(lua_State* l)
{
    // Get arguments
    int num;
    if (lua_isinteger(l, 1)) { num = lua_tointeger(l, 1); }
    else { luaL_error(l, "Invalid arg type for num"); }
    const char* text;
    if (lua_isstring(l, 2)) { text = lua_tostring(l, 2); }
    else { luaL_error(l, "Invalid arg type for text"); }

    // Do the work. One result.
    int ret = luainteropcb_Notification(l, num, text);
    lua_pushinteger(l, ret);
    return 1;
}


//============= Infrastructure .c =============//

static const luaL_Reg function_map[] =
{
    { "log", luainterop_Log },
    { "notif", luainterop_Notification },
    { NULL, NULL }
};

static int luainterop_Open(lua_State* l)
{
    luaL_newlib(l, function_map);
    return 1;
}

void luainterop_Load(lua_State* l)
{
    luaL_requiref(l, "luainterop", luainterop_Open, true);
}

const char* luainterop_Error()
{
    return _error;
}
