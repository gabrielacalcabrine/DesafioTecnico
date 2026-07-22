# TODO — Application

- [ ] Separar os casos de uso em comandos e consultas, se necessário.
- [x] Implementar validações de entrada para tipo, ativo, quantidade e preço no contrato e no domínio.
- [x] Garantir que o matching seja transacional e consistente.
- [x] Implementar prioridade preço-tempo no matching.
- [x] Definir tratamento de concorrência para ordens simultâneas.
- [x] Garantir que o preço de execução siga a regra definida para a ordem existente.
- [ ] Criar mapeadores entre domínio e DTOs sem acoplamento à API.
- [x] Publicar eventos de ordem criada, executada e cancelada.
- [ ] Adicionar paginação nas consultas de ordens e trades.
