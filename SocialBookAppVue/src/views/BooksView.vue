<template>
  <div>
    <h1>Books</h1>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getBooks } from '../services/bookService.ts'
import type { Book } from '../types/book.ts'

const books = ref<Book[]>([])

onMounted(async () => {
  books.value = await getBooks()
})
</script>
