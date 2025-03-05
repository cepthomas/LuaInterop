
:: Convert spec into interop library.
echo off
cls

set "ODIR=%cd%"
pushd ..
set LUA_PATH=;;"%ODIR%\?.lua";LBOT\?.lua;
lua gen_interop.lua -csh "%ODIR%\interop_spec.lua" "%ODIR%"
popd
