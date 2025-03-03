cls
echo off

:: Convert spec into interop library.

rem set LUA_PATH=;;?;?.lua;\Dev\Lua\LuaBagOfTricks\?.lua;;

pushd ..
lua gen_interop.lua -csh Csh\interop_spec.lua Csh
popd

