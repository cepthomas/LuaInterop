
:: Convert spec into interop library.
echo off
cls

set "ODIR=%cd%"
pushd ..\LBOT
set LUA_PATH="%ODIR%\?.lua";?.lua;;
lua gen_interop.lua -c "%ODIR%\interop_spec.lua" "%ODIR%\Interop"
lua gen_interop.lua -cppcli "%ODIR%\interop_spec.lua" "%ODIR%\Interop"
popd


rem :: Build app.
rem call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
rem msbuild CppCli.sln /p:Configuration=Debug /t:Restore -v:n
rem msbuild CppCli.sln /p:Configuration=Debug /t:Build -v:n
