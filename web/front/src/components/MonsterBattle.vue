<!-- eslint-disable -->
<template>
  <div
    ref="containerRef"
    class="battle-container"
    @keydown="handleKeyDown"
    @keyup="handleKeyUp"
    @mousedown="handleMouseDown"
    tabindex="0"
  >
    <SideBar />
    <div class="battlefield">
      <div
        v-for="(monster, userId) in monsters"
        :key="userId"
        class="monster"
        :style="monsterStyle(monster)"
      >
        <div class="name-tag">{{ monster.name }}</div>
      </div>

      <div
        v-for="(bullet, index) in bullets"
        :key="'bullet-' + index"
        class="bullet"
        :style="bulletStyle(bullet)"
      ></div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, onBeforeUnmount, reactive, ref } from 'vue'
import * as signalR from '@microsoft/signalr'
import SideBar from '@/components/SideBar.vue'

// JWT解析函数，和聊天广场保持一致
function parseJwt(token) {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join('')
    )
    return JSON.parse(jsonPayload)
  } catch {
    return null
  }
}

const monsters = reactive({})
const bullets = ref([])
const containerRef = ref(null)

let userName = '匿名'
const token = localStorage.getItem('jwt_token')
if (token) {
  const payload = parseJwt(token)
  const nameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
  if (payload && payload[nameClaim]) userName = payload[nameClaim]
}

const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:7201/monsterHub', {
    accessTokenFactory: () => localStorage.getItem('jwt_token')
  })
  .withAutomaticReconnect()
  .build()

// 当前移动方向，匀速移动用
const moveDirection = { x: 0, y: 0 }
let animationFrameId = null

function updatePosition() {
  const speed = 3 // 每帧移动速度(px)
  if (moveDirection.x !== 0 || moveDirection.y !== 0) {
    const dx = moveDirection.x * speed
    const dy = moveDirection.y * speed
    connection.invoke('Move', dx, dy).catch(console.error)
  }
  animationFrameId = requestAnimationFrame(updatePosition)
}

const handleKeyDown = (e) => {
  switch (e.key.toLowerCase()) {
    case 'w': moveDirection.y = -1; break
    case 's': moveDirection.y = 1; break
    case 'a': moveDirection.x = -1; break
    case 'd': moveDirection.x = 1; break
  }
}

const handleKeyUp = (e) => {
  switch (e.key.toLowerCase()) {
    case 'w':
    case 's':
      moveDirection.y = 0
      break
    case 'a':
    case 'd':
      moveDirection.x = 0
      break
  }
}

const handleMouseDown = (e) => {
  e.preventDefault()
  if (e.button === 0) {
    connection.invoke('Shoot').catch(console.error)
  } else if (e.button === 2) {
    connection.invoke('Rotate').catch(console.error)
  }
}

const monsterStyle = (monster) => ({
  left: monster.x + 'px',
  top: monster.y + 'px',
  transform: `rotate(${monster.rotation}deg)`
})

const bulletStyle = (bullet) => ({
  left: bullet.x + 'px',
  top: bullet.y + 'px'
})

onMounted(async () => {
  try {
    await connection.start()
    await connection.invoke('Register', userName)
    containerRef.value?.focus()

    connection.on('UpdateMonsters', (serverMonsters) => {
      for (const key in monsters) delete monsters[key]
      Object.assign(monsters, serverMonsters)
    })

    connection.on('UpdateBullets', (serverBullets) => {
      bullets.value = serverBullets
    })

    animationFrameId = requestAnimationFrame(updatePosition)
    window.addEventListener('keyup', handleKeyUp)
  } catch (err) {
    console.error('连接失败:', err)
  }
})

onBeforeUnmount(() => {
  if (animationFrameId) {
    cancelAnimationFrame(animationFrameId)
  }
  window.removeEventListener('keyup', handleKeyUp)
})
</script>

<style scoped>
.battle-container {
  display: flex;
  height: 100vh;
  outline: none;
  user-select: none;
}

.battlefield {
  width: 90vw;            /* 宽度占视口宽90% */
  max-width: 900px;       /* 最大宽度限制 */
  aspect-ratio: 4 / 4;    /* 宽高比 4:3 */
  margin: 20px auto;
  position: relative;
  background-color: #eef;
  border: 2px solid #444;
  overflow: hidden;
  user-select: none;
}


.monster {
  position: absolute;
  width: 40px;
  height: 40px;
  background: url('https://cdn-icons-png.flaticon.com/512/616/616408.png') no-repeat center;
  background-size: contain;
  transition: 0.1s linear;
  cursor: default;
  user-select: none;
}

.name-tag {
  position: absolute;
  bottom: -20px;
  background: white;
  border: 1px solid #444;
  padding: 2px 5px;
  font-size: 12px;
  white-space: nowrap;
  text-align: center;
  width: max-content;
  left: 50%;
  transform: translateX(-50%);
  pointer-events: none;
  user-select: none;
}

.bullet {
  position: absolute;
  width: 10px;
  height: 10px;
  background: red;
  border-radius: 50%;
  user-select: none;
}
</style>
