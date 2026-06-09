# ADR-002: Retries e DLQ para consumidores

Status: Aceita

Contexto:
- Consumidor pode falhar por dados inválidos ou problemas temporários de infra.

Decisão:
- Utilizar políticas de retry exponenciais no consumidor e configurar uma fila de DLQ (Dead Letter Queue) para mensagens que excederem tentativas.

Consequências:
- Permite recuperação automática de falhas transitórias.
- Preserva mensagens problemáticas para análise operacional.

Alternativas rejeitadas:
- Rejeitar mensagens imediatamente.

