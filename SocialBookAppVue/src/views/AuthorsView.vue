<template>
  <div class="authors-view">
    <div class="header">
      <div class="header-left">
        <h1>Authors</h1>
        <button class="btn-filter" @click="showFilter = true">
          <i class="fa-solid fa-filter"></i>
        </button>
      </div>
      <button class="btn-primary" @click="showAddAuthor = true">+ Add Author</button>
    </div>

    <div class="authors-grid" v-if="filteredAuthors.length > 0">
      <div
        class="author-card"
        v-for="author in filteredAuthors"
        :key="author.id"
        @click="selectedAuthor = author"
      >
        <h2 class="author-name">{{ author.firstName }} {{ author.lastName }}</h2>
        <p class="author-email">📧 {{ author.email }}</p>
        <p class="author-genres">🎭 {{ author.genres.join(', ') || 'No genres' }}</p>
        <div class="author-footer">
          <span class="author-rating">⭐ {{ getAverageRating(author) }}</span>
          <span class="author-books">📚 {{ author.books.length }} books</span>
        </div>
      </div>
    </div>

    <div class="empty-state" v-else>
      <p>
        {{
          authors.length === 0
            ? 'No authors yet — add one to get started!'
            : 'No authors match your search or filters.'
        }}
      </p>
    </div>
  </div>

  <!-- Add Author Modal -->
  <div class="modal-overlay" v-if="showAddAuthor" @click.self="showAddAuthor = false">
    <div class="modal">
      <h2>Add an Author</h2>
      <form @submit.prevent="submitAuthor">
        <input v-model="newAuthor.firstName" placeholder="First Name" required />
        <input v-model="newAuthor.lastName" placeholder="Last Name" required />
        <input v-model="newAuthor.email" placeholder="Email" type="email" required />
        <input v-model="newAuthor.password" placeholder="Password" required />
        <input v-model="newAuthor.dateOfBirth" placeholder="Date of Birth" type="date" required />
        <div class="modal-buttons">
          <button type="submit" :disabled="isSubmitting">
            {{ isSubmitting ? 'Adding...' : 'Add Author' }}
          </button>
          <button type="button" @click="showAddAuthor = false">Cancel</button>
        </div>
      </form>
    </div>
  </div>

  <!-- Author Detail Modal -->
  <div class="modal-overlay" v-if="selectedAuthor" @click.self="selectedAuthor = null">
    <div class="modal modal-detail">
      <div class="modal-detail-header">
        <h2>{{ selectedAuthor.firstName }} {{ selectedAuthor.lastName }}</h2>
        <button class="btn-close" @click="selectedAuthor = null">✕</button>
      </div>
      <p class="detail-meta">
        📧 {{ selectedAuthor.email }} · 🎂
        {{ new Date(selectedAuthor.dateOfBirth).toLocaleDateString() }} · 🌍
        {{ selectedAuthor.languages.join(', ') || 'No languages' }}
      </p>
      <p class="detail-meta">🎭 {{ selectedAuthor.genres.join(', ') || 'No genres' }}</p>
      <div class="detail-rating">⭐ Average Rating: {{ getAverageRating(selectedAuthor) }}</div>
      <div class="modal-actions">
        <button class="btn-primary" @click="showBooks = true">
          📚 {{ selectedAuthor.books.length }} Books
        </button>
        <button class="btn-secondary" @click="showAssignBook = true">+ Assign Book</button>
        <button class="btn-secondary" @click="showUpdateAuthor = true">✏️ Update Author</button>
        <button class="btn-delete-author" @click="handleDeleteAuthor(selectedAuthor!.id!)">
          🗑 Delete Author
        </button>
      </div>
    </div>
  </div>

  <!-- Books Modal -->
  <div class="modal-overlay" v-if="showBooks" @click.self="showBooks = false">
    <div class="modal modal-books">
      <div class="modal-detail-header">
        <h2>Books by {{ selectedAuthor?.firstName }} {{ selectedAuthor?.lastName }}</h2>
        <button class="btn-close" @click="showBooks = false">✕</button>
      </div>
      <div class="search-bar">
        <input v-model="bookSearch" placeholder="Search books..." class="search-input" />
        <button class="btn-filter" @click="showBookFilter = true">
          <i class="fa-solid fa-filter"></i>
        </button>
      </div>
      <div v-if="filteredBooks.length === 0" class="no-results">No books found.</div>
      <div class="book-list-item" v-for="book in filteredBooks" :key="book.id">
        <span class="book-list-title">{{ book.title }}</span>
        <span class="genre-badge">{{ book.genre }}</span>
        <button class="btn-delete" @click.stop="handleUnassignBook(book.id!)">🗑</button>
      </div>
    </div>
  </div>

  <!-- Book Filter Modal -->
  <div class="modal-overlay" v-if="showBookFilter" @click.self="showBookFilter = false">
    <div class="modal modal-filter">
      <div class="modal-detail-header">
        <h2>Filter Books</h2>
        <button class="btn-close" @click="showBookFilter = false">✕</button>
      </div>
      <label class="filter-label-authors">Genre</label>
      <select v-model="bookGenreFilter">
        <option value="">All Genres</option>
        <option
          v-for="key in Object.keys(Genre).filter((k) => isNaN(Number(k)))"
          :key="key"
          :value="key"
        >
          {{ key }}
        </option>
      </select>
      <div class="modal-buttons">
        <button type="button" class="btn-primary" @click="showBookFilter = false">Apply</button>
        <button type="button" @click="clearBookFilter">Clear</button>
      </div>
    </div>
  </div>

  <!-- Assign Book Modal -->
  <div class="modal-overlay" v-if="showAssignBook" @click.self="showAssignBook = false">
    <div class="modal modal-assign">
      <div class="modal-detail-header">
        <h2>Assign a Book</h2>
        <button class="btn-close" @click="showAssignBook = false">✕</button>
      </div>
      <input v-model="assignBookSearch" placeholder="Search for a book..." class="search-input" />
      <div
        class="book-list-item"
        v-for="book in filteredAssignBooks"
        :key="book.id"
        @click="submitAssignBook(book.id!)"
      >
        <span class="book-list-title">{{ book.title }}</span>
        <span class="genre-badge">{{ book.genre }}</span>
      </div>
      <div v-if="filteredAssignBooks.length === 0 && assignBookSearch" class="no-results">
        No books found.
      </div>
    </div>
  </div>

  <!-- Update Author Modal -->
  <div class="modal-overlay" v-if="showUpdateAuthor" @click.self="showUpdateAuthor = false">
    <div class="modal">
      <div class="modal-detail-header">
        <h2>Update Author</h2>
        <button class="btn-close" @click="showUpdateAuthor = false">✕</button>
      </div>
      <form @submit.prevent="submitUpdateAuthor">
        <div class="genre-select">
          <input v-model="updateAuthorLanguage" placeholder="Add a language" />
          <button type="button" class="btn-secondary" @click="addLanguageToUpdate">
            + Add Language
          </button>
        </div>
        <div class="tag-list">
          <span class="tag" v-for="lang in updateAuthorLanguages" :key="lang">
            {{ lang }}
            <button type="button" @click="removeLanguage(lang)">✕</button>
          </span>
        </div>
        <div class="genre-select">
          <select v-model="updateAuthorGenre">
            <option
              v-for="key in Object.keys(Genre).filter((k) => isNaN(Number(k)))"
              :key="key"
              :value="key"
            >
              {{ key }}
            </option>
          </select>
          <button type="button" class="btn-secondary" @click="addGenreToUpdate">+ Add Genre</button>
        </div>
        <div class="tag-list">
          <span class="tag" v-for="genre in updateAuthorGenres" :key="genre">
            {{ genre }}
            <button type="button" @click="removeGenre(genre)">✕</button>
          </span>
        </div>
        <div class="modal-buttons">
          <button type="submit" :disabled="isUpdating">
            {{ isUpdating ? 'Updating...' : 'Update' }}
          </button>
          <button type="button" @click="showUpdateAuthor = false">Cancel</button>
        </div>
      </form>
    </div>
  </div>

  <!-- Filter Modal -->
  <div class="modal-overlay" v-if="showFilter" @click.self="showFilter = false">
    <div class="modal modal-filter-authors">
      <div class="modal-detail-header">
        <h2>Filter Authors</h2>
        <button class="btn-close" @click="showFilter = false">✕</button>
      </div>
      <label class="filter-label-authors">Genre</label>
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
      <div class="modal-buttons">
        <button type="button" class="btn-primary" @click="showFilter = false">Apply</button>
        <button type="button" @click="clearFilters">Clear</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// --- Imports ---

import { onMounted, ref, computed, watch } from 'vue'
import {
  getAuthors,
  addAuthor,
  assignBook,
  unassignBook,
  updateAuthor,
  deleteAuthor,
} from '../services/authorService.ts'
import { getBooks } from '../services/bookService.ts'
import { Genre } from '../types/enums.ts'
import type { Author } from '../types/author.ts'
import type { Book } from '../types/book.ts'
import { useSearchStore } from '../stores/searchStore.ts'

// --- State ---

// Author data fetched from the API
const authors = ref<Author[]>([])
// All books fetched from the API, used for the assign book search
const books = ref<Book[]>([])
// Currently selected author, drives the detail modal
const selectedAuthor = ref<Author | null>(null)

// Modal visibility toggles
const showAddAuthor = ref(false)
const showBooks = ref(false)
const showAssignBook = ref(false)
const showUpdateAuthor = ref(false)
const showBookFilter = ref(false)
const showFilter = ref(false)

// Search and filter values
const bookSearch = ref('') // search term for the author's assigned book list
const assignBookSearch = ref('') // search term for the assign book modal
const bookGenreFilter = ref('') // genre filter for the author's assigned book list
const genreFilter = ref('') // genre filter for the main author grid

// Update author form input values
const updateAuthorLanguage = ref('') // current language being typed before adding
const updateAuthorGenre = ref('') // current genre being selected before adding
const updateAuthorLanguages = ref<string[]>([]) // accumulated list of languages to save
const updateAuthorGenres = ref<string[]>([]) // accumulated list of genres to save

// Loading states to prevent duplicate submissions
const isSubmitting = ref(false)
const isAssigning = ref(false)
const isUpdating = ref(false)

// Global search store shared across all views via the navbar search bar
const searchStore = useSearchStore()

// --- Computed ---

// Filters the author grid by the navbar search term and the genre filter
const filteredAuthors = computed(() => {
  return authors.value.filter((a) => {
    const matchesSearch = `${a.firstName} ${a.lastName}`
      .toLowerCase()
      .includes(searchStore.searchTerm.toLowerCase())
    const matchesGenre = !genreFilter.value || a.genres.includes(genreFilter.value)
    return matchesSearch && matchesGenre
  })
})

// Filters the selected author's assigned books by search term and genre
const filteredBooks = computed(() => {
  if (!selectedAuthor.value) return []
  return selectedAuthor.value.books.filter((b) => {
    const matchesSearch = b.title.toLowerCase().includes(bookSearch.value.toLowerCase())
    const matchesGenre = !bookGenreFilter.value || String(b.genre) === bookGenreFilter.value
    return matchesSearch && matchesGenre
  })
})

// Filters all books for the assign book search modal
const filteredAssignBooks = computed(() => {
  if (!assignBookSearch.value) return books.value
  return books.value.filter((b) =>
    b.title.toLowerCase().includes(assignBookSearch.value.toLowerCase()),
  )
})

// --- Helpers ---

// Calculates average rating by averaging each assigned book's own average rating
const getAverageRating = (author: Author): string => {
  const booksWithReviews = author.books.filter((b) => b.reviews.length > 0)
  if (booksWithReviews.length === 0) return 'No ratings'
  const avg =
    booksWithReviews.reduce((sum, b) => {
      const bookAvg = b.reviews.reduce((s, r) => s + r.rating, 0) / b.reviews.length
      return sum + bookAvg
    }, 0) / booksWithReviews.length
  return avg.toFixed(1)
}

// --- Filter actions ---

// Clears the author grid genre filter
const clearFilters = () => {
  genreFilter.value = ''
}

// Clears the assigned book list genre filter
const clearBookFilter = () => {
  bookGenreFilter.value = ''
}

// --- Form data ---

const newAuthor = ref({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  dateOfBirth: '',
  languages: [] as string[],
  genres: [] as Genre[],
  books: [],
  reviews: [],
})

// --- API actions ---

// Submits the add author form and refreshes the author list
const submitAuthor = async () => {
  if (isSubmitting.value) return
  isSubmitting.value = true
  try {
    await addAuthor({ ...newAuthor.value } as unknown as Author)
    authors.value = await getAuthors()
    showAddAuthor.value = false
    newAuthor.value = {
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      dateOfBirth: '',
      languages: [],
      genres: [],
      books: [],
      reviews: [],
    }
  } catch (error) {
    console.error('Error adding author:', error)
  } finally {
    isSubmitting.value = false
  }
}

// Assigns a book to the selected author and refreshes the author data
const submitAssignBook = async (bookId: number) => {
  if (isAssigning.value || !selectedAuthor.value) return
  isAssigning.value = true
  try {
    console.log('Assigning book:', bookId, 'to author:', selectedAuthor.value.id)
    await assignBook(selectedAuthor.value.id!, bookId)
    console.log('Assigned successfully')
    authors.value = await getAuthors()
    selectedAuthor.value = authors.value.find((a) => a.id === selectedAuthor.value!.id) ?? null
    showAssignBook.value = false
    assignBookSearch.value = ''
  } catch (error) {
    console.error('Error assigning book:', error)
  } finally {
    isAssigning.value = false
  }
}

// Removes a book from the selected author and refreshes the author data
const handleUnassignBook = async (bookId: number) => {
  if (!selectedAuthor.value) return
  try {
    await unassignBook(selectedAuthor.value.id!, bookId)
    authors.value = await getAuthors()
    selectedAuthor.value = authors.value.find((a) => a.id === selectedAuthor.value!.id) ?? null
  } catch (error) {
    console.error('Error unassigning book:', error)
  }
}

// --- Update author form ---

// Pre-populates the update form with the selected author's existing values when the modal opens
watch(showUpdateAuthor, (val) => {
  if (val && selectedAuthor.value) {
    updateAuthorLanguages.value = [...selectedAuthor.value.languages]
    updateAuthorGenres.value = [...selectedAuthor.value.genres] as string[]
  }
})

// Adds the current language input value to the list if it isn't already there
const addLanguageToUpdate = () => {
  if (
    updateAuthorLanguage.value &&
    !updateAuthorLanguages.value.includes(updateAuthorLanguage.value)
  ) {
    updateAuthorLanguages.value.push(updateAuthorLanguage.value)
    updateAuthorLanguage.value = ''
  }
}

// Removes a language from the update list
const removeLanguage = (lang: string) => {
  updateAuthorLanguages.value = updateAuthorLanguages.value.filter((l) => l !== lang)
}

// Adds the current genre selection to the list if it isn't already there
const addGenreToUpdate = () => {
  if (updateAuthorGenre.value && !updateAuthorGenres.value.includes(updateAuthorGenre.value)) {
    updateAuthorGenres.value.push(updateAuthorGenre.value)
    updateAuthorGenre.value = ''
  }
}

// Removes a genre from the update list
const removeGenre = (genre: string) => {
  updateAuthorGenres.value = updateAuthorGenres.value.filter((g) => g !== genre)
}

// Submits the update author form with the new languages and genres
const submitUpdateAuthor = async () => {
  if (isUpdating.value || !selectedAuthor.value) return
  isUpdating.value = true
  try {
    await updateAuthor(selectedAuthor.value.id!, {
      ...selectedAuthor.value,
      languages: updateAuthorLanguages.value,
      genres: updateAuthorGenres.value,
    } as unknown as Author)
    authors.value = await getAuthors()
    selectedAuthor.value = authors.value.find((a) => a.id === selectedAuthor.value!.id) ?? null
    showUpdateAuthor.value = false
  } catch (error) {
    console.error('Error updating author:', error)
  } finally {
    isUpdating.value = false
  }
}

// Deletes the selected author after confirmation and closes the detail modal
const handleDeleteAuthor = async (id: number) => {
  if (!confirm('Are you sure you want to delete this author?')) return
  try {
    await deleteAuthor(id)
    authors.value = await getAuthors()
    selectedAuthor.value = null
  } catch (error) {
    console.error('Error deleting author:', error)
  }
}

// --- Lifecycle ---

// Fetches all authors and books when the component mounts
onMounted(async () => {
  authors.value = await getAuthors()
  books.value = await getBooks()
})
</script>

<style scoped>
/* --- Page Layout --- */

/* Main page container */
.authors-view {
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

/* --- Author Cards --- */

/* Responsive grid that auto-fills columns at minimum 280px wide */
.authors-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

/* Card uses flex column so the footer can be pushed to the bottom */
.author-card {
  background: white;
  border-radius: var(--border-radius-lg);
  padding: 1.5rem;
  box-shadow: var(--shadow-md);
  cursor: pointer;
  transition:
    transform 0.2s,
    box-shadow 0.2s;
  display: flex;
  flex-direction: column;
}

/* Lifts card on hover for a tactile feel */
.author-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.author-name {
  color: var(--color-primary);
  font-size: 1.25rem;
  margin: 0 0 0.5rem 0;
}

.author-email {
  color: var(--color-secondary);
  font-size: 0.9rem;
  margin: 0 0 0.5rem 0;
}

.author-genres {
  color: var(--color-secondary);
  font-size: 0.9rem;
  margin: 0 0 1rem 0;
}

/* margin-top: auto pushes footer to the bottom of the card regardless of content height */
.author-footer {
  display: flex;
  justify-content: space-between;
  color: var(--color-secondary);
  font-size: 0.85rem;
  margin-top: auto;
}

/* Shown when no authors exist or none match the current filters */
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

/* Shared input, select and textarea styles inside modals */
.modal input,
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

/* Wider modal for the detail view */
.modal-detail {
  max-width: 600px;
}

/* Header row inside detail modals with title and close button */
.modal-detail-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

/* --- Buttons --- */

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

/* Destructive delete button - red */
.btn-delete-author {
  background-color: #ff4d4d;
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

.btn-delete-author:hover {
  background-color: #cc0000;
  transform: translateY(-1px);
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

/* Unused legacy filter button class - kept for reference */
.btn-filter-authors {
  background-color: var(--color-primary-mid);
  color: white;
  border: none;
  padding: 0.75rem 1rem;
  border-radius: var(--border-radius-md);
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.2s;
}

.btn-filter-authors:hover {
  background-color: var(--color-primary-light);
}

/* Row of action buttons inside detail modals */
.modal-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
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

/* --- Detail Modal Content --- */

/* Secondary info text in detail modals */
.detail-meta {
  color: var(--color-secondary);
  font-size: 0.9rem;
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

/* --- Search and Filter --- */

/* Standalone search input used in book list and assign modals */
.search-input {
  width: 100%;
  padding: 0.75rem;
  margin-bottom: 1rem;
  border: 2px solid var(--color-accent);
  border-radius: var(--border-radius-md);
  font-size: 1rem;
  outline: none;
  box-sizing: border-box;
}

.search-input:focus {
  border-color: var(--color-primary-mid);
}

/* Row combining a search input and filter button side by side */
.search-bar {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  align-items: center;
}

/* Search input inside search bar takes remaining space */
.search-bar .search-input {
  flex: 1;
  margin-bottom: 0;
}

/* Narrow filter modal for genre filtering */
.modal-filter {
  max-width: 350px;
}

/* Unused legacy filter modal class - kept for reference */
.modal-filter-authors {
  max-width: 350px;
}

/* Label above filter dropdowns */
.filter-label {
  display: block;
  color: var(--color-primary);
  font-weight: 600;
  margin-bottom: 0.5rem;
}

/* Unused legacy filter label - kept for reference */
.filter-label-authors {
  display: block;
  color: var(--color-primary);
  font-weight: 600;
  margin-bottom: 0.5rem;
}

/* --- Book List Items --- */

/* Row in the assigned books and assign book modals */
.book-list-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 1rem;
  background: #f0f7ff;
  border-radius: var(--border-radius-md);
  margin-bottom: 0.5rem;
}

/* Invisible delete button inside book list rows */
.book-list-item .btn-delete {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 1.1rem;
  opacity: 0.5;
  transition: opacity 0.2s;
  padding: 0;
  margin-left: 0.5rem;
}

.book-list-item .btn-delete:hover {
  opacity: 1;
}

/* Book title takes remaining space, pushing badge and delete to the right */
.book-list-title {
  color: var(--color-primary);
  font-weight: 500;
  flex: 1;
  margin-right: 0.5rem;
}

/* Small pill badge for genre labels */
.genre-badge {
  background-color: var(--color-highlight);
  color: var(--color-primary);
  padding: 0.25rem 0.75rem;
  border-radius: var(--border-radius-full);
  font-size: 0.75rem;
  font-weight: 600;
}

/* Shown when a search returns no results */
.no-results {
  text-align: center;
  color: var(--color-secondary);
  padding: 1rem;
}

/* Wider modal for the book list */
.modal-books {
  max-width: 500px;
}

/* Narrow modal for the assign book search */
.modal-assign {
  max-width: 400px;
}

/* --- Tags --- */

/* Wrapping row of tag pills for languages and genres */
.tag-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

/* Individual tag pill with remove button */
.tag {
  background: var(--color-highlight);
  color: var(--color-primary);
  padding: 0.25rem 0.75rem;
  border-radius: var(--border-radius-full);
  font-size: 0.85rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

/* Invisible remove button inside each tag */
.tag button {
  background: none;
  border: none;
  cursor: pointer;
  color: var(--color-primary);
  font-size: 0.75rem;
  padding: 0;
  line-height: 1;
}

/* Row combining an input or select with an add button side by side */
.genre-select {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  align-items: center;
}

/* Input and select inside genre-select take remaining space
   and override the full width set by .modal input */
.genre-select input,
.genre-select select {
  flex: 1;
  margin-bottom: 0;
  width: auto;
}

/* --- Responsive --- */

@media (max-width: 768px) {
  /* Single column on mobile */
  .authors-grid {
    grid-template-columns: 1fr;
  }

  /* Stack header vertically on mobile */
  .header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  /* Stack action buttons vertically on mobile */
  .modal-actions {
    flex-direction: column;
  }
}
</style>
