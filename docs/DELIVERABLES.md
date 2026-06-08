# Entregáveis e Guia para Entrevista

Este documento contém uma explicação detalhada do projeto e um roteiro para apresentar a solução em uma entrevista.

1. Visão Geral do Projeto
- Objetivo: Fornecer serviço para gerenciar lançamentos financeiros (débitos/créditos) e gerar consolidado diário por comerciante.
- Arquitetura: Microsserviços (LaunchesService e ConsolidatedService) comunicando-se via RabbitMQ. Persistência em PostgreSQL.

2. Componentes Principais
- LaunchesService
  - API REST para criar e consultar lançamentos.
  - Persiste lançamento em Postgres e publica evento LaunchCreated.
- ConsolidatedService
  - Consumidor de eventos.
  - Mantém tabela consolidated por merchant+dia.
- RabbitMQ
  - Fila/Exchange para desacoplar gravação e processamento do consolidado.
- Postgres
  - Armazena lançamentos e consolidado.

3. Fluxo de Dados
- Cliente -> POST /api/launches -> LaunchesService grava no DB e publica LaunchCreated
- RabbitMQ mantém a mensagem na fila
- ConsolidatedService consome LaunchCreated e atualiza tabela consolidated

4. Padrões e decisões
- CQRS (separação de escrita/leitura) com event-driven para garantir disponibilidade do serviço de escrita.
- MassTransit para integração com RabbitMQ pela simplicidade e padrões de retry.
- Docker Compose para facilitar execução local.

5. Requisitos Não-Funcionais
- Escalabilidade: cada serviço pode ser escalado horizontalmente.
- Resiliência: mensagens persistem no broker; enquanto o consumer estiver down, nenhuma perda.
- Segurança: recomenda-se usar TLS, OAuth2 em produção.

6. Como rodar localmente
- Instalar Docker
- Executar: docker compose up --build
- Endpoints:
  - LaunchesService: POST http://localhost:5100/api/launches
  - RabbitMQ Management: http://localhost:15672 (guest/guest)

7. Guia de Testes
- Testar criação de lançamento via curl/Postman
- Verificar tabela consolidated no Postgres
- Simular queda do consolidado e verificar retenção das mensagens

8. Roteiro para Entrevista
- Início (1-2 minutos): Contextualize o problema: o comerciante precisa controlar fluxo de caixa e ter consolidado diário.
- Arquitetura (3-4 minutos): Apresente o diagrama e explique os serviços, broker e DB; explique CQRS e event-driven.
- Decisões técnicas (3-4 minutos): Justifique Postgres, RabbitMQ, MassTransit, Docker. Explique trade-offs (simplicidade vs performance, consistência vs disponibilidade).
- Requisitos não-funcionais (2-3 minutos): Fale sobre escalabilidade, resiliência, segurança e observability.
- Demonstração (4-6 minutos): Mostre um POST criando lançamento e verifique o consolidated. Se possível, mostre RabbitMQ Management com mensagens na fila.
- Evoluções (2-3 minutos): O que faria a seguir: idempotência, deduplicação, particionamento, uso de Kafka para alto throughput, monitoramento, infra como código.
- Perguntas (2-3 minutos): Prepare-se para perguntas sobre consistência eventual, perda de mensagens, garantia de entrega e latência.

9. Pontos para aprofundar se perguntado
- Políticas de retry, Dead Letter Queue, idempotência, esquema de versionamento de eventos, GDPR e criptografia.

Boa apresentação!
