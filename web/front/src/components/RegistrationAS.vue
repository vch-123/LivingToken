<template>
  <div class="registration-container">
    <!-- 左侧栏 -->
    <SideBar @goToUserInfo="goToUserInfo" />
    <!-- 主内容区域 -->
    <div class="main-content">
      <div class="registration-card">
        <h2>用户注册</h2>
        <form @submit.prevent="registerUser">
          <div class="form-group">
            <label for="username">用户名</label>
            <input
              id="username"
              v-model="user.UserName"
              type="text"
              placeholder="请输入用户名"
              @blur="checkUserName"
            />
            <div v-if="usernameExists !== null" class="validation-message">
              {{ usernameExists ? '用户名已存在' : '用户名可用' }}
            </div>
          </div>

          <div class="form-group">
            <label for="email">邮箱</label>
            <input
              id="email"
              v-model="user.Email"
              type="email"
              placeholder="请输入邮箱"
            />
          </div>

          <div class="form-group">
            <label for="password">密码</label>
            <input
              id="password"
              v-model="user.Password"
              type="password"
              placeholder="请输入密码"
            />
          </div>

          <div class="form-group">
            <label>性别</label>
            <div class="gender-group">
              <label>
                <input
                  type="radio"
                  v-model="user.Gender"
                  value="Male"
                /> 男
              </label>
              <label>
                <input
                  type="radio"
                  v-model="user.Gender"
                  value="Female"
                /> 女
              </label>
              <label>
                <input
                  type="radio"
                  v-model="user.Gender"
                  value="Secrecy"
                /> 保密
              </label>
            </div>
          </div>

          <div class="form-group">
            <label for="phone">手机号码（选填）</label>
            <input
              id="phone"
              v-model="user.PhoneNumber"
              type="text"
              placeholder="请输入手机号码"
            />
          </div>

          <div class="button-group">
            <button
              type="submit"
              :disabled="isRegisterButtonDisabled"
              class="register-button"
            >
              注册
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
  
  components: {
    SideBar
  },
  data() {
    return {
      user: {
        UserName: '',
        Email: '',
        Password: '',
        Gender: 'Male', // 默认值为 Male
        PhoneNumber: ''
      },
      usernameExists: null, // 用户名是否存在，null 表示未检查
      isCheckingUsername: false // 正在检查用户名状态
    };
  },
  computed: {
    isRegisterButtonDisabled() {
      // 注册按钮禁用条件：
      // 1. 用户名已存在
      // 2. 用户名为空
      // 3. 邮箱为空
      // 4. 密码为空
      // 5. 性别未选择
      return (
        this.usernameExists === true ||
        !this.user.UserName ||
        !this.user.Email ||
        !this.user.Password ||
        !this.user.Gender
      );
    }
  },
  methods: {
    goToUserInfo() {
      this.$router.push('/user-info');
    },
    async checkUserName() {
      // 如果用户名为空，重置状态并返回
      if (this.user.UserName.trim() === '') {
        this.usernameExists = null;
        return;
      }

      // 设置检查状态，防止重复请求
      this.isCheckingUsername = true;

      try {
        // 调用后端接口检查用户名是否存在
        const response = await axios.get(
          `https://localhost:7201/User/checkUserNameIsExisted/${this.user.UserName}`
        );

        // 根据后端返回的结果更新用户名状态
        this.usernameExists = response.data;
      } catch (error) {
        console.error('检查用户名时出错:', error);
        this.usernameExists = null;
      } finally {
        // 检查完成，重置检查状态
        this.isCheckingUsername = false;
      }
    },
    async registerUser() {
      // 如果注册按钮被禁用，直接返回
      if (this.isRegisterButtonDisabled) {
        return;
      }

      try {
        // 调用后端注册接口
        await axios.post('https://localhost:7201/User/addUser', JSON.stringify(this.user));
        
        // 注册成功，显示提示信息
        alert('注册成功！');

        // 跳转到登录页面或其他页面
        this.$router.push('/login');
      } catch (error) {
        console.error('注册失败:', error);
        alert('注册失败，请稍后重试。');
      }
    }
  }
};
</script>

<style scoped>
.registration-container {
  display: flex;
  height: 100vh;
}

.main-content {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #f5f5f5;
}

.registration-card {
  background-color: white;
  border-radius: 10px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 30px;
  width: 100%;
  max-width: 400px;
}

h2 {
  text-align: center;
  margin-bottom: 20px;
  color: #333;
}

.form-group {
  margin-bottom: 20px;
}

label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
}

input[type="text"],
input[type="email"],
input[type="password"] {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  box-sizing: border-box;
  font-size: 14px;
}

.gender-group {
  display: flex;
  gap: 15px;
  margin-top: 5px;
}

.gender-group label {
  margin-bottom: 0;
  display: flex;
  align-items: center;
}

.gender-group input {
  margin-right: 5px;
}

.validation-message {
  margin-top: 5px;
  font-size: 12px;
  color: #ff4d4f;
}

.button-group {
  display: flex;
  justify-content: center;
  margin-top: 20px;
}

.register-button {
  background-color: #4a90e2;
  color: white;
  border: none;
  border-radius: 5px;
  padding: 10px 20px;
  cursor: pointer;
  font-size: 16px;
  transition: background-color 0.3s;
}

.register-button:hover {
  background-color: #3a7bc8;
}

.register-button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}
</style>