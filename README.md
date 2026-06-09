# MerchantCashFlow - Solution Architecture Challenge

## Visão Geral

MerchantCashFlow é uma solução demonstrativa para o desafio de Arquiteto de Soluções.
Implementa um fluxo de lançamentos financeiros (LaunchesService) e um consumidor que gera consolidações diárias (ConsolidatedService), usando RabbitMQ e PostgreSQL.

## Arquitetura de Alto Nível

A solução utiliza uma arquitetura orientada a eventos para garantir desacoplamento entre serviços, resiliência e escalabilidade.

```text
Cliente
   |
   v
LaunchesService
   |
RabbitMQ
   |
   v
ConsolidatedService
   |
PostgreSQL

Observabilidade:
Prometheus
Zipkin
Serilog
```

## Fluxo

O cliente envia um lançamento financeiro para o LaunchesService.
O LaunchesService persiste os dados e publica um evento no RabbitMQ.
O ConsolidatedService consome o evento de forma assíncrona.
O ConsolidatedService atualiza os saldos consolidados no PostgreSQL.
Métricas, logs e traces são disponibilizados através de Prometheus, Serilog e Zipkin.

## Benefícios da Arquitetura

- Desacoplamento entre serviços
- Escalabilidade independente
- Resiliência a falhas
- Observabilidade completa
- Facilidade de evolução da solução

## Componentes Principais

- **LaunchesService:** API REST responsável por receber lançamentos e publicar eventos.
- **ConsolidatedService:** Consumidor responsável por processar eventos e atualizar os saldos consolidados.
- **Shared:** Contratos compartilhados entre serviços para evitar divergências de integração.
- **Infraestrutura:** PostgreSQL (persistência), RabbitMQ (mensageria), Prometheus (métricas), Zipkin (tracing) e Serilog (logging).

## Resiliência e Desacoplamento

A solução foi projetada para garantir que o LaunchesService continue operacional mesmo em cenários de indisponibilidade do ConsolidatedService.

Para isso foi adotada uma arquitetura orientada a eventos utilizando RabbitMQ, permitindo processamento assíncrono e desacoplamento entre serviços.

**Objetivo principal:** garantir que o serviço de lançamentos permaneça disponível mesmo quando o serviço de consolidação estiver indisponível.

## Requisitos atendidos

- Serviços desacoplados, processamento assíncrono via RabbitMQ.
- Idempotência no consumidor (ProcessedEvents).
- Migrations EF Core para ambas as bases.
- Observabilidade: /health, /metrics (Prometheus), logs Serilog e traces (Zipkin).
- Docker + Docker Compose para reproduzir o ambiente local.
- CI: GitHub Actions (build + test).

## Como executar (rápido)

Pré-requisitos: Docker Desktop e dotnet SDK 8+

1) Na raiz do repositório:

   docker compose up --build

2) Endpoints úteis:

- API Lançamentos (POST): http://localhost:5100/api/launches
- Health: http://localhost:5100/health
- Metrics: http://localhost:5100/metrics
- Zipkin UI: http://localhost:9411

Exemplo (PowerShell):

   ```powershell
$body = '{ "merchantId":"00000000-0000-0000-0000-000000000001", "amount":100.5, "currency":"BRL", "occurredAt":"2026-06-01T12:00:00Z" }'

Invoke-RestMethod `
  -Method Post `
  -Uri http://localhost:5100/api/launches `
  -ContentType 'application/json' `
  -Body $body
```

## Observabilidade

- Prometheus metrics disponíveis em /metrics.
- Health checks em /health.
- Traces enviados para Zipkin (http://localhost:9411).
- Logs estruturados com Serilog; opção de Seq configurável via Logging:SeqUrl.

## Testes

- Unit tests: dotnet test MerchantCashFlow.sln
- Teste de carga: tools/k6/loadtest.js (ex.: target 50 req/s)

## Decisões arquiteturais (resumo)

- Mensageria (RabbitMQ) para desacoplamento e resiliência.
- Serviços separados para permitir escalabilidade independente e tolerância a falhas.

## Documentação adicional (ADRs)

- ADR-001 – Event Driven Architecture
- ADR-002 – Retry Strategy and Dead Letter Queue
- ADR-003 – Deployment and Infrastructure Strategy

## Entrega e evidências

- Release v1.0.0 contém um evidence.zip com logs e captura de execução (se necessário).

## Segurança e recomendações

- Não commitar segredos em repositório. Use GitHub Secrets / Key Vault.
- Revisar dependências com alertas de segurança (ex.: OpenTelemetry.Exporter.Zipkin) e planejar atualização.

## Estimativa de custos (exemplo)

Estimativas mensais aproximadas para uma implantação em nuvem (valores exemplares):

| Serviço/Componente | Configuração típica | Estimativa/mês (USD) |
|-------------------|---------------------|----------------------|
| PostgreSQL (managed) | db.t3.medium (single AZ) | 40 - 120 |
| RabbitMQ (managed) | small cluster | 20 - 80 |
| App Service / Container | 2 x small instances | 30 - 100 |
| Storage / Logs | 10GB de logs / mês | 5 - 20 |
| Monitoring (Prometheus/Grafana managed) | basic plan | 10 - 50 |
| Zipkin / Traces | OTLP collector (small) | 5 - 25 |

Total estimado aproximado: 110 - 395 USD / mês

Observações:
- Valores dependem de provedores (AWS/Azure/GCP) e de opções gerenciadas vs self-hosted.
- Em ambiente de produção, considerar HA (multi-AZ) e backups, o que aumenta custo.

## Próximos passos sugeridos

1. Adicionar ADRs adicionais (deploy, retries, DLQ).
2. Adicionar diagrama C4 em docs/architecture.md (draw.io ou ASCII).
3. Automatizar E2E e carga no CI (opcional).

## Contato

Repo: https://github.com/Luisfernando2027/MerchantCashFlow
