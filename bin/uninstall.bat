@echo off & setlocal

net SESSION 1>nul 2>nul
if %errorlevel% NEQ 0 (
    echo Please run as ADMIN.
    pause >nul
    exit /b 2
)

set TARGET=RiotClientUx.exe

reg delete "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\%TARGET%" /v "Debugger" /f

echo Done!
pause >nul