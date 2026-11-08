#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
WIN_PROJECT="$(wslpath -w "$ROOT/src/Refugio.App/Refugio.App.csproj")"
DOTNET="/mnt/c/Program Files/dotnet/dotnet.exe"

"$DOTNET" run --project "$WIN_PROJECT"
