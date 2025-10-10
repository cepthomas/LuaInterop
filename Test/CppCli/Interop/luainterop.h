#pragma once
///// Warning - this file is created by do_gen.lua - do not edit. /////

#include <stdbool.h>

#ifdef __cplusplus
#include "lua.hpp"
extern "C" {
#include "luaex.h"
};
#else
#include "lua.h"
#include "luaex.h"
#endif

//============= interop C => Lua functions =============//

// Initialize stuff
// @param[in] l Internal lua state.
// @param[in] opt Option
// @return int Return integer
int luainterop_Setup(lua_State* l, int opt);

// Arbitrary lua function with all arg types
// @param[in] l Internal lua state.
// @param[in] cmd Specific command
// @param[in] arg_B bool argument
// @param[in] arg_I int argument
// @param[in] arg_N number/double argument
// @param[in] arg_S string argument
// @return const char* Function response
const char* luainterop_DoCommand(lua_State* l, const char* cmd, bool arg_B, int arg_I, double arg_N, const char* arg_S);


//============= Lua => interop C callback functions =============//

// Script wants to log something.
// @param[in] l Internal lua state.
// @param[in] msg Log message
// @return Unused
int luainteropcb_Log(lua_State* l, const char* msg);

// Script wants to say something.
// @param[in] l Internal lua state.
// @param[in] arg_I A number
// @param[in] arg_S Some text
// @param[in] arg_B boooooool
// @param[in] arg_N numero/doublo
// @return Back at you
int luainteropcb_Notification(lua_State* l, int arg_I, const char* arg_S, bool arg_B, double arg_N);

//============= Infrastructure =============//

/// Load Lua C lib.
void luainterop_Load(lua_State* l);

/// Operation result: Error info string or NULL if OK. 
const char* luainterop_Info();

/// Operation result: lua traceback or NULL if OK. 
const char* luainterop_Context();
