import { api } from './api'
import type { Book } from '../types/book.ts'
import type { Review } from '../types/review.ts'

export const getBooks = async () => {
  const response = await api.get('/book')
  return response.data
}

export const getBookById = async (id: number) => {
  const response = await api.get(`/book/${id}`)
  return response.data
}

export const getUserBooks = async (userId: number) => {
  const response = await api.get(`/book/user/${userId}`)
  return response.data
}

export const getRatingPercentage = async (id: number) => {
  const response = await api.get(`/book/${id}/rating`)
  return response.data
}

export const addReview = async (bookId: number, userId: number, review: Review) => {
  const response = await api.post(`/book/${bookId}/review/${userId}`, review)
  return response.data
}

export const addBook = async (book: Book) => {
  const response = await api.post('/book', book)
  return response.data
}

export const deleteBook = async (id: number) => {
  await api.delete(`/book/${id}`)
}

export const updateBook = async (id: number, book: Book) => {
  const response = await api.put(`/book/${id}`, book)
  return response.data
}
