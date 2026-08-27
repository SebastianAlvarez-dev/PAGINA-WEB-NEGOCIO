import { useState, type ReactNode } from 'react'
import { Link, NavLink } from 'react-router-dom'
import { BagIcon, CloseIcon, MenuIcon } from './Icons'
import { CartDrawer } from './CartDrawer'
import { useCart } from '../context/CartContext'
import { BrandMark } from './BrandMark'

export function Layout({ children }: { children: ReactNode }) {
  const [menuOpen, setMenuOpen] = useState(false)
  const cart = useCart()

  return <div className="site-shell">
    <div className="announcement"><span>✦ Envíos a todo Ecuador</span><span>Atención personalizada por WhatsApp</span><span>Stock actualizado</span></div>
    <header className="site-header">
      <Link className="brand" to="/" aria-label="Faraluna Bisutería, inicio">
        <BrandMark />
        <span className="brand-name"><strong>FARALUNA</strong><small>B I S U T E R Í A</small></span>
      </Link>

      <button className="icon-button mobile-menu" onClick={() => setMenuOpen(!menuOpen)} aria-label={menuOpen ? 'Cerrar menú' : 'Abrir menú'} aria-expanded={menuOpen}>
        {menuOpen ? <CloseIcon /> : <MenuIcon />}
      </button>

      <nav className={menuOpen ? 'main-nav open' : 'main-nav'} onClick={() => setMenuOpen(false)}>
        <NavLink to="/">Inicio</NavLink>
        <NavLink to="/catalogo">Catálogo</NavLink>
        <a href="/#nosotros">Nosotros</a>
        <a href="/#contacto">Contacto</a>
      </nav>

      <button className="cart-button" onClick={() => cart.setOpen(true)} aria-label={`Abrir mi pedido${cart.count > 0 ? `, ${cart.count} productos` : ''}`}>
        <BagIcon />
        <span>Mi selección</span>
        {cart.count > 0 && <b>{cart.count}</b>}
      </button>
    </header>

    <main>{children}</main>

    <footer id="contacto" className="site-footer">
      <div className="footer-brand">
        <BrandMark inverted />
        <div><strong>FARALUNA</strong><p>Tu brillo, tu esencia, tu historia.</p></div>
      </div>
      <div><small>EXPLORA</small><Link to="/">Inicio</Link><Link to="/catalogo">Catálogo</Link><a href="/#nosotros">Nuestra historia</a></div>
      <div><small>HABLEMOS</small><a href="https://wa.me/593996359219">WhatsApp · 099 6359 219</a><Link to="/admin">Acceso administrativo</Link></div>
      <div className="footer-phrase"><span>Brilla</span><strong>a tu manera.</strong></div>
      <p className="copyright">© {new Date().getFullYear()} Faraluna Bisutería. Hecho con amor en Ecuador.</p>
    </footer>
    <CartDrawer />
  </div>
}
