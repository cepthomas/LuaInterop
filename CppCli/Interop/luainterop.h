#pragma once
///// Warning - this file is created by gen_interop.lua - do not edit. /////

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

//============= C => Lua functions .h =============//

// Initialize stuff.
// @param[in] l Internal lua state.
// @param[in] opt Option
// @return int Return some integer
int luainterop_Setup(lua_State* l, int opt);

// Arbitrary lua function.
// @param[in] l Internal lua state.
// @param[in] cmd Specific command
// @param[in] arg Optional argument
// @return const char* Script response
const char* luainterop_DoCommand(lua_State* l, const char* cmd, int arg);


//============= Lua => C callback functions .h =============//

// Script wants to log something.
// @param[in] l Internal lua state.
// @param[in] level Log level
// @param[in] msg Log message
// @return Unused
int luainteropcb_Log(lua_State* l, int level, const char* msg);

// Script wants to say something.
// @param[in] l Internal lua state.
// @param[in] num A number
// @param[in] text Some text
// @return Unused
int luainteropcb_Notification(lua_State* l, int num, const char* text);

//============= Infrastructure .h =============//

/// Load Lua C lib.
void luainterop_Load(lua_State* l);

/// Return operation error or NULL if ok.
const char* luainterop_Error();
