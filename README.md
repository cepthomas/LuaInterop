# Refactored to:
- https://github.com/cepthomas/LuaBagOfTricks
- https://github.com/cepthomas/LbotImpl

# LuaInterop

Contains several flavors of how to embed Lua scripting in host languages:
- C: Bog standard using Lua C API.
- CppCli: Creates a .NET assembly for consumption by host.
- Csh: Call directly using [KeraLuaEx](https://github.com/cepthomas/KeraLuaEx.git) which exposes the Lua C API as .NET native methods.

Uses submodule [LuaBagOfTricks](https://github.com/cepthomas/LuaBagOfTricks.git).

It's mostly a Windows project but parts would probably work elsewhere.

## Code Generation

What makes this really interesting is that all the interop code is generated using some Lua template magic.

Generates C# and/or C code using `gen_interop.lua`, `interop_flavor.lua`, and a custom `interop_spec.lua`
file that describes the api and events you need for your application.

`interop_spec.lua` is a plain Lua data file like this:
```lua
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

This is turned into the flavors of interop code using something like:
```
lua gen_interop.lua -csh input_dir\interop_spec.lua output_dir
```

Currently the supported data types are limited to Boolean, Integer, Number, String.
Eventually to add tables, enums, markdown, out refs ...

The intended way to use this is to copy one of the flavors directly, modify the spec file, run the code generator,
modify the host file(s), build the application in VS.
I could have gotten fancier with the code/app generation but this seems more than adequate for something that
will have minimal changes after settling down.

## The Flavors

Each flavor has these files:
- interop_spec.lua - defines your api
- gen_interop.cmd - code generation script
- script_xxx.lua - test/example scripts

These may also appear:
- err_dcode.lua - may be useful for debugging spec file errors
- log.txt - per the application

### C
- C.sln/vcxproj - VS solution
- Host.cpp - main application and callbacks
- luainterop.c/h - generated C <=> Lua interop code

### C++/CLI
Note that this flavor also requires the C flavor.

- CppCli.sln, Host.csproj, Interop.vcxproj - VS solution
- Host.cs - main application and events
- HostInterop.cpp/h - generated C# <=> C interop code
- luainterop.c/h - generated C <=> Lua interop code
- Core.cpp/h - common stuff for C# <=> C interop code

### C#   
- Csh.sln/csproj - VS solution
- Host.cs - main application and events
- HostInterop.cs - interop bindings and callbacks
- Interop.cs - generated C# <=> KeraLuaEx interop code

## Other Stuff

The files that do the work:
- gen_interop.lua - the driver
- interop_c.lua - C flavor
- interop_cppcli.lua - C++/CLI flavor
- interop_csh.lua - C# flavor
