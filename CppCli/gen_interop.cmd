cls
echo off

:: Convert spec into interop library.
set "ODIR=%cd%"
pushd ..\LuaBagOfTricks

rem lua gen_interop.lua -c %ODIR%\interop_spec.lua %ODIR%\..\Interop

lua gen_interop.lua -cppcli %ODIR%\interop_spec.lua %ODIR%\..\Interop

popd
