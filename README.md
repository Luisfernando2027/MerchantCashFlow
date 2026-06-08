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

Health and CI
- Health endpoint (LaunchesService): GET /health
- CI: GitHub Actions workflow (.github/workflows/dotnet.yml) roda build/test em pushes

Como aplicar migrations (PowerShell)
Execute cada linha separadamente na raiz do repositório:

Set-Location C:\Users\luis.oliveira\source\repos\MerchantCashFlow
dotnet tool install --global dotnet-ef
dotnet restore MerchantCashFlow.sln
dotnet ef migrations add Initial_Launches -p src/LaunchesService/LaunchesService.csproj -s src/LaunchesService/LaunchesService.csproj --context LaunchesDbContext
dotnet ef migrations add Initial_Consolidated -p src/ConsolidatedService/ConsolidatedService.csproj -s src/ConsolidatedService/ConsolidatedService.csproj --context ConsolidatedDbContext

Como aplicar as migrations no banco (PowerShell)
dotnet ef database update -p src/LaunchesService/LaunchesService.csproj -s src/LaunchesService/LaunchesService.csproj --context LaunchesDbContext
dotnet ef database update -p src/ConsolidatedService/ConsolidatedService.csproj -s src/ConsolidatedService/ConsolidatedService.csproj --context ConsolidatedDbContext

Como rodar localmente (Docker)
Execute cada linha separadamente na raiz do repositório:
docker compose build --no-cache
docker compose up -d
Verificar health: curl.exe -v http://localhost:5100/health
Criar lançamento de teste (PowerShell):
$body = '{ "merchantId":"00000000-0000-0000-0000-000000000001", "amount":100.5, "currency":"BRL", "occurredAt":"2026-06-01T12:00:00Z" }'
Invoke-RestMethod -Method Post -Uri http://localhost:5100/api/launches -ContentType 'application/json' -Body $body

Evidências e release
- evidence.zip contém docker_logs.txt e response.json com teste E2E.
- Para atualizar a release (opcional): use gh CLI ou faça upload manual em GitHub Releases.

Checklist final antes do envio
- Confirmar README e docs atualizados
- Gerar e commitar migrations (se necessário)
- Executar docker compose e validar E2E
- Empacotar evidências e anexar à release

Contato
- Repo público: https://github.com/Luisfernando2027/MerchantCashFlow
