@ECHO OFF

ECHO =======================================
ECHO Starting Universe Grid Region . . .
ECHO =======================================

chdir /D  %~dp0
cd .\bin
.\Universe.exe -skipconfig
cd ..
Echo.
Echo Universe stopped . . .

set /p nothing= Enter to continue
exit
