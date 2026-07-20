# Coding Assessment

Please read [instructions.md](instructions.md) carefully before you begin.

This project is a applications that allows users to manage meeting room bookings within a small office environment.
It consists of a Vue front end client, a .NET API, and uses SQLite for persistance.

## How to Run the app

### Backend

1. Open a terminal in `MeetingRoom.API`
2. Run:

```bash
cd MeetingRoom.API
dotnet restore
dotnet run
```

3. The API starts on HTTPS, typically at `https://localhost:7006`. This can be changed in the Luanchsettings.json file.

### Frontend

1. Open a terminal in `meeting-room-client`
2. Run:

```bash
cd meeting-room-client
npm install
npm run dev
```

3. The frontend runs on `http://localhost:5173` by default.

### Use the app

- Open the Vite URL shown in the terminal.
- Browse rooms and bookings.
- Create, edit, and delete bookings.

## Project structure

```
MeetingRoom.API/
|-- Controllers/
|   |-- BookingsController.cs
|   |-- RoomsController.cs
|-- DTOs/
|-- Persistance/
|-- Services/
|-- Properties/
|-- appsettings.json

meeting-room-client/
|-- src/
|   |-- api/
|   |-- components/
|   |-- validators/
|   |-- views/
|-- package.json
|-- vite.config.js
```

## Booking Service
The booking service is where the main logic for the application lies. 

Responsible for:
```
CreateBooking()
UpdateBooking()
DeleteBooking()
GetBookings()
ValidateBooking()
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
The api definitions can be viewed with the Swagger-UI by running the API project going to `https://localhost:7006/swagger/index.html`,

Also included in this project is the [Bruno API Client](https://www.usebruno.com/) files that define a collection which devs can use to interact with with API.

### Rooms
`GET /Rooms`

### Bookings
`GET /Bookings`

`GET /Bookings/{id}`

`POST /Bookings`

`PUT /Bookings/{id}`

`DELETE /Bookings/{id}`

