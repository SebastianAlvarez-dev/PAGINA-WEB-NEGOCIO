import { useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { ProductCard } from '../components/ProductCard'
import { SearchIcon } from '../components/Icons'
import { api } from '../lib/api'
import type { Category, ProductPage } from '../types'

export function CatalogPage() {
  const [params, setParams] = useSearchParams()
  const [categories, setCategories] = useState<Category[]>([])
  const [products, setProducts] = useState<ProductPage | null>(null)
  const [search, setSearch] = useState(params.get('buscar') ?? '')
  const categorySlug = params.get('categoria') ?? ''

  useEffect(() => { void api<Category[]>('/api/catalog/categories').then(setCategories) }, [])
  const categoryId = useMemo(() => categories.find(category => category.slug === categorySlug)?.id, [categories, categorySlug])

  useEffect(() => {
    const query = new URLSearchParams({ page: '1', pageSize: '48' })
    if (search) query.set('search', search)
    if (categoryId) query.set('categoryId', categoryId)
    setProducts(null)
    const timer = window.setTimeout(() => {
      void api<ProductPage>(`/api/catalog/products?${query}`).then(setProducts).catch(() => setProducts({ items: [], total: 0, page: 1, pageSize: 48 }))
    }, 200)
    return () => window.clearTimeout(timer)
  }, [search, categoryId])

  const selectCategory = (slug: string) => {
    const next = new URLSearchParams(params)
    if (slug) next.set('categoria', slug); else next.delete('categoria')
    setParams(next)
  }

  return <section className="catalog-page section-wrap">
    <header className="catalog-header"><span className="eyebrow">ENCUENTRA TU PIEZA</span><h1>Nuestro <em>catálogo</em></h1><p>Explora la colección y arma tu pedido con tus favoritos.</p></header>
    <div className="catalog-toolbar">
      <div className="category-pills">
        <button className={!categorySlug ? 'active' : ''} onClick={() => selectCategory('')}>Todo</button>
        {categories.map(category => <button key={category.id} className={category.slug === categorySlug ? 'active' : ''} onClick={() => selectCategory(category.slug)}>{category.name}</button>)}
      </div>
      <label className="search-field"><SearchIcon/><input value={search} onChange={event => setSearch(event.target.value)} placeholder="Buscar una pieza…" /></label>
    </div>
    <div className="result-count">{products ? `${products.total} ${products.total === 1 ? 'pieza' : 'piezas'}` : 'Buscando piezas…'}</div>
    {products && products.items.length > 0 && <div className="product-grid">{products.items.map(product => <ProductCard key={product.id} product={product} />)}</div>}
    {products?.items.length === 0 && <div className="catalog-empty"><span>◇</span><h3>No encontramos piezas</h3><p>Prueba con otra categoría o cambia el texto de búsqueda.</p></div>}
  </section>
}

