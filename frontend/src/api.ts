export const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8080'

export type OrderType = 'Compra' | 'Venda'
export type OrderStatus = 'Aberta' | 'ParcialmenteExecutada' | 'Executada' | 'Cancelada'

export interface CreateOrderInput { tipo: OrderType; ativo: string; quantidade: number; preco: number }
export interface Order { id: string; tipo: OrderType; ativo: string; quantidade: number; preco: number; quantidadeExecutada: number; status: OrderStatus; dataHoraCriacao: string }
export interface Trade { id: string; ordemCompraId: string; ordemVendaId: string; ativo: string; quantidade: number; precoExecucao: number; dataHoraExecucao: string }
export interface OrderBookLevel { preco: number; quantidade: number }
export interface OrderBook { ativo: string; compras: OrderBookLevel[]; vendas: OrderBookLevel[] }
interface ApiErrorPayload { message?: string; errors?: string[] }

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_URL}${path}`, { headers: { 'Content-Type': 'application/json', ...options?.headers }, ...options })
  if (!response.ok) {
    let message = `Não foi possível concluir a operação (${response.status}).`
    try { const payload = (await response.json()) as ApiErrorPayload; message = payload.errors?.join(' ') ?? payload.message ?? message } catch { /* resposta sem JSON */ }
    throw new Error(message)
  }
  if (response.status === 204) return undefined as T
  return response.json() as Promise<T>
}

export function createOrder(input: CreateOrderInput) { return request<Order>('/orders', { method: 'POST', body: JSON.stringify(input) }) }
export function listOrders(asset?: string) { return request<Order[]>(`/orders${asset ? `?ativo=${encodeURIComponent(asset)}` : ''}`) }
export function getOrderBook(asset: string) { return request<OrderBook>(`/orderbook/${encodeURIComponent(asset)}`) }
export function listTrades(asset?: string) { return request<Trade[]>(`/trades${asset ? `?ativo=${encodeURIComponent(asset)}` : ''}`) }
export function cancelOrder(id: string) { return request<Order>(`/orders/${id}/cancel`, { method: 'POST' }) }
export function checkHealth() { return request<{ status: string }>('/health') }
