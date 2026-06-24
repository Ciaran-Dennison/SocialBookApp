import type { Book } from './book'
import type { User } from './user'

export interface Review {
  id?: number
  user?: User
  book?: Book
  rating: number
  comment: string
  createdDate?: string
}
