# Travelis Local AI Service

Local Hugging Face powered AI service for Travelis.

It exposes an OpenAI-compatible endpoint:

```text
POST http://127.0.0.1:8001/v1/chat/completions
```

The ASP.NET API can call this endpoint through:

```json
"AI": {
  "LocalModel": {
    "Endpoint": "http://127.0.0.1:8001/v1/chat/completions",
    "Model": "travelis-local",
    "ApiKey": ""
  }
}
```

## Setup

From `LocalAIService`:

```powershell
.\.venv\Scripts\python.exe -m pip install --upgrade pip
.\.venv\Scripts\python.exe -m pip install -r requirements.txt
.\.venv\Scripts\python.exe download_model.py
.\.venv\Scripts\python.exe -m uvicorn app:app --host 127.0.0.1 --port 8001
```

Or start it with one command:

```powershell
.\start.ps1
```

You can also double-click or run:

```cmd
start.bat
```

The model is stored locally under `models/` and loaded from there by the API service.

## Endpoints

- `GET /health`
- `POST /v1/chat/completions`

The chat completions endpoint accepts the OpenAI-style `model`, `messages`, `temperature`, `max_tokens`, and `stream` fields. Streaming is not implemented; send `stream: false`.
