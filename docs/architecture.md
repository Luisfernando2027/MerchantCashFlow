# Arquitetura - MerchantCashFlow

Resumo: arquitetura baseada em microsserviços com padrões CQRS e event-driven.

Componentes
- LaunchesService: API REST para operações de escrita (criar lançamentos). Persiste em PostgreSQL. Publica evento LaunchCreated no RabbitMQ.
- ConsolidatedService: consumidor que processa LaunchCreated e atualiza a tabela consolidated por merchant+dia.

Decisões principais
- PostgreSQL para persistência relacional e consistência.
- RabbitMQ como message broker para integração assíncrona.
- MassTransit para simplificar integração com RabbitMQ.
- Docker Compose para facilitar execução local.

Escalabilidade e Resiliência
- Isolar serviços permite escalar tanto LaunchesService quanto ConsolidatedService.
- Failover: enquanto Consolidated estiver indisponível, mensagens ficam na fila do RabbitMQ.
- Idempotência: o consumidor atualiza balance com operações aditivas; em produção adicionar deduplicação por eventId.

Observability
- Recomendado integrar OpenTelemetry, Prometheus e Grafana.

Segurança
- Em produção usar TLS, autenticação OAuth2 e políticas de autorização.
