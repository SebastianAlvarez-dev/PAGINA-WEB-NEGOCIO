import { Link } from 'react-router-dom'
import type { Product } from '../types'
import { money } from '../lib/api'
import { BagIcon } from './Icons'
import { useCart } from '../context/CartContext'

export function ProductCard({ product }: { product: Product }) {
  const cart = useCart()

  return <article className="product-card">
    <Link to={`/producto/${product.slug}`} className="product-image">
      {product.imageUrl
        ? <img src={product.imageUrl} alt={product.name} loading="lazy" />
        : <div className="image-placeholder"><span>◇</span><small>FARALUNA</small></div>}
      {product.stock === 0 && <span className="stock-badge sold">Agotado</span>}
      {product.stock > 0 && product.stock <= 3 && <span className="stock-badge">Últimas {product.stock}</span>}
    </Link>
    <div className="product-info">
      <span className="product-category">{product.category}</span>
      <Link to={`/producto/${product.slug}`}><h3>{product.name}</h3></Link>
      <div className="product-bottom">
        <strong>{money(product.price, product.currency)}</strong>
        <button disabled={product.stock === 0} onClick={() => cart.add(product)} aria-label={`Agregar ${product.name}`}><BagIcon /></button>
      </div>
    </div>
  </article>
}
