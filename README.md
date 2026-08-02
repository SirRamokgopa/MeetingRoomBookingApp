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

## Architecture

This solution follows a layered / clean-architecture style split into four .NET projects, so that business rules stay independent of frameworks, persistence, and delivery mechanisms:

```
MeetingRoom.Domain          <- pure domain entities, no external dependencies
MeetingRoom.Application     <- application/business logic, repository abstractions, DTOs
MeetingRoom.Infrastructure  <- EF Core persistence, repository implementations, mappers
MeetingRoom.API             <- ASP.NET Core Web API (controllers, composition root)
MeetingRoom.Tests           <- MSTest unit tests
```

Dependencies only ever point inward: `API` and `Infrastructure` depend on `Application`, which depends on `Domain`. `Domain` depends on nothing. This means the core business rules (booking validation, conflict detection) don't know or care that EF Core, SQLite, or ASP.NET Core exist.

### Repository pattern

`MeetingRoom.Application` defines `IBookingRepository` / `IRoomRepository` as abstractions. `MeetingRoom.Infrastructure` provides the EF Core-backed implementations (`BookingRepository`, `RoomRepository`), which are registered for DI via `AddInfrastructureRepositories()` and injected into `BookingService`. This keeps persistence concerns (and EF Core itself) out of the application layer entirely.

### Domain vs. persistence entities

EF Core has its own persistence models (`BookingEntity`, `RoomEntity` in `MeetingRoom.Infrastructure/Persistance/Entities`) that are kept separate from the plain domain entities (`Booking`, `Room` in `MeetingRoom.Domain/Entities`). Mapper classes (`BookingMapper`, `RoomMapper`) translate between the two, so the domain model never has to carry EF Core attributes, navigation-property quirks, or change-tracking concerns.

### Error handling & logging

Repositories catch persistence-specific failures (`DbUpdateException` on writes, general `Exception` on reads), log them via `ILogger<T>`, and re-throw as a domain-friendly `InvalidOperationException` so callers don't need to know about EF Core exception types.

## Project structure

```
MeetingRoom.Domain/
|-- Entities/
|   |-- Booking.cs
|   |-- Room.cs

MeetingRoom.Application/
|-- Abstractions/
|   |-- Repositories/
|       |-- IBookingRepository.cs
|       |-- IRoomRepository.cs
|-- DTOs/
|   |-- Booking.cs        (BookingDTO, CreateBookingDTO, UpdateBookingDTO)
|   |-- Room.cs            (RoomDTO)
|-- Services/
|   |-- BookingService.cs
|   |-- IBookingService.cs
|   |-- BookingExtentions.cs   (DTO <-> Domain mapping)

MeetingRoom.Infrastructure/
|-- Persistance/
|   |-- MeetingRoomContext.cs
|   |-- DbSeeder.cs
|   |-- Entities/           (BookingEntity, RoomEntity - EF Core models)
|   |-- Data/               (SQLite database file)
|-- Repositories/
|   |-- BookingRepository.cs
|   |-- RoomRepository.cs
|-- Mappings/
|   |-- BookingMapper.cs    (Domain <-> EF entity mapping)
|   |-- RoomMapper.cs
|-- Migrations/
|-- InfrastructureServiceCollectionExtensions.cs

MeetingRoom.API/
|-- Controllers/
|   |-- BookingsController.cs
|   |-- RoomsController.cs
|-- Properties/
|-- Program.cs
|-- appsettings.json

MeetingRoom.Tests/
|-- BookingServiceTests.cs
|-- BookingRepositoryTests.cs
|-- RoomRepositoryTest.cs

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
The booking service (`MeetingRoom.Application/Services/BookingService.cs`) is where the main application logic lives, orchestrating repository calls and enforcing business rules.

Responsible for:
```
CreateBooking()
UpdateBooking()
DeleteBookingAsync()
GetBookings()
GetBookingById()
GetRooms()
ValidateBooking()
```

> **Note:** A CQRS-style `UseCases/` structure (one `Command` + `Handler` + `Validator` per operation, with an `OperationResult` wrapper to avoid throwing exceptions for expected validation/conflict failures) was considered as a further refinement to more closely mirror a typical enterprise Clean Architecture template. For the current scope, a single cohesive `BookingService` was kept instead — this remains a candidate for a future branch.

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

## Tests

Unit tests live in `MeetingRoom.Tests` and use MSTest with EF Core's in-memory provider:

- `BookingServiceTests.cs` — tests the application service end-to-end (validation, conflict detection, CRUD behavior).
- `BookingRepositoryTests.cs` / `RoomRepositoryTest.cs` — tests the EF Core repository implementations directly.

Run them with:

```bash
dotnet test MeetingRoom.Tests/MeetingRoom.Tests.csproj
```

