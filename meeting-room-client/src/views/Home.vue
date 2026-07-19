<script setup>
import { ref, onMounted } from 'vue'
import { getBookings, getRooms } from '../api/meetingRoomClient'
import { useRouter } from 'vue-router'

const bookings = ref([])
const rooms = ref([])
const error = ref('')
const loading = ref(true)
const router = useRouter()

const loadData = async () => {
  loading.value = true
  error.value = ''

  try {
    const [bookingResults, roomResults] = await Promise.all([getBookings(), getRooms()])
    bookings.value = bookingResults
    rooms.value = roomResults
  } catch (err) {
    error.value = err?.message || 'Unable to load data from the MeetingRoom API.'
  } finally {
    loading.value = false
  }
}

onMounted(loadData)

const openRoom = (room) => {
  router.push({ name: 'room', params: { id: room.id } })
}

const editBooking = (booking) => {
  router.push({ name: 'edit-booking', params: { id: booking.id } })
}
</script>

<template>
  <section>
    <p v-if="loading">Loading bookings and rooms…</p>
    <p v-if="error" class="error">{{ error }}</p>

    <section v-if="!loading && !error">
      <h2>Rooms</h2>
      <ul>
        <li v-for="room in rooms" :key="room.id">
          <button @click="openRoom(room)">{{ room.name || `Room ${room.id}` }}</button>
        </li>
      </ul>

      <h2>All Bookings</h2>
      <table>
        <thead>
          <tr>
            <th>""</th>
            <th>ID</th>
            <th>Title</th>
            <th>Room</th>
            <th>Start</th>
            <th>End</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="booking in bookings" :key="booking.id">
            <td><button @click="editBooking(booking)">Edit booking</button></td>
            <td>{{ booking.id }}</td>
            <td>{{ booking.title }}</td>
            <td>{{ booking.roomId }}</td>
            <td>{{ new Date(booking.startTime).toLocaleString() }}</td>
            <td>{{ new Date(booking.endTime).toLocaleString() }}</td>
          </tr>
        </tbody>
      </table>
      <button @click="router.push({ name: 'create-booking' })">Create Booking</button>
    </section>
  </section>
</template>

<style scoped>
.error { color: #c92a2a; }
button { background: none; border: none; color: #0066cc; cursor: pointer; }
button:hover { text-decoration: underline; }
</style>
