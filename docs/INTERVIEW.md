# Guia para apresentação na entrevista

Tempo estimado: 12-15 minutos

1) Abertura (1 min)
- Cumprimente e apresente rapidamente: objetivo do projeto é controlar lançamentos financeiros e gerar consolidado diário por comerciante, garantindo disponibilidade e escalabilidade.

2) Problema e requisitos (1-2 min)
- Explique requisitos funcionais: criar/consultar lançamentos, gerar consolidado diário.
- Requisitos não-funcionais: alta disponibilidade do serviço de gravação, consolidado suporta 50 req/s com perda <=5%.

3) Arquitetura e componentes (3-4 min)
- Mostre o diagrama (docs/diagram.svg).
- Explique cada componente: LaunchesService (escrita), RabbitMQ (broker), ConsolidatedService (consumo e agregação), PostgreSQL (persistência).
- Explique o padrão: CQRS + event-driven; por que separação entre writes e reads melhora disponibilidade e escala.

4) Fluxo de Request (2-3 min)
- Demonstre o fluxo: cliente -> LaunchesService (persistência + publica evento) -> RabbitMQ -> ConsolidatedService consome e atualiza consolidado.
- Explique garantia de entrega: mensagens duráveis no RabbitMQ; adicionaria idempotência e DLQ em produção.

5) Decisões Técnicas e Trade-offs (2-3 min)
- Por que PostgreSQL: consistência relacional e consultas analíticas simples.
- Por que RabbitMQ: fácil de operar, suporte a delivery guarantees; se precisar de throughput maior, considerar Kafka.
- MassTransit: simplifica integração, padrões de retry, middlewares.
- Trade-offs: consistência eventual no consolidado vs disponibilidade do endpoint de gravação.

6) Não-funcionais e Operação (1-2 min)
- Escalabilidade: escalar pods/containers, particionamento por merchant se necessário.
- Observability: logs estruturados, métricas (Prometheus) e tracing (OpenTelemetry).
- Segurança: TLS, OAuth2 e controle de acesso por scopes.

7) Evoluções e próximos passos (1-2 min)
- Deduplicação de eventos, event versioning, testes de contrato, monitoramento de SLOs, infra como código (Terraform/K8s).

8) Demonstração prática (5 min)
- Execute um POST em /api/launches e mostre o registro no DB e atualização no consolidated.
- Mostre RabbitMQ Management (mensagens/filas) se necessário.

9) Perguntas possíveis e respostas curtas
- "E se o consolidated cair?" -> Mensagens ficam na fila; consumer processará quando voltar; garantir DLQ e monitoramento.
- "Como garantir idempotência?" -> Persistir eventId e checar antes de aplicar, ou usar operações de upsert com controle otimista.
- "Por que não Kafka?" -> Kafka para alto throughput e retenção longa; optei por RabbitMQ pela simplicidade e pelas garantias necessárias no escopo.

Dica: controle o tempo, foque nas decisões e trade-offs, mostre entendimento sobre disponibilidade, consistência e observability.
