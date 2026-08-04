import * as yup from 'yup'

function rangesOverlap(aStart, aEnd, bStart, bEnd) {
  return aStart < bEnd && aEnd > bStart
}

export const bookingSchema = yup.object({
  title: yup
    .string()
    .trim()
    .required('Title must not be empty.'),

  roomId: yup
    .number()
    .typeError('Room selection is required.')
    .required('Room selection is required.')
    .test('room-exists', 'Selected room does not exist.', function (roomId) {
      const rooms = this.options.context?.rooms ?? []
      return rooms.some((room) => room.id === roomId)
    }),

  startTime: yup
    .date()
    .typeError('Invalid date/time format.')
    .required('Start and end times are required.')
    .min(new Date(), 'Start time must be in the future.'),

  endTime: yup
    .date()
    .typeError('Invalid date/time format.')
    .required('Start and end times are required.')
    // `this.parent` gives you the sibling fields on the same object (startTime here).
    .test('is-after-start', 'Start time must be before end time.', function (endTime) {
      const { startTime } = this.parent
      if (!startTime || !endTime) return true // let .required()/.typeError() report that instead
      return endTime > startTime
    })
    .test('room-available', 'Room is already booked for the selected time range.', function (endTime) {
      const { startTime, roomId } = this.parent
      const { allBookings = [], bookingId = null } = this.options.context ?? {}
      if (!startTime || !endTime || roomId == null) return true

      return !allBookings.some((booking) => {
        if (booking.id === bookingId) return false // ignore the booking we're editing
        if (booking.roomId !== Number(roomId)) return false
        return rangesOverlap(startTime, endTime, new Date(booking.startTime), new Date(booking.endTime))
      })
    })
})
