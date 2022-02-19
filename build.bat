@echo off & setlocal

rem This script uses the MSBuild Tools, which can be downloaded and installed separately.

set "MSBuild_dir=C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild"
set "csc=%MSBuild_dir%\Current\Bin\Roslyn\csc.exe"

"%csc%" -nologo -platform:x64 -out:kbSend.exe src\* %*
