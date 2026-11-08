#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
WIN_ROOT="$(wslpath -w "$ROOT")"
DOTNET="/mnt/c/Program Files/dotnet/dotnet.exe"

"$DOTNET" build "$WIN_ROOT\\RefugioAnimales.sln"
