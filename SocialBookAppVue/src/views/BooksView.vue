<template>
  <div class="books-view">
    <div class="header">
      <div class="header-left">
        <h1>Books</h1>
        <button class="btn-filter" @click="showFilter = true">
          <i class="fa-solid fa-filter"></i>
        </button>
      </div>
      <button class="btn-primary" @click="showAddBook = true">+ Add Book</button>
    </div>

    <div class="books-grid" v-if="filteredBooks.length > 0">
      <div class="book-card" v-for="book in filteredBooks" :key="book.id" @click="selectBook(book)">
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
        <button class="btn-delete" @click.stop="handleDeleteBook(book.id!)">🗑</button>
      </div>
    </div>

    <div class="empty-state" v-else>
      <p>
        {{
          books.length === 0
            ? 'No books yet — add one to get started!'
            : 'No books match your search or filters.'
        }}
      </p>
    </div>
  </div>

  <!-- Add Book Modal -->
  <div class="modal-overlay" v-if="showAddBook" @click.self="showAddBook = false">
    <div class="modal">
      <h2>Add a Book</h2>
      <form @submit.prevent="submitBook">
        <input v-model="newBook.title" placeholder="Title" required />
        <input
          v-model.number="newBook.isbnNumber"
          placeholder="ISBN"
          type="number"
          min="1"
          required
        />
        <input v-model="newBook.language" placeholder="Language" required />
        <input v-model.number="newBook.pages" placeholder="Pages" type="number" min="1" required />
        <input
          v-model.number="newBook.chapters"
          placeholder="Chapters"
          type="number"
          min="1"
          required
        />
        <textarea v-model="newBook.blurb" placeholder="Blurb" required />
        <input v-model="newBook.published" placeholder="Published date" type="date" required />
        <select v-model="newBook.genre">
          <option
            v-for="key in Object.keys(Genre).filter((k) => isNaN(Number(k)))"
            :key="key"
            :value="Genre[key as keyof typeof Genre]"
          >
            {{ key }}
          </option>
        </select>
        <select v-model="newBook.format">
          <option
            v-for="key in Object.keys(BookFormat).filter((k) => isNaN(Number(k)))"
            :key="key"
            :value="BookFormat[key as keyof typeof BookFormat]"
          >
            {{ key }}
          </option>
        </select>
        <label class="checkbox-label">
          <input type="checkbox" v-model="newBook.isChildFriendly" />
          <span>Child Friendly</span>
        </label>
        <div class="modal-buttons">
          <button type="submit" :disabled="isSubmitting">
            {{ isSubmitting ? 'Adding...' : 'Add Book' }}
          </button>
          <button type="button" @click="showAddBook = false">Cancel</button>
        </div>
      </form>
    </div>
  </div>

  <!-- Book Detail Modal -->
  <div class="modal-overlay" v-if="selectedBook" @click.self="selectedBook = null">
    <div class="modal modal-detail">
      <div class="modal-detail-header">
        <div class="book-card-header">
          <span class="genre-badge">{{ selectedBook.genre }}</span>
          <span class="format-badge">{{ selectedBook.format }}</span>
        </div>
        <button class="btn-close" @click="selectedBook = null">✕</button>
      </div>
      <h2>{{ selectedBook.title }}</h2>
      <p class="detail-meta">
        🌍 {{ selectedBook.language }} · 📄 {{ selectedBook.pages }} pages · 📚
        {{ selectedBook.chapters }} chapters · 🧒
        {{ selectedBook.isChildFriendly ? 'Child Friendly' : 'Not Child Friendly' }}
      </p>
      <p class="detail-blurb">{{ selectedBook.blurb }}</p>
      <p class="detail-published">
        Published: {{ new Date(selectedBook.published).toLocaleDateString() }}
      </p>
      <div class="detail-rating">
        <span>⭐ Average Rating: {{ getAverageRating(selectedBook) }}</span>
      </div>
      <div class="detail-actions">
        <button class="btn-secondary" @click="showUpdateBook = true">✏️ Update Book</button>
        <button class="btn-primary btn-reviews" @click="showReviews = true">
          💬 {{ selectedBook.reviews.length }} Reviews
        </button>
      </div>
    </div>
  </div>

  <!-- Reviews Modal -->
  <div class="modal-overlay" v-if="showReviews" @click.self="showReviews = false">
    <div class="modal modal-reviews">
      <div class="modal-detail-header">
        <h2>Reviews for {{ selectedBook?.title }}</h2>
        <button class="btn-close" @click="showReviews = false">✕</button>
      </div>

      <div v-if="selectedBook?.reviews.length === 0" class="no-reviews">No reviews yet.</div>

      <div class="review-card" v-for="review in selectedBook?.reviews" :key="review.id">
        <div class="review-header">
          <span class="review-rating">⭐ {{ review.rating }}</span>
          <span class="review-date">{{
            review.createdDate ? new Date(review.createdDate).toLocaleDateString() : ''
          }}</span>
        </div>
        <p class="review-comment">{{ review.comment }}</p>
      </div>

      <button class="btn-primary btn-add-review" @click="showAddReview = !showAddReview">
        {{ showAddReview ? 'Cancel' : '+ Add Review' }}
      </button>

      <form v-if="showAddReview" @submit.prevent="submitReview" class="review-form">
        <input v-model="newReview.userName" placeholder="Username" required />
        <input
          v-model="newReview.rating"
          placeholder="Rating (1-5)"
          type="number"
          min="1"
          max="5"
          required
        />
        <textarea v-model="newReview.comment" placeholder="Write your review..." required />
        <div class="modal-buttons">
          <button type="submit" :disabled="isSubmittingReview">
            {{ isSubmittingReview ? 'Submitting...' : 'Submit Review' }}
          </button>
        </div>
      </form>
    </div>
  </div>

  <!-- Update Book Modal -->
  <div class="modal-overlay" v-if="showUpdateBook" @click.self="showUpdateBook = false">
    <div class="modal">
      <div class="modal-detail-header">
        <h2>Update Book</h2>
        <button class="btn-close" @click="showUpdateBook = false">✕</button>
      </div>
      <form @submit.prevent="submitUpdateBook">
        <input v-model="updateBookData.title" placeholder="Title" required />
        <input v-model="updateBookData.language" placeholder="Language" required />
        <input v-model="updateBookData.pages" placeholder="Pages" type="number" min="1" required />
        <input
          v-model="updateBookData.chapters"
          placeholder="Chapters"
          type="number"
          min="1"
          required
        />
        <textarea v-model="updateBookData.blurb" placeholder="Blurb" required />
        <input
          v-model="updateBookData.published"
          placeholder="Published date"
          type="date"
          required
        />
        <select v-model="updateBookData.genre">
          <option
            v-for="key in Object.keys(Genre).filter((k) => isNaN(Number(k)))"
            :key="key"
            :value="key"
          >
            {{ key }}
          </option>
        </select>
        <select v-model="updateBookData.format">
          <option
            v-for="key in Object.keys(BookFormat).filter((k) => isNaN(Number(k)))"
            :key="key"
            :value="key"
          >
            {{ key }}
          </option>
        </select>
        <label class="checkbox-label">
          <input type="checkbox" v-model="updateBookData.isChildFriendly" />
          <span>Child Friendly</span>
        </label>
        <div class="modal-buttons">
          <button type="submit" :disabled="isUpdatingBook">
            {{ isUpdatingBook ? 'Updating...' : 'Update Book' }}
          </button>
          <button type="button" @click="showUpdateBook = false">Cancel</button>
        </div>
      </form>
    </div>
  </div>

  <!-- Filter Modal -->
  <div class="modal-overlay" v-if="showFilter" @click.self="showFilter = false">
    <div class="modal modal-filter">
      <div class="modal-detail-header">
        <h2>Filter Books</h2>
        <button class="btn-close" @click="showFilter = false">✕</button>
      </div>
      <label class="filter-label">Genre</label>
      <select v-model="genreFilter">
        <option value="">All Genres</option>
        <option
          v-for="key in Object.keys(Genre).filter((k) => isNaN(Number(k)))"
          :key="key"
          :value="key"
        >
          {{ key }}
        </option>
      </select>
      <label class="filter-label">Format</label>
      <select v-model="formatFilter">
        <option value="">All Formats</option>
        <option
          v-for="key in Object.keys(BookFormat).filter((k) => isNaN(Number(k)))"
          :key="key"
          :value="key"
        >
          {{ key }}
        </option>
      </select>
      <label class="filter-label">Child Friendly</label>
      <select v-model="childFriendlyFilter">
        <option value="">All</option>
        <option value="true">Yes</option>
        <option value="false">No</option>
      </select>
      <div class="modal-buttons">
        <button type="button" class="btn-primary" @click="showFilter = false">Apply</button>
        <button type="button" @click="clearFilters">Clear</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch, computed } from 'vue'
import { getUserByUsername } from '../services/userService.ts'
import { getBooks, addBook, deleteBook, updateBook, addReview } from '../services/bookService.ts'
import { useSearchStore } from '../stores/searchStore.ts'
import type { Book } from '../types/book.ts'
import type { Review } from '@/types/review.ts'
import { Genre, BookFormat } from '../types/enums.ts'

const searchStore = useSearchStore()
const books = ref<Book[]>([])
const showAddBook = ref(false)
const selectedBook = ref<Book | null>(null)
const showAddReview = ref(false)
const isSubmittingReview = ref(false)
const showUpdateBook = ref(false)
const isUpdatingBook = ref(false)
const showReviews = ref(false)
const showFilter = ref(false)
const genreFilter = ref('')
const formatFilter = ref('')
const childFriendlyFilter = ref('')

const clearFilters = () => {
  genreFilter.value = ''
  formatFilter.value = ''
  childFriendlyFilter.value = ''
}

const selectBook = (book: Book) => {
  selectedBook.value = book
}

const getAverageRating = (book: Book): string => {
  if (book.reviews.length === 0) return 'No ratings'
  const avg = book.reviews.reduce((sum, r) => sum + r.rating, 0) / book.reviews.length
  return avg.toFixed(1)
}

const newBook = ref({
  title: '',
  isbnNumber: null as number | null,
  language: '',
  pages: null as number | null,
  chapters: null as number | null,
  blurb: '',
  published: '',
  genre: 'Horror',
  format: 'Paperback',
  isChildFriendly: false,
  authors: [],
  reviews: [],
})

const isSubmitting = ref(false)

const submitBook = async () => {
  if (isSubmitting.value) return
  isSubmitting.value = true
  try {
    console.log('Adding book:', JSON.stringify({ ...newBook.value }))
    await addBook({ ...newBook.value } as Book)
    books.value = await getBooks()
    showAddBook.value = false
    newBook.value = {
      title: '',
      isbnNumber: null,
      language: '',
      pages: null,
      chapters: null,
      blurb: '',
      published: '',
      genre: 'Horror',
      format: 'Paperback',
      isChildFriendly: false,
      authors: [],
      reviews: [],
    }
  } catch (error) {
    console.error('Error adding book:', error)
  } finally {
    isSubmitting.value = false
  }
}

const handleDeleteBook = async (id: number) => {
  if (!confirm('Are you sure you want to delete this book?')) return
  try {
    await deleteBook(id)
    books.value = await getBooks()
  } catch (error) {
    console.error('Error deleting book:', error)
  }
}

const newReview = ref({
  userName: '',
  rating: null as number | null,
  comment: '',
})

const submitReview = async () => {
  if (isSubmittingReview.value) return
  isSubmittingReview.value = true
  try {
    const user = await getUserByUsername(newReview.value.userName)
    await addReview(selectedBook.value!.id!, user.id!, {
      rating: newReview.value.rating,
      comment: newReview.value.comment,
    } as unknown as Review)
    books.value = await getBooks()
    selectedBook.value = books.value.find((b) => b.id === selectedBook.value!.id) ?? null
    showAddReview.value = false
    newReview.value = { userName: '', rating: null, comment: '' }
  } catch (error) {
    console.error('Error submitting review:', error)
  } finally {
    isSubmittingReview.value = false
  }
}

const updateBookData = ref({
  title: '',
  language: '',
  pages: null as number | null,
  chapters: null as number | null,
  blurb: '',
  published: '',
  genre: '',
  format: '',
  isChildFriendly: false,
})

watch(showUpdateBook, (val) => {
  if (val && selectedBook.value) {
    updateBookData.value = {
      title: selectedBook.value.title,
      language: selectedBook.value.language,
      pages: selectedBook.value.pages ?? null,
      chapters: selectedBook.value.chapters ?? null,
      blurb: selectedBook.value.blurb,
      published: selectedBook.value.published?.split('T')[0] ?? '',
      genre: String(selectedBook.value.genre),
      format: String(selectedBook.value.format),
      isChildFriendly: selectedBook.value.isChildFriendly,
    }
  }
})

const submitUpdateBook = async () => {
  if (isUpdatingBook.value || !selectedBook.value) return
  isUpdatingBook.value = true
  try {
    await updateBook(selectedBook.value.id!, { ...updateBookData.value } as unknown as Book)
    books.value = await getBooks()
    selectedBook.value = books.value.find((b) => b.id === selectedBook.value!.id) ?? null
    showUpdateBook.value = false
  } catch (error) {
    console.error('Error updating book:', error)
  } finally {
    isUpdatingBook.value = false
  }
}

const filteredBooks = computed(() => {
  return books.value.filter((b) => {
    const matchesSearch = b.title.toLowerCase().includes(searchStore.searchTerm.toLowerCase())
    const matchesGenre = !genreFilter.value || String(b.genre) === genreFilter.value
    const matchesFormat = !formatFilter.value || String(b.format) === formatFilter.value
    const matchesChildFriendly =
      !childFriendlyFilter.value || String(b.isChildFriendly) === childFriendlyFilter.value
    return matchesSearch && matchesGenre && matchesFormat && matchesChildFriendly
  })
})

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

.header-left {
  display: flex;
  align-items: center;
  gap: 0.75rem;
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

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(13, 68, 123, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}

.modal {
  background: white;
  padding: 2rem;
  border-radius: var(--border-radius-lg);
  box-shadow: var(--shadow-lg);
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal h2 {
  color: var(--color-primary);
  margin-bottom: 1.5rem;
}

.modal input:not([type='checkbox']),
.modal select,
.modal textarea {
  width: 100%;
  padding: 0.75rem;
  margin-bottom: 1rem;
  border: 2px solid var(--color-accent);
  border-radius: var(--border-radius-md);
  font-size: 1rem;
  outline: none;
  box-sizing: border-box;
}

.modal input:focus,
.modal select:focus,
.modal textarea:focus {
  border-color: var(--color-primary-mid);
}

.modal textarea {
  height: 100px;
  resize: vertical;
}

.modal-buttons {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}

.modal-buttons button {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: var(--border-radius-full);
  cursor: pointer;
  font-size: 1rem;
}

.modal-buttons button[type='submit'] {
  background-color: var(--color-primary-mid);
  color: white;
}

.modal-buttons button[type='button'] {
  background-color: var(--color-highlight);
  color: var(--color-primary);
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
  color: var(--color-primary);
  cursor: pointer;
}

.btn-delete {
  margin-top: 0.75rem;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1.1rem;
  opacity: 0.5;
  transition: opacity 0.2s;
  float: right;
}

.btn-delete:hover {
  opacity: 1;
}

.modal-reviews {
  max-width: 500px;
}

.modal-detail {
  max-width: 600px;
}

.modal-detail-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.btn-close {
  background: none;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: var(--color-secondary);
  transition: color 0.2s;
}

.btn-close:hover {
  color: var(--color-primary);
}

.btn-secondary {
  background-color: var(--color-highlight);
  color: var(--color-primary);
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

.btn-secondary:hover {
  background-color: var(--color-accent);
  transform: translateY(-1px);
}

.btn-update {
  margin-left: auto;
  display: block;
  margin-top: 1rem;
}

.detail-meta {
  color: var(--color-secondary);
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

.detail-blurb {
  color: #444;
  line-height: 1.6;
  margin-bottom: 1rem;
}

.detail-published {
  color: var(--color-secondary);
  font-size: 0.85rem;
  margin-bottom: 1rem;
}

.detail-rating {
  background: var(--color-highlight);
  color: var(--color-primary);
  padding: 0.5rem 1rem;
  border-radius: var(--border-radius-md);
  display: inline-block;
  margin-bottom: 1.5rem;
  font-weight: 600;
}

.detail-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
  margin-top: 1rem;
}

.reviews-section h3 {
  color: var(--color-primary);
  margin-bottom: 1rem;
}

.no-reviews {
  color: var(--color-secondary);
  text-align: center;
  padding: 1rem;
}

.review-card {
  background: #f0f7ff;
  border-radius: var(--border-radius-md);
  padding: 1rem;
  margin-bottom: 0.75rem;
  box-shadow: var(--shadow-sm);
}

.btn-reviews {
  margin-left: auto;
}

.review-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.review-rating {
  color: var(--color-primary);
  font-weight: 600;
}

.review-date {
  color: var(--color-secondary);
  font-size: 0.85rem;
}

.review-comment {
  color: #444;
  margin: 0;
}

.btn-add-review {
  margin-top: 1rem;
  width: 100%;
}

.review-form {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 2px solid var(--color-highlight);
}

.review-form input,
.review-form textarea {
  width: 100%;
  padding: 0.75rem;
  margin-bottom: 1rem;
  border: 2px solid var(--color-accent);
  border-radius: var(--border-radius-md);
  font-size: 1rem;
  outline: none;
  box-sizing: border-box;
}

.review-form input:focus,
.review-form textarea:focus {
  border-color: var(--color-primary-mid);
}

.review-form textarea {
  height: 100px;
  resize: vertical;
}

.btn-filter {
  background-color: var(--color-primary-mid);
  color: white;
  border: none;
  padding: 0.75rem 1rem;
  border-radius: var(--border-radius-md);
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.2s;
}

.btn-filter:hover {
  background-color: var(--color-primary-light);
}

.modal-filter {
  max-width: 350px;
}

.filter-label {
  display: block;
  color: var(--color-primary);
  font-weight: 600;
  margin-bottom: 0.5rem;
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
