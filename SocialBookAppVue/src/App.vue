<template>
  <div class="app">
    <nav class="navbar">
      <div class="navbar-brand">
        <h1>📚 SocialBook</h1>
        <div class="navbar-search">
          <i class="fa-solid fa-magnifying-glass navbar-search-icon"></i>
          <input
            v-model="searchStore.searchTerm"
            placeholder="Search..."
            class="navbar-search-input"
          />
        </div>
      </div>
      <div class="navbar-links">
        <RouterLink to="/books" @click="searchStore.clearSearch">Books</RouterLink>
        <RouterLink to="/authors" @click="searchStore.clearSearch">Authors</RouterLink>
        <RouterLink to="/users" @click="searchStore.clearSearch">Users</RouterLink>
      </div>
    </nav>
    <main class="content">
      <RouterView />
    </main>
  </div>
</template>

<script setup lang="ts">
import { useSearchStore } from './stores/searchStore.ts'
const searchStore = useSearchStore()
</script>

<style scoped>
.app {
  min-height: 100vh;
  background-color: #f0f7ff;
}

.navbar {
  background-color: var(--color-primary);
  padding: 1rem 2rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-radius: 0 0 var(--border-radius-lg) var(--border-radius-lg);
  box-shadow: var(--shadow-md);
}

.navbar-brand {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.navbar-brand h1 {
  color: white;
  margin: 0;
  font-size: 1.5rem;
  white-space: nowrap;
}

.navbar-links {
  display: flex;
  gap: 1rem;
}

.navbar-links a {
  color: white;
  text-decoration: none;
  padding: 0.5rem 1.25rem;
  border-radius: var(--border-radius-full);
  transition: background-color 0.2s;
}

.navbar-links a:hover {
  background-color: var(--color-primary-mid);
}

.navbar-links a.router-link-active {
  background-color: var(--color-primary-light);
}

.content {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.navbar-search {
  position: relative;
  flex: 1;
  max-width: 400px;
}

.navbar-search-input {
  width: 100%;
  padding: 0.5rem 1rem 0.5rem 2.5rem;
  border-radius: var(--border-radius-full);
  border: none;
  font-size: 1rem;
  outline: none;
  box-sizing: border-box;
  background: rgba(255, 255, 255, 0.2);
  color: white;
}

.navbar-search-input::placeholder {
  color: rgba(255, 255, 255, 0.7);
}

.navbar-search-input:focus {
  background: rgba(255, 255, 255, 0.3);
}

.navbar-search-icon {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  color: rgba(255, 255, 255, 0.7);
}

@media (max-width: 768px) {
  .navbar {
    flex-direction: column;
    gap: 1rem;
    border-radius: 0 0 var(--border-radius-md) var(--border-radius-md);
  }

  .content {
    padding: 1rem;
  }
}
</style>
