import { createRouter, createWebHistory } from 'vue-router'
import UsersView from '../views/UsersView.vue'
import BooksView from '../views/BooksView.vue'
import AuthorsView from '../views/AuthorsView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/books',
    },
    {
      path: '/users',
      name: 'users',
      component: UsersView,
    },
    {
      path: '/books',
      name: 'books',
      component: BooksView,
    },
    {
      path: '/authors',
      name: 'authors',
      component: AuthorsView,
    },
  ],
})

export default router
