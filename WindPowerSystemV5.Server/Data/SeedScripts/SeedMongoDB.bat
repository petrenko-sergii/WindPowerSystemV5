@echo off
cd /d C:\Windows\System32
echo Precondition: MongoDB Shell must be installed 
echo and the resulting path for mongosh.exe must be added to the PATH environment variable

echo Starting Mongo Shell and seeding News collection...
mongosh < "%~dp0SeedNews.ps1"
pause
