export function BrandMark({ inverted = false }: { inverted?: boolean }) {
  return <span className={inverted ? 'brand-mark inverted' : 'brand-mark'} aria-hidden="true">
    <span className="brand-crescent" />
    <span className="brand-star star-large">✦</span>
    <span className="brand-star star-small">✧</span>
  </span>
}
