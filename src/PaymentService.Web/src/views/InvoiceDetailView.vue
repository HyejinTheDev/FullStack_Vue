<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { invoiceService } from '@/services/invoice.service'
import type { Invoice } from '@/types/api'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

const invoice = ref<Invoice | null>(null)
const loading = ref(true)
const paying = ref(false)
const toast = ref<{ message: string; type: 'success' | 'error' } | null>(null)

onMounted(async () => {
  const id = route.params.id as string
  try {
    const res = await invoiceService.getInvoiceById(id)
    if (res.success && res.data) {
      invoice.value = res.data
    }
  } catch (err: any) {
    showToast(err?.message || 'Không thể tải chi tiết hóa đơn', 'error')
  } finally {
    loading.value = false
  }
})

async function handlePay() {
  if (!invoice.value) return
  paying.value = true
  try {
    const res = await invoiceService.payInvoice(invoice.value.invoiceId)
    if (res.success) {
      showToast('🎉 Thanh toán thành công!', 'success')
      invoice.value.status = 'Paid'
      invoice.value.paidAt = new Date().toISOString()
    } else {
      showToast(res.message, 'error')
    }
  } catch (err: any) {
    showToast(err?.message || 'Lỗi khi thanh toán', 'error')
  } finally {
    paying.value = false
  }
}

function showToast(message: string, type: 'success' | 'error') {
  toast.value = { message, type }
  setTimeout(() => { toast.value = null }, 3000)
}

function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount)
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit'
  })
}
</script>

<template>
  <div class="detail-page container">
    <Transition name="toast">
      <div v-if="toast" class="toast" :class="'toast-' + toast.type">{{ toast.message }}</div>
    </Transition>

    <button class="btn btn-ghost btn-sm back-btn" @click="router.push('/')">← Quay lại</button>

    <div v-if="loading" class="card skeleton-detail">
      <div class="skeleton" style="width: 50%; height: 24px; margin-bottom: 16px;"></div>
      <div class="skeleton" style="width: 80%; height: 16px; margin-bottom: 12px;"></div>
      <div class="skeleton" style="width: 40%; height: 16px;"></div>
    </div>

    <div v-else-if="invoice" class="detail-card card fade-in">
      <div class="detail-header">
        <div>
          <h2 class="detail-title">{{ invoice.courseName }}</h2>
          <p class="detail-id">Mã hóa đơn: {{ invoice.invoiceId }}</p>
        </div>
        <span class="badge" :class="{
          'badge-paid': invoice.status === 'Paid',
          'badge-pending': invoice.status === 'Pending',
          'badge-overdue': invoice.status === 'Overdue'
        }">
          <span class="badge-dot"></span>
          {{ invoice.status === 'Paid' ? 'Đã thanh toán' : invoice.status === 'Pending' ? 'Chờ thanh toán' : 'Quá hạn' }}
        </span>
      </div>

      <div class="detail-grid">
        <div class="detail-item">
          <span class="detail-label">Học viên</span>
          <span class="detail-value">{{ invoice.studentName }}</span>
        </div>
        <div class="detail-item">
          <span class="detail-label">Mã khóa học</span>
          <span class="detail-value">{{ invoice.courseId || 'N/A' }}</span>
        </div>
        <div class="detail-item">
          <span class="detail-label">Số tiền</span>
          <span class="detail-value detail-value--amount">{{ formatCurrency(invoice.amount) }}</span>
        </div>
        <div class="detail-item">
          <span class="detail-label">Ngày tạo</span>
          <span class="detail-value">{{ formatDate(invoice.createdAt) }}</span>
        </div>
        <div class="detail-item" v-if="invoice.paidAt">
          <span class="detail-label">Ngày thanh toán</span>
          <span class="detail-value">{{ formatDate(invoice.paidAt) }}</span>
        </div>
      </div>

      <div class="detail-actions" v-if="invoice.status !== 'Paid'">
        <button class="btn btn-success" :disabled="paying" @click="handlePay">
          <span v-if="paying">Đang xử lý...</span>
          <span v-else>💳 Thanh toán ngay</span>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.detail-page {
  max-width: 720px;
}

.back-btn {
  margin-bottom: 20px;
}

.detail-card {
  padding: 32px;
}

.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 32px;
  padding-bottom: 24px;
  border-bottom: 1px solid var(--border);
}

.detail-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 4px;
}

.detail-id {
  font-size: 0.8rem;
  color: var(--text-muted);
  font-family: monospace;
}

.detail-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
  margin-bottom: 32px;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.detail-label {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.detail-value {
  font-size: 1rem;
  color: var(--text-primary);
  font-weight: 500;
}

.detail-value--amount {
  font-size: 1.25rem;
  font-weight: 800;
  color: var(--primary-light);
}

.detail-actions {
  padding-top: 24px;
  border-top: 1px solid var(--border);
}

.badge-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  display: inline-block;
}
.badge-paid .badge-dot { background: var(--success); }
.badge-pending .badge-dot { background: var(--warning); }
.badge-overdue .badge-dot { background: var(--danger); }

.skeleton-detail {
  padding: 32px;
}

.toast-enter-active, .toast-leave-active { transition: all 0.3s ease; }
.toast-enter-from { opacity: 0; transform: translateY(-20px); }
.toast-leave-to { opacity: 0; transform: translateX(60px); }
</style>
