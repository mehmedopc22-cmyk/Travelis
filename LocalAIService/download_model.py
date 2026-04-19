import os
from pathlib import Path

from dotenv import load_dotenv
from huggingface_hub import snapshot_download


def main() -> None:
    load_dotenv()

    model_id = os.getenv("HF_MODEL_ID", "Qwen/Qwen2.5-0.5B-Instruct")
    model_dir = Path(os.getenv("HF_MODEL_DIR", "models/qwen2.5-0.5b-instruct"))
    token = os.getenv("HUGGINGFACE_TOKEN")

    model_dir.parent.mkdir(parents=True, exist_ok=True)

    snapshot_download(
        repo_id=model_id,
        local_dir=model_dir,
        token=token,
        local_dir_use_symlinks=False,
    )

    print(f"Downloaded {model_id} to {model_dir.resolve()}")


if __name__ == "__main__":
    main()
