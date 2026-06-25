import type { Author } from './author.ts'
import type { Review } from './review.ts'

export interface Book {
  id?: number
  title: string
  isbnNumber?: number
  authors: Author[]
  published: string
  genre: string
  language: string
  format: string
  isChildFriendly: boolean
  pages?: number
  chapters?: number
  blurb: string
  reviews: Review[]
}
