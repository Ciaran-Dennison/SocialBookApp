<template>
  <div>
    <h1>Authors</h1>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getAuthors } from '../services/authorService.ts'
import type { Author } from '../types/author.ts'

const authors = ref<Author[]>([])

onMounted(async () => {
  authors.value = await getAuthors()
})
</script>
