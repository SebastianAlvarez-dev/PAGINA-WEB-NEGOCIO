import { createClient } from '@supabase/supabase-js'

const url = import.meta.env.VITE_SUPABASE_URL as string | undefined
const publishableKey = import.meta.env.VITE_SUPABASE_PUBLISHABLE_KEY as string | undefined

export const supabase = url && publishableKey
  ? createClient(url, publishableKey)
  : null

export async function getAccessToken() {
  if (!supabase) {
    return localStorage.getItem('dev-admin-token')
  }

  const { data } = await supabase.auth.getSession()
  return data.session?.access_token ?? null
}
