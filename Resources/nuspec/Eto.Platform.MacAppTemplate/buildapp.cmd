@echo off
REM This script creates an .app bundle for your application

set DIR=%~dp0
IF %DIR:~-1%==\ SET DIR=%DIR:~0,-1%

SET target_dir=%~1
IF %target_dir:~-1%==\ SET target_dir=%target_dir:~0,-1%
SET assembly=%~2
SET project_name=%~3

SET input_app=%DIR%\MyApp.app
SET output_app=%target_dir%\%project_name%.app
SET output_mono=%output_app%\Contents\MonoBundle

SET cp=xcopy /y /i

echo Copy MyApp.app to %project_name%.app
del /s /q "%output_app%"
%cp% /e "%input_app%" "%output_app%"
del "%output_mono%\README.txt"

echo Copy assemblies into %project_name%.app bundle
%cp% "%target_dir%\%assembly%" "%output_mono%"
%cp% "%target_dir%\*.dll" "%output_mono%"
del /q "%output_mono%\Eto.*"

echo Copy Eto Mac platform into .app bundle
%cp% "%target_dir%\Eto.dll" "%output_mono%"
%cp% "%target_dir%\Eto.Mac.dll" "%output_mono%"
%cp% "%target_dir%\MonoMac.dll" "%output_mono%"

echo Update Info.plist app name and assembly
call :replacetext """>MyApp<""" """>%project_name%<""" "%output_app%\Contents\" Info.plist
call :replacetext """>MyApp.exe<""" """>%assembly%<""" "%output_app%\Contents\" Info.plist

GOTO :eof


:replacetext

set INTEXTFILE=%~4
set OUTTEXTFILE=%~4_temp.txt
set IN_PATH=%~3
IF %IN_PATH:~-1%==\ SET IN_PATH=%IN_PATH:~0,-1%
set INTEXTFILE_PATH="%IN_PATH%\%INTEXTFILE%"
set SEARCHTEXT="%~1"
set SEARCHTEXT="%SEARCHTEXT:""="%"
set REPLACETEXT="%~2"
set REPLACETEXT="%REPLACETEXT:""="%"
rem Replacing %SEARCHTEXT% with %REPLACETEXT%

rem Note: need to keep LF line endings otherwise script won't run on OS X
powershell -Command $contenttext = (Get-Content ""%INTEXTFILE_PATH%"") -join """`n"""^
| Foreach-Object {$_ -replace ""%SEARCHTEXT%"", ""%REPLACETEXT%""}^

Set-Content ""%INTEXTFILE_PATH%"" ([byte[]][char[]] $contenttext) -Encoding Byte
goto :eof