<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import BookingForm from '../components/BookingForm.vue'
import { createBooking, getBookings, getRooms } from '../api/meetingRoomClient'

const route = useRoute()
const router = useRouter()
const roomIdParam = route.params.id
const roomId = roomIdParam != null && roomIdParam !== '' ? Number(roomIdParam) : null

const rooms = ref([])
const allBookings = ref([])
const loading = ref(true)
const errors = ref([])

const initialForm = ref({
  title: '',
  roomId: Number.isFinite(roomId) ? roomId : null,
  startTime: '',
  endTime: ''
})

const load = async () => {
  loading.value = true
  errors.value = []

  try {
    rooms.value = await getRooms()
    allBookings.value = await getBookings()
  } catch (e) {
    errors.value = [e?.message || 'Failed to load create booking data']
  } finally {
    loading.value = false
  }
}

onMounted(load)

const handleSubmit = async (payload) => {
  try {
    const created = await createBooking(payload)
    router.push({ name: 'room', params: { id: created.roomId } })
  } catch (e) {
    errors.value = [e?.message || 'Failed to create booking']
  }
}

const handleCancel = () => {
  router.back()
}
</script>

<template>
  <section class="page-container form-page">
    <div class="page-header">
      <h2>Create booking</h2>
      <p class="page-subtitle">Reserve a room for your next meeting.</p>
    </div>

    <p v-if="loading" class="loading-text">Loading booking form…</p>
    <div v-if="errors.length" class="errors">
      <ul>
        <li v-for="(err, idx) in errors" :key="idx">{{ err }}</li>
      </ul>
    </div>

    <div v-if="!loading" class="card">
      <BookingForm
        :initialForm="initialForm"
        :rooms="rooms"
        :allBookings="allBookings"
        :bookingId="null"
        submitLabel="Create booking"
        @submit="handleSubmit"
        @cancel="handleCancel"
      />
    </div>
  </section>
</template>

<style scoped>
.form-page {
  max-width: 640px;
}
</style>
