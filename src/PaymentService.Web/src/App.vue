<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()

onMounted(async () => {
  await auth.init()
})

function handleLogout() {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <div class="app-layout">
    <!-- Navbar -->
    <nav class="navbar" v-if="auth.isAuthenticated">
      <div class="navbar-inner container">
        <div class="navbar-brand">
          <div class="logo-icon">💳</div>
          <div class="brand-text">
            <span class="brand-name">PaymentService</span>
            <span class="brand-tag">Nhóm 9</span>
          </div>
        </div>

        <div class="navbar-links">
          <router-link to="/" class="nav-link" active-class="nav-link--active">
            📄 Hóa đơn
          </router-link>
        </div>

        <div class="navbar-user">
          <div class="user-info">
            <span class="user-name">{{ auth.user?.fullName || '...' }}</span>
            <span class="user-role badge" :class="{
              'badge-paid': auth.userRole === 'Admin',
              'badge-pending': auth.userRole === 'Student'
            }">{{ auth.userRole }}</span>
          </div>
          <button class="btn btn-ghost btn-sm" @click="handleLogout">Đăng xuất</button>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="main-content">
      <router-view v-slot="{ Component }">
        <transition name="page" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>
  </div>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* === Navbar === */
.navbar {
  background: var(--bg-secondary);
  border-bottom: 1px solid var(--border);
  position: sticky;
  top: 0;
  z-index: 100;
  backdrop-filter: blur(12px);
}

.navbar-inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 64px;
}

.navbar-brand {
  display: flex;
  align-items: center;
  gap: 12px;
}

.logo-icon {
  font-size: 1.5rem;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--primary-bg);
  border-radius: var(--radius-sm);
}

.brand-text {
  display: flex;
  flex-direction: column;
}

.brand-name {
  font-weight: 700;
  font-size: 1rem;
  color: var(--text-primary);
  line-height: 1.2;
}

.brand-tag {
  font-size: 0.7rem;
  color: var(--text-muted);
  font-weight: 500;
}

.navbar-links {
  display: flex;
  gap: 4px;
}

.nav-link {
  padding: 8px 16px;
  border-radius: var(--radius-sm);
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--text-secondary);
  transition: var(--transition);
}

.nav-link:hover {
  background: var(--primary-bg);
  color: var(--primary-light);
}

.nav-link--active {
  background: var(--primary-bg);
  color: var(--primary-light) !important;
}

.navbar-user {
  display: flex;
  align-items: center;
  gap: 16px;
}

.user-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.user-name {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--text-primary);
  line-height: 1.2;
}

.user-role {
  margin-top: 2px;
}

/* === Main Content === */
.main-content {
  flex: 1;
  padding: 32px 0;
}

/* === Page Transition === */
.page-enter-active,
.page-leave-active {
  transition: all 0.25s ease;
}
.page-enter-from {
  opacity: 0;
  transform: translateY(8px);
}
.page-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
