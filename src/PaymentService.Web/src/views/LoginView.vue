<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

const email = ref('')
const password = ref('')
const loading = ref(false)
const errorMsg = ref('')

async function handleLogin() {
  errorMsg.value = ''
  loading.value = true
  try {
    const res = await auth.login(email.value, password.value)
    if (res.success) {
      router.push('/')
    } else {
      errorMsg.value = res.message || 'Đăng nhập thất bại'
    }
  } catch (err: any) {
    errorMsg.value = err?.message || 'Đã xảy ra lỗi khi đăng nhập'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-page">
    <div class="login-container">
      <!-- Left: Branding -->
      <div class="login-hero">
        <div class="hero-content">
          <div class="hero-icon">💳</div>
          <h1 class="hero-title">Payment Service</h1>
          <p class="hero-subtitle">Hệ thống Thanh toán & Báo cáo</p>
          <div class="hero-divider"></div>
          <div class="hero-info">
            <p>📚 Quản lý khóa học & học viên trung tâm</p>
            <p>👥 Nhóm 9 — Bài tập lớn Fullstack</p>
          </div>
          <div class="hero-members">
            <div class="member">Nguyễn Đình Minh Hiếu</div>
            <div class="member">Nguyễn Công Hiệp</div>
            <div class="member">Bùi Thế Đạt</div>
          </div>
        </div>
        <div class="hero-glow"></div>
      </div>

      <!-- Right: Login Form -->
      <div class="login-form-wrapper">
        <div class="login-form-inner">
          <h2 class="form-title">Đăng nhập</h2>
          <p class="form-subtitle">Nhập tài khoản để truy cập hệ thống</p>

          <form @submit.prevent="handleLogin" class="login-form">
            <div class="form-group">
              <label class="form-label">Email</label>
              <input
                v-model="email"
                type="email"
                class="form-input"
                placeholder="admin@trungtam.com"
                required
                autocomplete="email"
              />
            </div>

            <div class="form-group">
              <label class="form-label">Mật khẩu</label>
              <input
                v-model="password"
                type="password"
                class="form-input"
                placeholder="••••••••"
                required
                autocomplete="current-password"
              />
            </div>

            <!-- Error message -->
            <div v-if="errorMsg" class="error-box fade-in">
              ⚠️ {{ errorMsg }}
            </div>

            <button type="submit" class="btn btn-primary btn-login" :disabled="loading">
              <span v-if="loading" class="spinner"></span>
              <span v-else>Đăng nhập</span>
            </button>
          </form>

          <!-- Demo accounts -->
          <div class="demo-accounts">
            <p class="demo-title">🔑 Tài khoản demo</p>
            <div class="demo-grid">
              <div class="demo-item" @click="email = 'admin@trungtam.com'; password = 'Admin@123'">
                <span class="demo-role badge badge-paid">Admin</span>
                <span class="demo-email">admin@trungtam.com</span>
              </div>
              <div class="demo-item" @click="email = 'hieu@student.com'; password = 'Hieu@123'">
                <span class="demo-role badge badge-pending">Student</span>
                <span class="demo-email">hieu@student.com</span>
              </div>
              <div class="demo-item" @click="email = 'hiep@student.com'; password = 'Hiep@123'">
                <span class="demo-role badge badge-pending">Student</span>
                <span class="demo-email">hiep@student.com</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}

.login-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  max-width: 960px;
  width: 100%;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: var(--radius-lg);
  overflow: hidden;
  box-shadow: var(--shadow-lg);
  animation: fadeIn 0.5s ease-out;
}

/* === Hero (Left Side) === */
.login-hero {
  background: linear-gradient(135deg, var(--primary-dark), #7c3aed, var(--primary));
  padding: 48px 40px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  position: relative;
  overflow: hidden;
}

.hero-glow {
  position: absolute;
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, transparent 70%);
  top: -100px;
  right: -100px;
  border-radius: 50%;
}

.hero-content {
  position: relative;
  z-index: 1;
}

.hero-icon {
  font-size: 3rem;
  margin-bottom: 16px;
}

.hero-title {
  font-size: 2rem;
  font-weight: 800;
  color: white;
  margin-bottom: 8px;
  line-height: 1.1;
}

.hero-subtitle {
  color: rgba(255, 255, 255, 0.8);
  font-size: 1rem;
  font-weight: 400;
}

.hero-divider {
  width: 48px;
  height: 3px;
  background: rgba(255, 255, 255, 0.4);
  border-radius: 2px;
  margin: 24px 0;
}

.hero-info p {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.875rem;
  margin-bottom: 6px;
}

.hero-members {
  margin-top: 24px;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.member {
  padding: 8px 14px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-sm);
  color: white;
  font-size: 0.85rem;
  font-weight: 500;
  backdrop-filter: blur(4px);
}

/* === Form (Right Side) === */
.login-form-wrapper {
  padding: 48px 40px;
  display: flex;
  align-items: center;
}

.login-form-inner {
  width: 100%;
}

.form-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 4px;
}

.form-subtitle {
  color: var(--text-muted);
  font-size: 0.9rem;
  margin-bottom: 32px;
}

.login-form .form-input {
  padding: 14px 16px;
}

.btn-login {
  width: 100%;
  padding: 14px;
  font-size: 1rem;
  margin-top: 8px;
}

.error-box {
  padding: 12px 16px;
  background: var(--danger-bg);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: var(--radius-sm);
  color: var(--danger);
  font-size: 0.875rem;
  margin-bottom: 16px;
}

.spinner {
  width: 18px;
  height: 18px;
  border: 2px solid rgba(255,255,255,0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* === Demo Accounts === */
.demo-accounts {
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid var(--border);
}

.demo-title {
  font-size: 0.8rem;
  color: var(--text-muted);
  margin-bottom: 12px;
  font-weight: 600;
}

.demo-grid {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.demo-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 14px;
  background: var(--bg-input);
  border: 1px solid var(--border);
  border-radius: var(--radius-sm);
  cursor: pointer;
  transition: var(--transition);
}

.demo-item:hover {
  border-color: var(--primary);
  background: var(--primary-bg);
}

.demo-email {
  font-size: 0.85rem;
  color: var(--text-secondary);
  font-family: 'Inter', monospace;
}

/* === Responsive === */
@media (max-width: 768px) {
  .login-container {
    grid-template-columns: 1fr;
  }
  .login-hero {
    padding: 32px 24px;
  }
  .login-form-wrapper {
    padding: 32px 24px;
  }
}
</style>
