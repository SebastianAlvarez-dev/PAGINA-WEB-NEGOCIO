import type { SVGProps } from 'react'

type IconProps = SVGProps<SVGSVGElement>

const base = { width: 20, height: 20, viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', strokeWidth: 1.7, strokeLinecap: 'round' as const, strokeLinejoin: 'round' as const }

export const BagIcon = (props: IconProps) => <svg {...base} {...props}><path d="M6 8h12l1 13H5L6 8Z"/><path d="M9 9V6a3 3 0 0 1 6 0v3"/></svg>
export const SearchIcon = (props: IconProps) => <svg {...base} {...props}><circle cx="11" cy="11" r="7"/><path d="m20 20-4-4"/></svg>
export const MenuIcon = (props: IconProps) => <svg {...base} {...props}><path d="M4 7h16M4 12h16M4 17h16"/></svg>
export const CloseIcon = (props: IconProps) => <svg {...base} {...props}><path d="m6 6 12 12M18 6 6 18"/></svg>
export const ArrowIcon = (props: IconProps) => <svg {...base} {...props}><path d="M5 12h14M14 7l5 5-5 5"/></svg>
export const StarIcon = ({ filled, ...props }: IconProps & { filled?: boolean }) => <svg {...base} {...props} fill={filled ? 'currentColor' : 'none'}><path d="m12 3 2.7 5.5 6.1.9-4.4 4.3 1 6.1-5.4-2.9-5.4 2.9 1-6.1-4.4-4.3 6.1-.9L12 3Z"/></svg>

