from __future__ import annotations

import json
import logging
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from typing import Any

from bs4 import BeautifulSoup, NavigableString
from deep_translator import GoogleTranslator


HOST = "127.0.0.1"
PORT = 5000
LOG_PATH = Path(__file__).with_name("local_translate_server.log")


logging.basicConfig(
    filename=LOG_PATH,
    level=logging.INFO,
    format="%(message)s",
    encoding="utf-8",
)

_TRANSLATOR_CACHE: dict[tuple[str, str], GoogleTranslator] = {}


def log_line(message: str) -> None:
    logging.info(message)


def get_translator(source: str, target: str) -> GoogleTranslator:
    key = (source, target)
    translator = _TRANSLATOR_CACHE.get(key)
    if translator is None:
        translator = GoogleTranslator(source=source, target=target)
        _TRANSLATOR_CACHE[key] = translator
    return translator


def translate_text(text: str, source: str, target: str) -> str:
    text = (text or "").strip()
    if not text:
        return ""

    translator = get_translator(source, target)
    translated = translator.translate(text)
    return translated or text


def translate_html(html: str, source: str, target: str) -> str:
    soup = BeautifulSoup(html, "html.parser")

    for node in list(soup.descendants):
        if not isinstance(node, NavigableString):
            continue

        parent = getattr(node, "parent", None)
        if parent is None or getattr(parent, "name", "") in {"script", "style"}:
            continue

        original = str(node)
        if not original or not original.strip():
            continue

        translated = translate_text(original, source, target)
        if translated and translated != original:
            node.replace_with(translated)

    return str(soup)


class TranslateHandler(BaseHTTPRequestHandler):
    server_version = "LocalTranslateServer/1.0"

    def do_POST(self) -> None:
        if self.path != "/translate":
            self.send_error(404, "Not Found")
            return

        try:
            length = int(self.headers.get("Content-Length", "0"))
            raw = self.rfile.read(length).decode("utf-8")
            payload = json.loads(raw or "{}")
        except Exception as ex:
            self.write_json(400, {"error": f"invalid-json: {ex}"})
            return

        source = str(payload.get("source") or "auto").strip() or "auto"
        target = str(payload.get("target") or "").strip().lower()
        text = str(payload.get("q") or "")
        fmt = str(payload.get("format") or "text").strip().lower()

        log_line(f"request source={source} target={target} text={text[:160]!r}")

        if not text.strip():
            log_line("response empty-source")
            self.write_json(200, {"translatedText": text})
            return

        if not target:
            log_line("response missing-target")
            self.write_json(200, {"translatedText": text})
            return

        if source.lower() == target.lower():
            log_line("response same-language")
            self.write_json(200, {"translatedText": text})
            return

        try:
            if fmt == "html":
                translated = translate_html(text, source, target)
            else:
                translated = translate_text(text, source, target)
        except Exception as ex:
            log_line(f"response error={type(ex).__name__}:{ex}")
            self.write_json(500, {"error": str(ex), "translatedText": text})
            return

        log_line(f"response translated={translated[:160]!r}")
        self.write_json(200, {"translatedText": translated})

    def do_GET(self) -> None:
        if self.path in {"/", "/health", "/translate"}:
            self.write_json(200, {"status": "ok", "service": "local-translate", "host": HOST, "port": PORT})
            return

        self.send_error(404, "Not Found")

    def log_message(self, format: str, *args: Any) -> None:
        return

    def write_json(self, status: int, payload: dict[str, Any]) -> None:
        body = json.dumps(payload, ensure_ascii=False).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        self.send_header("Access-Control-Allow-Origin", "*")
        self.end_headers()
        self.wfile.write(body)


def main() -> None:
    server = ThreadingHTTPServer((HOST, PORT), TranslateHandler)
    print(f"Local translate server listening on http://{HOST}:{PORT}")
    server.serve_forever()


if __name__ == "__main__":
    main()
