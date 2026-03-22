import json
from http.server import BaseHTTPRequestHandler, HTTPServer
from threading import Lock
from urllib.parse import urlparse
from pathlib import Path

from deep_translator import GoogleTranslator


SUPPORTED_LANGUAGES = [
    {"code": "tr", "name": "Turkish"},
    {"code": "en", "name": "English"},
    {"code": "fr", "name": "French"},
    {"code": "es", "name": "Spanish"},
]

TRANSLATION_CACHE = {}
TRANSLATION_LOCK = Lock()
LOG_PATH = Path(__file__).with_name("local_translate_server.log")


def log(message):
    try:
        with LOG_PATH.open("a", encoding="utf-8") as handle:
            handle.write(f"{message}\n")
    except Exception:
        pass


class TranslateHandler(BaseHTTPRequestHandler):
    server_version = "LocalTranslate/1.0"

    def do_GET(self):
        parsed = urlparse(self.path)
        if parsed.path == "/languages":
            self._write_json(200, SUPPORTED_LANGUAGES)
            return

        if parsed.path == "/health":
            self._write_json(200, {"status": "ok"})
            return

        self._write_json(404, {"error": "Not found"})

    def do_POST(self):
        parsed = urlparse(self.path)
        if parsed.path != "/translate":
            self._write_json(404, {"error": "Not found"})
            return

        try:
            content_length = int(self.headers.get("Content-Length", "0"))
            body = self.rfile.read(content_length).decode("utf-8") if content_length > 0 else "{}"
            payload = json.loads(body)
        except Exception:
            self._write_json(400, {"error": "Invalid JSON body"})
            return

        text = (payload.get("q") or "").strip()
        source = (payload.get("source") or "tr").strip().lower()
        target = (payload.get("target") or "").strip().lower()
        log(f"request source={source} target={target} text={text[:120]!r}")

        if not text:
            log("response empty-source")
            self._write_json(200, {"translatedText": ""})
            return

        if not target:
            log("response missing-target")
            self._write_json(400, {"error": "Missing target language"})
            return

        if source == target:
            log("response source-equals-target")
            self._write_json(200, {"translatedText": text})
            return

        cache_key = (text, source, target)
        cached_translation = TRANSLATION_CACHE.get(cache_key)
        if cached_translation:
            log(f"response cache-hit translated={cached_translation[:120]!r}")
            self._write_json(200, {"translatedText": cached_translation})
            return

        try:
            with TRANSLATION_LOCK:
                translated = GoogleTranslator(source=source, target=target).translate(text)
        except Exception as exc:
            log(f"response exception error={exc!r}")
            self._write_json(502, {"error": str(exc)})
            return

        translated = (translated or "").strip()
        if not translated:
            log("response empty-translation")
            self._write_json(502, {"error": "Empty translation response"})
            return

        TRANSLATION_CACHE[cache_key] = translated
        log(f"response translated={translated[:120]!r}")
        self._write_json(200, {"translatedText": translated})

    def log_message(self, format, *args):
        return

    def _write_json(self, status_code, payload):
        body = json.dumps(payload, ensure_ascii=False).encode("utf-8")
        self.send_response(status_code)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)


def main():
    server = HTTPServer(("127.0.0.1", 5000), TranslateHandler)
    print("Local translate server listening on http://127.0.0.1:5000", flush=True)
    server.serve_forever()


if __name__ == "__main__":
    main()
