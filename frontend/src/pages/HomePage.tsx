import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../lib/api'
import type { ProductPage } from '../types'
import { ArrowIcon } from '../components/Icons'
import { ProductCard } from '../components/ProductCard'
import { CategoryIcon } from '../components/CategoryIcons'

export function HomePage() {
  const [products, setProducts] = useState<ProductPage | null>(null)

  useEffect(() => {
    void api<ProductPage>('/api/catalog/products?page=1&pageSize=4').then(setProducts).catch(() => setProducts({ items: [], total: 0, page: 1, pageSize: 4 }))
  }, [])

  return <>
    <section className="hero">
      <div className="hero-copy">
        <span className="hero-kicker">NUEVA COLECCIÓN · FARALUNA</span>
        <h1>Brilla tu esencia <em>cada día.</em></h1>
        <p>Piezas delicadas, juveniles y llenas de personalidad para convertir cada look en una forma de decir quién eres.</p>
        <div className="hero-actions">
          <Link className="primary-button hero-primary" to="/catalogo">Ver catálogo <ArrowIcon /></Link>
        </div>
      </div>
      <div className="hero-gallery">
        <figure className="hero-card hero-card-main">
          <img src="/campaigns/corazones-rojos.png" alt="Conjunto Faraluna de collar y aretes dorados con corazones rojos" fetchPriority="high" />
        </figure>
        <figure className="hero-card hero-card-accent">
          <img src="/campaigns/corazones-verdes.png" alt="Conjunto Faraluna de collar y aretes dorados con corazones verdes" />
        </figure>
        <div className="hero-sticker" aria-hidden="true"><span>☾</span> elegancia que<br/><strong>resalta tu estilo</strong></div>
        <div className="hero-moon" aria-hidden="true"><i/><span>✦</span></div>
        <span className="hero-spark spark-one" aria-hidden="true">✦</span>
        <span className="hero-spark spark-two" aria-hidden="true">✧</span>
      </div>
    </section>

    <section className="editorial-ribbon" aria-label="Manifiesto Faraluna"><span>PIEZAS ÚNICAS</span><em>para personas</em><span>ÚNICAS</span></section>

    <section className="category-strip">
      <Link to="/catalogo?categoria=cadenas"><CategoryIcon className="category-icon" type="chain"/><div><small>DESCUBRE</small><strong>Cadenas</strong></div></Link>
      <Link to="/catalogo?categoria=pulseras"><CategoryIcon className="category-icon" type="bracelet"/><div><small>COMBINA</small><strong>Pulseras</strong></div></Link>
      <Link to="/catalogo?categoria=aretes"><CategoryIcon className="category-icon" type="earrings"/><div><small>BRILLA</small><strong>Aretes</strong></div></Link>
      <Link to="/catalogo?categoria=anillos"><CategoryIcon className="category-icon" type="ring"/><div><small>CELEBRA</small><strong>Anillos</strong></div></Link>
    </section>

    <section id="favoritas" className="featured section-wrap">
      <div className="section-heading"><div><span className="eyebrow">DESTELLOS QUE CONECTAN CONTIGO</span><h2>Las favoritas de <em>Faraluna</em></h2></div></div>
      {!products && <div className="loading-grid">Cargando piezas…</div>}
      {products && products.items.length > 0 && <div className="product-grid">{products.items.map(product => <ProductCard key={product.id} product={product} />)}</div>}
      {products?.items.length === 0 && <div className="catalog-empty"><span>◇</span><h3>La colección está tomando forma</h3><p>Pronto encontrarás aquí nuevas piezas elegidas con mucho cariño.</p></div>}
    </section>
  </>
}
