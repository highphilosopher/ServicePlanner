@echo off
REM ServicePlanner Docker Build Script
REM This script builds the Docker image for ServicePlanner

echo Building ServicePlanner Docker image...

REM Build the Docker image
docker build -t serviceplanner:latest .

if %ERRORLEVEL% neq 0 (
    echo Error: Docker build failed!
    pause
    exit /b 1
)

echo Docker image built successfully!
echo.
echo To run the application:
echo   docker run -p 8080:8080 serviceplanner:latest
echo.
echo To run with persistent database:
echo   docker run -p 8080:8080 -v %cd%/data:/app/data serviceplanner:latest
echo.
echo To run with docker-compose:
echo   docker-compose up -d
echo.
echo Access the application at: http://localhost:8080
echo.
pause
