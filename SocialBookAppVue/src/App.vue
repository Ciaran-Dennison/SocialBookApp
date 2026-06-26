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
// Imports the global search store so the navbar search input
// can update the search term shared across all views
import { useSearchStore } from './stores/searchStore.ts'
const searchStore = useSearchStore()
</script>

<style scoped>
/* --- App Shell --- */

/* Full height app wrapper with light blue background */
.app {
  min-height: 100vh;
  background-color: #f0f7ff;
}

/* --- Navbar --- */

/* Fixed navbar at the top with rounded bottom corners */
.navbar {
  background-color: var(--color-primary);
  padding: 1rem 2rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-radius: 0 0 var(--border-radius-lg) var(--border-radius-lg);
  box-shadow: var(--shadow-md);
}

/* Groups the app title and search bar together on the left */
.navbar-brand {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

/* App title - white-space: nowrap prevents it wrapping on smaller screens */
.navbar-brand h1 {
  color: white;
  margin: 0;
  font-size: 1.5rem;
  white-space: nowrap;
}

/* Navigation links grouped on the right */
.navbar-links {
  display: flex;
  gap: 1rem;
}

/* Nav link styling - pill shaped with hover effect */
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

/* Highlights the currently active route */
.navbar-links a.router-link-active {
  background-color: var(--color-primary-light);
}

/* --- Main Content --- */

/* Centres and constrains page content width */
.content {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

/* --- Navbar Search --- */

/* Search bar container - grows to fill space between brand and nav links */
.navbar-search {
  position: relative;
  flex: 1;
  max-width: 400px;
}

/* Transparent search input with left padding to make room for the icon */
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

/* Semi-transparent placeholder text */
.navbar-search-input::placeholder {
  color: rgba(255, 255, 255, 0.7);
}

/* Slightly more opaque background when focused */
.navbar-search-input:focus {
  background: rgba(255, 255, 255, 0.3);
}

/* Magnifying glass icon absolutely positioned inside the search input */
.navbar-search-icon {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  color: rgba(255, 255, 255, 0.7);
}

/* --- Responsive --- */

@media (max-width: 768px) {
  /* Stack navbar vertically on mobile with smaller border radius */
  .navbar {
    flex-direction: column;
    gap: 1rem;
    border-radius: 0 0 var(--border-radius-md) var(--border-radius-md);
  }

  /* Less padding on mobile */
  .content {
    padding: 1rem;
  }
}
</style>
