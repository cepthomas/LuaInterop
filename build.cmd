
:: Build everythig.
echo off
cls

:: symlink: mklink /d C:\Dev\Libs\LuaInterop\LBOT C:\Dev\Libs\LuaBagOfTricks
:: verbosity levels: q[uiet], m[inimal], n[ormal] (default), d[etailed], and diag[nostic].

rem set "ODIR=%cd%"

call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"


rem pushd C
rem msbuild C.sln /p:Configuration=Debug /t:Restore -v:n
rem msbuild C.sln /p:Configuration=Debug /t:Build -v:n
rem popd

rem pushd KeraLuaEx
rem msbuild KeraLuaEx.sln /p:Configuration=Debug /t:Restore -v:n
rem msbuild KeraLuaEx.sln /p:Configuration=Debug /t:Build -v:n
rem popd

rem pushd Csh
rem msbuild Csh.sln /p:Configuration=Debug /t:Restore -v:n
rem msbuild Csh.sln /p:Configuration=Debug /t:Build -v:n
rem popd

pushd CppCli
msbuild CppCli.sln /p:Configuration=Debug /t:Restore -v:n
msbuild CppCli.sln /p:Configuration=Debug /t:Build -v:n
popd

