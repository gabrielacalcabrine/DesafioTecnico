Scripts SQL de inicialização do banco devem ser adicionados nesta pasta.

Eles serão executados automaticamente pelo PostgreSQL somente na primeira
inicialização de um volume novo.
The numbered SQL scripts are applied by PostgreSQL when a new volume is initialized.
They are kept versioned and mirror the EF Core model used by the API.
