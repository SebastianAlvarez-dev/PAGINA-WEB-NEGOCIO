import { useCallback, useEffect, useState, type FormEvent } from 'react'
import { api, money } from '../lib/api'
import { supabase } from '../lib/supabase'
import type { Category, Product, ProductPage, Review } from '../types'
import { Stars } from '../components/Stars'

type ProductForm = {
  id?: string
  name: string
  description: string
  categoryId: string
  price: string
  stock: string
  imageUrl: string
  isActive: boolean
}

const emptyForm: ProductForm = { name: '', description: '', categoryId: '', price: '', stock: '', imageUrl: '', isActive: true }

export function AdminPage() {
  const [authenticated, setAuthenticated] = useState(false)
  const [loading, setLoading] = useState(true)
  const [message, setMessage] = useState('')
  const [categories, setCategories] = useState<Category[]>([])
  const [products, setProducts] = useState<Product[]>([])
  const [reviews, setReviews] = useState<Review[]>([])
  const [form, setForm] = useState<ProductForm>(emptyForm)
  const [tab, setTab] = useState<'products' | 'reviews'>('products')

  const loadDashboard = useCallback(async () => {
    try {
      const [categoryData, productData, reviewData] = await Promise.all([
        api<Category[]>('/api/catalog/categories'),
        api<ProductPage>('/api/admin/products', { authenticated: true }),
        api<Review[]>('/api/admin/reviews/pending', { authenticated: true }),
      ])
      setCategories(categoryData)
      setProducts(productData.items)
      setReviews(reviewData)
      setAuthenticated(true)
    } catch {
      setAuthenticated(false)
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { void loadDashboard() }, [loadDashboard])

  const login = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setMessage('')
    const data = new FormData(event.currentTarget)
    try {
      if (supabase) {
        const { error } = await supabase.auth.signInWithPassword({
          email: String(data.get('email')),
          password: String(data.get('password')),
        })
        if (error) throw error
      } else {
        localStorage.setItem('dev-admin-token', 'dev-admin')
      }
      await loadDashboard()
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'No se pudo iniciar sesión.')
    }
  }

  const logout = async () => {
    localStorage.removeItem('dev-admin-token')
    if (supabase) await supabase.auth.signOut()
    setAuthenticated(false)
  }

  const addCategory = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const target = event.currentTarget
    const data = new FormData(target)
    try {
      await api('/api/admin/categories', { method: 'POST', authenticated: true, body: JSON.stringify({ name: data.get('name') }) })
      target.reset()
      setCategories(await api<Category[]>('/api/catalog/categories'))
      setMessage('Categoría creada.')
    } catch (error) { setMessage(error instanceof Error ? error.message : 'No se pudo crear.') }
  }

  const uploadImage = async (file?: File) => {
    if (!file) return
    setMessage('Subiendo imagen…')
    const data = new FormData()
    data.append('file', file)
    try {
      const result = await api<{ url: string }>('/api/admin/images', { method: 'POST', authenticated: true, body: data })
      setForm(current => ({ ...current, imageUrl: result.url }))
      setMessage('Imagen lista.')
    } catch (error) { setMessage(error instanceof Error ? error.message : 'No se pudo subir.') }
  }

  const saveProduct = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const body = JSON.stringify({ ...form, price: Number(form.price), stock: Number(form.stock), imageUrl: form.imageUrl || null })
    try {
      if (form.id) {
        await api(`/api/admin/products/${form.id}`, { method: 'PUT', authenticated: true, body })
        setMessage('Producto actualizado.')
      } else {
        await api('/api/admin/products', { method: 'POST', authenticated: true, body })
        setMessage('Producto agregado al catálogo.')
      }
      setForm(emptyForm)
      await loadDashboard()
    } catch (error) { setMessage(error instanceof Error ? error.message : 'No se pudo guardar.') }
  }

  const edit = (product: Product) => {
    setForm({
      id: product.id,
      name: product.name,
      description: product.description,
      categoryId: product.categoryId,
      price: String(product.price),
      stock: String(product.stock),
      imageUrl: product.imageUrl ?? '',
      isActive: product.isActive,
    })
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  const archive = async (id: string) => {
    if (!window.confirm('¿Ocultar este producto del catálogo?')) return
    await api(`/api/admin/products/${id}`, { method: 'DELETE', authenticated: true })
    await loadDashboard()
  }

  const moderate = async (id: string, approve: boolean) => {
    await api(`/api/admin/reviews/${id}/moderation`, { method: 'PUT', authenticated: true, body: JSON.stringify({ approve }) })
    await loadDashboard()
  }

  if (loading) return <div className="page-loading">Verificando sesión…</div>
  if (!authenticated) return <section className="admin-login"><div className="login-card"><img className="admin-logo" src="/LOGO%202.jpeg" alt="Faraluna Bisutería" /><span className="eyebrow">ACCESO PRIVADO</span><h1>Administración</h1><p>Gestiona el catálogo, el stock y las reseñas desde un solo lugar.</p><form onSubmit={login}>{supabase && <><label>Correo<input name="email" type="email" required /></label><label>Contraseña<input name="password" type="password" required /></label></>}<button className="primary-button full" type="submit">{supabase ? 'Ingresar' : 'Entrar en modo local'}</button>{message && <p className="form-feedback">{message}</p>}</form>{!supabase && <small>El acceso local solo funciona con la API en modo Development.</small>}</div></section>

  return <section className="admin-page">
    <header className="admin-header"><div><span className="eyebrow">PANEL DE CONTROL</span><h1>Hola, administradora</h1><p>Mantén el catálogo siempre al día.</p></div><button className="text-button" onClick={logout}>Cerrar sesión</button></header>
    <div className="admin-tabs"><button className={tab === 'products' ? 'active' : ''} onClick={() => setTab('products')}>Productos <b>{products.length}</b></button><button className={tab === 'reviews' ? 'active' : ''} onClick={() => setTab('reviews')}>Reseñas pendientes <b>{reviews.length}</b></button></div>
    {message && <div className="admin-message">{message}<button onClick={() => setMessage('')}>×</button></div>}

    {tab === 'products' && <div className="admin-grid">
      <aside className="admin-form-card">
        <div className="form-title"><span>{form.id ? 'EDITAR' : 'NUEVA'}</span><h2>{form.id ? 'Actualizar pieza' : 'Agregar al catálogo'}</h2></div>
        <form onSubmit={saveProduct}>
          <label>Fotografía<div className="image-upload">{form.imageUrl ? <img src={form.imageUrl} alt="Vista previa" /> : <span>＋<small>Subir JPG, PNG o WebP</small></span>}<input type="file" accept="image/jpeg,image/png,image/webp" onChange={event => void uploadImage(event.target.files?.[0])}/></div></label>
          <label>Nombre<input required minLength={2} maxLength={140} value={form.name} onChange={event => setForm({ ...form, name: event.target.value })} placeholder="Ej. Cadena Luna" /></label>
          <label>Categoría<select required value={form.categoryId} onChange={event => setForm({ ...form, categoryId: event.target.value })}><option value="">Seleccionar…</option>{categories.map(category => <option key={category.id} value={category.id}>{category.name}</option>)}</select></label>
          <div className="form-row"><label>Precio USD<input required type="number" min="0" step="0.01" value={form.price} onChange={event => setForm({ ...form, price: event.target.value })}/></label><label>Stock<input required type="number" min="0" step="1" value={form.stock} onChange={event => setForm({ ...form, stock: event.target.value })}/></label></div>
          <label>Descripción<textarea rows={4} maxLength={2000} value={form.description} onChange={event => setForm({ ...form, description: event.target.value })} placeholder="Materiales, medidas y detalles…"/></label>
          {form.id && <label className="toggle-label"><input type="checkbox" checked={form.isActive} onChange={event => setForm({ ...form, isActive: event.target.checked })}/><span/> Visible en el catálogo</label>}
          <button className="primary-button full" type="submit">{form.id ? 'Guardar cambios' : 'Publicar producto'}</button>
          {form.id && <button className="text-button full" type="button" onClick={() => setForm(emptyForm)}>Cancelar edición</button>}
        </form>
        <form className="quick-category" onSubmit={addCategory}><label>Nueva categoría<div><input required minLength={2} maxLength={80} name="name" placeholder="Ej. Tobilleras"/><button type="submit">Agregar</button></div></label></form>
      </aside>
      <div className="admin-list"><div className="list-heading"><h2>Inventario actual</h2><span>{products.filter(product => product.isActive).length} activos</span></div>{products.length === 0 && <div className="catalog-empty compact"><span>◇</span><h3>Aún no hay productos</h3><p>Agrega la primera pieza usando el formulario.</p></div>}{products.map(product => <article className={!product.isActive ? 'admin-product inactive' : 'admin-product'} key={product.id}><div className="admin-thumb">{product.imageUrl ? <img src={product.imageUrl} alt=""/> : <span>◇</span>}</div><div className="admin-product-main"><span>{product.category}</span><h3>{product.name}</h3><p>{money(product.price, product.currency)}</p></div><div className={product.stock > 0 ? 'stock-count' : 'stock-count zero'}><strong>{product.stock}</strong><span>en stock</span></div><div className="row-actions"><button onClick={() => edit(product)}>Editar</button><button onClick={() => void archive(product.id)}>Ocultar</button></div></article>)}</div>
    </div>}

    {tab === 'reviews' && <div className="moderation-list"><div className="list-heading"><h2>Reseñas por revisar</h2><span>Solo se publican cuando las apruebas</span></div>{reviews.length === 0 && <div className="catalog-empty compact"><span>♡</span><h3>Todo al día</h3><p>No hay reseñas pendientes de moderación.</p></div>}{reviews.map(review => <article className="moderation-card" key={review.id}><Stars value={review.rating}/><p>“{review.comment}”</p><strong>{review.authorName}</strong><small>{new Date(review.createdAt).toLocaleDateString('es-EC')}</small><div><button className="reject" onClick={() => void moderate(review.id, false)}>Rechazar</button><button className="approve" onClick={() => void moderate(review.id, true)}>Aprobar y publicar</button></div></article>)}</div>}
  </section>
}
