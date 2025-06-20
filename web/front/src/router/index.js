import { createRouter, createWebHistory } from 'vue-router';
import ChatInterface from '../components/ChatInterface.vue';
import UserInfo from '../components/UserInfo.vue';

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
  }
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
});

export default router;