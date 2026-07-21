import { afterEach, describe, expect, it, vi } from 'vitest'
import { cancelOrder, checkHealth, createOrder, getOrderBook, listOrders, listTrades } from './api'

describe('api client', () => {
  afterEach(() => vi.unstubAllGlobals())

  it('cria uma ordem enviando o payload correto', async () => {
    const response = { id: 'order-1', status: 'Aberta' }
    const fetchMock = vi.fn().mockResolvedValue(new Response(JSON.stringify(response), { status: 201 }))
    vi.stubGlobal('fetch', fetchMock)

    const result = await createOrder({ tipo: 'Compra', ativo: 'PETR4', quantidade: 10, preco: 30.5 })

    expect(result).toEqual(response)
    expect(fetchMock).toHaveBeenCalledWith('http://localhost:8080/orders', expect.objectContaining({
      method: 'POST',
      body: JSON.stringify({ tipo: 'Compra', ativo: 'PETR4', quantidade: 10, preco: 30.5 }),
    }))
  })

  it('codifica o ativo nas consultas', async () => {
    const fetchMock = vi.fn().mockImplementation(() => new Response('[]', { status: 200 }))
    vi.stubGlobal('fetch', fetchMock)

    await listOrders('BRK B')
    await listTrades('BRK B')
    await getOrderBook('BRK B')

    expect(fetchMock.mock.calls.map(([url]) => url)).toEqual([
      'http://localhost:8080/orders?ativo=BRK%20B',
      'http://localhost:8080/trades?ativo=BRK%20B',
      'http://localhost:8080/orderbook/BRK%20B',
    ])
  })

  it('converte erros da API em Error com a mensagem de validação', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response(JSON.stringify({ errors: ['Preço deve ser maior que zero.'] }), { status: 406 }))
    vi.stubGlobal('fetch', fetchMock)

    await expect(createOrder({ tipo: 'Venda', ativo: 'PETR4', quantidade: 1, preco: 0 })).rejects.toThrow('Preço deve ser maior que zero.')
  })

  it('aceita respostas sem conteúdo ao cancelar uma ordem', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response(null, { status: 204 }))
    vi.stubGlobal('fetch', fetchMock)

    await expect(cancelOrder('order-1')).resolves.toBeUndefined()
    expect(fetchMock).toHaveBeenCalledWith('http://localhost:8080/orders/order-1/cancel', expect.objectContaining({ method: 'POST' }))
  })

  it('consulta o health check', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response(JSON.stringify({ status: 'ok' }), { status: 200 }))
    vi.stubGlobal('fetch', fetchMock)

    await expect(checkHealth()).resolves.toEqual({ status: 'ok' })
  })
})
