<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { invoiceService } from '@/services/invoice.service'
import type { Invoice, PaginationMeta } from '@/types/api'

const auth = useAuthStore()

const invoices = ref<Invoice[]>([])
const pagination = ref<PaginationMeta | null>(null)
const loading = ref(true)
const paying = ref<string | null>(null) // invoiceId đang thanh toán
const toast = ref<{ message: string; type: 'success' | 'error' } | null>(null)
const currentPage = ref(1)
const filterStatus = ref<string>('all')

// Computed: filter invoices theo status
const filteredInvoices = computed(() => {
  if (filterStatus.value === 'all') return invoices.value
  return invoices.value.filter(i => i.status === filterStatus.value)
})

// Computed: Stats tổng hợp
const stats = computed(() => {
  const all = invoices.value
  return {
    total: all.length,
    paid: all.filter(i => i.status === 'Paid').length,
    pending: all.filter(i => i.status === 'Pending').length,
    overdue: all.filter(i => i.status === 'Overdue').length,
    totalAmount: all.reduce((sum, i) => sum + i.amount, 0),
    paidAmount: all.filter(i => i.status === 'Paid').reduce((sum, i) => sum + i.amount, 0)
  }
})

onMounted(() => {
  fetchInvoices()
})

async function fetchInvoices() {
  loading.value = true
  try {
    const res = await invoiceService.getInvoices(currentPage.value, 20)
    if (res.success && res.data) {
      invoices.value = res.data
      pagination.value = res.pagination || null
    }
  } catch (err: any) {
    showToast(err?.message || 'Lỗi khi tải danh sách hóa đơn', 'error')
  } finally {
    loading.value = false
  }
}

async function handlePay(invoiceId: string) {
  paying.value = invoiceId
  try {
    const res = await invoiceService.payInvoice(invoiceId)
    if (res.success) {
      showToast('🎉 Thanh toán thành công!', 'success')
      // Cập nhật lại trạng thái invoice trong list (không cần gọi lại API)
      const inv = invoices.value.find(i => i.invoiceId === invoiceId)
      if (inv) {
        inv.status = 'Paid'
        inv.paidAt = new Date().toISOString()
      }
    } else {
      showToast(res.message || 'Thanh toán thất bại', 'error')
    }
  } catch (err: any) {
    showToast(err?.message || 'Lỗi khi thanh toán', 'error')
  } finally {
    paying.value = null
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
    day: '2-digit', month: '2-digit', year: 'numeric'
  })
}

function goToPage(page: number) {
  currentPage.value = page
  fetchInvoices()
}
</script>

<template>
  <div class="invoices-page container">
    <!-- Toast Notification -->
    <Transition name="toast">
      <div v-if="toast" class="toast" :class="'toast-' + toast.type">
        {{ toast.message }}
      </div>
    </Transition>

    <!-- Page Header -->
    <div class="page-header fade-in">
      <div>
        <h1 class="page-title">
          {{ auth.isAdmin ? '📊 Quản lý Hóa đơn' : '📄 Hóa đơn của tôi' }}
        </h1>
        <p class="page-subtitle">
          {{ auth.isAdmin ? 'Xem và quản lý tất cả hóa đơn trong hệ thống' : 'Theo dõi và thanh toán các hóa đơn học phí' }}
        </p>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="stats-grid fade-in" v-if="!loading">
      <div class="stat-card">
        <div class="stat-icon stat-icon--total">📋</div>
        <div class="stat-body">
          <span class="stat-label">Tổng hóa đơn</span>
          <span class="stat-value">{{ stats.total }}</span>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon stat-icon--paid">✅</div>
        <div class="stat-body">
          <span class="stat-label">Đã thanh toán</span>
          <span class="stat-value stat-value--success">{{ stats.paid }}</span>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon stat-icon--pending">⏳</div>
        <div class="stat-body">
          <span class="stat-label">Chờ thanh toán</span>
          <span class="stat-value stat-value--warning">{{ stats.pending }}</span>
        </div>
      </div>
      <div class="stat-card">
        <div class="stat-icon stat-icon--amount">💰</div>
        <div class="stat-body">
          <span class="stat-label">Tổng tiền đã thu</span>
          <span class="stat-value stat-value--primary">{{ formatCurrency(stats.paidAmount) }}</span>
        </div>
      </div>
    </div>

    <!-- Filter Tabs -->
    <div class="filter-bar card fade-in" v-if="!loading">
      <div class="filter-tabs">
        <button
          class="filter-tab"
          :class="{ 'filter-tab--active': filterStatus === 'all' }"
          @click="filterStatus = 'all'"
        >Tất cả ({{ stats.total }})</button>
        <button
          class="filter-tab"
          :class="{ 'filter-tab--active': filterStatus === 'Pending' }"
          @click="filterStatus = 'Pending'"
        >⏳ Chờ TT ({{ stats.pending }})</button>
        <button
          class="filter-tab"
          :class="{ 'filter-tab--active': filterStatus === 'Paid' }"
          @click="filterStatus = 'Paid'"
        >✅ Đã TT ({{ stats.paid }})</button>
        <button
          class="filter-tab"
          :class="{ 'filter-tab--active': filterStatus === 'Overdue' }"
          @click="filterStatus = 'Overdue'"
        >🔴 Quá hạn ({{ stats.overdue }})</button>
      </div>
    </div>

    <!-- Loading Skeleton -->
    <div v-if="loading" class="skeleton-table card">
      <div v-for="i in 5" :key="i" class="skeleton-row">
        <div class="skeleton" style="width: 60%; height: 16px;"></div>
        <div class="skeleton" style="width: 30%; height: 16px;"></div>
      </div>
    </div>

    <!-- Invoice Table -->
    <div class="card table-card fade-in" v-else-if="filteredInvoices.length > 0">
      <div class="table-scroll">
        <table class="data-table">
          <thead>
            <tr>
              <th>Khóa học</th>
              <th v-if="auth.isAdmin">Học viên</th>
              <th>Số tiền</th>
              <th>Trạng thái</th>
              <th>Ngày tạo</th>
              <th>Ngày TT</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(invoice, index) in filteredInvoices" :key="invoice.invoiceId" :style="{ animationDelay: index * 0.05 + 's' }" class="table-row-animated">
              <td>
                <div class="course-info">
                  <span class="course-name">{{ invoice.courseName }}</span>
                  <span class="course-id">{{ invoice.courseId || 'N/A' }}</span>
                </div>
              </td>
              <td v-if="auth.isAdmin">
                <span class="student-name">{{ invoice.studentName }}</span>
              </td>
              <td class="amount">{{ formatCurrency(invoice.amount) }}</td>
              <td>
                <span class="badge" :class="{
                  'badge-paid': invoice.status === 'Paid',
                  'badge-pending': invoice.status === 'Pending',
                  'badge-overdue': invoice.status === 'Overdue'
                }">
                  <span class="badge-dot"></span>
                  {{ invoice.status === 'Paid' ? 'Đã TT' : invoice.status === 'Pending' ? 'Chờ TT' : 'Quá hạn' }}
                </span>
              </td>
              <td>{{ formatDate(invoice.createdAt) }}</td>
              <td>{{ invoice.paidAt ? formatDate(invoice.paidAt) : '—' }}</td>
              <td>
                <button
                  v-if="invoice.status !== 'Paid'"
                  class="btn btn-success btn-sm"
                  :disabled="paying === invoice.invoiceId"
                  @click="handlePay(invoice.invoiceId)"
                >
                  <span v-if="paying === invoice.invoiceId" class="spinner-sm"></span>
                  <span v-else>💳 Thanh toán</span>
                </button>
                <span v-else class="paid-check">✓</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="empty-state card fade-in">
      <div class="empty-icon">📭</div>
      <h3>Không có hóa đơn nào</h3>
      <p>{{ filterStatus !== 'all' ? 'Không tìm thấy hóa đơn với trạng thái này.' : 'Bạn chưa có hóa đơn nào trong hệ thống.' }}</p>
    </div>

    <!-- Pagination -->
    <div v-if="pagination && pagination.totalPages > 1" class="pagination fade-in">
      <button class="btn btn-ghost btn-sm" :disabled="!pagination.hasPreviousPage" @click="goToPage(currentPage - 1)">
        ← Trước
      </button>
      <span class="pagination-info">
        Trang {{ pagination.pageIndex }} / {{ pagination.totalPages }}
        <span class="pagination-total">({{ pagination.totalCount }} kết quả)</span>
      </span>
      <button class="btn btn-ghost btn-sm" :disabled="!pagination.hasNextPage" @click="goToPage(currentPage + 1)">
        Sau →
      </button>
    </div>
  </div>
</template>

<style scoped>
/* === Page Header === */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 28px;
}

.page-title {
  font-size: 1.75rem;
  font-weight: 800;
  color: var(--text-primary);
  margin-bottom: 4px;
}

.page-subtitle {
  color: var(--text-muted);
  font-size: 0.95rem;
}

/* === Stats Grid === */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 24px;
}

.stat-card {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: var(--radius);
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  transition: var(--transition);
}

.stat-card:hover {
  border-color: var(--border-light);
  transform: translateY(-2px);
  box-shadow: var(--shadow);
}

.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-sm);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
}

.stat-icon--total { background: var(--primary-bg); }
.stat-icon--paid { background: var(--success-bg); }
.stat-icon--pending { background: var(--warning-bg); }
.stat-icon--amount { background: rgba(168, 85, 247, 0.1); }

.stat-body {
  display: flex;
  flex-direction: column;
}

.stat-label {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 800;
  color: var(--text-primary);
  line-height: 1.2;
  margin-top: 2px;
}

.stat-value--success { color: var(--success); }
.stat-value--warning { color: var(--warning); }
.stat-value--primary { color: var(--primary-light); font-size: 1.15rem; }

/* === Filter Bar === */
.filter-bar {
  margin-bottom: 20px;
  padding: 8px;
}

.filter-tabs {
  display: flex;
  gap: 4px;
}

.filter-tab {
  padding: 8px 18px;
  background: transparent;
  border: none;
  border-radius: var(--radius-sm);
  color: var(--text-secondary);
  font-family: inherit;
  font-size: 0.85rem;
  font-weight: 500;
  cursor: pointer;
  transition: var(--transition);
}

.filter-tab:hover {
  background: var(--bg-card-hover);
  color: var(--text-primary);
}

.filter-tab--active {
  background: var(--primary-bg);
  color: var(--primary-light);
  font-weight: 600;
}

/* === Table Card === */
.table-card {
  padding: 0;
  overflow: hidden;
}

.table-scroll {
  overflow-x: auto;
}

.course-info {
  display: flex;
  flex-direction: column;
}

.course-name {
  font-weight: 600;
  color: var(--text-primary);
  font-size: 0.9rem;
}

.course-id {
  font-size: 0.75rem;
  color: var(--text-muted);
  margin-top: 2px;
}

.student-name {
  font-weight: 500;
  color: var(--text-primary);
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

.paid-check {
  color: var(--success);
  font-weight: 700;
  font-size: 1.1rem;
}

.table-row-animated {
  animation: fadeIn 0.3s ease-out both;
}

.spinner-sm {
  width: 14px;
  height: 14px;
  border: 2px solid rgba(255,255,255,0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
  display: inline-block;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* === Skeleton === */
.skeleton-table {
  padding: 24px;
}

.skeleton-row {
  display: flex;
  gap: 24px;
  padding: 16px 0;
  border-bottom: 1px solid var(--border);
}

/* === Empty State === */
.empty-state {
  text-align: center;
  padding: 64px 24px;
}

.empty-icon {
  font-size: 3rem;
  margin-bottom: 16px;
}

.empty-state h3 {
  color: var(--text-primary);
  margin-bottom: 8px;
}

.empty-state p {
  color: var(--text-muted);
  font-size: 0.9rem;
}

/* === Pagination === */
.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  margin-top: 24px;
}

.pagination-info {
  font-size: 0.875rem;
  color: var(--text-secondary);
}

.pagination-total {
  color: var(--text-muted);
}

/* === Toast Animation === */
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}
.toast-enter-from { opacity: 0; transform: translateY(-20px); }
.toast-leave-to { opacity: 0; transform: translateX(60px); }

/* === Responsive === */
@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  .filter-tabs {
    flex-wrap: wrap;
  }
  .page-title {
    font-size: 1.35rem;
  }
}
</style>
