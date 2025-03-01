cls
echo off

:: Convert spec into interop library.

:: Build the interop.
pushd ".."
lua gen_interop.lua -c C\interop_spec.lua C
popd
