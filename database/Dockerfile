FROM postgres:16-alpine

# Scripts colocados nesta pasta serão executados automaticamente pelo PostgreSQL
# na primeira inicialização do volume do banco.
COPY init/ /docker-entrypoint-initdb.d/
