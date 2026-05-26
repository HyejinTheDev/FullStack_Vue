import apiClient from './api'
import type { ApiResponse, Invoice, PayInvoiceResponse } from '@/types/api'

export const invoiceService = {
  getInvoices(pageIndex = 1, pageSize = 20) {
    return apiClient.get<any, ApiResponse<Invoice[]>>('/v1/invoices', {
      params: { pageIndex, pageSize }
    })
  },
  getInvoiceById(invoiceId: string) {
    return apiClient.get<any, ApiResponse<Invoice>>(`/v1/invoices/${invoiceId}`)
  },
  payInvoice(invoiceId: string) {
    return apiClient.post<any, ApiResponse<PayInvoiceResponse>>(`/v1/invoices/${invoiceId}/pay`)
  }
}
