<template>
  <div class="users-view">
    <div class="header">
      <div class="header-left">
        <h1>Users</h1>
        <button class="btn-filter" @click="showFilter = true">
          <i class="fa-solid fa-filter"></i>
        </button>
      </div>
      <button class="btn-primary" @click="showAddUser = true">+ Add User</button>
    </div>

    <div class="users-grid" v-if="filteredUsers.length > 0">
      <div class="user-card" v-for="user in filteredUsers" :key="user.id" @click="selectUser(user)">
        <div class="user-card-header">
          <span class="privacy-badge">Privacy Level {{ user.privacyLevel }}</span>
          <span class="date-badge"
            >Joined {{ new Date(user.createdDate).toLocaleDateString() }}</span
          >
        </div>
        <h2 class="user-username">{{ user.userName }}</h2>
        <p class="user-email">📧 {{ user.email }}</p>
        <div class="user-footer">
          <span class="user-languages">🌍 {{ user.languages.join(', ') || 'No languages' }}</span>
          <span class="user-genres">🎭 {{ user.favouriteGenres.join(', ') || 'No genres' }}</span>
        </div>
      </div>
    </div>

    <div class="empty-state" v-else>
      <p>
        {{
          users.length === 0
            ? 'No users yet — add one to get started!'
            : 'No users match your search or filters.'
        }}
      </p>
    </div>
  </div>

  <!-- Add User Modal -->
  <div class="modal-overlay" v-if="showAddUser" @click.self="showAddUser = false">
    <div class="modal">
      <h2>Add a User</h2>
      <form @submit.prevent="submitUser">
        <input v-model="newUser.userName" placeholder="Username" required />
        <input v-model="newUser.firstName" placeholder="First Name" required />
        <input v-model="newUser.lastName" placeholder="Last Name" required />
        <input v-model="newUser.email" placeholder="Email" type="email" required />
        <input v-model="newUser.password" placeholder="Password" required />
        <input v-model="newUser.dateOfBirth" type="date" required />
        <select v-model="newUser.privacyLevel">
          <option
            v-for="key in Object.keys(PrivacyLevel).filter((k) => isNaN(Number(k)))"
            :key="key"
            :value="key"
          >
            {{ key }}
          </option>
        </select>
        <div class="modal-buttons">
          <button type="submit" :disabled="isSubmitting">
            {{ isSubmitting ? 'Adding...' : 'Add User' }}
          </button>
          <button type="button" @click="showAddUser = false">Cancel</button>
        </div>
      </form>
    </div>
  </div>

  <!-- User Detail Modal -->
  <div class="modal-overlay" v-if="selectedUser" @click.self="selectedUser = null">
    <div class="modal modal-detail">
      <div class="modal-detail-header">
        <h2>{{ selectedUser.firstName }} {{ selectedUser.lastName }}</h2>
        <button class="btn-close" @click="selectedUser = null">✕</button>
      </div>
      <p class="detail-meta">
        📧 {{ selectedUser.email }} · 🎂
        {{ new Date(selectedUser.dateOfBirth).toLocaleDateString() }} · 🔒
        {{ selectedUser.privacyLevel }}
      </p>
      <p class="detail-meta">🌍 {{ selectedUser.languages.join(', ') || 'No languages' }}</p>
      <p class="detail-meta">🎭 {{ selectedUser.favouriteGenres.join(', ') || 'No genres' }}</p>
      <div class="detail-actions">
        <button class="btn-secondary" @click="showUpdateUser = true">✏️ Update User</button>
        <button class="btn-delete-user" @click="handleDeleteUser(selectedUser!.id!)">
          🗑 Delete User
        </button>
      </div>
    </div>
  </div>

  <!-- Update User Modal -->
  <div class="modal-overlay" v-if="showUpdateUser" @click.self="showUpdateUser = false">
    <div class="modal">
      <div class="modal-detail-header">
        <h2>Update User</h2>
        <button class="btn-close" @click="showUpdateUser = false">✕</button>
      </div>
      <form @submit.prevent="submitUpdateUser">
        <input v-model="updateUserData.userName" placeholder="Username" required />
        <input v-model="updateUserData.firstName" placeholder="First Name" required />
        <input v-model="updateUserData.lastName" placeholder="Last Name" required />
        <input v-model="updateUserData.email" placeholder="Email" type="email" required />
        <input v-model="updateUserData.password" placeholder="Password" required />
        <select v-model="updateUserData.privacyLevel">
          <option
            v-for="key in Object.keys(PrivacyLevel).filter((k) => isNaN(Number(k)))"
            :key="key"
            :value="key"
          >
            {{ key }}
          </option>
        </select>
        <div class="genre-select">
          <input v-model="updateLanguage" placeholder="Add a language" />
          <button type="button" class="btn-secondary" @click="addLanguage">+ Add Language</button>
        </div>
        <div class="tag-list">
          <span class="tag" v-for="lang in updateUserData.languages" :key="lang">
            {{ lang }}
            <button type="button" @click="removeLanguage(lang)">✕</button>
          </span>
        </div>

        <div class="genre-select">
          <select v-model="updateGenre">
            <option
              v-for="key in Object.keys(Genre).filter((k) => isNaN(Number(k)))"
              :key="key"
              :value="key"
            >
              {{ key }}
            </option>
          </select>
          <button type="button" class="btn-secondary" @click="addGenre">+ Add Genre</button>
        </div>
        <div class="tag-list">
          <span class="tag" v-for="genre in updateUserData.favouriteGenres" :key="genre">
            {{ genre }}
            <button type="button" @click="removeGenre(genre)">✕</button>
          </span>
        </div>
        <div class="modal-buttons">
          <button type="submit" :disabled="isUpdating">
            {{ isUpdating ? 'Updating...' : 'Update' }}
          </button>
          <button type="button" @click="showUpdateUser = false">Cancel</button>
        </div>
      </form>
    </div>
  </div>

  <!-- Filter Modal -->
  <div class="modal-overlay" v-if="showFilter" @click.self="showFilter = false">
    <div class="modal modal-filter">
      <div class="modal-detail-header">
        <h2>Filter Users</h2>
        <button class="btn-close" @click="showFilter = false">✕</button>
      </div>
      <label class="filter-label">Privacy Level</label>
      <select v-model="privacyFilter">
        <option value="">All</option>
        <option
          v-for="key in Object.keys(PrivacyLevel).filter((k) => isNaN(Number(k)))"
          :key="key"
          :value="key"
        >
          {{ key }}
        </option>
      </select>
      <label class="filter-label">Favourite Genre</label>
      <select v-model="genreFilter">
        <option value="">All</option>
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
import { onMounted, ref, computed, watch } from 'vue'
import { getUsers, createUser, updateUser, deleteUser } from '../services/userService.ts'
import { useSearchStore } from '../stores/searchStore.ts'
import type { User } from '../types/user.ts'
import { PrivacyLevel, Genre } from '../types/enums.ts'

const updateGenre = ref('')
const genreFilter = ref('')
const searchStore = useSearchStore()
const users = ref<User[]>([])
const selectedUser = ref<User | null>(null)
const showAddUser = ref(false)
const showUpdateUser = ref(false)
const showFilter = ref(false)
const isSubmitting = ref(false)
const isUpdating = ref(false)
const privacyFilter = ref('')
const updateLanguage = ref('')

const newUser = ref({
  userName: '',
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  dateOfBirth: '',
  privacyLevel: 'Low',
  languages: [] as string[],
  favouriteGenres: [] as string[],
})

const updateUserData = ref({
  userName: '',
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  privacyLevel: '',
  languages: [] as string[],
  favouriteGenres: [] as string[],
})

const filteredUsers = computed(() => {
  return users.value.filter((u) => {
    const matchesSearch = u.userName.toLowerCase().includes(searchStore.searchTerm.toLowerCase())
    const matchesPrivacy = !privacyFilter.value || String(u.privacyLevel) === privacyFilter.value
    const matchesGenre = !genreFilter.value || u.favouriteGenres.includes(genreFilter.value)
    return matchesSearch && matchesPrivacy && matchesGenre
  })
})

const clearFilters = () => {
  privacyFilter.value = ''
  genreFilter.value = ''
}

const selectUser = (user: User) => {
  selectedUser.value = user
}

watch(showUpdateUser, (val) => {
  if (val && selectedUser.value) {
    updateUserData.value = {
      userName: selectedUser.value.userName,
      firstName: selectedUser.value.firstName,
      lastName: selectedUser.value.lastName,
      email: selectedUser.value.email,
      password: selectedUser.value.password,
      privacyLevel: String(selectedUser.value.privacyLevel),
      languages: [...selectedUser.value.languages],
      favouriteGenres: [...selectedUser.value.favouriteGenres],
    }
  }
})

const addLanguage = () => {
  if (updateLanguage.value && !updateUserData.value.languages.includes(updateLanguage.value)) {
    updateUserData.value.languages.push(updateLanguage.value)
    updateLanguage.value = ''
  }
}

const removeLanguage = (lang: string) => {
  updateUserData.value.languages = updateUserData.value.languages.filter((l) => l !== lang)
}

const submitUser = async () => {
  if (isSubmitting.value) return
  isSubmitting.value = true
  try {
    await createUser({ ...newUser.value } as unknown as User)
    users.value = await getUsers()
    showAddUser.value = false
    newUser.value = {
      userName: '',
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      dateOfBirth: '',
      privacyLevel: 'Low',
      languages: [],
      favouriteGenres: [],
    }
  } catch (error) {
    console.error('Error adding user:', error)
  } finally {
    isSubmitting.value = false
  }
}

const submitUpdateUser = async () => {
  if (isUpdating.value || !selectedUser.value) return
  isUpdating.value = true
  try {
    await updateUser(selectedUser.value.id!, { ...updateUserData.value } as unknown as User)
    users.value = await getUsers()
    selectedUser.value = users.value.find((u) => u.id === selectedUser.value!.id) ?? null
    showUpdateUser.value = false
  } catch (error) {
    console.error('Error updating user:', error)
  } finally {
    isUpdating.value = false
  }
}

const handleDeleteUser = async (id: number) => {
  if (!confirm('Are you sure you want to delete this user?')) return
  try {
    await deleteUser(id)
    users.value = await getUsers()
    selectedUser.value = null
  } catch (error) {
    console.error('Error deleting user:', error)
  }
}

const addGenre = () => {
  if (updateGenre.value && !updateUserData.value.favouriteGenres.includes(updateGenre.value)) {
    updateUserData.value.favouriteGenres.push(updateGenre.value)
    updateGenre.value = ''
  }
}

const removeGenre = (genre: string) => {
  updateUserData.value.favouriteGenres = updateUserData.value.favouriteGenres.filter(
    (g) => g !== genre,
  )
}

onMounted(async () => {
  users.value = await getUsers()
})
</script>

<style scoped>
.users-view {
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

.users-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.user-card {
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

.user-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.user-card-header {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.privacy-badge {
  background-color: var(--color-highlight);
  color: var(--color-primary);
  padding: 0.25rem 0.75rem;
  border-radius: var(--border-radius-full);
  font-size: 0.75rem;
  font-weight: 600;
}

.date-badge {
  background-color: var(--color-accent);
  color: var(--color-primary);
  padding: 0.25rem 0.75rem;
  border-radius: var(--border-radius-full);
  font-size: 0.75rem;
  font-weight: 600;
}

.user-username {
  color: var(--color-primary);
  font-size: 1.25rem;
  margin: 0 0 0.5rem 0;
}

.user-email {
  color: var(--color-secondary);
  font-size: 0.9rem;
  margin: 0 0 1rem 0;
}

.user-footer {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
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
.modal select:focus {
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

.btn-delete-user {
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

.btn-delete-user:hover {
  background-color: #cc0000;
  transform: translateY(-1px);
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

.detail-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
  margin-top: 1rem;
}

.genre-select {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.genre-select input {
  flex: 1;
  margin-bottom: 0;
  width: auto;
}

.genre-select select {
  flex: 1;
  margin-bottom: 0;
  width: auto;
}

.tag-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 0.5rem;
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
  .users-grid {
    grid-template-columns: 1fr;
  }

  .header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .detail-actions {
    flex-direction: column;
  }
}
</style>
