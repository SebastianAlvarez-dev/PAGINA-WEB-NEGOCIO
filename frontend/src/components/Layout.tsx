import { useState, type ReactNode } from 'react'
import { Link, NavLink } from 'react-router-dom'
import { BagIcon, CloseIcon, MenuIcon } from './Icons'
import { CartDrawer } from './CartDrawer'
import { useCart } from '../context/CartContext'

export function Layout({ children }: { children: ReactNode }) {
  const [menuOpen, setMenuOpen] = useState(false)
  const cart = useCart()

  return <div className="site-shell">
    <div className="announcement">Envíos a todo Ecuador · Atención personalizada por WhatsApp</div>
    <header className="site-header">
      <Link className="brand" to="/" aria-label="Faraluna Bisutería, inicio">
        <img className="brand-logo" src="/LOGO%202.jpeg" alt="" />
        <span><strong>FARALUNA</strong><small>bisutería hecha con amor</small></span>
      </Link>

      <button className="icon-button mobile-menu" onClick={() => setMenuOpen(!menuOpen)} aria-label="Abrir menú">
        {menuOpen ? <CloseIcon /> : <MenuIcon />}
      </button>

      <nav className={menuOpen ? 'main-nav open' : 'main-nav'} onClick={() => setMenuOpen(false)}>
        <NavLink to="/">Inicio</NavLink>
        <NavLink to="/catalogo">Catálogo</NavLink>
        <a href="/#nosotros">Nosotros</a>
        <a href="/#contacto">Contacto</a>
      </nav>

      <button className="cart-button" onClick={() => cart.setOpen(true)}>
        <BagIcon />
        <span>Mi pedido</span>
        {cart.count > 0 && <b>{cart.count}</b>}
      </button>
    </header>

    <main>{children}</main>

    <footer id="contacto" className="site-footer">
      <div className="footer-brand">
        <img className="footer-logo" src="/LOGO%202.jpeg" alt="" />
        <div><strong>Faraluna Bisutería</strong><p>Detalles hechos con amor.</p></div>
      </div>
      <div><small>EXPLORA</small><Link to="/catalogo">Catálogo</Link><Link to="/admin">Administración</Link></div>
      <div><small>CONTÁCTANOS</small><a href="https://wa.me/593996359219">WhatsApp: 099 6359 219</a></div>
      <p className="copyright">© {new Date().getFullYear()} Faraluna Bisutería. Hecho con amor en Ecuador.</p>
    </footer>
    <CartDrawer />
  </div>
}
