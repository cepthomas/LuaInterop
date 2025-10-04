:: Convert spec into interop library.

echo off
cls

set "ODIR=%cd%"
cd ..\
set "LDIR=%cd%\LBOT"
set LUA_PATH=%LDIR%\?.lua;%ODIR%\?.lua;?.lua;
cd Generator
lua gen_interop.lua -c "%ODIR%\interop_spec.lua" "%ODIR%\Interop"
:: Cpp flavor also requires the C generaated code.
lua gen_interop.lua -cppcli "%ODIR%\interop_spec.lua" "%ODIR%\Interop"
cd %ODIR%
