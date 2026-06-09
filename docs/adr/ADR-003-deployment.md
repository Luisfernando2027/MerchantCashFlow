# ADR-003: Estratégia de deployment e escalabilidade

Status: Proposta

Decisão:
- Deploy em Kubernetes (AKS/EKS) com Horizontal Pod Autoscaler para escalabilidade de ambos os serviços.
- Utilizar o broker gerenciado (RabbitMQ Cloud) ou cluster em k8s.

Consequências:
- Escala automática baseada em CPU/Requests.
- Observability integrada via Prometheus operator.

