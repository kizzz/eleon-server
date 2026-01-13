@echo off
cd /d %~dp0
.\Eleonsoft.Auth.Host.exe --migrate=true --seed=true
pause