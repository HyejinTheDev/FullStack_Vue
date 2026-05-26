import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { UserProfile } from '@/types/api'
import { authService } from '@/services/auth.service'
import apiClient from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<UserProfile | null>(null)
  const accessToken = ref<string | null>(localStorage.getItem('accessToken'))

  const isAuthenticated = computed(() => !!accessToken.value)
  const isAdmin = computed(() => user.value?.role === 'Admin')
  const userRole = computed(() => user.value?.role || '')

  async function login(email: string, password: string) {
    const res = await authService.login({ email, password })
    if (res.success && res.data) {
      accessToken.value = res.data.accessToken
      localStorage.setItem('accessToken', res.data.accessToken)
      localStorage.setItem('refreshToken', res.data.refreshToken)
      await fetchProfile()
    }
    return res
  }

  async function fetchProfile() {
    try {
      const res = await apiClient.get<any, any>('/v1/users/me')
      if (res.success && res.data) {
        user.value = res.data
      }
    } catch {
      logout()
    }
  }

  function logout() {
    user.value = null
    accessToken.value = null
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
  }

  // Khôi phục session nếu đã có token
  async function init() {
    if (accessToken.value) {
      await fetchProfile()
    }
  }

  return { user, accessToken, isAuthenticated, isAdmin, userRole, login, logout, fetchProfile, init }
})
