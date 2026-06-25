import type { Author } from '../types/author.ts'
import { api } from './api'

export const getAuthors = async () => {
  const response = await api.get('/author')
  return response.data
}

export const getAuthorById = async (id: number) => {
  const response = await api.get(`/author/${id}`)
  return response.data
}

export const getAuthorBooks = async (id: number) => {
  const response = await api.get(`/author/${id}/books`)
  return response.data
}

export const getRatingPercentage = async (id: number) => {
  const response = await api.get(`/author/${id}/rating`)
  return response.data
}

export const addAuthor = async (author: Author) => {
  const response = await api.post('/author', author)
  return response.data
}

export const assignBook = async (authorId: number, bookId: number) => {
  const response = await api.put(`/author/${authorId}/book/${bookId}`)
  return response.data
}

export const unassignBook = async (authorId: number, bookId: number) => {
  await api.delete(`/author/${authorId}/book/${bookId}`)
}

export const updateAuthor = async (id: number, author: Author) => {
  const response = await api.put(`/author/${id}`, author)
  return response.data
}

export const deleteAuthor = async (id: number) => {
  await api.delete(`/author/${id}`)
}
