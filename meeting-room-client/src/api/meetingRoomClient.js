import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7006',
  headers: {
    'Content-Type': 'application/json'
  }
})

export async function getBookings() {
  const response = await api.get('/Bookings')
  return response.data
}

export async function getBooking(id) {
  const response = await api.get(`/Bookings/${id}`)
  return response.data
}

export async function createBooking(createBookingDto) {
  const response = await api.post('/Bookings', createBookingDto)
  return response.data
}

export async function updateBooking(id, updateBookingDto) {
  const response = await api.put(`/Bookings/${id}`, updateBookingDto)
  return response.data
}

export async function deleteBooking(id) {
  await api.delete(`/Bookings/${id}`)
}

export async function getRooms() {
  const response = await api.get('/Rooms')
  return response.data
}
