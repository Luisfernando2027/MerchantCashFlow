-- Inicializa as tabelas mínimas para o projeto
CREATE TABLE IF NOT EXISTS "Launches" (
  "Id" uuid PRIMARY KEY,
  "MerchantId" uuid NOT NULL,
  "Amount" numeric NOT NULL,
  "Currency" varchar(10) NOT NULL,
  "OccurredAt" timestamp without time zone NOT NULL,
  "CreatedAt" timestamp without time zone NOT NULL
);

CREATE TABLE IF NOT EXISTS "Consolidateds" (
  "Id" uuid PRIMARY KEY,
  "MerchantId" uuid NOT NULL,
  "Date" date NOT NULL,
  "Balance" numeric NOT NULL
);
