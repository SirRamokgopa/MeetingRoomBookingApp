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
  <section class="page-container">
    <p v-if="loading" class="loading-text">Loading bookings and rooms…</p>
    <p v-if="error" class="error">{{ error }}</p>

    <section v-if="!loading && !error">
      <div class="page-header">
        <h2>Rooms</h2>
        <p class="page-subtitle">Pick a room to see its schedule.</p>
      </div>

      <div class="room-grid">
        <button
          v-for="room in rooms"
          :key="room.id"
          class="room-card"
          @click="openRoom(room)"
        >
          <span class="room-name">{{ room.name || `Room ${room.id}` }}</span>
          <span class="badge">Capacity {{ room.capacity }}</span>
        </button>
      </div>

      <div class="page-header section-gap booking-header">
        <div>
          <h2>All Bookings</h2>
          <p class="page-subtitle">Everything on the calendar across every room.</p>
        </div>
        <button class="btn btn-primary" @click="router.push({ name: 'create-booking' })">+ Create booking</button>
      </div>

      <div v-if="bookings.length" class="table-wrapper">
        <table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Room</th>
              <th>Start Time</th>
              <th>End Time</th>
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
              <td><button class="btn-sm" @click="editBooking(booking)">Edit</button></td>
              <td><button class="btn-sm btn-danger" @click="showDeleteModal(booking)">Delete</button></td>
            </tr>
          </tbody>
        </table>
      </div>
      <p v-else class="loading-text">No bookings yet — create the first one.</p>

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
.room-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
}

.room-card {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.5rem;
  background: linear-gradient(180deg, var(--c-surface-1), var(--c-surface-0));
  border: 1px solid var(--c-border);
  border-radius: var(--radius-md);
  padding: 1.1rem 1.25rem;
  text-align: left;
  color: var(--c-text);
  transition: border-color var(--transition-fast), transform var(--transition-fast), box-shadow var(--transition-fast);
}

.room-card:hover {
  border-color: var(--c-blue-400);
  transform: translateY(-2px);
  box-shadow: var(--shadow-elevated);
  background: linear-gradient(180deg, var(--c-surface-1), var(--c-surface-0));
  color: var(--c-text);
}

.room-name {
  font-size: 1.1rem;
  font-weight: 600;
}

.booking-header {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
}

@media (max-width: 480px) {
  .booking-header {
    align-items: flex-start;
  }
}
</style>
