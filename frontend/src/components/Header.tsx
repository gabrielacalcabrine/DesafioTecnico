type HeaderProps = {
  isHealthy: boolean
}

export function Header({ isHealthy }: HeaderProps) {
  return (
    <header className="topbar">
      <div className="brand">
        <span className="brand-mark">↗</span>
        <div>
          <p className="eyebrow">MERCADO ABERTO</p>
          <h1>Raiz Trading</h1>
        </div>
      </div>
      <div className="connection-pill">
        <span className={`status-dot ${isHealthy ? 'online' : ''}`} />
        {isHealthy ? 'API conectada' : 'API indisponível'}
      </div>
    </header>
  )
}

type HeroProps = {
  asset: string
  onAssetChange: (asset: string) => void
}

export function Hero({ asset, onAssetChange }: HeroProps) {
  return (
    <section className="hero">
      <div>
        <p className="eyebrow">PAINEL DE NEGOCIAÇÃO</p>
        <h2>Decida com clareza.</h2>
        <p className="hero-copy">Acompanhe o livro, envie ordens e veja os negócios executados em um só lugar.</p>
      </div>
      <label className="asset-selector">
        <span>Ativo em acompanhamento</span>
        <input value={asset} onChange={(event) => onAssetChange(event.target.value.toUpperCase())} maxLength={20} aria-label="Ativo em acompanhamento" />
      </label>
    </section>
  )
}
