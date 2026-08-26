import { useEffect, useState, type FormEvent } from 'react'
import { Link, useParams } from 'react-router-dom'
import { Stars } from '../components/Stars'
import { BagIcon, StarIcon } from '../components/Icons'
import { useCart } from '../context/CartContext'
import { api, money } from '../lib/api'
import type { Product, ReviewSummary } from '../types'

export function ProductPage() {
  const { slug } = useParams()
  const cart = useCart()
  const [product, setProduct] = useState<Product | null>()
  const [reviews, setReviews] = useState<ReviewSummary | null>(null)
  const [rating, setRating] = useState(5)
  const [feedback, setFeedback] = useState('')

  useEffect(() => {
    void api<Product>(`/api/catalog/products/${slug}`)
      .then(value => {
        setProduct(value)
        return api<ReviewSummary>(`/api/products/${value.id}/reviews`)
      })
      .then(setReviews)
      .catch(() => setProduct(null))
  }, [slug])

  const submitReview = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const form = new FormData(event.currentTarget)
    try {
      const result = await api<{ message: string }>(`/api/products/${product!.id}/reviews`, {
        method: 'POST',
        body: JSON.stringify({ authorName: form.get('name'), comment: form.get('comment'), rating }),
      })
      setFeedback(result.message)
      event.currentTarget.reset()
      setRating(5)
    } catch (error) {
      setFeedback(error instanceof Error ? error.message : 'No se pudo enviar la reseña.')
    }
  }

  if (product === undefined) return <div className="page-loading">Preparando los detalles…</div>
  if (product === null) return <div className="not-found"><span>◇</span><h1>Esta pieza no está disponible</h1><Link to="/catalogo">Volver al catálogo</Link></div>

  return <section className="product-page section-wrap">
    <div className="breadcrumbs"><Link to="/catalogo">Catálogo</Link><span>/</span><span>{product.category}</span><span>/</span><b>{product.name}</b></div>
    <div className="product-detail">
      <div className="detail-image">{product.imageUrl ? <img src={product.imageUrl} alt={product.name} /> : <div className="image-placeholder large"><span>◇</span><small>FARALUNA</small></div>}</div>
      <div className="detail-copy">
        <span className="product-category">{product.category}</span>
        <h1>{product.name}</h1>
        {reviews && <div className="detail-rating"><Stars value={reviews.averageRating}/><span>{reviews.total ? `${reviews.averageRating} · ${reviews.total} reseñas` : 'Aún sin reseñas'}</span></div>}
        <strong className="detail-price">{money(product.price, product.currency)}</strong>
        <p className="detail-description">{product.description || 'Una pieza delicada y versátil para acompañarte todos los días.'}</p>
        <div className={product.stock > 0 ? 'availability' : 'availability sold'}><i />{product.stock > 0 ? `${product.stock} disponibles` : 'Agotado temporalmente'}</div>
        <button className="primary-button full detail-add" disabled={product.stock === 0} onClick={() => cart.add(product)}><BagIcon/> Agregar a mi pedido</button>
        <div className="detail-notes"><span>◇ Stock actualizado</span><span>♡ Preparado con cuidado</span><span>↗ Consulta el envío por WhatsApp</span></div>
      </div>
    </div>

    <div className="reviews-section">
      <div className="reviews-list"><span className="eyebrow">OPINIONES</span><h2>Lo que dicen de esta pieza</h2>
        {reviews?.reviews.length === 0 && <p className="muted">Sé la primera persona en compartir su experiencia.</p>}
        {reviews?.reviews.map(review => <article className="review-card" key={review.id}><Stars value={review.rating} size={15}/><p>“{review.comment}”</p><div><strong>{review.authorName}</strong><small>{new Date(review.createdAt).toLocaleDateString('es-EC')}</small></div></article>)}
      </div>
      <form className="review-form" onSubmit={submitReview}><span className="eyebrow">CUÉNTANOS</span><h3>Deja tu reseña</h3><label>Tu puntuación<div className="rating-picker">{[1,2,3,4,5].map(value => <button type="button" key={value} onClick={() => setRating(value)} aria-label={`${value} estrellas`}><StarIcon filled={value <= rating}/></button>)}</div></label><label>Tu nombre<input required minLength={2} maxLength={80} name="name" /></label><label>Comentario<textarea required minLength={5} maxLength={1000} name="comment" rows={4}/></label><button className="secondary-button" type="submit">Enviar reseña</button>{feedback && <p className="form-feedback">{feedback}</p>}</form>
    </div>
  </section>
}
