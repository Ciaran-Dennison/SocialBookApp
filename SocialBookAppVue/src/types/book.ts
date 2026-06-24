import type { Author } from './author.ts'
import type { Review } from './review.ts'
import type { BookFormat, Genre } from './enums'

export interface Book {
  id: number
  title: string
  isbnNumber: number
  authors: Author[]
  published: string
  genre: Genre
  language: string
  format: BookFormat
  isChildFriendly: boolean
  pages: number
  chapters: number
  blurb: string
  reviews: Review[]
}
