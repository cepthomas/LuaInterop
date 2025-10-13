-- Generate .NET C++/CLI interop code.
-- Uses .NET style markup.

-- local ut = require('lbot_utils')
local tmpl = require('template')

-- Get specification.
local args = {...}
local spec = args[1]


--------------------------------------------------------------------------------
---------------------------- Gen C++ file --------------------------------------
--------------------------------------------------------------------------------
local tmpl_interop_cpp =
[[
>local ut = require('lbot_utils')
>local sx = require("stringex")
>local os = require("os")
>local snow = os.date('%Y-%m-%d %H:%M:%S')
///// Warning - this file is created by do_gen.lua - do not edit. /////

#include <windows.h>
#include "$(config.lua_lib_name).h"
#include "$(config.class_name).h"
>if config.add_refs then
>  for _, inc in ipairs(config.add_refs) do
#include $(inc)
>  end
>end

using namespace System;
using namespace System::Collections::Generic;


//============= Cpp/CLI => C/Lua functions =============//

>for _, func in ipairs(script_funcs) do
>  local arg_spec = {}
>  local arg_impl = {}
>  for _, arg in ipairs(func.args or {}) do
>    table.insert(arg_spec, cpp_types(arg.type).." "..arg.name)
>    if arg.type == 'S' then
>       table.insert(arg_impl, "ToCString("..arg.name..")")
>    else
>       table.insert(arg_impl, arg.name)
>    end
>  end -- func.args
>  sarg_spec = sx.strjoin(", ", arg_spec)
>  sarg_impl = sx.strjoin(", ", arg_impl)
>  if #sarg_impl > 0 then sarg_impl = ", "..sarg_impl end
//--------------------------------------------------------//
>  if #sarg_spec > 0 then
$(cpp_types(func.ret.type)) $(config.class_name)::$(func.host_func_name)($(sarg_spec))
>  else
$(cpp_types(func.ret.type)) $(config.class_name)::$(func.host_func_name)()
>  end -- #sarg_spec
{
    SCOPE();
>    if func.ret.type == 'S' then
    $(cpp_types(func.ret.type)) ret = gcnew String($(config.lua_lib_name)_$(func.host_func_name)(_l$(sarg_impl)));
>    else
    $(cpp_types(func.ret.type)) ret = $(config.lua_lib_name)_$(func.host_func_name)(_l$(sarg_impl));
>    end
    if (luainterop_Error() != NULL) { throw(gcnew LuaException(gcnew String(luainterop_Error()), luainterop_Context() == NULL ? "" : gcnew String(luainterop_Context()))); }
    return ret; 
}

>end -- script_funcs

//============= C/Lua => Cpp/CLI functions =============//

>for _, func in ipairs(host_funcs) do
>  local arg_spec = {}
>  local arg_impl = {}
>  for _, arg in ipairs(func.args or {}) do
>    table.insert(arg_spec, c_types(arg.type).." "..arg.name)
>    table.insert(arg_impl, arg.name)
>  end -- func.args
>  sarg_spec = sx.strjoin(", ", arg_spec)
>  sarg_impl = sx.strjoin(", ", arg_impl)

//--------------------------------------------------------//

int $(config.lua_lib_name)_cb_$(func.host_func_name)(lua_State* l, $(sarg_spec))
{
    SCOPE();
    $(func.host_func_name)Args^ args = gcnew $(func.host_func_name)Args($(sarg_impl));
    $(config.class_name)::Notify(args);
    return args->ret;
}

>end -- host_funcs

//============= Infrastructure =============//

//--------------------------------------------------------//
void $(config.class_name)::RunScript(String^ scriptFn, String^ luaPath)
{
    SCOPE();
    InitLua(luaPath);
    // Load C host funcs into lua space.
    $(config.lua_lib_name)_Load(_l);
    // Clean up stack.
    lua_pop(_l, 1);
    OpenScript(scriptFn);
}

//--------------------------------------------------------//
void $(config.class_name)::RunChunk(String^ code, String^ name, String^ luaPath)
{
    SCOPE();
    InitLua(luaPath);
    // Load C host funcs into lua space.
    $(config.lua_lib_name)_Load(_l);
    // Clean up stack.
    lua_pop(_l, 1);
    OpenChunk(code, name);
}
]]


--------------------------------------------------------------------------------
---------------------------- Gen H file ----------------------------------------
--------------------------------------------------------------------------------
local tmpl_interop_h =
[[
>local ut = require('lbot_utils')
>local sx = require("stringex")
>local os = require("os")
>local snow = os.date('%Y-%m-%d %H:%M:%S')
///// Warning - this file is created by do_gen.lua - do not edit. /////

#pragma once
#include "cliex.h"

using namespace System;
using namespace System::Collections::Generic;

//============= C/Lua => Cpp/CLI payloads =============//

>for _, func in ipairs(host_funcs) do
//--------------------------------------------------------//
public ref class $(func.host_func_name)Args : public EventArgs
{
public:
>  local arg_spec = {}
>  for _, arg in ipairs(func.args or {}) do
>    table.insert(arg_spec, c_types(arg.type).." "..arg.name)
    /// <summary>$(arg.description)</summary>
    property $(cpp_types(arg.type)) $(arg.name);
>  end -- func.args
    /// <summary>$(func.ret.description)</summary>
    property $(cpp_types(func.ret.type)) ret;
>  sarg_spec = sx.strjoin(", ", arg_spec)
    /// <summary>Constructor.</summary>
    $(func.host_func_name)Args($(sarg_spec))
    {
>  for _, arg in ipairs(func.args or {}) do
>    if arg.type == 'S' then
        this->$(arg.name) = gcnew String($(arg.name));
>    else
        this->$(arg.name) = $(arg.name);
>    end
>  end -- func.args
    }
};

>end -- host_funcs

//----------------------------------------------------//
public ref class $(config.class_name) : CliEx
{

//============= Cpp/CLI => C/Lua functions =============//
public:

>for _, func in ipairs(script_funcs) do
>  local arg_spec = {}
    /// <summary>$(func.host_func_name)</summary>
>  for _, arg in ipairs(func.args or {}) do
>    table.insert(arg_spec, cpp_types(arg.type).." "..arg.name)
    /// <param name="$(arg.name)">$(arg.description)</param>
>  end -- func.args
>  sarg_spec = sx.strjoin(", ", arg_spec)
    /// <returns>Script return</returns>
    $(cpp_types(func.ret.type)) $(func.host_func_name)($(sarg_spec));

>end -- script_funcs
//============= C/Lua => Cpp/CLI functions =============//
public:
>for _, func in ipairs(host_funcs) do
    static event EventHandler<$(func.host_func_name)Args^>^ $(func.host_func_name);
    static void Notify($(func.host_func_name)Args^ args) { $(func.host_func_name)(nullptr, args); }

>end -- host_funcs

//============= Infrastructure =============//
public:
    /// <summary>Initialize and execute script file.</summary>
    /// <param name="scriptFn">The script to load.</param>
    /// <param name="luaPath">LUA_PATH components</param>
    void RunScript(String^ scriptFn, String^ luaPath);

    /// <summary>Initialize and execute a chunk of lua code.</summary>
    /// <param name="code">The lua code to load.</param>
    /// <param name="name">Lua chunk name</param>
    /// <param name="luaPath">LUA_PATH components</param>
    void RunChunk(String^ code, String^ name, String^ luaPath);
};
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
        else return 'Invalid spec type: '..t
        end
    end,
    cpp_types=function(t)
        if t == 'B' then return "bool"
        elseif t == 'I' then return "int"
        elseif t == 'N' then return "double"
        elseif t == 'S' then return "String^"
        else return 'Invalid spec type: '..t
        end
    end
}

local ret = {}

-- cpp interop part
print('Generating cpp file')
local rendered, err, dcode = tmpl.substitute(tmpl_interop_cpp, tmpl_env)
if not err then -- ok
    ret[spec.config.class_name..".cpp"] = rendered
else -- failed, look at intermediary code
    ret.err = err
    ret.dcode = dcode
end

-- h interop part
print('Generating h file')
rendered, err, dcode = tmpl.substitute(tmpl_interop_h, tmpl_env)
if not err then -- ok
    ret[spec.config.class_name..".h"] = rendered
else -- failed, look at intermediary code
    ret.err = err
    ret.dcode = dcode
end

return ret
