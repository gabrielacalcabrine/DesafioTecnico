import { Order, Trade } from '../api'

const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })
const dateTime = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' })
const remaining = (order: Order) => order.quantidade - order.quantidadeExecutada

export function TradesPanel({ asset, trades, loading }: { asset: string; trades: Trade[]; loading: boolean }) {
  return <section className="card"><div className="card-heading"><div><p className="eyebrow">LIQUIDEZ</p><h3>Negócios executados</h3></div><span className="count-badge">{trades.length}</span></div>{loading ? <p className="empty-state">Carregando negócios...</p> : trades.length === 0 ? <p className="empty-state">Nenhum negócio executado para {asset}.</p> : <div className="trade-list">{trades.slice().reverse().map((trade) => <div className="trade-row" key={trade.id}><div><strong>{trade.ativo}</strong><span>{dateTime.format(new Date(trade.dataHoraExecucao))}</span></div><div><strong>{trade.quantidade} ações</strong><span>{money.format(trade.precoExecucao)}</span></div></div>)}</div>}</section>
}

export function OpenOrdersPanel({ asset, orders, onCancel }: { asset: string; orders: Order[]; onCancel: (order: Order) => void }) {
  return <section className="card"><div className="card-heading"><div><p className="eyebrow">ACOMPANHAMENTO</p><h3>Ordens abertas</h3></div><span className="count-badge">{orders.length}</span></div>{orders.length === 0 ? <p className="empty-state">Nenhuma ordem aberta para {asset}.</p> : <div className="open-order-list">{orders.map((order) => <div className="open-order-row" key={order.id}><div className="order-summary"><span className={`order-type ${order.tipo === 'Compra' ? 'buy' : 'sell'}`}>{order.tipo === 'Compra' ? 'C' : 'V'}</span><div><strong>{order.quantidade} ações · {money.format(order.preco)}</strong><span>{order.status} · {remaining(order)} remanescentes</span></div></div><button className="cancel-button" onClick={() => onCancel(order)}>Cancelar</button></div>)}</div>}</section>
}
