<script setup>
import { reactive, ref, watch } from 'vue'
import { bookingSchema } from '../validators/bookingSchema'

const props = defineProps({
  initialForm: { type: Object, required: true },
  rooms: { type: Array, required: true },
  allBookings: { type: Array, required: true },
  bookingId: { type: [Number, String], default: null },
  submitLabel: { type: String, default: 'Save' }
})
const emit = defineEmits(['submit', 'cancel'])

const form = reactive({
  title: '',
  roomId: null,
  startTime: '',
  endTime: ''
})
const errors = ref([])

function syncForm(value) {
  if (!value) return
  form.title = value.title ?? ''
  form.roomId = value.roomId ?? null
  form.startTime = value.startTime ?? ''
  form.endTime = value.endTime ?? ''
}

watch(
  () => props.initialForm,
  (next) => {
    if (next) syncForm(next)
  },
  { immediate: true, deep: true }
)

function fromLocalInput(value) {
  const d = new Date(value)
  const pad = (n) => String(n).padStart(2, '0')
  const YYYY = d.getFullYear()
  const MM = pad(d.getMonth() + 1)
  const DD = pad(d.getDate())
  const hh = pad(d.getHours())
  const mm = pad(d.getMinutes())
  const ss = pad(d.getSeconds())
  const tzMin = -d.getTimezoneOffset()
  const sign = tzMin >= 0 ? '+' : '-'
  const tzAbs = Math.abs(tzMin)
  const tzh = pad(Math.floor(tzAbs / 60))
  const tzm = pad(tzAbs % 60)
  return `${YYYY}-${MM}-${DD}T${hh}:${mm}:${ss}${sign}${tzh}:${tzm}`
}

const submitForm = async () => {
  errors.value = []
  const startIso = fromLocalInput(form.startTime)
  const endIso = fromLocalInput(form.endTime)

  const candidate = {
    title: form.title,
    roomId: form.roomId === null || form.roomId === '' ? null : Number(form.roomId),
    startTime: startIso,
    endTime: endIso
  }

  try {
    await bookingSchema.validate(candidate, {
      abortEarly: false,
      context: { rooms: props.rooms, allBookings: props.allBookings, bookingId: props.bookingId }
    })
  } catch (validationError) {
    errors.value = validationError.errors
    return
  }

  emit('submit', candidate)
}
</script>

<template>
  <div>
    <div v-if="errors.length" class="errors">
      <ul>
        <li v-for="(err, idx) in errors" :key="idx">{{ err }}</li>
      </ul>
    </div>

    <form class="booking-form" @submit.prevent="submitForm">
      <div class="field field-full">
        <label>Title</label>
        <input v-model="form.title" placeholder="e.g. Sprint planning" />
      </div>

      <div class="field">
        <label>Room</label>
        <select v-model="form.roomId">
          <option :value="null">Select room</option>
          <option v-for="r in rooms" :key="r.id" :value="r.id">
            {{ r.name || `Room ${r.id}` }}
          </option>
        </select>
      </div>

      <div class="field">
        <label>Start</label>
        <input type="datetime-local" v-model="form.startTime" />
      </div>

      <div class="field">
        <label>End</label>
        <input type="datetime-local" v-model="form.endTime" />
      </div>

      <div class="actions field-full">
        <button type="submit" class="btn-primary">{{ submitLabel }}</button>
        <button type="button" class="btn-ghost" @click="$emit('cancel')">Cancel</button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.booking-form {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0 1.25rem;
}

.field-full {
  grid-column: 1 / -1;
}

.field label {
  margin-top: 0;
}

.actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 1.5rem;
}

@media (max-width: 480px) {
  .booking-form {
    grid-template-columns: 1fr;
  }
}
</style>
