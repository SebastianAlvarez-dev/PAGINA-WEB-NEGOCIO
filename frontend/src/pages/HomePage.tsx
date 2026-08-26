import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../lib/api'
import type { ProductPage } from '../types'
import { ArrowIcon } from '../components/Icons'
import { ProductCard } from '../components/ProductCard'

export function HomePage() {
  const [products, setProducts] = useState<ProductPage | null>(null)

  useEffect(() => {
    void api<ProductPage>('/api/catalog/products?page=1&pageSize=4').then(setProducts).catch(() => setProducts({ items: [], total: 0, page: 1, pageSize: 4 }))
  }, [])

  return <>
    <section className="hero">
      <div className="hero-copy">
        <span className="eyebrow">NUEVA COLECCIÓN · 2026</span>
        <h1>Detalles que hablan<br/><em>por ti.</em></h1>
        <p>Piezas delicadas para acompañar tus días, celebrar tus momentos y regalar un poquito de intención.</p>
        <div className="hero-actions">
          <Link className="primary-button" to="/catalogo">Explorar catálogo <ArrowIcon /></Link>
          <a className="text-link" href="#nosotros">Nuestra historia</a>
        </div>
        <div className="hero-trust"><span>✦ Hecho con cuidado</span><span>◇ Stock actualizado</span><span>♡ Atención cercana</span></div>
      </div>
      <div className="hero-art" aria-label="Identidad visual de Faraluna Bisutería">
        <div className="hero-logo-frame"><img src="/LOGO%202.jpeg" alt="Faraluna Bisutería" /></div>
        <div className="art-note">hecho<br/><strong>con amor</strong><span>↗</span></div>
        <div className="hero-spark spark-one">✦</div><div className="hero-spark spark-two">✧</div>
      </div>
    </section>

    <section className="category-strip">
      <Link to="/catalogo?categoria=cadenas"><span className="category-symbol">⌒</span><div><small>DESCUBRE</small><strong>Cadenas</strong></div></Link>
      <Link to="/catalogo?categoria=pulseras"><span className="category-symbol">◯</span><div><small>COMBINA</small><strong>Pulseras</strong></div></Link>
      <Link to="/catalogo?categoria=aretes"><span className="category-symbol">♢</span><div><small>BRILLA</small><strong>Aretes</strong></div></Link>
      <Link to="/catalogo?categoria=anillos"><span className="category-symbol">◎</span><div><small>CELEBRA</small><strong>Anillos</strong></div></Link>
    </section>

    <section className="featured section-wrap">
      <div className="section-heading"><div><span className="eyebrow">LAS FAVORITAS</span><h2>Piezas para <em>enamorarte</em></h2></div><Link to="/catalogo">Ver todo <ArrowIcon /></Link></div>
      {!products && <div className="loading-grid">Cargando piezas…</div>}
      {products && products.items.length > 0 && <div className="product-grid">{products.items.map(product => <ProductCard key={product.id} product={product} />)}</div>}
      {products?.items.length === 0 && <div className="catalog-empty"><span>◇</span><h3>La colección está tomando forma</h3><p>Pronto encontrarás aquí nuestras piezas. Si eres administradora, agrega el primer producto desde el panel.</p><Link className="secondary-button" to="/admin">Abrir administración</Link></div>}
    </section>

    <section id="nosotros" className="story-section">
      <div className="story-art"><span className="story-ring">◇</span><p>hecho<br/><em>con alma</em></p></div>
      <div className="story-copy"><span className="eyebrow">NUESTRA HISTORIA</span><h2>No vendemos accesorios.<br/><em>Creamos pequeños recuerdos.</em></h2><p>Faraluna Bisutería nace del gusto por los detalles: una pieza que completa un look, celebra una amistad o se convierte en ese regalo que sí dice lo que sentimos.</p><p>Seleccionamos y preparamos cada pedido con el mismo cuidado con el que nos gustaría recibirlo.</p><Link className="text-link" to="/catalogo">Conoce la colección <ArrowIcon /></Link></div>
    </section>

    <section className="promise-section section-wrap"><div><strong>01</strong><h3>Stock real</h3><p>El catálogo muestra la disponibilidad actual de cada pieza.</p></div><div><strong>02</strong><h3>Compra acompañada</h3><p>Confirmamos tu pedido, entrega y forma de pago por WhatsApp.</p></div><div><strong>03</strong><h3>Tu opinión importa</h3><p>Comparte una reseña y ayuda a otras personas a elegir.</p></div></section>

    <section className="contact-feature">
      <img src="/LOGO1.jpeg" alt="Faraluna Bisutería. Contáctanos al 099 6359 219" />
      <div><span className="eyebrow">ATENCIÓN PERSONALIZADA</span><h2>¿Encontraste una pieza<br/><em>para ti?</em></h2><p>Envíanos tu pedido por WhatsApp. Confirmaremos contigo el stock, la entrega y la forma de pago.</p><a className="primary-button" href="https://wa.me/593996359219">Escribir por WhatsApp <ArrowIcon /></a></div>
    </section>
  </>
}
