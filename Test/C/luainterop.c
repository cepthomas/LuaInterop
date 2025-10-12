///// Generated C and h files that bind interop C to lua C code.       /////
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
double luainterop_Calculator(lua_State* l, double op_one, const char* oper, double op_two)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    double ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "calculator");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function calculator()";
        return ret;
    }

    // Push arguments. No error checking required.
    lua_pushnumber(l, op_one);
    num_args++;
    lua_pushstring(l, oper);
    num_args++;
    lua_pushnumber(l, op_two);
    num_args++;

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isnumber(l, -1)) { ret = lua_tonumber(l, -1); }
        else { _error = "Script function calculator() returned wrong type - should be number"; }
    }
    else
    {
        _error = "Script function calculator() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

//--------------------------------------------------------//
int luainterop_DayOfWeek(lua_State* l, const char* day)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    int ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "day_of_week");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function day_of_week()";
        return ret;
    }

    // Push arguments. No error checking required.
    lua_pushstring(l, day);
    num_args++;

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isinteger(l, -1)) { ret = lua_tointeger(l, -1); }
        else { _error = "Script function day_of_week() returned wrong type - should be integer"; }
    }
    else
    {
        _error = "Script function day_of_week() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

//--------------------------------------------------------//
const char* luainterop_FirstDay(lua_State* l)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    const char* ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "first_day");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function first_day()";
        return ret;
    }

    // Push arguments. No error checking required.

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isstring(l, -1)) { ret = lua_tostring(l, -1); }
        else { _error = "Script function first_day() returned wrong type - should be string"; }
    }
    else
    {
        _error = "Script function first_day() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

//--------------------------------------------------------//
bool luainterop_InvalidFunc(lua_State* l)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    bool ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "invalid_func");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function invalid_func()";
        return ret;
    }

    // Push arguments. No error checking required.

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isboolean(l, -1)) { ret = lua_toboolean(l, -1); }
        else { _error = "Script function invalid_func() returned wrong type - should be boolean"; }
    }
    else
    {
        _error = "Script function invalid_func() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

//--------------------------------------------------------//
bool luainterop_InvalidArgType(lua_State* l, const char* arg1)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    bool ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "invalid_arg_type");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function invalid_arg_type()";
        return ret;
    }

    // Push arguments. No error checking required.
    lua_pushstring(l, arg1);
    num_args++;

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isboolean(l, -1)) { ret = lua_toboolean(l, -1); }
        else { _error = "Script function invalid_arg_type() returned wrong type - should be boolean"; }
    }
    else
    {
        _error = "Script function invalid_arg_type() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

//--------------------------------------------------------//
int luainterop_InvalidRetType(lua_State* l)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    int ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "invalid_ret_type");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function invalid_ret_type()";
        return ret;
    }

    // Push arguments. No error checking required.

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isinteger(l, -1)) { ret = lua_tointeger(l, -1); }
        else { _error = "Script function invalid_ret_type() returned wrong type - should be integer"; }
    }
    else
    {
        _error = "Script function invalid_ret_type() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

//--------------------------------------------------------//
bool luainterop_ErrorFunc(lua_State* l, int flavor)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    bool ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "error_func");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function error_func()";
        return ret;
    }

    // Push arguments. No error checking required.
    lua_pushinteger(l, flavor);
    num_args++;

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isboolean(l, -1)) { ret = lua_toboolean(l, -1); }
        else { _error = "Script function error_func() returned wrong type - should be boolean"; }
    }
    else
    {
        _error = "Script function error_func() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

//--------------------------------------------------------//
int luainterop_OptionalFunc(lua_State* l)
{
    _error = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    int ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "optional_func");
    if (ltype != LUA_TFUNCTION)
    {
        _error = "Script does not implement function optional_func()";
        return ret;
    }

    // Push arguments. No error checking required.

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_isinteger(l, -1)) { ret = lua_tointeger(l, -1); }
        else { _error = "Script function optional_func() returned wrong type - should be integer"; }
    }
    else
    {
        _error = "Script function optional_func() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}


//============= Lua => interop C callback functions =============//

//--------------------------------------------------------//
// Record something for me.
// @param[in] l Internal lua state.
// @return Number of lua return values.
// Lua arg: level Log level.
// Lua arg: msg What to log.
// Lua return: bool Dummy return value.
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
    bool ret = luainterop_cb_Log(l, level, msg);
    lua_pushboolean(l, ret);
    return 1;
}

//--------------------------------------------------------//
// How hot are you?
// @param[in] l Internal lua state.
// @return Number of lua return values.
// Lua arg: temp Temperature.
// Lua return: const char* String environment.
static int luainterop_GetEnvironment(lua_State* l)
{
    // Get arguments
    double temp;
    if (lua_isnumber(l, 1)) { temp = lua_tonumber(l, 1); }
    else { luaL_error(l, "Invalid arg type for temp"); }

    // Do the work. One result.
    const char* ret = luainterop_cb_GetEnvironment(l, temp);
    lua_pushstring(l, ret);
    return 1;
}

//--------------------------------------------------------//
// Milliseconds.
// @param[in] l Internal lua state.
// @return Number of lua return values.
// Lua return: int The time.
static int luainterop_GetTimestamp(lua_State* l)
{
    // Get arguments

    // Do the work. One result.
    int ret = luainterop_cb_GetTimestamp(l);
    lua_pushinteger(l, ret);
    return 1;
}

//--------------------------------------------------------//
// Raise an error from lua code.
// @param[in] l Internal lua state.
// @return Number of lua return values.
// Lua return: bool Dummy return value.
static int luainterop_ForceError(lua_State* l)
{
    // Get arguments

    // Do the work. One result.
    bool ret = luainterop_cb_ForceError(l);
    lua_pushboolean(l, ret);
    return 1;
}


//============= Infrastructure =============//

static const luaL_Reg function_map[] =
{
    { "log", luainterop_Log },
    { "get_environment", luainterop_GetEnvironment },
    { "get_timestamp", luainterop_GetTimestamp },
    { "force_error", luainterop_ForceError },
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
