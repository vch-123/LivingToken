import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import store from './store/store.js'; // 引入 store

const app = createApp(App);
app.use(router);
app.use(store); // 使用 store
app.mount('#app');