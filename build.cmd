
:: Build everything in this repository.

echo off
cls

:: verbosity levels: q[uiet], m[inimal], n[ormal] (default), d[etailed], and diag[nostic].

rem set "ODIR=%cd%"

call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"


pushd KeraLuaEx
msbuild KeraLuaEx.sln /p:Configuration=Debug /t:Restore -v:n
msbuild KeraLuaEx.sln /p:Configuration=Debug /t:Build -v:n
popd

pushd Test/C
msbuild C.sln /p:Configuration=Debug /t:Restore -v:n
msbuild C.sln /p:Configuration=Debug /t:Build -v:n
popd

pushd Test/Csh
msbuild Csh.sln /p:Configuration=Debug /t:Restore -v:n
msbuild Csh.sln /p:Configuration=Debug /t:Build -v:n
popd

pushd Test/CppCli
msbuild CppCli.sln /p:Configuration=Debug /t:Restore -v:n
msbuild CppCli.sln /p:Configuration=Debug /t:Build -v:n
popd

