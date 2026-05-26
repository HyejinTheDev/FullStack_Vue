import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

const apiClient = axios.create({
  baseURL: '/api',
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' }
})

// Request interceptor: gắn JWT Token
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken')
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Response interceptor: bóc tách data + auto refresh token
apiClient.interceptors.response.use(
  (response) => response.data,
  async (error) => {
    const originalRequest = error.config

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true
      try {
        const refreshToken = localStorage.getItem('refreshToken')
        if (refreshToken) {
          const res = await axios.post('/api/v1/auth/refresh-token', { refreshToken })
          if (res.data.success) {
            localStorage.setItem('accessToken', res.data.data.accessToken)
            localStorage.setItem('refreshToken', res.data.data.refreshToken)
            originalRequest.headers.Authorization = `Bearer ${res.data.data.accessToken}`
            return apiClient(originalRequest)
          }
        }
      } catch {
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        window.location.href = '/login'
      }
    }

    return Promise.reject(error.response?.data || error)
  }
)

export default apiClient
