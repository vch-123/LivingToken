<template>
  <div class="chat-interface">
    <SideBar @goToUserInfo="goToUserInfo" />
    <div class="main-content">
      <div class="chat-header">
        <div class="chat-title"><h3>聊天广场</h3></div>
        <div class="chat-actions">
          <button class="action-button" title="设置">⚙️</button>
          <button class="action-button" title="收藏">⭐</button>
          <button @click="goToUserInfo" class="avatar-button">
            <img src="/user-avatar.png" alt="User Avatar" class="avatar" />
          </button>
        </div>
      </div>

      <div class="chat-messages">
        <div
          v-for="(msg, index) in messages"
          :key="index"
          class="message"
          :class="msg.username === currentUser ? 'user-message' : 'system-message'"
        >
          <p><strong>{{ msg.username }}</strong>: {{ msg.content }}</p>
        </div>
      </div>

      <div class="chat-input">
        <input
          type="text"
          v-model="inputMessage"
          @keyup.enter="sendMessage"
          :disabled="!isAuthenticated"
          placeholder="请输入内容..."
        />
        <button @click="sendMessage" class="send-button" :disabled="!isAuthenticated">
          {{ isAuthenticated ? '发送' : '登录后可发送' }}
        </button>
      </div>

      <div class="register-link" v-if="!isAuthenticated">
        <router-link to="/register">注册新用户</router-link><br />
        <router-link to="/login">已有账号？去登录</router-link>
      </div>
    </div>
  </div>
</template>


<script>
import SideBar from './SideBar.vue';
import * as signalR from '@microsoft/signalr';
import { mapState } from 'vuex';

function parseJwt(token) {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(
    atob(base64)
      .split('')
      .map(c => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
      .join('')
  );

  return JSON.parse(jsonPayload);
}

export default {
  components: { SideBar },
  data() {
    return {
      connection: null,
      inputMessage: '',
      messages: [],
      currentUser: '', // 当前登录用户名
    };
  },
  computed: {
    ...mapState(['isSidebarCollapsed']),
    isAuthenticated() {
      return !!localStorage.getItem('jwt_token');
    },
  },
  mounted() {
    const token = localStorage.getItem('jwt_token');
    if (token) {
      try {
  const payload = parseJwt(token);
  const nameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
  this.currentUser = payload[nameClaim] || '匿名';
} catch (err) {
  console.error('解析 token 失败', err);
}

    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7201/chatHub', {
    accessTokenFactory: () => localStorage.getItem('jwt_token')
})
      .withAutomaticReconnect()
      .build();

    this.connection.start().then(() => {
      console.log('✅ SignalR连接成功');
    }).catch(err => {
      console.error('❌ SignalR连接失败', err);
    });

    this.connection.on('ReceiveMessage', (username, content) => {
  console.log('🔍 当前用户:', this.currentUser);
  console.log('🟡 收到消息的用户名:', username);
  console.log('是否相等:', username === this.currentUser);

  this.messages.push({ username, content });
});


    this.connection.on('ReceiveSystemMessage', (msg) => {
      this.messages.push({ username: '系统', content: msg });
    });
  },
  methods: {
    goToUserInfo() {
      this.$router.push('/user-info');
    },
    sendMessage() {
      if (this.inputMessage.trim() && this.connection && this.isAuthenticated) {
        this.connection.invoke('SendMessage', this.inputMessage)
          .then(() => {
            this.inputMessage = '';
          })
          .catch(err => {
            console.error('发送失败', err);
          });
      }
    }
  }
}
</script>


<style scoped>
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
  word-break: break-word;
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
.send-button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}
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
</style>
