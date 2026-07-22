# TODO — Infrastructure

- [x] Substituir os repositórios em memória por Entity Framework Core.
- [x] Criar `TradingDbContext`.
- [x] Criar configurações EF para `Order` e `Trade`.
- [ ] Criar migrations do PostgreSQL.
- [x] Configurar índices para ativo, status, preço e data de criação.
- [x] Garantir transação única para matching e gravação de trades.
- [x] Configurar retry/resiliência para conexão com PostgreSQL.
- [x] Implementar o reset usando o banco de dados.
- [x] Adicionar logs das operações de matching.
