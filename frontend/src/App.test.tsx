import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import App from './App'
import * as api from './api'

vi.mock('./api', async () => {
  const actual = await vi.importActual<typeof import('./api')>('./api')
  return { ...actual, cancelOrder: vi.fn(), checkHealth: vi.fn(), createOrder: vi.fn(), getOrderBook: vi.fn(), listOrders: vi.fn(), listTrades: vi.fn() }
})

const order = {
  id: 'order-12345678', tipo: 'Compra' as const, ativo: 'PETR4', quantidade: 100, preco: 30.5,
  quantidadeExecutada: 20, status: 'ParcialmenteExecutada' as const, dataHoraCriacao: '2026-07-21T12:00:00Z',
}

const trade = {
  id: 'trade-1', ordemCompraId: 'buy-1', ordemVendaId: 'sell-1', ativo: 'PETR4', quantidade: 20,
  precoExecucao: 30.5, dataHoraExecucao: '2026-07-21T12:01:00Z',
}

describe('App', () => {
  beforeEach(() => {
    vi.mocked(api.checkHealth).mockResolvedValue({ status: 'ok' })
    vi.mocked(api.listOrders).mockResolvedValue([order])
    vi.mocked(api.listTrades).mockResolvedValue([trade])
    vi.mocked(api.getOrderBook).mockResolvedValue({ ativo: 'PETR4', compras: [{ preco: 30.5, quantidade: 80 }], vendas: [{ preco: 31, quantidade: 50 }] })
    vi.mocked(api.createOrder).mockResolvedValue(order)
    vi.mocked(api.cancelOrder).mockResolvedValue(order)
  })

  afterEach(() => {
    vi.clearAllMocks()
  })

  it('carrega a conexão, livro, trades e ordens abertas', async () => {
    render(<App />)

    expect(await screen.findByText('API conectada')).toBeInTheDocument()
    expect(await screen.findByText('Negócios executados')).toBeInTheDocument()
    expect(screen.getByText('Ordens abertas')).toBeInTheDocument()
    expect(screen.getByText('20 ações')).toBeInTheDocument()
    expect(screen.getByText('80')).toBeInTheDocument()
  })

  it('valida os campos antes de chamar a API', async () => {
    render(<App />)
    const quantity = screen.getByLabelText('Quantidade')
    fireEvent.change(quantity, { target: { value: '0' } })
    fireEvent.submit(screen.getByRole('button', { name: /Enviar ordem de compra/i }).closest('form')!)

    expect(await screen.findByRole('button', { name: /Enviar ordem de compra/i })).toBeInTheDocument()
    expect(api.createOrder).not.toHaveBeenCalled()
    expect(document.querySelector('.alert.error')).toBeInTheDocument()
  })

  it('envia uma ordem com preço decimal digitado com vírgula', async () => {
    render(<App />)
    fireEvent.change(screen.getByLabelText('Quantidade'), { target: { value: '25' } })
    fireEvent.change(screen.getByLabelText(/Pre/), { target: { value: '30,75' } })
    fireEvent.submit(screen.getByRole('button', { name: /Enviar ordem de compra/i }).closest('form')!)

    await waitFor(() => expect(api.createOrder).toHaveBeenCalledWith({ tipo: 'Compra', ativo: 'PETR4', quantidade: 25, preco: 30.75 }))
    expect(await screen.findByText(/Ordem order-12 criada com sucesso/i)).toBeInTheDocument()
  })

  it('permite alternar para venda e cancelar uma ordem aberta', async () => {
    render(<App />)
    fireEvent.click(screen.getByRole('button', { name: 'Venda' }))
    expect(screen.getByRole('button', { name: /Enviar ordem de venda/i })).toBeInTheDocument()

    fireEvent.click(await screen.findByRole('button', { name: 'Cancelar' }))
    await waitFor(() => expect(api.cancelOrder).toHaveBeenCalledWith(order.id))
  })
})
