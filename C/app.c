#include <windows.h>
#include <assert.h>

#include "lua.h"
#include "lualib.h"
#include "lauxlib.h"
#include "luaex.h"
#include "luainterop.h"


// The main Lua thread.
static lua_State* _l;
static int _timestamp = 1000;
static char _last_log[500];

// Top level error handler for status.
static const char* CheckStatus(int stat);


//---------------- Callback host functions from Lua -------------//

//--------------------------------------------------------//
int luainteropcb_GetTimestamp(lua_State* l)
{
    _timestamp += 100;
    return _timestamp;
}

//--------------------------------------------------------//
bool luainteropcb_Log(lua_State* l, int level, const char* msg)
{
    snprintf(_last_log, sizeof(_last_log) - 1, "Log LVL%d %s", level, msg);
    printf(_last_log);
    printf("\n");
    return true;
}

//--------------------------------------------------------//
const char* luainteropcb_GetEnvironment(lua_State* l, double temp)
{
    static char buff[50];
    snprintf(buff, sizeof(buff) - 1, "Temperature is %.1f degrees", temp);
    return buff;
}

//--------------------------------------------------------//
bool luainteropcb_ForceError(lua_State* l)
{
    luaL_error(_l, "Let's blow something up in lua");
    return true;
}


//---------------- Helpers -------------//

//--------------------------------------------------------//
const char* CheckStatus(int stat)
{
    static char buff[500];
    char* sret = NULL;

    if (stat >= LUA_ERRRUN)
    {
        // Error message.
        const char* errmsg = NULL;
        if (stat <= LUA_ERRFILE && lua_gettop(_l) > 0) // internal lua error - get error message on stack if provided.
        {
            errmsg = lua_tostring(_l, -1);
            strncpy(buff, errmsg, sizeof(buff) - 1);
            sret = buff;
        }
    }

    return sret;
}

//---------------- Start here -------------//
int main()
{
    int stat;
    const char* slua_error;
    const char* sinterop_error;

    // Init internal stuff.
    _l = luaL_newstate();

    // Load std libraries.
    luaL_openlibs(_l);

    // Load host funcs into lua space. This table gets pushed on the stack and into globals.
    luainterop_Load(_l);

    // Pop the table off the stack as it interferes with calling the module functions.
    lua_pop(_l, 1);

    ///// Load the bad script file.
    // Pushes the compiled chunk as a Lua function on top of the stack or pushes an error message.
    const char* fn = "";

    // Try to load/compile non-existent file.
    fn = "bad_script_file_name.lua";
    stat = luaL_loadfile(_l, fn);
    assert(stat == LUA_ERRFILE);
    slua_error = CheckStatus(stat);
    assert(slua_error != NULL);
    assert(strstr(slua_error, "cannot open bad_script_file_name.lua: No such file or directory") != NULL);

    fn = "script_load_error.lua";
    stat = luaL_loadfile(_l, fn);
    assert(stat == LUA_ERRSYNTAX);
    slua_error = CheckStatus(stat);
    assert(slua_error != NULL);
    assert(strstr(slua_error, "syntax error near 'ts'") != NULL);

    ///// Load the good script file.
    fn = "script_main.lua";
    stat = luaL_loadfile(_l, fn);
    assert(stat == LUA_OK);
    slua_error = CheckStatus(stat);
    assert(slua_error == NULL);

    // If stat is ok, run the script to init everything.
    stat = lua_pcall(_l, 0, LUA_MULTRET, 0);
    slua_error = CheckStatus(stat);
    assert(stat == LUA_OK);
    assert(slua_error == NULL);

    // Should be ok. This should be set by the script execution.
    assert(strstr(_last_log, "Log LVL1 I know this: ts:1100 env:Temperature is 27.3 degrees") != NULL);

    //luautils_DumpGlobals(_l, stdout);

    // Call script functions.
    char op[] = {"*"};
    double answer = luainterop_Calculator(_l, 12.96, op, 3.15);
    sinterop_error = luainterop_Error();
    assert(sinterop_error == NULL);
    assert(answer > 40.823 && answer < 40.825);

    char day[] = { "Moonday" };
    int daynum = luainterop_DayOfWeek(_l, day);
    sinterop_error = luainterop_Error();
    assert(sinterop_error == NULL);
    assert(daynum == 3);

    const char* dayname = luainterop_FirstDay(_l);
    sinterop_error = luainterop_Error();
    assert(sinterop_error == NULL);
    assert(strcmp(dayname, "Hamday") == 0);

    int dummy = luainterop_InvalidFunc(_l);
    sinterop_error = luainterop_Error();
    assert(sinterop_error != NULL);
    assert(strstr(sinterop_error, "Bad function name: invalid_func()") != NULL);

    char arg[] = { "abc" };
    dummy = luainterop_InvalidArgType(_l, arg);
    sinterop_error = luainterop_Error();
    assert(sinterop_error != NULL);
    assert(strstr(sinterop_error, "attempt to add a 'string' with a 'number'") != NULL);

    dummy = luainterop_InvalidRetType(_l);
    sinterop_error = luainterop_Error();
    assert(sinterop_error != NULL);
    assert(strstr(sinterop_error, "Bad return type for invalid_ret_type(): should be integer") != NULL);

    // Force error - C calls lua which calls error(). This is fatal.
    dummy = luainterop_ErrorFunc(_l, 1);
    sinterop_error = luainterop_Error();
    assert(sinterop_error != NULL);
    assert(strstr(sinterop_error, "user_lua_func3() raises error()") != NULL);

    // Force error - C calls lua which calls C which calls luaL_error().
    dummy = luainterop_ErrorFunc(_l, 2);
    sinterop_error = luainterop_Error();
    assert(sinterop_error != NULL);
    assert(strstr(sinterop_error, "Let's blow something up in lua") != NULL);

    // Try to call optional function.
    dummy = luainterop_OptionalFunc(_l);
    sinterop_error = luainterop_Error();
    assert(sinterop_error == NULL);

    // Done.
    lua_close(_l);

    return 0;
}
