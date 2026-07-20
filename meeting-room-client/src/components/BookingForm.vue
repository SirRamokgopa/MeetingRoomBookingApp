<script setup>
import { reactive, ref, watch } from 'vue'
import { validateAll } from '../validators/bookingValidators'

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

const submitForm = () => {
  errors.value = []
  const startIso = fromLocalInput(form.startTime)
  const endIso = fromLocalInput(form.endTime)
  const validationErrors = validateAll(
    props.bookingId,
    form.title,
    Number(form.roomId),
    startIso,
    endIso,
    props.allBookings,
    props.rooms
  )
  if (validationErrors.length) {
    errors.value = validationErrors
    return
  }

  emit('submit', {
    title: form.title,
    roomId: Number(form.roomId),
    startTime: startIso,
    endTime: endIso
  })
}
</script>

<template>
  <div>
    <div v-if="errors.length" class="errors">
      <ul>
        <li v-for="(err, idx) in errors" :key="idx">{{ err }}</li>
      </ul>
    </div>

    <form @submit.prevent="submitForm">
      <div>
        <label>Title</label>
        <input v-model="form.title" />
      </div>

      <div>
        <label>Room</label>
        <select v-model="form.roomId">
          <option :value="null">Select room</option>
          <option v-for="r in rooms" :key="r.id" :value="r.id">
            {{ r.name || `Room ${r.id}` }}
          </option>
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

      <div class="actions">
        <button type="submit">{{ submitLabel }}</button>
        <button type="button" @click="$emit('cancel')">Cancel</button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.errors { color: #c92a2a; margin-bottom: 1rem }
label { display: block; margin-top: 0.5rem }
input, select { min-width: 20%; padding: 0.4rem; margin-top: 0.25rem }
.actions { margin-top: 1rem }
button { margin-right: 0.5rem }
</style>
