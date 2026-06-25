import type { PrivacyLevel, Genre } from './enums'

export interface User {
  id: number
  userName: string
  firstName: string
  lastName: string
  email: string
  password: string
  dateOfBirth: string
  createdDate: string
  languages: string[]
  privacyLevel: PrivacyLevel
  favouriteGenres: string[]
}
