<template>
  <div>
    <h1>Users</h1>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getUsers } from '../services/userService.ts'
import type { User } from '../types/user.ts'

const users = ref<User[]>([])

onMounted(async () => {
  users.value = await getUsers()
})
</script>
