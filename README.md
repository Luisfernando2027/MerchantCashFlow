# MerchantCashFlow

Projeto demonstrativo para o desafio de Arquiteto de Soluções.

Run local (recomendado: Docker)
1. Instale Docker Desktop.
2. Na raiz do projeto execute:
   docker compose up --build
3. API de lançamentos:
   POST http://localhost:5100/api/launches

Exemplo curl:

curl -X POST http://localhost:5100/api/launches -H "Content-Type: application/json" -d '{\"merchantId\":\"00000000-0000-0000-0000-000000000001\",\"amount\":100.5,\"currency\":\"BRL\",\"occurredAt\":\"2026-06-01T12:00:00Z\"}'

Docs importantes:
- docs/architecture.md -> decisões arquiteturais
- tools/postman/Launches.postman_collection.json -> coleção Postman
- tools/k6/loadtest.js -> script k6 para carga

Nota: arquivos internos de entrevista foram removidos do repo público.
