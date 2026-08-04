<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getBookings, getRooms, deleteBooking } from '../api/meetingRoomClient'
import DeleteConfirmModal from '../components/DeleteConfirmModal.vue'

const route = useRoute()
const roomId = Number(route.params.id)

const router = useRouter()

const bookings = ref([])
const room = ref(null)
const error = ref('')
const loading = ref(true)
const bookingToDelete = ref(null)

const loadData = async () => {
  loading.value = true
  error.value = ''

  try {
    const [bookingResults, roomResults] = await Promise.all([getBookings(), getRooms()])
    bookings.value = bookingResults
      .filter(b => b.roomId === roomId)
      .slice()
      .sort((a, b) => new Date(a.startTime) - new Date(b.startTime))
    room.value = roomResults.find(r => r.id === roomId) || { id: roomId, name: `Room ${roomId}` }
  } catch (err) {
    error.value = err?.message || 'Unable to load data from the MeetingRoom API.'
  } finally {
    loading.value = false
  }
}

onMounted(loadData)

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
    ? `You can't waste peoples' time if you delete booking "${bookingToDelete.value.title}"?`
    : ''
})
</script>

<template>
  <section class="page-container">
    <p v-if="loading" class="loading-text">Loading room details…</p>
    <p v-if="error" class="error">{{ error }}</p>

    <section v-if="!loading && !error">
      <div class="card room-summary">
        <div>
          <h2>{{ room.name || `Room ${room.id}` }}</h2>
          <span class="badge">Capacity {{ room.capacity }}</span>
        </div>
        <button class="btn btn-primary" @click="router.push({ name: 'create-booking', params: { id: room.id } })">
          + Create booking
        </button>
      </div>

      <div class="page-header section-gap">
        <h2>Bookings for this room</h2>
      </div>

      <ul v-if="bookings.length" class="booking-list">
        <li v-for="b in bookings" :key="b.id" class="booking-item">
          <div class="booking-details">
            <strong>{{ b.title }}</strong>
            <div class="booking-time">
              {{ new Date(b.startTime).toLocaleString() }} — {{ new Date(b.endTime).toLocaleString() }}
            </div>
          </div>
          <div class="booking-actions">
            <button class="btn-sm" @click="router.push({ name: 'edit-booking', params: { id: b.id } })">Edit</button>
            <button class="btn-sm btn-danger" @click="showDeleteModal(b)">Delete</button>
          </div>
        </li>
      </ul>
      <p v-else class="loading-text">No bookings for this room yet.</p>

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
.room-summary {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
}

.room-summary h2 {
  margin-bottom: 0.4rem;
}

.booking-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.booking-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  background: var(--c-surface-0);
  border: 1px solid var(--c-border);
  border-radius: var(--radius-md);
  padding: 1rem 1.25rem;
  flex-wrap: wrap;
  transition: border-color var(--transition-fast);
}

.booking-item:hover {
  border-color: var(--c-border-strong);
}

.booking-details {
  flex: 1;
  min-width: 200px;
}

.booking-time {
  color: var(--c-text-muted);
  font-size: 0.85rem;
  margin-top: 0.2rem;
}

.booking-actions {
  display: flex;
  gap: 0.5rem;
}
</style>
