cls
echo off

:: Convert spec into interop library.

:: Build the interop.
pushd ".."
lua gen_interop.lua -csh test_cs\interop_spec_csh.lua test_cs
popd
