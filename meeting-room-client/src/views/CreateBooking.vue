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
  <section>
    <p v-if="loading">Loading booking form…</p>
    <div v-if="errors.length" class="errors">
      <ul>
        <li v-for="(err, idx) in errors" :key="idx">{{ err }}</li>
      </ul>
    </div>

    <BookingForm
      v-if="!loading"
      :initialForm="initialForm"
      :rooms="rooms"
      :allBookings="allBookings"
      :bookingId="null"
      submitLabel="Create booking"
      @submit="handleSubmit"
      @cancel="handleCancel"
    />
  </section>
</template>

<style scoped>
.errors { color: #c92a2a; margin-bottom: 1rem }
</style>
