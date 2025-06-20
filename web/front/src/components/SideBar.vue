<template>
  <div :class="['sidebar', isCollapsed ? 'collapsed' : '']">
    <div class="logo-section">
      <button @click="goToUserInfo" class="avatar-button">
        <img src="/user-avatar.png" alt="User Avatar" class="avatar" />
      </button>
    </div>
    <button @click="toggleSidebar" class="sidebar-toggle">
      <span v-if="!isCollapsed">≡</span>
      <span v-else>>-</span>
    </button>
    <div class="navigation">
      <div class="nav-item active">
        <span class="icon">🔄</span>
        <span class="text">新建会话</span>
      </div>
      <div class="nav-item">
        <span class="icon">🌐</span>
        <span class="text">AC+</span>
      </div>
      <div class="nav-item">
        <span class="icon">📱</span>
        <span class="text">医疗搜索</span>
      </div>
      <div class="nav-item">
        <span class="icon">📚</span>
        <span class="text">学术搜索</span>
      </div>
      <div class="nav-item">
        <span class="icon">💬</span>
        <span class="text">Kimi探索版</span>
      </div>
      <div class="nav-item">
        <span class="icon">📋</span>
        <span class="text">历史对话</span>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState, mapActions } from 'vuex';

export default {
  name: 'SideBar',
  computed: {
    ...mapState(['isSidebarCollapsed']),
    isCollapsed: {
      get() {
        return this.isSidebarCollapsed;
      },
      set(value) {
        this.setSidebarCollapsed(value);
      }
    }
  },
  methods: {
    ...mapActions(['toggleSidebar', 'setSidebarCollapsed']),
    goToUserInfo() {
      this.$emit('goToUserInfo');
    }
  }
}
</script>

<style scoped>
.sidebar {
  width: 250px;
  background-color: #1e1e1e;
  color: white;
  display: flex;
  flex-direction: column;
  transition: width 0.3s ease;
  overflow: hidden;
  border-right: 1px solid #333;
}

.collapsed {
  width: 60px;
}

.logo-section {
  padding: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-bottom: 1px solid #333;
}

.avatar-button {
  background: transparent;
  border: none;
  cursor: pointer;
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
}

.sidebar-toggle {
  width: 100%;
  padding: 10px;
  background: transparent;
  border: none;
  color: white;
  cursor: pointer;
  font-size: 18px;
  text-align: center;
}

.navigation {
  flex: 1;
  overflow-y: auto;
}

.nav-item {
  padding: 12px 15px;
  display: flex;
  align-items: center;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.nav-item:hover {
  background-color: #2a2a2a;
}

.nav-item.active {
  background-color: #2a2a2a;
}

.icon {
  font-size: 18px;
  margin-right: 10px;
  width: 20px;
  text-align: center;
}

.text {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  opacity: 1;
  transition: opacity 0.3s ease;
}

.collapsed .text {
  display: none;
}
</style>