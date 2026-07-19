<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { getBookings, getRooms } from '../api/meetingRoomClient'

const route = useRoute()
const roomId = Number(route.params.id)

const bookings = ref([])
const room = ref(null)
const error = ref('')
const loading = ref(true)

const loadData = async () => {
  loading.value = true
  error.value = ''

  try {
    const [bookingResults, roomResults] = await Promise.all([getBookings(), getRooms()])
    bookings.value = bookingResults.filter(b => b.roomId === roomId)
    room.value = roomResults.find(r => r.id === roomId) || { id: roomId, name: `Room ${roomId}` }
  } catch (err) {
    error.value = err?.message || 'Unable to load data from the MeetingRoom API.'
  } finally {
    loading.value = false
  }
}

onMounted(loadData)
</script>

<template>
  <section>
    <p v-if="loading">Loading room details…</p>
    <p v-if="error" class="error">{{ error }}</p>

    <section v-if="!loading && !error">
      <h2>{{ room.name || `Room ${room.id}` }}</h2>

      <h3>Bookings for this room</h3>
      <ul>
        <li v-for="b in bookings" :key="b.id">
          <strong>{{ b.title }}</strong> — {{ new Date(b.startTime).toLocaleString() }} to {{ new Date(b.endTime).toLocaleString() }}
        </li>
      </ul>
    </section>
  </section>
</template>

<style scoped>
.error { color: #c92a2a; }
</style>
