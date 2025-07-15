<template>
  <div class="login-container">
    <!-- 左侧栏 -->
    <SideBar @goToUserInfo="goToUserInfo" />

    <!-- 主内容区域 -->
    <div class="main-content">
      <div class="login-card">
        <h2>用户登录</h2>
        <form @submit.prevent="login">
          <!-- 用户名或邮箱 -->
          <div class="form-group">
            <label for="username">用户名或邮箱</label>
            <input
              id="username"
              v-model="loginForm.usernameOrEmail"
              type="text"
              placeholder="请输入用户名或邮箱"
            />
          </div>

          <!-- 密码 -->
          <div class="form-group">
            <label for="password">密码</label>
            <input
              id="password"
              v-model="loginForm.password"
              type="password"
              placeholder="请输入密码"
            />
          </div>

          <!-- 错误提示 -->
          <div v-if="loginError" class="validation-message">
            {{ loginError }}
          </div>

          <div class="button-group">
            <button
              type="submit"
              :disabled="!loginForm.usernameOrEmail || !loginForm.password"
              class="login-button"
            >
              登录
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import SideBar from './SideBar.vue';

export default {
  name: 'LoginPage',
  components: { SideBar },
  data() {
    return {
      loginForm: {
        usernameOrEmail: '',
        password: ''
      },
      loginError: ''
    };
  },
  methods: {
    goToUserInfo() {
      this.$router.push('/user-info');
    },
    async login() {
      try {
        const response = await axios.post('https://localhost:7201/User/login', {
          usernameOrEmail: this.loginForm.usernameOrEmail,
          password: this.loginForm.password
        });

        // 假设 response.data 是用户信息或 token
        const userData = response.data;
        localStorage.setItem('user', JSON.stringify(userData));

        // 跳转页面
        this.$router.push('/user-info');
      } catch (error) {
        console.error('登录失败:', error);
        this.loginError = '用户名或密码错误，请重试';
      }
    }
  }
};
</script>

<style scoped>
.login-container {
  display: flex;
  height: 100vh;
  background-color: #e7e6e4; /* 莫兰迪浅灰 */
}

.main-content {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
}

.login-card {
  background-color: #ffffff;
  border-radius: 12px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15); /* 加强阴影 */
  padding: 32px;
  width: 100%;
  max-width: 400px;
}

h2 {
  text-align: center;
  margin-bottom: 24px;
  color: #333;
  font-weight: 600;
}

.form-group {
  margin-bottom: 20px;
}

label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: #444;
}

input[type="text"],
input[type="password"] {
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 6px;
  box-sizing: border-box;
  font-size: 14px;
  background-color: #f9f9f9;
}

.validation-message {
  margin-top: 6px;
  padding: 8px 12px;
  font-size: 13px;
  font-weight: 600;
  color: #a94442;
  background-color: #f8d7da;
  border: 1px solid #f5c6cb;
  border-radius: 6px;
}

.button-group {
  display: flex;
  justify-content: center;
  margin-top: 24px;
}

.login-button {
  background-color: #4a90e2;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 12px 24px;
  cursor: pointer;
  font-size: 16px;
  transition: background-color 0.3s;
}

.login-button:hover {
  background-color: #3a7bc8;
}

.login-button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}
</style>
