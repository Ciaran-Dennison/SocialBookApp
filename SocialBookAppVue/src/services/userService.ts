import { api } from './api'
import type { User } from '../types/user.ts'

export const getUsers = async () => {
  const response = await api.get('/user')
  return response.data
}

export const getUserById = async (id: number) => {
  const response = await api.get(`/user/${id}`)
  return response.data
}

export const createUser = async (user: User) => {
  const response = await api.post('/user', user)
  return response.data
}

export const updateUser = async (id: number, user: User) => {
  const response = await api.put(`/user/${id}`, user)
  return response.data
}

export const deleteUser = async (id: number) => {
  await api.delete(`/user/${id}`)
}

export const addLanguage = async (id: number, language: string) => {
  await api.put(`/user/${id}/language`, language)
}

export const updateFavouriteGenres = async (id: number, genres: string[]) => {
  await api.put(`/user/${id}/genres`, genres)
}
