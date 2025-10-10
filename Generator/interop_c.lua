-- Generate C specific interop code.
-- Uses doxygen style markup.

-- local ut = require('lbot_utils')
local tmpl = require('template')

-- Get specification.
local args = {...}
local spec = args[1]

-- TODO: function required = "true",

--------------------------------------------------------------------------------
---------------------------- Gen C file ----------------------------------------
--------------------------------------------------------------------------------
local tmpl_interop_c =
[[
>local ut = require('lbot_utils')
>local sx = require("stringex")
>local os = require("os")
>local snow = os.date('%Y-%m-%d %H:%M:%S')
///// Generated C and h files that bind interop C to lua C code.       /////
///// Warning - this file is created by do_gen.lua - do not edit. /////

#include "$(config.lua_lib_name).h"
>if config.add_refs then
>for _, inc in ipairs(config.add_refs) do
#include $(inc)
>end
>end

#if defined(_MSC_VER)
// Ignore some generated code warnings
#pragma warning( disable : 6001 4244 4703 4090 )
#endif

static const char* _info;
static const char* _context;


//============= interop C => Lua functions =============//

>for _, func in ipairs(script_funcs) do
>local lua_ret_type = lua_types(func.ret.type)
>local c_ret_type = c_types(func.ret.type)
>local arg_specs = {}
>for _, arg in ipairs(func.args or {}) do
>table.insert(arg_specs, c_types(arg.type).." "..arg.name)
>end -- func.args
>sargs = sx.strjoin(", ", arg_specs)
//--------------------------------------------------------//
>if #sargs > 0 then
$(c_ret_type) $(config.lua_lib_name)_$(func.host_func_name)(lua_State* l, $(sargs))
>else
$(c_ret_type) $(config.lua_lib_name)_$(func.host_func_name)(lua_State* l)
>end -- #sargs
{
    _info = NULL;
    _context = NULL;
    int stat = LUA_OK;
    int num_args = 0;
    int num_ret = 1;
    $(c_ret_type) ret = 0;

    // Get function.
    int ltype = lua_getglobal(l, "$(func.lua_func_name)");
    if (ltype != LUA_TFUNCTION)
    {
        _info = "Script does not implement required function $(func.lua_func_name)()";
        return ret;
    }

    // Push arguments. No error checking required.
>for _, arg in ipairs(func.args or {}) do
>local lua_arg_type = lua_types(arg.type)
    lua_push$(lua_arg_type)(l, $(arg.name));
    num_args++;
>end -- func.args

    // Do the protected call.
    stat = luaex_docall(l, num_args, num_ret);
    if (stat == LUA_OK)
    {
        // Get the results from the stack.
        if (lua_is$(lua_ret_type)(l, -1)) { ret = lua_to$(lua_ret_type)(l, -1); }
        else { _info = "Script function $(func.lua_func_name)() returned wrong type - should be $(lua_ret_type)"; }
    }
    else
    {
        _info = "Script function $(func.lua_func_name)() error";
        // Get the traceback from the stack.
         _context = lua_tostring(l, -1);
    }
    lua_pop(l, num_ret); // Clean up results.
    return ret;
}

>end -- script_funcs

//============= Lua => interop C callback functions =============//

>for _, func in ipairs(host_funcs) do
>local lua_ret_type = lua_types(func.ret.type)
>local c_ret_type = c_types(func.ret.type)
//--------------------------------------------------------//
// $(func.description or "")
// @param[in] l Internal lua state.
// @return Number of lua return values.
>for _, arg in ipairs(func.args or {}) do
// Lua arg: $(arg.name) $(arg.description or "")
>end -- func.args
// Lua return: $(c_ret_type) $(func.ret.description or "")
static int $(config.lua_lib_name)_$(func.host_func_name)(lua_State* l)
{
    // Get arguments
>for i, arg in ipairs(func.args or {}) do
>local lua_arg_type = lua_types(arg.type)
>local c_arg_type = c_types(arg.type)
    $(c_arg_type) $(arg.name);
    if (lua_is$(lua_arg_type)(l, $(i))) { $(arg.name) = lua_to$(lua_arg_type)(l, $(i)); }
    else { luaL_error(l, "Invalid arg type for $(arg.name)"); }
>end -- func.args

    // Do the work. One result.
>local arg_specs = { }
>for _, arg in ipairs(func.args or {}) do
>local c_ret_type = c_types(func.ret.type)
>table.insert(arg_specs, arg.name)
>end -- func.args
>sargs = sx.strjoin(", ", arg_specs)
>if #sargs > 0 then
    $(c_ret_type) ret = $(config.lua_lib_name)cb_$(func.host_func_name)(l, $(sargs));
>else
    $(c_ret_type) ret = $(config.lua_lib_name)cb_$(func.host_func_name)(l);
>end -- #sargs
    lua_push$(lua_ret_type)(l, ret);
    return 1;
}

>end -- host_funcs

//============= Infrastructure =============//

static const luaL_Reg function_map[] =
{
>for _, func in ipairs(host_funcs) do
    { "$(func.lua_func_name)", $(config.lua_lib_name)_$(func.host_func_name) },
>end -- host_funcs
    { NULL, NULL }
};

static int $(config.lua_lib_name)_Open(lua_State* l)
{
    luaL_newlib(l, function_map);
    return 1;
}

void $(config.lua_lib_name)_Load(lua_State* l)
{
    luaL_requiref(l, "$(config.lua_lib_name)", $(config.lua_lib_name)_Open, true);
}

const char* $(config.lua_lib_name)_Info()
{
    return _info;
}

const char* $(config.lua_lib_name)_Context()
{
    return _context;
}
]]


--------------------------------------------------------------------------------
---------------------------- Gen H file ----------------------------------------
--------------------------------------------------------------------------------
local tmpl_interop_h =
[[
#pragma once
>local ut = require('lbot_utils')
>local sx = require("stringex")
>local os = require("os")
>local snow = os.date('%Y-%m-%d %H:%M:%S')
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

>for _, func in ipairs(script_funcs) do
>local lua_ret_type = lua_types(func.ret.type)
>local c_ret_type = c_types(func.ret.type)
// $(func.description or "")
// @param[in] l Internal lua state.
>for _, arg in ipairs(func.args or {}) do
// @param[in] $(arg.name) $(arg.description or "")
>end -- func.args
// @return $(c_ret_type) $(func.ret.description or "")
>local arg_specs = {}
>for _, arg in ipairs(func.args or {}) do
>table.insert(arg_specs, c_types(arg.type).." "..arg.name)
>end -- func.args
>sargs = sx.strjoin(", ", arg_specs)
>if #sargs > 0 then
$(c_ret_type) $(config.lua_lib_name)_$(func.host_func_name)(lua_State* l, $(sargs));
>else
$(c_ret_type) $(config.lua_lib_name)_$(func.host_func_name)(lua_State* l);
>end -- #sargs

>end -- script_funcs

//============= Lua => interop C callback functions =============//
>for _, func in ipairs(host_funcs) do

// $(func.description or "")
// @param[in] l Internal lua state.
>for _, arg in ipairs(func.args or {}) do
// @param[in] $(arg.name) $(arg.description or "")
>end -- func.args
// @return $(func.ret.description)
>local arg_specs = { }
>for _, arg in ipairs(func.args or {}) do
>table.insert(arg_specs, c_types(arg.type).." "..arg.name)
>end -- func.args
>sargs = sx.strjoin(", ", arg_specs)
>if #sargs > 0 then
$(c_types(func.ret.type)) $(config.lua_lib_name)cb_$(func.host_func_name)(lua_State* l, $(sargs));
>else
$(c_types(func.ret.type)) $(config.lua_lib_name)cb_$(func.host_func_name)(lua_State* l);
>end -- #sargs
>end -- host_funcs

//============= Infrastructure =============//

/// Load Lua C lib.
void $(config.lua_lib_name)_Load(lua_State* l);

/// Operation result: Error info string or NULL if OK. 
const char* $(config.lua_lib_name)_Info();

/// Operation result: lua traceback or NULL if OK. 
const char* $(config.lua_lib_name)_Context();
]]


----------------------------------------------------------------------------

-- Make the output content.
local tmpl_env =
{
    _parent=_G,
    _escape='>',
    _debug=true,
    config=spec.config,
    script_funcs=spec.script_funcs,
    host_funcs=spec.host_funcs,
    -- Type name conversions.
    c_types=function(t)
        if t == 'B' then return "bool"
        elseif t == 'I' then return "int"
        elseif t == 'N' then return "double"
        elseif t == 'S' then return "const char*"
        else return 'Invalid spec type '..t
        end
    end,
    lua_types=function(t)
        if t == 'B' then return "boolean"
        elseif t == 'I' then return "integer"
        elseif t == 'N' then return "number"
        elseif t == 'S' then return "string"
        else return 'Invalid spec type '..t
        end
    end
}

local ret = {}

-- c interop part
print('Generating c file')
local rendered, err, dcode = tmpl.substitute(tmpl_interop_c, tmpl_env)
if not err then -- ok
    ret[spec.config.lua_lib_name..".c"] = rendered
else -- failed, look at intermediary code
    ret.err = err
    ret.dcode = dcode
end

-- h interop part
print('Generating h file')
rendered, err, dcode = tmpl.substitute(tmpl_interop_h, tmpl_env)
if not err then -- ok
    ret[spec.config.lua_lib_name..".h"] = rendered
else -- failed, look at intermediary code
    ret.err = err
    ret.dcode = dcode
end

return ret
