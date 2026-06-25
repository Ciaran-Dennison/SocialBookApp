import type { Book } from './book.ts'
import type { Genre } from './enums.ts'
import type { Review } from './review.ts'

export interface Author {
  id?: number
  firstName: string
  lastName: string
  dateOfBirth: string
  createdDate?: string
  email: string
  password: string
  languages: string[]
  books: Book[]
  genres: string[]
  reviews: Review[]
}
