<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getBooking, getBookings, getRooms, updateBooking } from '../api/meetingRoomClient'
import { validateAll } from '../validators/bookingValidators'

const route = useRoute()
const router = useRouter()
const id = Number(route.params.id)

const booking = ref(null)
const rooms = ref([])
const allBookings = ref([])
const errors = ref([])
const loading = ref(true)

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

function fromLocalInput(value) {
  const d = new Date(value)
  const pad = (n) => String(n).padStart(2, '0')
  const YYYY = d.getFullYear()
  const MM = pad(d.getMonth() + 1)
  const DD = pad(d.getDate())
  const hh = pad(d.getHours())
  const mm = pad(d.getMinutes())
  const ss = pad(d.getSeconds())
  const tzMin = -d.getTimezoneOffset() // minutes ahead of UTC
  const sign = tzMin >= 0 ? '+' : '-'
  const tzAbs = Math.abs(tzMin)
  const tzh = pad(Math.floor(tzAbs / 60))
  const tzm = pad(tzAbs % 60)
  return `${YYYY}-${MM}-${DD}T${hh}:${mm}:${ss}${sign}${tzh}:${tzm}`
}

const load = async () => {
  loading.value = true
  try {
    booking.value = await getBooking(id)
    rooms.value = await getRooms()
    allBookings.value = await getBookings()
    // initialize form fields
    form.value.title = booking.value.title
    form.value.roomId = booking.value.roomId
    form.value.startTime = toLocalInput(booking.value.startTime)
    form.value.endTime = toLocalInput(booking.value.endTime)
  } catch (e) {
    errors.value = [e?.message || 'Failed to load booking']
  } finally {
    loading.value = false
  }
}

onMounted(load)

const form = ref({ title: '', roomId: null, startTime: '', endTime: '' })

const submit = async () => {
  errors.value = []
  const startIso = fromLocalInput(form.value.startTime)
  const endIso = fromLocalInput(form.value.endTime)
  const validationErrors = validateAll(id, form.value.title, Number(form.value.roomId), startIso, endIso, allBookings.value)
  if (validationErrors.length) {
    errors.value = validationErrors
    return
  }

  const payload = {
    id: id,
    title: form.value.title,
    roomId: Number(form.value.roomId),
    startTime: startIso,
    endTime: endIso
  }

  try {
    await updateBooking(id, payload)
    router.push({ name: 'room', params: { id: payload.roomId } })
  } catch (e) {
    errors.value = [e?.message || 'Failed to update booking']
  }
}
</script>

<template>
  <section>
    <p v-if="loading">Loading booking…</p>
    <div v-if="errors.length" class="errors">
      <ul>
        <li v-for="(err, idx) in errors" :key="idx">{{ err }}</li>
      </ul>
    </div>

    <form v-if="!loading" @submit.prevent="submit">
      <div>
        <label>Title</label>
        <input v-model="form.title" />
      </div>

      <div>
        <label>Room</label>
        <select v-model="form.roomId">
          <option v-for="r in rooms" :key="r.id" :value="r.id">{{ r.name || `Room ${r.id}` }}</option>
        </select>
      </div>

      <div>
        <label>Start</label>
        <input type="datetime-local" v-model="form.startTime" />
      </div>

      <div>
        <label>End</label>
        <input type="datetime-local" v-model="form.endTime" />
      </div>

      <div>
        <button type="submit">Save</button>
        <button type="button" @click="router.back()">Cancel</button>
      </div>
    </form>
  </section>
</template>

<style scoped>
.errors { color: #c92a2a; margin-bottom: 1rem }
label { display: block; margin-top: 0.5rem }
input, select { width: 100%; padding: 0.4rem; margin-top: 0.25rem }
button { margin-top: 1rem; margin-right: 0.5rem }
</style>
