@echo off

set root=%~dp0
set file_solution=%root%\pcsc-sharp.sln
set dir_packages=%root%\packages
set paket=%root%\paket.cmd

call %paket% restore
if errorlevel 1 (
  exit /b %errorlevel%
)

set file_buildlog=%~dp0\%~n0.log
del %file_buildlog% >nul 2>&1

IF NOT [%1]==[] (
  "%dir_packages%\FAKE\tools\Fake.exe" "%root%\build.fsx" "%1" --logfile "%file_buildlog%"
) ELSE (
  "%dir_packages%\FAKE\tools\Fake.exe" "%root%\build.fsx" --logfile "%file_buildlog%"
)