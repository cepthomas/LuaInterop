cls
echo off

:: Convert spec into interop library.

set LUA_PATH=;;?;?.lua;\Dev\Lua\LuaBagOfTricks\?.lua;;

pushd ..
lua gen_interop.lua -c CppCli\interop_spec.lua CppCli\Interop
lua gen_interop.lua -cppcli CppCli\interop_spec.lua CppCli\Interop
popd
