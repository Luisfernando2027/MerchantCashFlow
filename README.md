# MerchantCashFlow - Desafio Arquiteto de Soluções

Repositório com scaffold mínimo para o desafio: dois serviços (.NET 8) usando PostgreSQL e RabbitMQ.

Componentes:
- LaunchesService: API para criar lançamentos (persistência em Postgres e publicação de evento no RabbitMQ).
- ConsolidatedService: consumidor de eventos que atualiza o consolidado diário (em Postgres).
- docker-compose.yml para rodar Postgres, RabbitMQ e os serviços em modo local.

Como rodar localmente
1. Instale Docker e Docker Compose.
2. Na raiz do repositório, execute:
   docker compose up --build
3. A API de lançamentos estará em http://localhost:5100 (POST /api/launches)
4. O serviço de consolidado roda como background consumer e atualiza a tabela consolidated.

Testes
- Unit tests não foram adicionados por limitação de tempo; recomenda-se adicionar xUnit e Testcontainers para integração.

Entrega
- Gere um repositório público no GitHub e suba todo o conteúdo deste diretório (git add . && git commit -m "scaffold" && git push).
- Inclua no README o link do repo para avaliação.

Observações
- Arquitetura e decisões documentadas em docs/architecture.md.
- Diagrama simples em docs/diagram.svg.
