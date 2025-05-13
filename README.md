# LuaInterop

Contains several flavors of how to embed Lua scripting in host languages:
- C: Bog standard using Lua C API.
- CppCli: Creates a .NET assembly for consumption by host. This also demonstrates use of embedded debugger.
- Csh: Call directly using [KeraLuaEx](https://github.com/cepthomas/KeraLuaEx.git) which exposes the Lua C API as .NET native methods.

Building and running requires access to LuaBagOfTricks in a folder named `LBOT` at the top level
of the repo. You can add it is a submodule, plain copy, or symlink as preferred.
`mklink /d some_path\NTerm\Script\LBOT other_path\LuaBagOfTricks'.

It's mostly a Windows project but parts would probably work elsewhere.

The intended way to use this is to copy one of the flavors directly, modify the spec file, run the code generator,
modify the host file(s), build the application in VS.
I could have gotten fancier with the code/app generation but this seems more than adequate for something that
will have minimal changes after settling down.

# Code Generation

C# and/or C code is generated using `gen_interop.lua`, `interop_<flavor>.lua`, and a custom `interop_spec.lua`
file that describes the bidirectional api you need for your application.

`interop_spec.lua` is a plain Lua data file. It has thrree sections:
  - `M.config` specifies identifiers to be used for artifacts.
  - `M.script_funcs` specifies the script functions the application can call.
  - `M.host_funcs` specifies the application functions the script can call.

```lua
-- For C:
M.config =
{
    lua_lib_name = "luainterop",    -- for require
}

-- For CppCli:
M.config =
{
    lua_lib_name = "luainterop",    -- for require
    class_name = "Interop",         -- host filenames
    namespace = "CppCli"            -- host namespace
    add_refs = { "other.h", },      -- for #include (optional)
}

-- For Csh:
{
    lua_lib_name = "luainterop",            -- for require, also filename
    file_name = "Interop",                  -- host filename
    namespace = "Csh",                      -- host namespace
    class_name = "App",                     -- host classname
    add_refs = { "System.Diagnostics", },   -- for using (optional)
}

------------------------ Host => Script ------------------------
M.script_funcs =
{
    {
        lua_func_name = "my_lua_func",
        host_func_name = "MyLuaFunc",
        required = "true",
        description = "Tell me something good.",
        args =
        {
            { name = "arg_one", type = "S", description = "some strings" },
            { name = "arg_two", type = "I", description = "a nice integer" },
        },
        ret = { type = "T", description = "a returned thing" }
    },
}

------------------------ Script => Host ------------------------
M.host_funcs =
{
    {
        lua_func_name = "log",
        host_func_name = "Log",
        description = "Script wants to log something.",
        args =
        {
            { name = "level", type = "I", description = "Log level" },
            { name = "msg", type = "S", description = "Log message" },
        },
        ret = { type = "I", description = "Unused" }
    },
}
```

This is turned into the flavors of interop code using a command like:
```
lua gen_interop.lua -csh input_dir\interop_spec.lua output_dir
```

Currently the supported api data types are limited to boolean, integer, number, string.
The Csh flavor has an experimental table implementation.

# The Flavors

Comprehensive examples are provided in the `C`, `CppCli`, and `Csh` folders.

Each flavor has these files:
- interop_spec.lua - defines your api
- gen_interop.cmd - code generation script
- script_xxx.lua - test/example scripts

The files that do the work:
- gen_interop.lua - the driver
- interop_c.lua - C flavor
- interop_cppcli.lua - C++/CLI flavor
- interop_csh.lua - C# flavor

These may also appear:
- err_dcode.lua - may be useful for debugging spec file errors
- log.txt - per the application

## C
- C.sln/vcxproj - VS solution
- app.c - main application and callbacks
- luainterop.c/h - generated C <=> Lua interop code

## C++/CLI
Note that this flavor also requires the C flavor.

- CppCli.sln, CppCli.csproj, Interop.vcxproj - VS solution
- App.cs - main application and events
- Interop.cpp/h - generated C# <=> C interop code
- luainterop.c/h - generated C <=> Lua interop code
- Core.cpp/h - common stuff for C# <=> C interop code

## C#   
- Csh.sln/csproj - VS solution
- App.cs - main application and events
- Interop.cs - generated C# <=> KeraLuaEx interop code

## Other Stuff

