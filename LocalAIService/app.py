import os
import time
import uuid
from pathlib import Path
from typing import Any, Literal

import torch
from dotenv import load_dotenv
from fastapi import FastAPI, HTTPException
from huggingface_hub import snapshot_download
from pydantic import BaseModel, Field
from transformers import AutoModelForCausalLM, AutoTokenizer


load_dotenv()

APP_MODEL_NAME = "travelis-local"
HF_MODEL_ID = os.getenv("HF_MODEL_ID", "Qwen/Qwen2.5-0.5B-Instruct")
HF_MODEL_DIR = Path(os.getenv("HF_MODEL_DIR", "models/qwen2.5-0.5b-instruct"))
HF_TOKEN = os.getenv("HUGGINGFACE_TOKEN")
MAX_NEW_TOKENS = int(os.getenv("MAX_NEW_TOKENS", "384"))
DEFAULT_TEMPERATURE = float(os.getenv("TEMPERATURE", "0.1"))
DEVICE_SETTING = os.getenv("DEVICE", "auto").lower()

app = FastAPI(title="Travelis Local AI Service")

_tokenizer: AutoTokenizer | None = None
_model: AutoModelForCausalLM | None = None


class ChatMessage(BaseModel):
    role: Literal["system", "user", "assistant"]
    content: str


class ChatCompletionRequest(BaseModel):
    model: str = APP_MODEL_NAME
    messages: list[ChatMessage]
    temperature: float | None = None
    max_tokens: int | None = Field(default=None, alias="max_tokens")
    stream: bool = False


def ensure_model_downloaded() -> Path:
    if not HF_MODEL_DIR.exists() or not any(HF_MODEL_DIR.iterdir()):
        HF_MODEL_DIR.parent.mkdir(parents=True, exist_ok=True)
        snapshot_download(
            repo_id=HF_MODEL_ID,
            local_dir=HF_MODEL_DIR,
            token=HF_TOKEN,
            local_dir_use_symlinks=False,
        )

    return HF_MODEL_DIR


def get_device() -> str:
    if DEVICE_SETTING != "auto":
        return DEVICE_SETTING

    return "cuda" if torch.cuda.is_available() else "cpu"


def load_model() -> tuple[AutoTokenizer, AutoModelForCausalLM]:
    global _tokenizer, _model

    if _tokenizer is not None and _model is not None:
        return _tokenizer, _model

    model_path = ensure_model_downloaded()
    device = get_device()

    _tokenizer = AutoTokenizer.from_pretrained(model_path, token=HF_TOKEN)
    _model = AutoModelForCausalLM.from_pretrained(
        model_path,
        token=HF_TOKEN,
        dtype=torch.float16 if device == "cuda" else torch.float32,
        low_cpu_mem_usage=True,
    )

    _model.to(device)
    _model.eval()

    return _tokenizer, _model


def render_messages(tokenizer: AutoTokenizer, messages: list[ChatMessage]) -> str:
    message_dicts = [message.model_dump() for message in messages]

    if hasattr(tokenizer, "apply_chat_template") and tokenizer.chat_template:
        return tokenizer.apply_chat_template(
            message_dicts,
            tokenize=False,
            add_generation_prompt=True,
        )

    return "\n".join(f"{message.role}: {message.content}" for message in messages) + "\nassistant:"


def generate_text(request: ChatCompletionRequest) -> str:
    if request.stream:
        raise HTTPException(status_code=400, detail="Streaming is not supported by this local service.")

    tokenizer, model = load_model()
    prompt = render_messages(tokenizer, request.messages)
    device = next(model.parameters()).device

    inputs = tokenizer(prompt, return_tensors="pt").to(device)
    input_token_count = inputs["input_ids"].shape[-1]
    temperature = request.temperature if request.temperature is not None else DEFAULT_TEMPERATURE
    max_new_tokens = request.max_tokens or MAX_NEW_TOKENS

    generation_kwargs = {
        "max_new_tokens": max_new_tokens,
        "do_sample": temperature > 0,
        "pad_token_id": tokenizer.eos_token_id,
    }

    if temperature > 0:
        generation_kwargs["temperature"] = temperature

    with torch.no_grad():
        output_ids = model.generate(**inputs, **generation_kwargs)

    generated_ids = output_ids[0][input_token_count:]
    return tokenizer.decode(generated_ids, skip_special_tokens=True).strip()


@app.get("/health")
def health() -> dict[str, Any]:
    return {
        "status": "ok",
        "model_id": HF_MODEL_ID,
        "model_dir": str(HF_MODEL_DIR),
        "model_loaded": _model is not None,
    }


@app.post("/v1/chat/completions")
def chat_completions(request: ChatCompletionRequest) -> dict[str, Any]:
    content = generate_text(request)
    now = int(time.time())

    return {
        "id": f"chatcmpl-{uuid.uuid4().hex}",
        "object": "chat.completion",
        "created": now,
        "model": request.model,
        "choices": [
            {
                "index": 0,
                "message": {
                    "role": "assistant",
                    "content": content,
                },
                "finish_reason": "stop",
            }
        ],
    }


if __name__ == "__main__":
    import uvicorn

    host = os.getenv("HOST", "127.0.0.1")
    port = int(os.getenv("PORT", "8001"))
    uvicorn.run("app:app", host=host, port=port, reload=False)
