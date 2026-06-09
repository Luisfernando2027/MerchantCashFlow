# MerchantCashFlow - Solution Architecture Challenge

Visão geral
------------
MerchantCashFlow é uma solução demonstrativa para o desafio de Arquiteto de Soluções.
Implementa um fluxo de lançamentos financeiros (LaunchesService) e um consumidor que gera consolidações diárias (ConsolidatedService), usando RabbitMQ e PostgreSQL.

Arquitetura (resumo)
--------------------
- LaunchesService: API REST responsável por receber lançamentos e publicar eventos.
- ConsolidatedService: consumidor que processa eventos e atualiza as tabelas consolidadas.
- Shared: contratos compartilhados entre serviços para evitar conflitos de tipos.
- Infra: PostgreSQL (dados), RabbitMQ (mensageria), Prometheus (métricas), Zipkin (traces).

Requisitos atendidos
--------------------
- Serviços desacoplados, processamento assíncrono via RabbitMQ.
- Idempotência no consumidor (ProcessedEvents).
- Migrations EF Core para ambas as bases.
- Observabilidade: /health, /metrics (Prometheus), logs Serilog e traces (Zipkin).
- Docker + Docker Compose para reproduzir o ambiente local.
- CI: GitHub Actions (build + test).

Como executar (rápido)
---------------------
Pré-requisitos: Docker Desktop e dotnet SDK 8+

1) Na raiz do repositório:

   docker compose up --build

2) Endpoints úteis:

   - API Lançamentos (POST): http://localhost:5100/api/launches
   - Health: http://localhost:5100/health
   - Metrics (Prometheus): http://localhost:5100/metrics
   - Zipkin UI (traces): http://localhost:9411

Exemplo (PowerShell):

   $body = '{ "merchantId":"00000000-0000-0000-0000-000000000001", "amount":100.5, "currency":"BRL", "occurredAt":"2026-06-01T12:00:00Z" }'
   Invoke-RestMethod -Method Post -Uri http://localhost:5100/api/launches -ContentType 'application/json' -Body $body

Observabilidade
---------------
- Prometheus metrics disponíveis em /metrics.
- Health checks em /health.
- Traces enviados para Zipkin (http://localhost:9411).
- Logs estruturados com Serilog; opção de Seq configurável via Logging:SeqUrl.

Testes
------
- Unit tests: dotnet test MerchantCashFlow.sln
- Teste de carga: tools/k6/loadtest.js (ex.: target 50 req/s)

Decisões arquiteturais (resumo)
------------------------------
- Mensageria (RabbitMQ) para desacoplamento e resiliência.
- Serviços separados para permitir escalabilidade independente e tolerância a falhas.

Documentação adicional
----------------------
- docs/architecture.md: diagrama e descrição geral
- docs/adr/ADR-001-architecture.md: decisão arquitetural principal

Entrega e evidências
--------------------
- Release v1.0.0 contém um evidence.zip com logs e captura de execução (se necessário).

Segurança e recomendações
------------------------
- Não commitar segredos em repositório. Use GitHub Secrets / Key Vault.
- Revisar dependências com alertas de segurança (ex.: OpenTelemetry.Exporter.Zipkin) e planejar atualização.

Estimativa de custos (exemplo)
-----------------------------
Estimativas mensais aproximadas para uma implantação em nuvem (valores exemplares):

Serviço/Componente    | Configuração típica        | Estimativa/mês (USD)
--------------------- | ------------------------- | -------------------
PostgreSQL (managed)  | db.t3.medium (single AZ)  | 40 - 120
RabbitMQ (managed)    | small cluster              | 20 - 80
App Service / Container| 2 x small instances       | 30 - 100
Storage / Logs        | 10GB de logs / month      | 5 - 20
Monitoring (Prometheus/Grafana managed) | basic plan | 10 - 50
Zipkin / Traces       | OTLP collector (small)    | 5 - 25

Total estimado aproximado: 110 - 395 USD / mês

Observações:
- Valores dependem de provedores (AWS/Azure/GCP) e de opções gerenciadas vs self-hosted.
- Em ambiente de produção, considerar HA (multi-AZ) e backups, o que aumenta custo.

Próximos passos sugeridos
------------------------
1. Adicionar ADRs adicionais (deploy, retries, DLQ).
2. Adicionar diagrama C4 em docs/architecture.md (draw.io ou ASCII).
3. Automatizar E2E e carga no CI (opcional).

Contato
-------
Repo: https://github.com/Luisfernando2027/MerchantCashFlow
