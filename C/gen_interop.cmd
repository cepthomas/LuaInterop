cls
echo off

:: Convert spec into interop library.


set LUA_PATH=;;?;?.lua;\Dev\Lua\LuaBagOfTricks\\?.lua;;
rem set LUA_PATH=;;%~dp0source_code\?.lua;
rem "LUA_PATH": "?;?.lua;..\\..\\lua\\?.lua;;",
rem C:\Dev\Lua\LuaBagOfTricks

echo %LUA_PATH%


:: Build the interop.
pushd ".."
lua gen_interop.lua -c C\interop_spec.lua C
popd
