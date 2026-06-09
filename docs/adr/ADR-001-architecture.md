# ADR-001: Escolha da arquitetura orientada a eventos com RabbitMQ

Status: Aceita

Contexto:
- Requisito: serviço de lançamentos deve continuar disponível mesmo quando o consolidado estiver indisponível.
- Necessidade de desacoplamento entre escrita e processamento de agregações.

Decisão:
- Adotar arquitetura event-driven usando RabbitMQ como message broker e MassTransit como biblioteca de integração.

Consequências:
- Desacoplamento: produtores não dependem do consumidor estar disponível.
- Resiliência: mensagens são enfileiradas até o consumidor processá-las.
- Escalabilidade: cada serviço pode escalar independentemente.

Alternativas consideradas:
- REST síncrono direto entre serviços (rejeitado por acoplamento e latência).
- Banco compartilhado (rejeitado por coupling e coordenação entre times).

