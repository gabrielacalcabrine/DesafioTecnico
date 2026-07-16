// Cliente HTTP da API. A URL base pode ser sobrescrita em build com VITE_API_URL.
export const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8080'

// TODO: implemente as chamadas do contrato de API (ver README na raiz):
//   POST /orders, GET /orders, GET /orderbook/{ativo}, GET /trades, POST /orders/{id}/cancel
