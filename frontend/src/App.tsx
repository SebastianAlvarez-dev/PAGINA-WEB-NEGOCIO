import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { CartProvider } from './context/CartContext'
import { Layout } from './components/Layout'
import { HomePage } from './pages/HomePage'
import { CatalogPage } from './pages/CatalogPage'
import { ProductPage } from './pages/ProductPage'
import { AdminPage } from './pages/AdminPage'

export default function App() {
  return <BrowserRouter>
    <CartProvider>
      <Layout>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/catalogo" element={<CatalogPage />} />
          <Route path="/producto/:slug" element={<ProductPage />} />
          <Route path="/admin" element={<AdminPage />} />
          <Route path="*" element={<div className="not-found"><span>◇</span><h1>Página no encontrada</h1><a href="/">Volver al inicio</a></div>} />
        </Routes>
      </Layout>
    </CartProvider>
  </BrowserRouter>
}

