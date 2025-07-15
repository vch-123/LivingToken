import { createRouter, createWebHistory } from 'vue-router';
import ChatInterface from '../components/ChatInterface.vue';
import UserInfo from '../components/UserInfo.vue';
import Registration from '../components/RegistrationAS.vue'; // 引入注册组件
import Login from '../components/LoginPage.vue';

const routes = [
  {
    path: '/',
    name: 'Chat',
    component: ChatInterface
  },
  {
    path: '/user-info',
    name: 'UserInfo',
    component: UserInfo
  },
  {
    path: '/register',
    name: 'Register',
    component: Registration // 添加注册路由
  },
  {
    path: '/login',
    name: 'Login',
    component: Login // 添加注册路由
  }
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

export default router;