#pragma once
///// Warning - this file is created by gen_interop.lua - do not edit. 2025-03-04 13:01:56 /////

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

// Initialize.
// @param[in] l Internal lua state.
// @param[in] opt Option
// @return int Unused
int luainterop_Setup(lua_State* l, int opt);

// Host executes arbitrary lua function.
// @param[in] l Internal lua state.
// @param[in] cmd Specific command
// @param[in] arg Optional argument
// @return const char* Script response
const char* luainterop_DoCommand(lua_State* l, const char* cmd, const char* arg);


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
