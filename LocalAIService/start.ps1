$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

$python = Join-Path $scriptDir ".venv\Scripts\python.exe"

if (-not (Test-Path $python)) {
    Write-Host "Virtual environment was not found. Creating .venv..."
    & "python" -m venv ".venv"
}

if (-not (Test-Path $python)) {
    throw "Could not find .venv\Scripts\python.exe. Install Python or recreate the virtual environment."
}

Write-Host "Starting Travelis Local AI Service on http://127.0.0.1:8001"
& $python -m uvicorn app:app --host 127.0.0.1 --port 8001
