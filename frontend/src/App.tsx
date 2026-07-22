import { FormEvent, useCallback, useEffect, useMemo, useState } from 'react'
import './styles.css'
import { cancelOrder, checkHealth, createOrder, getOrderBook, listOrders, listTrades, Order, OrderBook as OrderBookData, OrderType, Trade } from './api'
import { Footer, Header, Hero, OpenOrdersPanel, OrderBook, OrderForm, TradesPanel } from './components'

function App() {
  const [asset, setAsset] = useState('PETR4')
  const [orderType, setOrderType] = useState<OrderType>('Compra')
  const [quantity, setQuantity] = useState('100')
  const [price, setPrice] = useState('30.50')
  const [orders, setOrders] = useState<Order[]>([])
  const [trades, setTrades] = useState<Trade[]>([])
  const [book, setBook] = useState<OrderBookData>({ ativo: 'PETR4', compras: [], vendas: [] })
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
    <Header isHealthy={isHealthy} />
    <Hero asset={asset} onAssetChange={setAsset} />
    {message && <div className={`alert ${message.type}`}>{message.text}</div>}
    <section className="dashboard-grid"><OrderForm asset={asset} orderType={orderType} quantity={quantity} price={price} submitting={submitting} onAssetChange={setAsset} onOrderTypeChange={setOrderType} onQuantityChange={setQuantity} onPriceChange={setPrice} onSubmit={handleSubmit} /><OrderBook book={book} /></section>
    <section className="lower-grid"><TradesPanel asset={asset} trades={trades} loading={loading} /><OpenOrdersPanel asset={asset} orders={openOrders} onCancel={(order) => void handleCancel(order)} /></section>
    <Footer />
  </main>
}

export default App
