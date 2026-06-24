import type { Review } from '../types/review.ts'
import { api } from './api'

export const getReviews = async () => {
  const response = await api.get('/review')
  return response.data
}

export const editReview = async (id: number, review: Review) => {
  const response = await api.put(`/review/${id}`, review)
  return response.data
}
