#!/usr/bin/env bash
set -euo pipefail

service mysql status >/dev/null 2>&1 && echo "MySQL ya está en ejecución." && exit 0

echo "Iniciando MySQL (puede solicitar la contraseña de sudo)..."
sudo service mysql start
echo "MySQL iniciado."
