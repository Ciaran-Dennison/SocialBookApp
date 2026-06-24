<template>
  <div class="app">
    <nav class="navbar">
      <div class="navbar-brand">
        <h1>📚 SocialBook</h1>
      </div>
      <div class="navbar-links">
        <RouterLink to="/books">Books</RouterLink>
        <RouterLink to="/authors">Authors</RouterLink>
        <RouterLink to="/users">Users</RouterLink>
      </div>
    </nav>
    <main class="content">
      <RouterView />
    </main>
  </div>
</template>

<script setup lang="ts"></script>

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

.navbar-brand h1 {
  color: white;
  margin: 0;
  font-size: 1.5rem;
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
