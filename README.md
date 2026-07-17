# Coding Assessment

Please read [instructions.md](instructions.md) carefully before you begin.

## Getting Started

1. Read through [instructions.md](instructions.md) in full before writing any code.
2. Follow all requirements and constraints outlined in the instructions.

## Your README
Preliminary project structure

```
MeetingRoomBooking.Api/
|----Controllers/
|    |----BookingsController
|    |----RoomsController
|----DTOs/
|----Services/
|    |----BookingService
|----Persistance/
|    |----MeetingRoomDbContext
|    |----BookingRepository
|    |----SeedData
|----Validators/
|----Tests/

meeting-room-client/
|----Components/
|----Services/
|----Types/
```

## Entities

```
Room
- Id
- Name
- Capacity
```

```
Booking
- Id
- Title
- RoomId
- Room
- StartTime
- EndTime
```

## API

### Rooms
`GET /rooms`

### Bookings
`GET /bookings`

`GET /bookings/{id}`

`POST /bookings`

`PUT /bookings/{ID}`

`DELETE /bookings/{ID}`


## Booking Service
Responsible for:
```
CreateBooking()
UpdateBooking()
DeleteBooking()
GetBookings()
ValidateBooking()
```