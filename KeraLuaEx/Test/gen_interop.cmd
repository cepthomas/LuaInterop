:: Convert spec into interop library.

echo off
cls

set "ODIR=%cd%"
cd ..\..\
set "LDIR=%cd%\LBOT"
set LUA_PATH=%LDIR%\?.lua;%ODIR%\?.lua;?.lua;
cd Generator
lua gen_interop.lua -csh "%ODIR%\interop_spec.lua" "%ODIR%"
cd %ODIR%
