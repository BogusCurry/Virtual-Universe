@echo off
setLocal EnableDelayedExpansion
IF NOT EXIST ..\.svn GOTO DIE

for /f "tokens=* delims= " %%a in (..\.git\logs\HEAD) do (
set var=%%a
)
for /f "tokens=1-7" %%a in ("%var%") do (
echo VirtualReality-%%b %%f>.version
)

:DIE
pause