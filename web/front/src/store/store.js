import { createStore } from 'vuex';

export default createStore({
  state: {
    isSidebarCollapsed: false, // 初始状态为打开
  },
  mutations: {
    toggleSidebar(state) {
      state.isSidebarCollapsed = !state.isSidebarCollapsed;
    },
    setSidebarCollapsed(state, isCollapsed) {
      state.isSidebarCollapsed = isCollapsed;
    }
  },
  actions: {
    toggleSidebar({ commit }) {
      commit('toggleSidebar');
    },
    setSidebarCollapsed({ commit }, isCollapsed) {
      commit('setSidebarCollapsed', isCollapsed);
    }
  },
  getters: {
    isSidebarCollapsed: state => state.isSidebarCollapsed,
  }
});