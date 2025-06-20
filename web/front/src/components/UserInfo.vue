<template>
  <div class="user-info">
    <!-- 左侧栏 -->
    <SideBar @goToUserInfo="goToUserInfo" />
    <!-- 用户信息内容 -->
    <div class="user-content">
      <div class="card">
        <h2>用户信息</h2>
        <div class="user-details">
          <img :src="userAvatar" alt="User Avatar" class="avatar" />
          <div class="info">
            <p>用户名: {{ userName }}</p>
            <p>邮箱: {{ userEmail }}</p>
            <p>注册日期: {{ registrationDate }}</p>
          </div>
        </div>
        <button @click="goBack" class="back-button">返回聊天</button>
      </div>
    </div>
  </div>
</template>

<script>
// 引入 SideBar 组件
import SideBar from './SideBar.vue';
import { mapState } from 'vuex';

export default {
  components: {
    SideBar
  },
  data() {
    return {
      userAvatar: '/user-avatar.png',
      userName: '张三',
      userEmail: 'zhangsan@example.com',
      registrationDate: '2024-01-15',
    };
  },
  computed: {
    ...mapState(['isSidebarCollapsed'])
  },
  methods: {
    goToUserInfo() {
      this.$router.push('/user-info');
    },
    goBack() {
      this.$router.push('/');
    }
  }
}
</script>

<style scoped>
.user-info {
  display: flex;
  height: 100vh;
}

.user-content {
  flex: 1;
  padding: 20px;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #f5f5f5;
}

.card {
  background-color: white;
  border-radius: 10px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 30px;
  width: 100%;
  max-width: 600px;
}

h2 {
  text-align: center;
  margin-bottom: 20px;
  color: #333;
}

.user-details {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}

.avatar {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  object-fit: cover;
  border: 3px solid #4a90e2;
  margin-right: 30px;
}

.info {
  flex: 1;
}

.info p {
  margin-bottom: 10px;
  font-size: 16px;
  line-height: 1.5;
  color: #444;
}

.back-button {
  display: block;
  margin: 0 auto;
  padding: 12px 24px;
  background-color: #4a90e2;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 16px;
  transition: background-color 0.3s ease;
}

.back-button:hover {
  background-color: #3a7bc8;
}
</style>