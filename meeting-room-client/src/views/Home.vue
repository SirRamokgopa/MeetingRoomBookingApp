<script setup>
import { ref, onMounted, computed } from 'vue'
import { getBookings, getRooms, deleteBooking } from '../api/meetingRoomClient'
import { useRouter } from 'vue-router'
import DeleteConfirmModal from '../components/DeleteConfirmModal.vue'

const bookings = ref([])
const rooms = ref([])
const error = ref('')
const loading = ref(true)
const bookingToDelete = ref(null)
const router = useRouter()

const loadData = async () => {
  loading.value = true
  error.value = ''

  try {
    const [bookingResults, roomResults] = await Promise.all([getBookings(), getRooms()])
    bookings.value = bookingResults.slice().sort((a, b) => new Date(a.startTime) - new Date(b.startTime))
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

const showDeleteModal = (booking) => {
  bookingToDelete.value = booking
}

const closeDeleteModal = () => {
  bookingToDelete.value = null
}

const confirmDelete = async () => {
  if (!bookingToDelete.value) return

  loading.value = true
  error.value = ''

  try {
    await deleteBooking(bookingToDelete.value.id)
    bookingToDelete.value = null
    await loadData()
  } catch (err) {
    error.value = err?.message || 'Unable to delete booking.'
    loading.value = false
  }
}

const deleteDescription = computed(() => {
  return bookingToDelete.value
    ? `I hope you know what you're doing, bro. "${bookingToDelete.value.title}" looked like a solid idea.`
    : ''
})
</script>

<template>
  <section>
    <p v-if="loading">Loading bookings and rooms…</p>
    <p v-if="error" class="error">{{ error }}</p>

    <section v-if="!loading && !error">
      <h2>Rooms</h2>
      <ul>
        <li v-for="room in rooms" :key="room.id">
          <button class="room-button" @click="openRoom(room)">{{ room.name || `Room ${room.id}` }}<span class="capacity-label">{{ ` Capacity: ${room.capacity}` || `` }}</span></button>
        </li>
      </ul>

      <h2>All Bookings</h2>
      <table>
        <thead>
          <tr>
            <th class="booking-heading">Title</th>
            <th class="booking-heading">Room</th>
            <th class="booking-heading">Start Time</th>
            <th class="booking-heading">End Time</th>
            <th></th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="booking in bookings" :key="booking.id">
            <td>{{ booking.title }}</td>
            <td>{{ booking.roomName || booking.roomId }}</td>
            <td>{{ new Date(booking.startTime).toLocaleString() }}</td>
            <td>{{ new Date(booking.endTime).toLocaleString() }}</td>
            <td><button class="edit-button" @click="editBooking(booking)">Edit booking</button></td>
            <td><button class="delete-button" @click="showDeleteModal(booking)">Delete</button></td>
          </tr>
        </tbody>
      </table>
      <button @click="router.push({ name: 'create-booking' })">Create Booking</button>

      <DeleteConfirmModal
        :show="Boolean(bookingToDelete)"
        title="Confirm delete"
        :description="deleteDescription"
        confirmLabel="Delete"
        cancelLabel="Cancel"
        @confirm="confirmDelete"
        @cancel="closeDeleteModal"
      />
    </section>
  </section>
</template>

<style scoped>
.error { color: #c92a2a; }

.room-button {
  font-size: 1.5rem;
  border: none;

}
.room-button:hover {
  background: rgba(77, 98, 119, 0.08);
}
.capacity-label {
  color: var(--color-text);
  font-size: 1rem;
}

h2 {
  padding-top: 1rem;
}
td {
  padding: 0 1rem;
}
th {
  padding: 0 1rem;
  border-top: 1px solid #ccc;
  border-bottom: 1px solid #ccc;
}
table {
  padding: 1rem 0;
  border-collapse: collapse;
  table-layout: auto;
  width: auto;
  margin: 0;
}

td,
th {
  white-space: normal;
}

.booking-heading {
  text-align: left;
}

</style>
