import type { OrderBook as OrderBookData } from '../api'

const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })

type OrderBookProps = { book: OrderBookData }

function BookSide({ label, levels, tone }: { label: string; levels: OrderBookData['compras']; tone: 'buy' | 'sell' }) {
  return <div><div className={`book-label ${tone === 'buy' ? 'buy-label' : 'sell-label'}`}><span>{label}</span><span>QTD.</span></div>{levels.length ? levels.map((level) => <div className="book-row" key={`${tone}-${level.preco}`}><strong>{money.format(level.preco)}</strong><span>{level.quantidade}</span></div>) : <p className="empty-state">Sem ofertas de {tone === 'buy' ? 'compra' : 'venda'}</p>}</div>
}

export function OrderBook({ book }: OrderBookProps) {
  return <section className="card order-book-card"><div className="card-heading"><div><p className="eyebrow">MARKET DEPTH</p><h3>Livro de ofertas <span className="ticker">{book.ativo}</span></h3></div><span className="live-label"><span className="status-dot online" /> AO VIVO</span></div><div className="book-columns"><BookSide label="COMPRAS" levels={book.compras} tone="buy" /><BookSide label="VENDAS" levels={book.vendas} tone="sell" /></div></section>
}
