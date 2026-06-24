<template>
  <div class="books-view">
    <div class="header">
      <h1>Books</h1>
      <button class="btn-primary" @click="showAddBook = true">+ Add Book</button>
    </div>

    <div class="books-grid" v-if="books.length > 0">
      <div class="book-card" v-for="book in books" :key="book.id" @click="selectBook(book)">
        <div class="book-card-header">
          <span class="genre-badge">{{ book.genre }}</span>
          <span class="format-badge">{{ book.format }}</span>
        </div>
        <h2 class="book-title">{{ book.title }}</h2>
        <p class="book-blurb">{{ book.blurb }}</p>
        <div class="book-footer">
          <span class="book-language">🌍 {{ book.language }}</span>
          <span class="book-rating">⭐ {{ getAverageRating(book) }}</span>
        </div>
      </div>
    </div>

    <div class="empty-state" v-else>
      <p>No books yet — add one to get started!</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getBooks } from '../services/bookService.ts'
import type { Book } from '../types/book.ts'

const books = ref<Book[]>([])
const showAddBook = ref(false)
const selectedBook = ref<Book | null>(null)

const selectBook = (book: Book) => {
  selectedBook.value = book
}

const getAverageRating = (book: Book): string => {
  if (book.reviews.length === 0) return 'No ratings'
  const avg = book.reviews.reduce((sum, r) => sum + r.rating, 0) / book.reviews.length
  return avg.toFixed(1)
}

onMounted(async () => {
  books.value = await getBooks()
})
</script>

<style scoped>
.books-view {
  padding: 1rem;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header h1 {
  color: var(--color-primary);
  font-size: 2rem;
  margin: 0;
}

.btn-primary {
  background-color: var(--color-primary-mid);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: var(--border-radius-full);
  cursor: pointer;
  font-size: 1rem;
  box-shadow: var(--shadow-sm);
  transition:
    background-color 0.2s,
    transform 0.1s;
}

.btn-primary:hover {
  background-color: var(--color-primary-light);
  transform: translateY(-1px);
}

.books-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.book-card {
  background: white;
  border-radius: var(--border-radius-lg);
  padding: 1.5rem;
  box-shadow: var(--shadow-md);
  cursor: pointer;
  transition:
    transform 0.2s,
    box-shadow 0.2s;
}

.book-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.book-card-header {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.genre-badge,
.format-badge {
  background-color: var(--color-highlight);
  color: var(--color-primary);
  padding: 0.25rem 0.75rem;
  border-radius: var(--border-radius-full);
  font-size: 0.75rem;
  font-weight: 600;
}

.format-badge {
  background-color: var(--color-accent);
}

.book-title {
  color: var(--color-primary);
  font-size: 1.25rem;
  margin: 0 0 0.75rem 0;
}

.book-blurb {
  color: #666;
  font-size: 0.9rem;
  line-height: 1.5;
  margin: 0 0 1rem 0;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.book-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: var(--color-secondary);
  font-size: 0.85rem;
}

.empty-state {
  text-align: center;
  padding: 4rem;
  color: var(--color-secondary);
  background: white;
  border-radius: var(--border-radius-lg);
  box-shadow: var(--shadow-sm);
}

@media (max-width: 768px) {
  .books-grid {
    grid-template-columns: 1fr;
  }

  .header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }
}
</style>
