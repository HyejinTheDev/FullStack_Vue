import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresGuest: true }
    },
    {
      path: '/',
      name: 'invoices',
      component: () => import('@/views/InvoicesView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/invoices/:id',
      name: 'invoice-detail',
      component: () => import('@/views/InvoiceDetailView.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

// Navigation guard
router.beforeEach(async (to) => {
  const auth = useAuthStore()

  // Khôi phục session nếu có token
  if (auth.accessToken && !auth.user) {
    await auth.init()
  }

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { name: 'login' }
  }

  if (to.meta.requiresGuest && auth.isAuthenticated) {
    return { name: 'invoices' }
  }
})

export default router
