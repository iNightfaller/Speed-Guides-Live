@echo off

if [%1]==[] goto usage

set OUT_DIR=.\official_builds\%1
mkdir %OUT_DIR%

REM copy relevant files to proper directory
xcopy src\LiveSplit %OUT_DIR%\LiveSplit /S /Y /I
xcopy .\README.md %OUT_DIR% /Y
echo F|xcopy .\LICENSE %OUT_DIR%\LICENSE_SGL.txt /Y
echo F|xcopy src\ext\markdig\license.txt %OUT_DIR%\LICENSE_Markdig.txt /Y

set ZIP_NAME=%OUT_DIR%\SpeedGuidesLive(%1).zip
echo Creating %ZIP_NAME% ...
REM actually create the zip file

goto end

:usage
echo Package.bat VERSION
pause

:end
