<template>
  <div class="registration-container">
    <SideBar @goToUserInfo="goToUserInfo" />
    <div class="main-content">
      <div class="registration-card">
        <h2>用户注册</h2>
        <form @submit.prevent="registerUser">
          <!-- 用户名 -->
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

          <!-- 密码 -->
          <div class="form-group">
            <label for="password">密码</label>
            <input
              id="password"
              v-model="user.Password"
              type="password"
              placeholder="请输入密码"
            />
          </div>

          <!-- 确认密码 -->
          <div class="form-group">
            <label for="confirmPassword">确认密码</label>
            <input
              id="confirmPassword"
              v-model="confirmPassword"
              type="password"
              placeholder="请再次输入密码"
            />
            <div v-if="passwordMismatch" class="validation-message">
              两次密码输入不一致
            </div>
          </div>

          <!-- 性别 -->
          <div class="form-group">
            <label>性别</label>
            <div class="gender-group">
              <label>
                <input type="radio" v-model="user.Gender" :value="0" /> 男
              </label>
              <label>
                <input type="radio" v-model="user.Gender" :value="1" /> 女
              </label>
              <label>
                <input type="radio" v-model="user.Gender" :value="2" /> 保密
              </label>
            </div>
          </div>

          <!-- 邮箱 -->
          <div class="form-group email-group">
            <label for="email">邮箱</label>
            <div class="email-input-group">
              <input
                id="email"
                v-model="user.Email"
                type="email"
                placeholder="请输入邮箱"
              />
              <button
                type="button"
                class="send-code-btn"
                @click="sendCode"
                :disabled="!user.Email || sendingCode || codeCooldown > 0"
              >
                <template v-if="codeCooldown > 0">
                  重新发送 ({{ codeCooldown }}s)
                </template>
                <template v-else>发送验证码</template>
              </button>
            </div>
            <!-- 发送验证码错误提示 -->
            <div v-if="sendCodeError" class="validation-message" style="margin-top:8px;">
              {{ sendCodeError }}
            </div>
            <!-- 发送验证码成功提示 -->
            <div v-if="sendCodeSuccess" class="success-message" style="margin-top:8px;">
              验证码已发送，请检查邮箱
            </div>
          </div>

          <!-- 验证码 -->
          <div class="form-group">
            <label for="code">验证码</label>
            <input
              id="code"
              v-model="user.Code"
              type="text"
              placeholder="请输入验证码"
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
import axios from "axios";
import SideBar from "./SideBar.vue";

export default {
  components: { SideBar },
  data() {
    return {
      user: {
        UserName: "",
        Email: "",
        Password: "",
        Gender: 0,
        Code: "",
      },
      confirmPassword: "",
      usernameExists: null,
      isCheckingUsername: false,
      sendingCode: false,
      codeCooldown: 0,
      cooldownTimer: null,
      sendCodeError: "", // 错误消息
      sendCodeSuccess: false, // 成功提示开关
    };
  },
  computed: {
    passwordMismatch() {
      return (
        this.user.Password &&
        this.confirmPassword &&
        this.user.Password !== this.confirmPassword
      );
    },
    isRegisterButtonDisabled() {
      return (
        this.usernameExists === true ||
        !this.user.UserName ||
        !this.user.Email ||
        !this.user.Password ||
        !this.confirmPassword ||
        this.passwordMismatch ||
        this.user.Gender == null ||
        !this.user.Code
      );
    },
  },
  methods: {
    goToUserInfo() {
      this.$router.push("/user-info");
    },
    async checkUserName() {
      if (this.user.UserName.trim() === "") {
        this.usernameExists = null;
        return;
      }

      this.isCheckingUsername = true;
      try {
        const response = await axios.get(
          `https://localhost:7201/User/checkUserNameIsExisted/${this.user.UserName}`
        );
        this.usernameExists = response.data;
      } catch (error) {
        console.error("检查用户名时出错:", error);
        this.usernameExists = null;
      } finally {
        this.isCheckingUsername = false;
      }
    },
    async sendCode() {
      if (!this.user.Email || this.sendingCode || this.codeCooldown > 0) return;

      this.sendingCode = true;
      this.sendCodeError = "";
      this.sendCodeSuccess = false;
      try {
        await axios.post("https://localhost:7201/User/send-code", {
          email: this.user.Email,
        });
        // 成功时显示成功提示并开始倒计时
        this.sendCodeSuccess = true;
        this.codeCooldown = 20;
        this.cooldownTimer = setInterval(() => {
          if (this.codeCooldown > 0) {
            this.codeCooldown--;
          }
          if (this.codeCooldown === 0) {
            clearInterval(this.cooldownTimer);
            this.cooldownTimer = null;
            this.sendCodeSuccess = false; // 倒计时结束，隐藏成功提示
          }
        }, 1000);
      } catch (error) {
        console.error("发送验证码失败:", error);
        this.sendCodeError = "验证码发送失败，请稍后重试";
      } finally {
        this.sendingCode = false;
      }
    },
    async registerUser() {
      if (this.isRegisterButtonDisabled) return;

      try {
        await axios.post(
          "https://localhost:7201/User/addUser",
          JSON.stringify(this.user),
          { headers: { "Content-Type": "application/json" } }
        );
        alert("注册成功！");
        this.$router.push("/login");
      } catch (error) {
        console.error("注册失败:", error);
        alert("注册失败，请稍后重试。");
      }
    },
  },
  beforeUnmount() {
    if (this.cooldownTimer) {
      clearInterval(this.cooldownTimer);
      this.cooldownTimer = null;
    }
  },
};
</script>

<style scoped>
.registration-container {
  display: flex;
  height: 100vh;
  background-color: #e7e6e4; /* 莫兰迪浅背景 */
}

.main-content {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
}

.registration-card {
  background-color: #ffffff;
  border-radius: 12px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15); /* 增强阴影 */
  padding: 32px;
  width: 100%;
  max-width: 420px;
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
input[type="email"],
input[type="password"] {
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 6px;
  box-sizing: border-box;
  font-size: 14px;
  background-color: #f9f9f9;
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

.email-group .email-input-group {
  display: flex;
  gap: 10px;
}

.send-code-btn {
  padding: 8px 12px;
  background-color: #4a90e2;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 13px;
  cursor: pointer;
  white-space: nowrap;
  transition: background-color 0.3s;
}

.send-code-btn:hover {
  background-color: #3a7bc8;
}

.send-code-btn:disabled {
  background-color: #ccc;
  cursor: not-allowed;
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

/* 成功提示样式 */
.success-message {
  margin-top: 6px;
  padding: 8px 12px;
  font-size: 13px;
  font-weight: 600;
  color: #155724;
  background-color: #d4edda;
  border: 1px solid #c3e6cb;
  border-radius: 6px;
}

.button-group {
  display: flex;
  justify-content: center;
  margin-top: 24px;
}

.register-button {
  background-color: #4a90e2;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 12px 24px;
  cursor: pointer;
  font-size: 16px;
  transition: background-color 0.3s, box-shadow 0.3s;
}

.register-button:hover {
  background-color: #3a7bc8;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.register-button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
  box-shadow: none;
}
</style>
