# TODO — API

- [x] Revisar o contrato de JSON e garantir `camelCase` em todas as respostas.
- [x] Criar tratamento global de exceções com erro estruturado (`406`, `404`, `409` e `500`).
- [x] Adicionar validação completa do payload de criação de ordens.
- [x] Validar filtros e datas inválidas nos endpoints de consulta.
- [x] Configurar Swagger/OpenAPI.
- [x] Restringir CORS às origens permitidas por configuração, com fallback local.
- [x] Configurar logs estruturados e correlation ID.
- [x] Implementar health check verificando o banco de dados.
- [x] Revisar a proteção do endpoint `/admin/reset`.
