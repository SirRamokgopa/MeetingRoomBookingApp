export function validateTitle(title) {
  if (!title || !title.toString().trim()) return 'Title must not be empty.'
  return null
}

export function validateTimes(startIso, endIso) {
  if (!startIso || !endIso) return 'Start and end times are required.'
  const start = new Date(startIso)
  const end = new Date(endIso)
  if (isNaN(start.getTime()) || isNaN(end.getTime())) return 'Invalid date/time format.'
  if (start >= end) return 'Start time must be before end time.'
  const now = new Date()
  if (start < now) return 'Start time must be in the future.'
  return null
}

// bookings: array of existing bookings { id, roomId, startTime, endTime }
export function isRoomAvailable(bookingId, roomId, startIso, endIso, bookings) {
  const start = new Date(startIso)
  const end = new Date(endIso)
  for (const b of bookings) {
    if (b.id === bookingId) continue
    if (b.roomId !== roomId) continue
    const bStart = new Date(b.startTime)
    const bEnd = new Date(b.endTime)
    if (start < bEnd && end > bStart) return false
  }
  return true
}

export function validateAll(bookingId, title, roomId, startIso, endIso, bookings) {
  const errors = []
  const t = validateTitle(title)
  if (t) errors.push(t)
  const timeErr = validateTimes(startIso, endIso)
  if (timeErr) errors.push(timeErr)
  if (!isRoomAvailable(bookingId, roomId, startIso, endIso, bookings)) errors.push('Room is already booked for the selected time range.')
  return errors
}
