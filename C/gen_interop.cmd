cls
rem echo off

:: Convert spec into interop library.

set LUA_PATH=;;?;?.lua;\Dev\Lua\LuaBagOfTricks\\?.lua;;

pushd ..
lua gen_interop.lua -c C\interop_spec.lua C\Interop
popd
