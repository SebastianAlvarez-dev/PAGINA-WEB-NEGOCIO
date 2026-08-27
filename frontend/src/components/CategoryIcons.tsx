import type { SVGProps } from 'react'

type CategoryIconProps = SVGProps<SVGSVGElement> & {
  type: 'chain' | 'bracelet' | 'earrings' | 'ring'
}

export function CategoryIcon({ type, ...props }: CategoryIconProps) {
  const common = {
    viewBox: '0 0 64 64',
    fill: 'none',
    stroke: 'currentColor',
    strokeWidth: 1.8,
    strokeLinecap: 'round' as const,
    strokeLinejoin: 'round' as const,
    'aria-hidden': true,
  }

  if (type === 'chain') return <svg {...common} {...props}>
    <path d="M11 15c3 19 11 29 21 29s18-10 21-29" />
    <path d="M16 18c4 14 9 21 16 21s12-7 16-21" opacity=".45" />
    <path d="m32 41-7 7 7 8 7-8-7-7Z" className="icon-gem" />
    <circle cx="11" cy="14" r="2" /><circle cx="53" cy="14" r="2" />
  </svg>

  if (type === 'bracelet') return <svg {...common} {...props}>
    <circle cx="32" cy="32" r="20" />
    <circle cx="32" cy="12" r="3" className="icon-gem" />
    <circle cx="49" cy="21" r="3" /><circle cx="52" cy="38" r="3" />
    <circle cx="40" cy="50" r="3" /><circle cx="23" cy="50" r="3" />
    <circle cx="12" cy="37" r="3" /><circle cx="15" cy="21" r="3" />
    <path d="m29 32 3-5 3 5-3 5-3-5Z" className="icon-gem" />
  </svg>

  if (type === 'earrings') return <svg {...common} {...props}>
    <path d="M20 12v11M44 12v11" />
    <circle cx="20" cy="10" r="3" className="icon-gem" />
    <circle cx="44" cy="10" r="3" className="icon-gem" />
    <path d="M20 22c-7 7-9 13-9 18a9 9 0 0 0 18 0c0-5-2-11-9-18Z" />
    <path d="M44 22c-7 7-9 13-9 18a9 9 0 0 0 18 0c0-5-2-11-9-18Z" />
    <path d="m20 34-4 6 4 6 4-6-4-6Zm24 0-4 6 4 6 4-6-4-6Z" className="icon-gem" />
  </svg>

  return <svg {...common} {...props}>
    <ellipse cx="32" cy="40" rx="16" ry="13" />
    <path d="M22 31 18 20l8-10h12l8 10-4 11" />
    <path d="m18 20 14 9 14-9M26 10l6 19 6-19" />
    <path d="M18 20h28" className="icon-gem" />
    <path d="M24 42c2 5 5 7 8 7s6-2 8-7" opacity=".45" />
  </svg>
}
