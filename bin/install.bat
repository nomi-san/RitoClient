@echo off & setlocal

net SESSION 1>nul 2>nul
if %errorlevel% NEQ 0 (
    echo Please run as ADMIN.
    pause >nul
    exit /b 2
)

set TARGET=Riot Client.exe
set DEBUGGER="rundll32 \"%~dp0RitoClient.dll\", BootstrapEntry "

reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\%TARGET%" /v "Debugger" /t REG_SZ /d %DEBUGGER% /f

echo Done!
pause >nul
