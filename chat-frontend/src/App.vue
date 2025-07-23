<template>
  <div
    class="container"
    tabindex="0"
    @keydown="handleKeyDown"
    @touchstart="onTouchStart"
    @touchmove.prevent="onTouchMove"
  >
    <h1>实时怪兽控制（键盘和触摸支持）</h1>
    <p>用 WASD 或手指滑动控制怪兽移动，所有客户端同步位置。</p>

    <div class="play-area">
      <div class="monster" :style="{ left: posX + 'px', top: posY + 'px' }"></div>
    </div>

    <p>当前坐标：X: {{ posX }}, Y: {{ posY }}</p>
    <p v-if="!isConnected" class="status">尚未连接服务器...</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import * as signalR from '@microsoft/signalr'

const posX = ref(100)
const posY = ref(100)
const isConnected = ref(false)

let lastTouchX = null
let lastTouchY = null

const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://192.168.1.223:7247/monsterHub') // 改成你的后端IP
  .withAutomaticReconnect()
  .build()

const move = async (deltaX, deltaY) => {
  if (!isConnected.value) return

  posX.value += deltaX
  posY.value += deltaY

  try {
    await connection.invoke('Move', deltaX, deltaY)
  } catch (err) {
    console.error('发送移动指令失败:', err)
  }
}

const handleKeyDown = (e) => {
  let deltaX = 0
  let deltaY = 0

  switch (e.key.toLowerCase()) {
    case 'w':
      deltaY = -10
      break
    case 's':
      deltaY = 10
      break
    case 'a':
      deltaX = -10
      break
    case 'd':
      deltaX = 10
      break
  }

  if (deltaX !== 0 || deltaY !== 0) {
    move(deltaX, deltaY)
  }
}

// 触摸开始
const onTouchStart = (event) => {
  const touch = event.touches[0]
  lastTouchX = touch.clientX
  lastTouchY = touch.clientY
}

// 触摸滑动
const onTouchMove = (event) => {
  const touch = event.touches[0]
  if (lastTouchX === null || lastTouchY === null) {
    lastTouchX = touch.clientX
    lastTouchY = touch.clientY
    return
  }

  let deltaX = touch.clientX - lastTouchX
  let deltaY = touch.clientY - lastTouchY

  // 减少移动灵敏度，可以除以某个数，比如 5
  deltaX = Math.round(deltaX / 5)
  deltaY = Math.round(deltaY / 5)

  if (deltaX !== 0 || deltaY !== 0) {
    move(deltaX, deltaY)
    lastTouchX = touch.clientX
    lastTouchY = touch.clientY
  }
}

onMounted(async () => {
  connection.on('UpdatePosition', (deltaX, deltaY) => {
    // 其他客户端的移动增量，累加
    posX.value += deltaX
    posY.value += deltaY
  })

  try {
    await connection.start()
    isConnected.value = true
    console.log('SignalR 连接成功')
  } catch (err) {
    console.error('SignalR 连接失败:', err)
  }
})
</script>

<style scoped>
.container {
  max-width: 600px;
  margin: 2rem auto;
  font-family: Arial, sans-serif;
  outline: none;
  user-select: none;
  touch-action: none; /* 阻止默认触摸事件 */
}

h1 {
  text-align: center;
}

.play-area {
  position: relative;
  width: 520px;
  height: 520px;
  border: 2px solid #333;
  margin: 1rem auto;
  background-color: #e0f7fa;
  overflow: hidden;
}

.monster {
  position: absolute;
  width: 40px;
  height: 40px;
  background: url('https://cdn-icons-png.flaticon.com/512/616/616408.png') no-repeat center center;
  background-size: contain;
  transition: left 0.1s linear, top 0.1s linear;
}

.status {
  color: red;
  text-align: center;
  font-weight: bold;
}
</style>
