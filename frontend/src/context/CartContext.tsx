import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import type { Product } from '../types'

export type CartItem = { product: Product; quantity: number }

type CartValue = {
  items: CartItem[]
  isOpen: boolean
  count: number
  total: number
  add: (product: Product) => void
  remove: (id: string) => void
  change: (id: string, quantity: number) => void
  setOpen: (open: boolean) => void
  clear: () => void
}

const CartContext = createContext<CartValue | null>(null)

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CartItem[]>(() => {
    try {
      return JSON.parse(localStorage.getItem('faraluna-cart') ?? '[]') as CartItem[]
    } catch {
      return []
    }
  })
  const [isOpen, setOpen] = useState(false)

  useEffect(() => {
    localStorage.setItem('faraluna-cart', JSON.stringify(items))
  }, [items])

  const value = useMemo<CartValue>(() => ({
    items,
    isOpen,
    count: items.reduce((sum, item) => sum + item.quantity, 0),
    total: items.reduce((sum, item) => sum + item.product.price * item.quantity, 0),
    add(product) {
      setItems(current => {
        const existing = current.find(item => item.product.id === product.id)
        if (existing) {
          return current.map(item => item.product.id === product.id
            ? { ...item, quantity: Math.min(item.quantity + 1, product.stock) }
            : item)
        }
        return [...current, { product, quantity: 1 }]
      })
      setOpen(true)
    },
    remove(id) {
      setItems(current => current.filter(item => item.product.id !== id))
    },
    change(id, quantity) {
      setItems(current => current.map(item => item.product.id === id
        ? { ...item, quantity: Math.max(1, Math.min(quantity, item.product.stock)) }
        : item))
    },
    setOpen,
    clear: () => setItems([]),
  }), [items, isOpen])

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>
}

export function useCart() {
  const value = useContext(CartContext)
  if (!value) throw new Error('useCart must be inside CartProvider')
  return value
}
