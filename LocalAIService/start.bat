@echo off
setlocal

cd /d "%~dp0"

if not exist ".venv\Scripts\python.exe" (
    echo Virtual environment was not found. Creating .venv...
    python -m venv .venv
)

if not exist ".venv\Scripts\python.exe" (
    echo Could not find .venv\Scripts\python.exe. Install Python or recreate the virtual environment.
    exit /b 1
)

echo Starting Travelis Local AI Service on http://127.0.0.1:8001
".venv\Scripts\python.exe" -m uvicorn app:app --host 127.0.0.1 --port 8001
