import type { ApiProblem } from '../types'
import { getAccessToken } from './supabase'

type ApiOptions = RequestInit & { authenticated?: boolean }
const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(/\/$/, '') ?? ''

export async function api<T>(path: string, options: ApiOptions = {}): Promise<T> {
  const headers = new Headers(options.headers)
  if (!(options.body instanceof FormData)) {
    headers.set('Content-Type', 'application/json')
  }

  if (options.authenticated) {
    const token = await getAccessToken()
    if (token) headers.set('Authorization', `Bearer ${token}`)
  }

  const response = await fetch(`${apiBaseUrl}${path}`, { ...options, headers })
  if (!response.ok) {
    let problem: ApiProblem = {}
    try {
      problem = await response.json() as ApiProblem
    } catch {
      // Keep a useful fallback when an upstream proxy returns non-JSON.
    }
    throw new Error(problem.detail ?? problem.message ?? problem.title ?? 'No se pudo completar la operación.')
  }

  if (response.status === 204) return undefined as T
  return response.json() as Promise<T>
}

export const money = (amount: number, currency = 'USD') =>
  new Intl.NumberFormat('es-EC', { style: 'currency', currency }).format(amount)
