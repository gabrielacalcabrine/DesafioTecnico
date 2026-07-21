# TODO — Infrastructure

- [ ] Substituir os repositórios em memória por Entity Framework Core.
- [ ] Criar `TradingDbContext`.
- [ ] Criar configurações EF para `Order` e `Trade`.
- [ ] Criar migrations do PostgreSQL.
- [ ] Configurar índices para ativo, status, preço e data de criação.
- [ ] Garantir transação única para matching e gravação de trades.
- [ ] Configurar retry/resiliência para conexão com PostgreSQL.
- [ ] Implementar o reset usando o banco de dados.
- [ ] Adicionar logs de persistência e operações de matching.
