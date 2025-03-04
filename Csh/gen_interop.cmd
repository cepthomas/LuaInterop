cls
echo off

:: Convert spec into interop library.

pushd ..
lua gen_interop.lua -csh Csh\interop_spec.lua Csh
popd

