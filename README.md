# LuaInterop TODO1
- Making lua life easier. For me.
- Pure Lua so far so *should* work anywhere.
- Uses 64 bit Lua 5.4.2 from https://luabinaries.sourceforge.net/download.html. lua54 contains the reference lib used in other repos.


Handy collected odds and ends for tables, math, validation, errors, ...


## interop
Generates C# and C code for the standard lua interop via `gen_interop.lua`.
Two test projects demonstrate how to use it:
- Ch: Fully-formed and functional, used by Nebulua
- Csh: Partially implemented, uses KeraLuaEx/



===================

AppInterop is between C# and C++/CLI.

luainterop is between C and lua.



{
  "profiles": {
    "LuaSharp": {
      "commandName": "Project",
      "commandLineArgs": "C:\\Dev\\Apps\\LuaSharp\\App\\test.lua",
      "nativeDebugging": true
    }
  }
}

