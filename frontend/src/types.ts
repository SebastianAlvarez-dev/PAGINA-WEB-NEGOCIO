export type Category = {
  id: string
  name: string
  slug: string
}

export type Product = {
  id: string
  name: string
  slug: string
  description: string
  categoryId: string
  category: string
  price: number
  currency: string
  stock: number
  imageUrl?: string | null
  isActive: boolean
  createdAt?: string
  updatedAt?: string
}

export type ProductPage = {
  items: Product[]
  total: number
  page: number
  pageSize: number
}

export type Review = {
  id: string
  productId: string
  authorName: string
  comment: string
  rating: number
  status: string
  createdAt: string
}

export type ReviewSummary = {
  productId: string
  averageRating: number
  total: number
  reviews: Review[]
}

export type ApiProblem = {
  title?: string
  detail?: string
  message?: string
}

