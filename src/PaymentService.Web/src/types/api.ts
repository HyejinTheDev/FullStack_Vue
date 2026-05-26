// === Standard API Response Wrapper ===
export interface ApiResponse<T> {
  success: boolean
  statusCode: number
  message: string
  data: T | null
  errors: ApiError[] | null
  pagination?: PaginationMeta
  timestamp: string
}

export interface ApiError {
  field: string
  message: string
}

export interface PaginationMeta {
  pageIndex: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

// === Auth ===
export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  expiresIn: number
}

export interface RegisterRequest {
  fullName: string
  email: string
  password: string
}

// === User ===
export interface UserProfile {
  userId: string
  fullName: string
  email: string
  role: 'Student' | 'Teacher' | 'Admin' | 'Staff'
  createdAt: string
}

// === Invoice ===
export interface Invoice {
  invoiceId: string
  userId: string
  studentName: string
  courseName: string
  courseId: string | null
  amount: number
  status: 'Pending' | 'Paid' | 'Overdue'
  createdAt: string
  paidAt: string | null
}

export interface PayInvoiceResponse {
  paymentUrl: string
  message: string
}

// === Report ===
export interface RevenueReport {
  totalRevenue: number
  totalInvoices: number
  paidInvoices: number
  pendingInvoices: number
  overdueInvoices: number
  monthlyRevenue: { month: string; revenue: number; invoiceCount: number }[]
  courseRevenue: { courseId: string; courseName: string; revenue: number; studentCount: number }[]
}
