@echo off


IF NOT [%1]==[] (
    fake run build.fsx --target "%1"
) ELSE (
    fake run build.fsx
)

if errorlevel 1 (
  exit /b %errorlevel%
)