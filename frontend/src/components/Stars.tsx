import { StarIcon } from './Icons'

export function Stars({ value, size = 18 }: { value: number; size?: number }) {
  return <span className="stars" aria-label={`${value} de 5 estrellas`}>
    {[1, 2, 3, 4, 5].map(star => <StarIcon key={star} filled={star <= Math.round(value)} width={size} height={size} />)}
  </span>
}
