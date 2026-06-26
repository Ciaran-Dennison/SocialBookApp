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
// --- Imports ---

import { onMounted, ref, watch, computed } from 'vue'
import { getUserByUsername } from '../services/userService.ts'
import { getBooks, addBook, deleteBook, updateBook, addReview } from '../services/bookService.ts'
import { useSearchStore } from '../stores/searchStore.ts'
import type { Book } from '../types/book.ts'
import type { Review } from '@/types/review.ts'
import { Genre, BookFormat } from '../types/enums.ts'

// --- State ---

// Global search store shared across all views via the navbar search bar
const searchStore = useSearchStore()

// Book data fetched from the API
const books = ref<Book[]>([])

// Currently selected book, drives the detail modal
const selectedBook = ref<Book | null>(null)

// Modal visibility toggles
const showAddBook = ref(false)
const showReviews = ref(false)
const showUpdateBook = ref(false)
const showFilter = ref(false)
const showAddReview = ref(false)

// Filter values for the book grid
const genreFilter = ref('')
const formatFilter = ref('')
const childFriendlyFilter = ref('')

// Loading states to prevent duplicate submissions
const isSubmitting = ref(false)
const isSubmittingReview = ref(false)
const isUpdatingBook = ref(false)

// --- Helpers ---

// Sets the selected book to drive the detail modal
const selectBook = (book: Book) => {
  selectedBook.value = book
}

// Calculates the average rating from a book's reviews
const getAverageRating = (book: Book): string => {
  if (book.reviews.length === 0) return 'No ratings'
  const avg = book.reviews.reduce((sum, r) => sum + r.rating, 0) / book.reviews.length
  return avg.toFixed(1)
}

// --- Filter actions ---

// Resets all book grid filters
const clearFilters = () => {
  genreFilter.value = ''
  formatFilter.value = ''
  childFriendlyFilter.value = ''
}

// --- Computed ---

// Filters the book grid by the navbar search term and any active filters
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

// --- Form data ---

// Add book form data with default values
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

// Update book form data - pre-populated when the update modal opens
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

// Add review form data
const newReview = ref({
  userName: '',
  rating: null as number | null,
  comment: '',
})

// --- API actions ---

// Submits the add book form and refreshes the book list
const submitBook = async () => {
  if (isSubmitting.value) return
  isSubmitting.value = true
  try {
    console.log('Adding book:', JSON.stringify({ ...newBook.value }))
    await addBook({ ...newBook.value } as Book)
    books.value = await getBooks()
    showAddBook.value = false
    // Reset form to defaults after successful submission
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

// Deletes a book after confirmation and refreshes the book list
const handleDeleteBook = async (id: number) => {
  if (!confirm('Are you sure you want to delete this book?')) return
  try {
    await deleteBook(id)
    books.value = await getBooks()
  } catch (error) {
    console.error('Error deleting book:', error)
  }
}

// Looks up the user by username then submits a review for the selected book
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
    // Re-selects the same book from fresh data so the reviews list updates
    selectedBook.value = books.value.find((b) => b.id === selectedBook.value!.id) ?? null
    showAddReview.value = false
    newReview.value = { userName: '', rating: null, comment: '' }
  } catch (error) {
    console.error('Error submitting review:', error)
  } finally {
    isSubmittingReview.value = false
  }
}

// --- Update book form ---

// Pre-populates the update form with the selected book's existing values when the modal opens
// published is split on 'T' to strip the time component for the date input
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

// Submits the update book form and refreshes the selected book from fresh data
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

// --- Lifecycle ---

// Fetches all books when the component mounts
onMounted(async () => {
  books.value = await getBooks()
})
</script>

<style scoped>
/* --- Page Layout --- */

/* Main page container */
.books-view {
  padding: 1rem;
}

/* Page header - space between title/filter and add button */
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

/* Groups the title and filter button together on the left */
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

/* --- Book Cards --- */

/* Responsive grid that auto-fills columns at minimum 280px wide */
.books-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

/* Individual book card */
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

/* Lifts card on hover for a tactile feel */
.book-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

/* Row of genre and format badges at the top of each card */
.book-card-header {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

/* Small pill badge for genre - teal background */
.genre-badge,
.format-badge {
  background-color: var(--color-highlight);
  color: var(--color-primary);
  padding: 0.25rem 0.75rem;
  border-radius: var(--border-radius-full);
  font-size: 0.75rem;
  font-weight: 600;
}

/* Format badge uses a slightly different colour to distinguish from genre */
.format-badge {
  background-color: var(--color-accent);
}

.book-title {
  color: var(--color-primary);
  font-size: 1.25rem;
  margin: 0 0 0.75rem 0;
}

/* Blurb truncated to 3 lines to keep cards a consistent height */
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

/* Card footer with language and rating side by side */
.book-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: var(--color-secondary);
  font-size: 0.85rem;
}

/* Shown when no books exist or none match the current filters */
.empty-state {
  text-align: center;
  padding: 4rem;
  color: var(--color-secondary);
  background: white;
  border-radius: var(--border-radius-lg);
  box-shadow: var(--shadow-sm);
}

/* --- Modals --- */

/* Full screen semi-transparent overlay behind all modals */
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

/* Base modal container - scrollable for long content */
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

/* Shared input, select and textarea styles inside modals
   excludes checkboxes which have their own styling */
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

/* Highlight border on focus */
.modal input:focus,
.modal select:focus,
.modal textarea:focus {
  border-color: var(--color-primary-mid);
}

/* Fixed height textarea that can be resized vertically */
.modal textarea {
  height: 100px;
  resize: vertical;
}

/* Right-aligned confirm/cancel buttons at the bottom of forms */
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

/* Checkbox row with label aligned alongside it */
.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
  color: var(--color-primary);
  cursor: pointer;
}

/* --- Buttons --- */

/* Primary action button - dark blue */
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

/* Secondary action button - light teal */
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

/* Invisible delete button on book cards - fades in on hover */
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

/* Invisible close button in modal headers */
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

/* Filter icon button */
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

/* Unused legacy update button class */
.btn-update {
  margin-left: auto;
  display: block;
  margin-top: 1rem;
}

/* Reviews button pushed to the right in the detail actions row */
.btn-reviews {
  margin-left: auto;
}

/* Full width add review toggle button */
.btn-add-review {
  margin-top: 1rem;
  width: 100%;
}

/* --- Detail Modal --- */

/* Wider modal for the book detail view */
.modal-detail {
  max-width: 600px;
}

/* Header row inside detail modals with title/badges and close button */
.modal-detail-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

/* Secondary info text in detail modals */
.detail-meta {
  color: var(--color-secondary);
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

/* Full blurb text in the detail modal */
.detail-blurb {
  color: #444;
  line-height: 1.6;
  margin-bottom: 1rem;
}

/* Published date in the detail modal */
.detail-published {
  color: var(--color-secondary);
  font-size: 0.85rem;
  margin-bottom: 1rem;
}

/* Highlighted rating pill in detail modals */
.detail-rating {
  background: var(--color-highlight);
  color: var(--color-primary);
  padding: 0.5rem 1rem;
  border-radius: var(--border-radius-md);
  display: inline-block;
  margin-bottom: 1.5rem;
  font-weight: 600;
}

/* Row of action buttons at the bottom of the detail modal */
.detail-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
  margin-top: 1rem;
}

/* --- Reviews Modal --- */

/* Narrower modal for the reviews list */
.modal-reviews {
  max-width: 500px;
}

.reviews-section h3 {
  color: var(--color-primary);
  margin-bottom: 1rem;
}

/* Shown when a book has no reviews yet */
.no-reviews {
  color: var(--color-secondary);
  text-align: center;
  padding: 1rem;
}

/* Individual review card in the reviews list */
.review-card {
  background: #f0f7ff;
  border-radius: var(--border-radius-md);
  padding: 1rem;
  margin-bottom: 0.75rem;
  box-shadow: var(--shadow-sm);
}

/* Row with rating and date side by side */
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

/* Add review form revealed by toggling the add review button */
.review-form {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 2px solid var(--color-highlight);
}

/* Inputs inside the review form - defined separately from .modal input
   since the review form sits inside an existing modal */
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

/* --- Filter Modal --- */

/* Narrow filter modal */
.modal-filter {
  max-width: 350px;
}

/* Label above filter dropdowns */
.filter-label {
  display: block;
  color: var(--color-primary);
  font-weight: 600;
  margin-bottom: 0.5rem;
}

/* --- Responsive --- */

@media (max-width: 768px) {
  /* Single column on mobile */
  .books-grid {
    grid-template-columns: 1fr;
  }

  /* Stack header vertically on mobile */
  .header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }
}
</style>
