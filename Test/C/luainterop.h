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

//============= App => C/Lua functions =============//

// Simple calculations.
// @param[in] l Internal lua state.
// @param[in] op_one Operand 1.
// @param[in] oper Operator: + - * /
// @param[in] op_two Operand 2.
// @return double Calculated answer
double luainterop_Calculator(lua_State* l, double op_one, const char* oper, double op_two);

// String to integer.
// @param[in] l Internal lua state.
// @param[in] day Day name.
// @return int Day number.
int luainterop_DayOfWeek(lua_State* l, const char* day);

// Function with no args.
// @param[in] l Internal lua state.
// @return const char* Day name.
const char* luainterop_FirstDay(lua_State* l);

// Function not implemented in script.
// @param[in] l Internal lua state.
// @return bool Dummy return value.
bool luainterop_InvalidFunc(lua_State* l);

// Function argument type incorrect.
// @param[in] l Internal lua state.
// @param[in] arg1 The arg.
// @return bool Dummy return value.
bool luainterop_InvalidArgType(lua_State* l, const char* arg1);

// Function return type incorrect.
// @param[in] l Internal lua state.
// @return int Dummy return value.
int luainterop_InvalidRetType(lua_State* l);

// Function that calls error().
// @param[in] l Internal lua state.
// @param[in] flavor Tweak behavior.
// @return bool Dummy return value.
bool luainterop_ErrorFunc(lua_State* l, int flavor);

// Function is optional.
// @param[in] l Internal lua state.
// @return int Dummy return value.
int luainterop_OptionalFunc(lua_State* l);


//============= C/Lua => App functions =============//


// Record something for me.
// @param[in] l Internal lua state.
// @param[in] level Log level.
// @param[in] msg What to log.
// @return Dummy return value.
bool luainterop_cb_Log(lua_State* l, int level, const char* msg);

// How hot are you?
// @param[in] l Internal lua state.
// @param[in] temp Temperature.
// @return String environment.
const char* luainterop_cb_GetEnvironment(lua_State* l, double temp);

// Milliseconds.
// @param[in] l Internal lua state.
// @return The time.
int luainterop_cb_GetTimestamp(lua_State* l);

// Raise an error from lua code.
// @param[in] l Internal lua state.
// @return Dummy return value.
bool luainterop_cb_ForceError(lua_State* l);

//============= Infrastructure =============//

/// Load Lua C lib.
void luainterop_Load(lua_State* l);

/// Operation result: Error info string or NULL if OK. 
const char* luainterop_Error();

/// Operation result: lua traceback or NULL if OK. 
const char* luainterop_Context();
