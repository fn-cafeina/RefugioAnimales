#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "Se solicitará la contraseña de root de MySQL."
cat "$ROOT/database/01_schema.sql" \
    "$ROOT/database/02_triggers.sql" \
    "$ROOT/database/03_procedures_views.sql" \
    "$ROOT/database/04_seed.sql" \
    "$ROOT/database/05_roles.sql" | mysql -u root -p

echo "Base de datos 'refugio' creada y poblada."
