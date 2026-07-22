import { FormEvent } from 'react'
import { OrderType } from '../api'

type OrderFormProps = {
  asset: string
  orderType: OrderType
  quantity: string
  price: string
  submitting: boolean
  onAssetChange: (asset: string) => void
  onOrderTypeChange: (type: OrderType) => void
  onQuantityChange: (quantity: string) => void
  onPriceChange: (price: string) => void
  onSubmit: (event: FormEvent<HTMLFormElement>) => void
}

export function OrderForm({ asset, orderType, quantity, price, submitting, onAssetChange, onOrderTypeChange, onQuantityChange, onPriceChange, onSubmit }: OrderFormProps) {
  return (
    <form className="card order-form" onSubmit={onSubmit}>
      <div className="card-heading"><div><p className="eyebrow">NOVA ORDEM</p><h3>Enviar ordem</h3></div><span className="card-icon">＋</span></div>
      <div className="segmented-control">
        {(['Compra', 'Venda'] as OrderType[]).map((type) => <button key={type} type="button" className={orderType === type ? `selected ${type.toLowerCase()}` : ''} onClick={() => onOrderTypeChange(type)}>{type}</button>)}
      </div>
      <label>Ativo<input value={asset} onChange={(event) => onAssetChange(event.target.value.toUpperCase())} placeholder="PETR4" maxLength={20} /></label>
      <div className="form-row">
        <label>Quantidade<input type="number" min="1" value={quantity} onChange={(event) => onQuantityChange(event.target.value)} /></label>
        <label>Preço por ação<input inputMode="decimal" value={price} onChange={(event) => onPriceChange(event.target.value)} placeholder="30,50" /></label>
      </div>
      <button className={`primary-button ${orderType === 'Venda' ? 'sell-button' : ''}`} disabled={submitting} type="submit">{submitting ? 'Enviando...' : `Enviar ordem de ${orderType.toLowerCase()}`}</button>
      <p className="form-note">As ordens compatíveis são executadas automaticamente pelo motor de matching.</p>
    </form>
  )
}
