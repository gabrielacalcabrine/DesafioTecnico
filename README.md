# Desafio Técnico — Plataforma Simplificada de Negociação de Ativos

Bem-vindo(a)! Este repositório é o ponto de partida do desafio. Ele já contém:

- `backend/` — esqueleto de uma Web API .NET 8 (apenas `/health` implementado);
- `frontend/` — esqueleto React 18 + TypeScript + Vite;
- `docker-compose.yml` — sobe API, banco (PostgreSQL) e frontend.

Você pode reorganizar a estrutura interna como preferir (camadas, projetos, pastas),
desde que o **Contrato de API** (abaixo) e o `docker-compose up` continuem funcionando.

---

## Contexto do Problema

Você foi designado para desenvolver um sistema simplificado de negociação de ativos
financeiros. O sistema permitirá que usuários registrem ordens de compra e venda de
ativos e que um mecanismo de matching execute automaticamente operações quando houver
compatibilidade entre preços.

### Exemplo

| Tipo   | Ativo | Quantidade | Preço    |
|--------|-------|------------|----------|
| Compra | PETR4 | 100        | R$ 30,50 |
| Venda  | PETR4 | 50         | R$ 30,00 |

Resultado:

- Executar negócio de **50 ações de PETR4 a R$ 30,00**.
- Permanece aberta: Compra de 50 ações a R$ 30,50.

---

## Entidades principais

### Ordem

- Id
- Tipo (`Compra` | `Venda`)
- Ativo
- Quantidade
- Preco
- QuantidadeExecutada
- Status (`Aberta` | `ParcialmenteExecutada` | `Executada` | `Cancelada`)
- DataHoraCriacao

### Negócio Executado (Trade)

- Id
- OrdemCompraId
- OrdemVendaId
- Ativo
- Quantidade
- PrecoExecucao
- DataHoraExecucao

---

## Regras de negócio

### Motor de Matching

Sempre que uma ordem for criada:

- Ordem de **compra** executa contra a **melhor oferta de venda** disponível (menor preço).
- Ordem de **venda** executa contra a **melhor oferta de compra** disponível (maior preço).

Regras:

- Compra executa se `preço_compra >= preço_venda`.
- Venda executa se `preço_venda <= preço_compra`.
- **Execução parcial deve ser suportada.**
- Matching ocorre apenas entre ordens do **mesmo ativo**.

### Ordenação do livro de ofertas

- **Compras**: maior preço primeiro.
- **Vendas**: menor preço primeiro.

---

## CONTRATO DE API (obrigatório — não altere rotas, portas nem nomes de campos)

A avaliação é feita por uma suíte automatizada que consome exatamente este contrato.
Divergências de rota, porta ou shape de JSON reprovam os testes automatizados.

- A API deve escutar em **`http://localhost:8080`** (via `docker-compose up`).
- O frontend deve escutar em **`http://localhost:3000`**.
- JSON em **camelCase**; enums como **string** (`"Compra"`, `"Venda"`, `"Aberta"`,
  `"ParcialmenteExecutada"`, `"Executada"`, `"Cancelada"`).
- Datas em ISO 8601 (UTC recomendado).
- A API deve habilitar **CORS** para o frontend.

### 1. Criar ordem

```http
POST /orders
Content-Type: application/json

{
  "tipo": "Compra",
  "ativo": "PETR4",
  "quantidade": 100,
  "preco": 30.50
}
```

Resposta: **201 Created**

```json
{
  "id": "b7e6...",
  "tipo": "Compra",
  "ativo": "PETR4",
  "quantidade": 100,
  "preco": 30.50,
  "quantidadeExecutada": 0,
  "status": "Aberta",
  "dataHoraCriacao": "2026-07-15T12:00:00Z"
}
```

> O matching acontece de forma síncrona ou assíncrona (sua escolha), mas os efeitos
> devem estar visíveis nos endpoints de consulta em, no máximo, **1 segundo** após o 201.
> Entrada inválida (quantidade ≤ 0, preço ≤ 0, tipo desconhecido, ativo vazio) deve
> retornar **400** com corpo de erro estruturado.

### 2. Consultar ordem / listar ordens

```http
GET /orders/{id}          → 200 com o shape acima; 404 se não existir
GET /orders               → 200 com lista (filtros opcionais: ?ativo=&status=)
```

### 3. Livro de ofertas

```http
GET /orderbook/{ativo}
```

Resposta: **200**

```json
{
  "ativo": "PETR4",
  "compras": [ { "preco": 30.50, "quantidade": 100 } ],
  "vendas":  [ { "preco": 31.00, "quantidade": 80 } ]
}
```

- Somente quantidade **remanescente** de ordens `Aberta`/`ParcialmenteExecutada`.
- Compras em ordem **decrescente** de preço; vendas em ordem **crescente**.
- Níveis com o mesmo preço podem ser agregados (somar quantidades) ou listados
  individualmente — a suíte aceita ambos.

### 4. Negócios executados

```http
GET /trades
GET /trades?ativo=PETR4
GET /trades?inicio=2026-07-15T00:00:00Z&fim=2026-07-16T00:00:00Z
GET /trades?ordemId={id}
```

Resposta: **200**

```json
[
  {
    "id": "…",
    "ordemCompraId": "…",
    "ordemVendaId": "…",
    "ativo": "PETR4",
    "quantidade": 50,
    "precoExecucao": 30.00,
    "dataHoraExecucao": "2026-07-15T12:00:01Z"
  }
]
```

### 5. Cancelar ordem

```http
POST /orders/{id}/cancel
```

- **200** com a ordem atualizada (`status: "Cancelada"`) se estava `Aberta`;
- **404** se a ordem não existe;
- **409** (ou 422) se a ordem já está `Executada` ou `Cancelada`.
- Cancelar uma ordem `ParcialmenteExecutada` cancela apenas a quantidade remanescente
  (a parte executada permanece registrada nos trades).

### 6. Suporte a testes automatizados (obrigatório)

```http
GET  /health        → 200 quando a API está pronta (banco acessível)
POST /admin/reset   → 200; apaga todas as ordens e trades
```

> `/admin/reset` existe apenas para a suíte de avaliação. Se preferir, proteja-o com a
> variável de ambiente `ENABLE_TEST_ENDPOINTS=true` (o docker-compose de avaliação a define).

---

## Frontend (React)

Telas obrigatórias (layout livre):

1. **Cadastro de ordem** — tipo, ativo, quantidade, preço.
2. **Livro de ofertas** — atualização automática (polling a cada 2s; WebSocket/SignalR é diferencial).
3. **Negócios executados** — horário, ativo, quantidade, preço.
4. **Ordens abertas** — com possibilidade de cancelar.

---

## Requisitos técnicos obrigatórios

- .NET 8+, ASP.NET Core Web API, C#;
- Clean Architecture ou arquitetura em camadas;
- SOLID; Repository Pattern ou equivalente; injeção de dependência;
- Tratamento global de exceções; validação de entrada; logs estruturados;
- PostgreSQL (já no compose) com **Entity Framework Core**;
- `docker-compose up` deve subir API + banco + frontend sem passos manuais.

> **Sobre testes**: você **não precisa** escrever testes automatizados. A avaliação usa
> uma suíte própria que exercita sua API pelo contrato acima. Escrever testes seus conta
> como diferencial, não como requisito.

## Diferenciais

SignalR em tempo real; CQRS; arquitetura orientada a eventos; Redis para cache do book;
OpenTelemetry; manifests Kubernetes; rate limiting; health checks detalhados;
price-time priority (prioridade por tempo entre ordens de mesmo preço); auditoria;
limites de risco por cliente.

## Entrega

- Repositório Git (público ou privado) baseado neste template;
- README atualizado com: instruções, decisões arquiteturais, trade-offs e melhorias futuras.

## Critérios de avaliação

| Critério                     | Peso |
|------------------------------|------|
| Qualidade do código          | 25%  |
| Modelagem do domínio         | 20%  |
| Arquitetura                  | 20%  |
| Testes (suíte automatizada)  | 15%  |
| Performance e escalabilidade | 10%  |
| Frontend                     | 10%  |

## Como executar

```bash
docker compose up --build
# API:      http://localhost:8080/health
# Frontend: http://localhost:3000
```

## CI incluída

O workflow `.github/workflows/ci.yml` roda a cada push: sobe a stack com
`docker compose up` e verifica `GET /health` e o frontend. É o pré-requisito
mínimo da entrega — mantenha-o verde. A avaliação final usa uma suíte
automatizada adicional contra o contrato de API acima.
