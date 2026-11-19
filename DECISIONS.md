# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

Rule R7. It lived in Service/BookingService.cs. Now it lives in Room.Book() in Domain/Rooms/Room.cs. Rooms own its booking, so it's the only one that can see them and decide.

## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

An occupied room is an expected business case, not a crash. Exceptions are for unexpected things.
Room.Book returns BookingErrors.Overlapping.

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

Everything in Infrastructure/Persistence knows about it. All of Domain/ (Room, Booking IRoomRepository, IUnitOfWork) and the command handlers don't know.
 Gain: The domain is clean and could be tested without a database.
 Pay: more files and interfaces.

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

POST /api/rooms/{roomId}/bookings -> RoomsEndpoints.MapRooms -> CreateBookingEndpoint.Handle

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

It sugested case-insensitively for R9, and I chose case-sensitive.

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

- Unit tests for Room.Book and DateRange, using a fake TimeProvider
- EF migrations instead of EnsureCreated
