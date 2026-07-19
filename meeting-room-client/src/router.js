import { createRouter, createWebHistory } from 'vue-router'
import Home from './views/Home.vue'
import RoomDetails from './views/RoomDetails.vue'
import EditBooking from './views/EditBooking.vue'
import CreateBooking from './views/CreateBooking.vue'

const routes = [
  { path: '/', name: 'home', component: Home },
  { path: '/rooms/:id', name: 'room', component: RoomDetails, props: true },
  { path: '/bookings/:id/edit', name: 'edit-booking', component: EditBooking, props: true },
  { path: '/rooms/:id/create', name: 'create-booking', component: CreateBooking, props: true }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
