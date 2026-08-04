<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import BookingForm from '../components/BookingForm.vue'
import { getBooking, getBookings, getRooms, updateBooking } from '../api/meetingRoomClient'

const route = useRoute()
const router = useRouter()
const id = Number(route.params.id)

const rooms = ref([])
const allBookings = ref([])
const errors = ref([])
const loading = ref(true)
const initialForm = ref({ title: '', roomId: null, startTime: '', endTime: '' })

function toLocalInput(iso) {
  if (!iso) return ''
  const d = new Date(iso)
  const pad = (n) => String(n).padStart(2, '0')
  const YYYY = d.getFullYear()
  const MM = pad(d.getMonth() + 1)
  const DD = pad(d.getDate())
  const hh = pad(d.getHours())
  const mm = pad(d.getMinutes())
  return `${YYYY}-${MM}-${DD}T${hh}:${mm}`
}

const load = async () => {
  loading.value = true
  try {
    const [booking, roomResults, bookingResults] = await Promise.all([getBooking(id), getRooms(), getBookings()])
    rooms.value = roomResults
    allBookings.value = bookingResults
    initialForm.value = {
      title: booking.title,
      roomId: booking.roomId,
      startTime: toLocalInput(booking.startTime),
      endTime: toLocalInput(booking.endTime)
    }
  } catch (e) {
    errors.value = [e?.message || 'Failed to load booking']
  } finally {
    loading.value = false
  }
}

onMounted(load)

const handleSubmit = async (payload) => {
  errors.value = []
  try {
    await updateBooking(id, { id, ...payload })
    router.push({ name: 'room', params: { id: payload.roomId } })
  } catch (e) {
    errors.value = [e?.message || 'Failed to update booking']
  }
}

const handleCancel = () => {
  router.back()
}
</script>

<template>
  <section class="page-container form-page">
    <div class="page-header">
      <h2>Edit booking</h2>
      <p class="page-subtitle">Update the details of this booking.</p>
    </div>

    <p v-if="loading" class="loading-text">Loading booking…</p>
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
        :bookingId="id"
        submitLabel="Save"
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
