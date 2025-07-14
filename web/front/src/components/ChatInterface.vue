<template>
  <div class="chat-interface">
    <!-- 左侧栏 -->
    <SideBar @goToUserInfo="goToUserInfo" />
    <!-- 主内容区域 -->
    <div class="main-content">
      <div class="chat-header">
        <div class="chat-title">
          <h3>对话</h3>
        </div>
        <div class="chat-actions">
          <button class="action-button" title="设置">⚙️</button>
          <button class="action-button" title="收藏">⭐</button>
          <button @click="goToUserInfo" class="avatar-button">
            <img src="/user-avatar.png" alt="User Avatar" class="avatar" />
          </button>
        </div>
      </div>
      <div class="chat-messages">
        <div class="message system-message">
          <p>{{ message }}</p>
        </div>
        <!-- 其他消息 -->
      </div>
      <div class="chat-input">
        <input
          type="text"
          v-model="inputMessage"
          @keyup.enter="sendMessage"
          placeholder="请输入内容..."
        />
        <button @click="sendMessage" class="send-button">发送</button>
      </div>
      <div class="register-link">
        <router-link to="/register">注册新用户</router-link>
      </div>
      <button @click="goToXX" class="avatar-button"></button>
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
      message: "无限进步。",
      inputMessage: "",
    };
  },
  methods: {
    sendMessage() {
      if (this.inputMessage.trim()) {
        console.log("发送消息:", this.inputMessage);
        this.inputMessage = "";
      }
    },
    computed: {
    ...mapState(['isSidebarCollapsed'])
  },
    goToUserInfo() {
      this.$router.push('/user-info');
    },
    goToXX(){
      this.$router.push('/register');
    }
  }
}
</script>

<style scoped>
.register-link {
  margin: 10px;
  text-align: center;
}
.register-link a {
  color: #4a90e2;
  text-decoration: none;
}
.register-link a:hover {
  text-decoration: underline;
}

.chat-interface {
  display: flex;
  height: 100vh;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  background-color: #f5f5f5;
  color: #333;
}

.chat-header {
  padding: 15px;
  border-bottom: 1px solid #ddd;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: #fff;
}

.chat-actions {
  display: flex;
  gap: 10px;
}

.action-button {
  background: transparent;
  border: none;
  color: #666;
  cursor: pointer;
  font-size: 16px;
  transition: color 0.2s ease;
}

.avatar-button {
  background: transparent;
  border: none;
  cursor: pointer;
}

.avatar {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  object-fit: cover;
}

.action-button:hover {
  color: #333;
}

.chat-messages {
  flex: 1;
  padding: 15px;
  overflow-y: auto;
  background-color: #fff;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.message {
  max-width: 70%;
  padding: 10px 15px;
  border-radius: 18px;
}

.system-message {
  background-color: #f0f0f0;
  align-self: flex-start;
}

.user-message {
  background-color: #4a90e2;
  color: white;
  align-self: flex-end;
}

.message p {
  margin: 0;
  font-size: 14px;
  line-height: 1.4;
}

.chat-input {
  padding: 15px;
  border-top: 1px solid #ddd;
  display: flex;
  align-items: center;
  background-color: #fff;
}

.chat-input input {
  flex: 1;
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 20px;
  background-color: #f9f9f9;
  color: #333;
  font-size: 14px;
  margin-right: 10px;
  outline: none;
  transition: border-color 0.2s ease;
}

.chat-input input:focus {
  border-color: #4a90e2;
}

.send-button {
  padding: 10px 15px;
  border: none;
  border-radius: 20px;
  background-color: #4a90e2;
  color: white;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.2s ease;
}

.send-button:hover {
  background-color: #3a7bc8;
}
</style>