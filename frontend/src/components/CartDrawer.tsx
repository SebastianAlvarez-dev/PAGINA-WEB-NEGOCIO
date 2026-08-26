import { useCart } from '../context/CartContext'
import { money } from '../lib/api'
import { CloseIcon } from './Icons'

export function CartDrawer() {
  const cart = useCart()
  const number = (import.meta.env.VITE_WHATSAPP_NUMBER as string | undefined)
    || '593996359219'

  const sendOrder = () => {
    const lines = cart.items.map(item =>
      `${item.quantity} × ${item.product.name} — ${money(item.product.price * item.quantity, item.product.currency)}`)
    const message = [
      'Hola, deseo consultar este pedido:',
      '',
      ...lines,
      '',
      `Total estimado: ${money(cart.total)}`,
      '',
      '¿Me confirman disponibilidad y envío?'
    ].join('\n')
    window.open(`https://wa.me/${number}?text=${encodeURIComponent(message)}`, '_blank', 'noopener,noreferrer')
  }

  return <>
    <div className={cart.isOpen ? 'drawer-backdrop visible' : 'drawer-backdrop'} onClick={() => cart.setOpen(false)} />
    <aside className={cart.isOpen ? 'cart-drawer open' : 'cart-drawer'} aria-hidden={!cart.isOpen}>
      <div className="drawer-header">
        <div><span className="eyebrow">TU SELECCIÓN</span><h2>Mi pedido</h2></div>
        <button className="icon-button" onClick={() => cart.setOpen(false)} aria-label="Cerrar"><CloseIcon /></button>
      </div>

      <div className="drawer-items">
        {cart.items.length === 0 && <div className="empty-state"><div className="empty-gem">◇</div><h3>Tu pedido está vacío</h3><p>Explora el catálogo y guarda las piezas que te enamoren.</p></div>}
        {cart.items.map(item => <article className="cart-item" key={item.product.id}>
          <div className="cart-thumb">
            {item.product.imageUrl ? <img src={item.product.imageUrl} alt="" /> : <span>◇</span>}
          </div>
          <div><h3>{item.product.name}</h3><p>{money(item.product.price, item.product.currency)}</p>
            <div className="quantity">
              <button onClick={() => cart.change(item.product.id, item.quantity - 1)}>−</button>
              <span>{item.quantity}</span>
              <button onClick={() => cart.change(item.product.id, item.quantity + 1)}>+</button>
            </div>
          </div>
          <button className="remove" onClick={() => cart.remove(item.product.id)}>Eliminar</button>
        </article>)}
      </div>

      {cart.items.length > 0 && <div className="drawer-footer">
        <div className="drawer-total"><span>Total estimado</span><strong>{money(cart.total)}</strong></div>
        <p>La disponibilidad y el costo de envío se confirman por WhatsApp.</p>
        <button className="primary-button full" onClick={sendOrder}>Continuar por WhatsApp</button>
      </div>}
    </aside>
  </>
}
