cls
echo off

:: Convert spec into interop library.

set LUA_PATH=;;?;?.lua;\Dev\Lua\LuaBagOfTricks\?.lua;;

pushd ..
lua gen_interop.lua -csh Csh\interop_spec.lua Csh
popd


rem pushd ".."
rem lua gen_interop.lua -csh test_cs\interop_spec_csh.lua test_cs
rem popd
