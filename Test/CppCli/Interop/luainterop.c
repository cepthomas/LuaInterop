///// Warning - this file is created by do_gen.lua - do not edit. /////

#include "luainterop.h"

#if defined(_MSC_VER)
// Ignore some generated code warnings
#pragma warning( disable : 6001 4244 4703 4090 )
#endif

static const char* _error;
static const char* _context;


//============= interop C => Lua functions =============//

//--------------------------------------------------------//
int luainterop_Setup(lua_State* l, int opt)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    int ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "setup");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function setup()";
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
        else { _error = "Script function setup() returned wrong type - should be integer"; }
    }
    else
    {
        _error = "Script function setup() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

//--------------------------------------------------------//
const char* luainterop_DoCommand(lua_State* l, const char* cmd, bool arg_B, int arg_I, double arg_N, const char* arg_S)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    const char* ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "do_command");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function do_command()";
        return ret;
    }

    // Push arguments. No error checking required.
    lua_pushstring(l, cmd);
    num_args++;
    lua_pushboolean(l, arg_B);
    num_args++;
    lua_pushinteger(l, arg_I);
    num_args++;
    lua_pushnumber(l, arg_N);
    num_args++;
    lua_pushstring(l, arg_S);
    num_args++;

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isstring(l, -1)) { ret = lua_tostring(l, -1); }
        else { _error = "Script function do_command() returned wrong type - should be string"; }
    }
    else
    {
        _error = "Script function do_command() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}


//============= Lua => interop C callback functions =============//

//--------------------------------------------------------//
// Script wants to log something.
// @param[in] l Internal lua state.
// @return Number of lua return values.
// Lua arg: msg Log message
// Lua return: int Unused
static int luainterop_Log(lua_State* l)
{
    // Get arguments
    const char* msg;
    if (lua_isstring(l, 1)) { msg = lua_tostring(l, 1); }
    else { luaL_error(l, "Invalid arg type for msg"); }

    // Do the work. One result.
    int ret = luainteropcb_Log(l, msg);
    lua_pushinteger(l, ret);
    return 1;
}

//--------------------------------------------------------//
// Script wants to say something.
// @param[in] l Internal lua state.
// @return Number of lua return values.
// Lua arg: arg_I A number
// Lua arg: arg_S Some text
// Lua arg: arg_B boooooool
// Lua arg: arg_N numero/doublo
// Lua return: int Back at you
static int luainterop_Notification(lua_State* l)
{
    // Get arguments
    int arg_I;
    if (lua_isinteger(l, 1)) { arg_I = lua_tointeger(l, 1); }
    else { luaL_error(l, "Invalid arg type for arg_I"); }
    const char* arg_S;
    if (lua_isstring(l, 2)) { arg_S = lua_tostring(l, 2); }
    else { luaL_error(l, "Invalid arg type for arg_S"); }
    bool arg_B;
    if (lua_isboolean(l, 3)) { arg_B = lua_toboolean(l, 3); }
    else { luaL_error(l, "Invalid arg type for arg_B"); }
    double arg_N;
    if (lua_isnumber(l, 4)) { arg_N = lua_tonumber(l, 4); }
    else { luaL_error(l, "Invalid arg type for arg_N"); }

    // Do the work. One result.
    int ret = luainteropcb_Notification(l, arg_I, arg_S, arg_B, arg_N);
    lua_pushinteger(l, ret);
    return 1;
}


//============= Infrastructure =============//

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

const char* luainterop_Context()
{
    return _context;
}
