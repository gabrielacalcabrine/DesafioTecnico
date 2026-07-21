import { FormEvent, useCallback, useEffect, useMemo, useState } from 'react'
import './styles.css'
import { cancelOrder, checkHealth, createOrder, getOrderBook, listOrders, listTrades, Order, OrderBook, OrderType, Trade } from './api'

const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })
const dateTime = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' })
const formatMoney = (value: number) => money.format(value)
const formatDate = (value: string) => dateTime.format(new Date(value))
const remaining = (order: Order) => order.quantidade - order.quantidadeExecutada

function App() {
  const [asset, setAsset] = useState('PETR4')
  const [orderType, setOrderType] = useState<OrderType>('Compra')
  const [quantity, setQuantity] = useState('100')
  const [price, setPrice] = useState('30.50')
  const [orders, setOrders] = useState<Order[]>([])
  const [trades, setTrades] = useState<Trade[]>([])
  const [book, setBook] = useState<OrderBook>({ ativo: 'PETR4', compras: [], vendas: [] })
  const [isHealthy, setIsHealthy] = useState(false)
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null)

  const refresh = useCallback(async () => {
    const normalizedAsset = asset.trim().toUpperCase()
    if (!normalizedAsset) return
    try {
      const [nextOrders, nextTrades, nextBook] = await Promise.all([listOrders(normalizedAsset), listTrades(normalizedAsset), getOrderBook(normalizedAsset)])
      setOrders(nextOrders); setTrades(nextTrades); setBook(nextBook); setIsHealthy(true)
    } catch { setIsHealthy(false) } finally { setLoading(false) }
  }, [asset])

  useEffect(() => { void refresh(); const interval = window.setInterval(() => void refresh(), 2000); return () => window.clearInterval(interval) }, [refresh])
  useEffect(() => { void checkHealth().then(() => setIsHealthy(true)).catch(() => setIsHealthy(false)) }, [])

  const openOrders = useMemo(() => orders.filter((order) => order.status === 'Aberta' || order.status === 'ParcialmenteExecutada'), [orders])

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setMessage(null)
    const normalizedAsset = asset.trim().toUpperCase(); const numericQuantity = Number(quantity); const numericPrice = Number(price.replace(',', '.'))
    if (!normalizedAsset || numericQuantity <= 0 || numericPrice <= 0) { setMessage({ type: 'error', text: 'Preencha ativo, quantidade e preço com valores válidos.' }); return }
    try {
      setSubmitting(true)
      const created = await createOrder({ tipo: orderType, ativo: normalizedAsset, quantidade: numericQuantity, preco: numericPrice })
      setMessage({ type: 'success', text: `Ordem ${created.id.slice(0, 8)} criada com sucesso.` }); await refresh()
    } catch (error) { setMessage({ type: 'error', text: error instanceof Error ? error.message : 'Não foi possível criar a ordem.' }) } finally { setSubmitting(false) }
  }

  async function handleCancel(order: Order) {
    try { await cancelOrder(order.id); setMessage({ type: 'success', text: 'Ordem cancelada.' }); await refresh() }
    catch (error) { setMessage({ type: 'error', text: error instanceof Error ? error.message : 'Não foi possível cancelar a ordem.' }) }
  }

  return <main className="app-shell">
    <header className="topbar"><div className="brand"><span className="brand-mark">↗</span><div><p className="eyebrow">MERCADO ABERTO</p><h1>Raiz Trading</h1></div></div><div className="connection-pill"><span className={`status-dot ${isHealthy ? 'online' : ''}`} />{isHealthy ? 'API conectada' : 'API indisponível'}</div></header>
    <section className="hero"><div><p className="eyebrow">PAINEL DE NEGOCIAÇÃO</p><h2>Decida com clareza.</h2><p className="hero-copy">Acompanhe o livro, envie ordens e veja os negócios executados em um só lugar.</p></div><label className="asset-selector"><span>Ativo em acompanhamento</span><input value={asset} onChange={(event) => setAsset(event.target.value.toUpperCase())} maxLength={20} aria-label="Ativo em acompanhamento" /></label></section>
    {message && <div className={`alert ${message.type}`}>{message.text}</div>}
    <section className="dashboard-grid">
      <form className="card order-form" onSubmit={handleSubmit}><div className="card-heading"><div><p className="eyebrow">NOVA ORDEM</p><h3>Enviar ordem</h3></div><span className="card-icon">＋</span></div><div className="segmented-control">{(['Compra', 'Venda'] as OrderType[]).map((type) => <button key={type} type="button" className={orderType === type ? `selected ${type.toLowerCase()}` : ''} onClick={() => setOrderType(type)}>{type}</button>)}</div><label>Ativo<input value={asset} onChange={(event) => setAsset(event.target.value.toUpperCase())} placeholder="PETR4" maxLength={20} /></label><div className="form-row"><label>Quantidade<input type="number" min="1" value={quantity} onChange={(event) => setQuantity(event.target.value)} /></label><label>Preço por ação<input inputMode="decimal" value={price} onChange={(event) => setPrice(event.target.value)} placeholder="30,50" /></label></div><button className={`primary-button ${orderType === 'Venda' ? 'sell-button' : ''}`} disabled={submitting} type="submit">{submitting ? 'Enviando...' : `Enviar ordem de ${orderType.toLowerCase()}`}</button><p className="form-note">As ordens compatíveis são executadas automaticamente pelo motor de matching.</p></form>
      <section className="card order-book-card"><div className="card-heading"><div><p className="eyebrow">MARKET DEPTH</p><h3>Livro de ofertas <span className="ticker">{book.ativo}</span></h3></div><span className="live-label"><span className="status-dot online" /> AO VIVO</span></div><div className="book-columns"><div><div className="book-label buy-label"><span>COMPRAS</span><span>QTD.</span></div>{book.compras.length ? book.compras.map((level) => <div className="book-row" key={`buy-${level.preco}`}><strong>{formatMoney(level.preco)}</strong><span>{level.quantidade}</span></div>) : <p className="empty-state">Sem ofertas de compra</p>}</div><div><div className="book-label sell-label"><span>VENDAS</span><span>QTD.</span></div>{book.vendas.length ? book.vendas.map((level) => <div className="book-row" key={`sell-${level.preco}`}><strong>{formatMoney(level.preco)}</strong><span>{level.quantidade}</span></div>) : <p className="empty-state">Sem ofertas de venda</p>}</div></div></section>
    </section>
    <section className="lower-grid"><section className="card"><div className="card-heading"><div><p className="eyebrow">LIQUIDEZ</p><h3>Negócios executados</h3></div><span className="count-badge">{trades.length}</span></div>{loading ? <p className="empty-state">Carregando negócios...</p> : trades.length === 0 ? <p className="empty-state">Nenhum negócio executado para {asset}.</p> : <div className="trade-list">{trades.slice().reverse().map((trade) => <div className="trade-row" key={trade.id}><div><strong>{trade.ativo}</strong><span>{formatDate(trade.dataHoraExecucao)}</span></div><div><strong>{trade.quantidade} ações</strong><span>{formatMoney(trade.precoExecucao)}</span></div></div>)}</div>}</section><section className="card"><div className="card-heading"><div><p className="eyebrow">ACOMPANHAMENTO</p><h3>Ordens abertas</h3></div><span className="count-badge">{openOrders.length}</span></div>{openOrders.length === 0 ? <p className="empty-state">Nenhuma ordem aberta para {asset}.</p> : <div className="open-order-list">{openOrders.map((order) => <div className="open-order-row" key={order.id}><div className="order-summary"><span className={`order-type ${order.tipo === 'Compra' ? 'buy' : 'sell'}`}>{order.tipo === 'Compra' ? 'C' : 'V'}</span><div><strong>{order.quantidade} ações · {formatMoney(order.preco)}</strong><span>{order.status} · {remaining(order)} remanescentes</span></div></div><button className="cancel-button" onClick={() => void handleCancel(order)}>Cancelar</button></div>)}</div>}</section></section>
    <footer className="footer"><span>Raiz Trading</span><span>Atualização automática a cada 2 segundos · API local</span></footer>
  </main>
}

export default App
