<template>
  <div class="authors-view">
    <div class="header">
      <h1>Authors</h1>
      <button class="btn-primary" @click="showAddAuthor = true">+ Add Author</button>
    </div>

    <div class="authors-grid" v-if="authors.length > 0">
      <div
        class="author-card"
        v-for="author in authors"
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
      <p>No authors yet — add one to get started!</p>
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
      <label class="filter-label">Genre</label>
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
</template>

<script setup lang="ts">
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

const authors = ref<Author[]>([])
const books = ref<Book[]>([])
const selectedAuthor = ref<Author | null>(null)
const showAddAuthor = ref(false)
const showBooks = ref(false)
const showAssignBook = ref(false)
const bookSearch = ref('')
const assignBookSearch = ref('')
const isSubmitting = ref(false)
const isAssigning = ref(false)
const showUpdateAuthor = ref(false)
const isUpdating = ref(false)
const updateAuthorLanguage = ref('')
const updateAuthorGenre = ref('')
const updateAuthorLanguages = ref<string[]>([])
const updateAuthorGenres = ref<string[]>([])
const showBookFilter = ref(false)
const bookGenreFilter = ref('')

// filters the author's books by the search term
const filteredBooks = computed(() => {
  if (!selectedAuthor.value) return []
  return selectedAuthor.value.books.filter((b) => {
    const matchesSearch = b.title.toLowerCase().includes(bookSearch.value.toLowerCase())
    const matchesGenre = !bookGenreFilter.value || String(b.genre) === bookGenreFilter.value
    return matchesSearch && matchesGenre
  })
})

const clearBookFilter = () => {
  bookGenreFilter.value = ''
  showBookFilter.value = false
}

// filters all books by the search term to allow for easy assignment
const filteredAssignBooks = computed(() => {
  if (!assignBookSearch.value) return books.value
  return books.value.filter((b) =>
    b.title.toLowerCase().includes(assignBookSearch.value.toLowerCase()),
  )
})

// calculates average rating from the average rating of each assigned book
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

// Unassigns a book from an author
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

// when opening update modal, pre-populate with existing values
watch(showUpdateAuthor, (val) => {
  if (val && selectedAuthor.value) {
    updateAuthorLanguages.value = [...selectedAuthor.value.languages]
    updateAuthorGenres.value = [...selectedAuthor.value.genres] as string[]
  }
})

const addLanguageToUpdate = () => {
  if (
    updateAuthorLanguage.value &&
    !updateAuthorLanguages.value.includes(updateAuthorLanguage.value)
  ) {
    updateAuthorLanguages.value.push(updateAuthorLanguage.value)
    updateAuthorLanguage.value = ''
  }
}

const removeLanguage = (lang: string) => {
  updateAuthorLanguages.value = updateAuthorLanguages.value.filter((l) => l !== lang)
}

const addGenreToUpdate = () => {
  if (updateAuthorGenre.value && !updateAuthorGenres.value.includes(updateAuthorGenre.value)) {
    updateAuthorGenres.value.push(updateAuthorGenre.value)
    updateAuthorGenre.value = ''
  }
}

const removeGenre = (genre: string) => {
  updateAuthorGenres.value = updateAuthorGenres.value.filter((g) => g !== genre)
}

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

onMounted(async () => {
  authors.value = await getAuthors()
  books.value = await getBooks()
})
</script>

<style scoped>
.authors-view {
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

.authors-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

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

.author-footer {
  display: flex;
  justify-content: space-between;
  color: var(--color-secondary);
  font-size: 0.85rem;
  margin-top: auto;
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

.modal input:focus,
.modal select:focus,
.modal textarea:focus {
  border-color: var(--color-primary-mid);
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

.modal-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
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

.detail-meta {
  color: var(--color-secondary);
  font-size: 0.9rem;
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

.search-bar {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  align-items: center;
}

.search-bar .search-input {
  flex: 1;
  margin-bottom: 0;
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

.book-list-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 1rem;
  background: #f0f7ff;
  border-radius: var(--border-radius-md);
  margin-bottom: 0.5rem;
}

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

.book-list-title {
  color: var(--color-primary);
  font-weight: 500;
  flex: 1;
  margin-right: 0.5rem;
}

.genre-badge {
  background-color: var(--color-highlight);
  color: var(--color-primary);
  padding: 0.25rem 0.75rem;
  border-radius: var(--border-radius-full);
  font-size: 0.75rem;
  font-weight: 600;
}

.no-results {
  text-align: center;
  color: var(--color-secondary);
  padding: 1rem;
}

.modal-books {
  max-width: 500px;
}

.modal-assign {
  max-width: 400px;
}

.tag-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

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

.tag button {
  background: none;
  border: none;
  cursor: pointer;
  color: var(--color-primary);
  font-size: 0.75rem;
  padding: 0;
  line-height: 1;
}

.genre-select {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  align-items: center;
}

.genre-select select {
  flex: 1;
  margin-bottom: 0;
}

.genre-select input,
.genre-select select {
  flex: 1;
  margin-bottom: 0;
  width: auto;
}

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

@media (max-width: 768px) {
  .authors-grid {
    grid-template-columns: 1fr;
  }

  .header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .modal-actions {
    flex-direction: column;
  }
}
</style>
