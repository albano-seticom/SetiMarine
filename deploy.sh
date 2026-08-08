#!/bin/bash
set -e
echo "=== SetiMarine Deploy - $(date) ==="
cd /opt/apps/SetiMarine
git pull origin main
docker compose build --no-cache setimarine_blazor
docker compose up -d setimarine_blazor
echo "=== Deploy concluído ==="
