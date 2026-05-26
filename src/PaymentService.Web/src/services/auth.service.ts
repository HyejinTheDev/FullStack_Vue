import apiClient from './api'
import type { ApiResponse, LoginResponse, LoginRequest, RegisterRequest } from '@/types/api'

export const authService = {
  login(payload: LoginRequest) {
    return apiClient.post<any, ApiResponse<LoginResponse>>('/v1/auth/login', payload)
  },
  register(payload: RegisterRequest) {
    return apiClient.post<any, ApiResponse<any>>('/v1/auth/register', payload)
  },
  changePassword(payload: { oldPassword: string; newPassword: string }) {
    return apiClient.put<any, ApiResponse<any>>('/v1/auth/change-password', payload)
  }
}
