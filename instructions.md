# DVT Meeting Room Booking Challenge

Your task is to develop a full stack application in C# and JavaScript or TypeScript that allows users to manage meeting room bookings within a small office environment. The backend must be built as a .NET API, and the frontend must consume that API. For this challenge, the frontend should be implemented in Vue, but the backend should remain independent of the frontend technology so that it could be replaced with another client framework in future.

The purpose of this challenge is to assess your ability to design and build a well-structured, maintainable, API-first full stack solution. Your submission should demonstrate sound software design principles, clear separation of concerns, good validation and error handling, and a user experience that is simple and effective.

This challenge is intentionally scoped to be completed in a reasonable amount of time alongside normal work commitments. We are not looking for a large feature set or a production-complete system. We are looking for a thoughtful implementation of the required functionality, backed by clean code and good technical decisions.

## Key Application Features

### 1. Meeting Room Management

The system should provide a predefined set of meeting rooms. Each room should contain enough information to support bookings, such as a unique identifier, a name, and a capacity.

Rooms may be seeded into the system rather than managed through an admin interface. The goal is not to test room administration, but rather the booking workflow itself.

### 2. View Existing Bookings

Users must be able to view existing bookings for meeting rooms. The system should make it clear which room is booked, for what date and time, and any other relevant information needed to understand the current schedule.

The frontend should present this information in a clear and usable manner. The user should not need to inspect raw API responses to understand the room schedule.

### 3. Create a Booking

Users must be able to create a new booking for a room by providing the required information. At a minimum, this should include the room being booked, the booking title or subject, and a start and end date and time.

The system must validate the booking before it is accepted.

### 4. Edit an Existing Booking

Users must be able to edit an existing booking. This should allow changes such as updating the title, changing the room, or adjusting the booking time.

The same validation rules that apply to booking creation must also apply when a booking is edited.

### 5. Cancel a Booking

Users must be able to cancel an existing booking. This may be implemented as a hard delete or a cancellation status, provided the behaviour is clear and consistent throughout the application.

The frontend should provide meaningful feedback when a booking has been cancelled successfully.

### 6. Booking Conflict Prevention

The system must prevent overlapping bookings for the same room.

For example, if a room is already booked from 10:00 to 11:00, the API should reject an attempt to create or edit another booking in that same room during an overlapping period.

This is one of the key business rules of the challenge and should be handled reliably.

### 7. Input Validation

The application should validate user input and reject invalid requests. At a minimum, consider the following:

- The room must exist
- The booking must include required fields
- The start time must be before the end time
- Bookings may not overlap for the same room
- Edited bookings must remain valid after changes

Validation should be handled in a structured and maintainable way, and the user should receive clear feedback when validation fails.

### 8. Clear User Feedback

The application should provide good feedback to the user throughout the booking flow.

Examples include:

- Confirmation that a booking was created successfully
- Confirmation that a booking was updated successfully
- Confirmation that a booking was cancelled successfully
- Helpful validation messages when input is invalid
- Meaningful error messages when something goes wrong

Good user feedback is an important part of the assessment. Even though this is not a large application, the user should always understand what the system is doing.

### 9. API-First Design

The backend must expose a clear API that can be consumed by the Vue frontend. The design of the backend should not be tightly coupled to the frontend implementation.

Your API should be structured in a way that would allow another frontend framework, such as React or Angular, to consume it without requiring backend changes.

### 10. Full Stack Operation

The solution must include both:

- A .NET backend API
- A Vue frontend client

The frontend must communicate with the backend through the API rather than bypassing it with shared storage, server-side rendering shortcuts, or tightly coupled implementation details.

Your solution should demonstrate an understanding of both backend and frontend development, including data flow, validation, state handling, and user interaction.

## Scope Assumptions

To keep the challenge focused and achievable, please work within the following assumptions:

- Authentication and authorization are not required
- Rooms may be seeded with sample data
- Only a single office location needs to be supported
- Recurring bookings are not required
- Email notifications are not required
- Realtime updates are not required
- Advanced calendar visualisation is not required

You are welcome to add more features, but the core required functionality should be completed first.

## Submission Review Criteria

Those reviewing your submission consider the following:

### GitHub Submission

- The front-end and back-end solution should both be in the repo provided.
- Don't create a separate repo, make use to use the one these instructions are in. 
- `.gitignore` present
- Code committed using Git commands, not uploaded as a single dump
- Main branch builds and runs
- Meaningful commit messages
- Informative README present
- Bonus: tags, branching strategy, automated builds

### Comments and Documentation

- Informative README present
- Code comments in line with generally accepted industry guidelines
- Public APIs and important behaviours documented where appropriate
- Setup instructions are clear and complete

### Solution Structure

Project or projects should be structured according to clean architecture or a similarly sensible separation of concerns.

The purpose of this is not to force a specific folder structure, but to encourage maintainable and understandable code. A well-structured solution should make it clear where business rules live, how the API is exposed, how persistence is handled, and how the frontend is organised.

### API Design

Reviewers will consider whether the API is clear, predictable, and appropriately designed for the problem.

This includes:

- Sensible endpoints
- Appropriate request and response models
- Correct use of HTTP verbs and status codes
- Clear validation responses
- A design that is usable from any frontend client

### Frontend Implementation

Reviewers will consider whether the frontend is well structured and easy to follow.

This includes:

- Clear component design
- Sensible data fetching
- State handled appropriately
- Forms that are easy to use
- Good handling of loading, success, and error states

### SOLID

**Single Responsibility Principle**

Each class, component, or module in the application should have a clear and focused responsibility.

For example, booking validation, persistence, API endpoint handling, and frontend display concerns should not be unnecessarily mixed together.

**Open/Closed Principle**

Design the application so that behaviour can be extended without needing to constantly modify existing code.

For example, the system should be open to future additions such as room equipment, room availability filters, or support for another frontend client without requiring a rewrite of core booking logic.

**Liskov Substitution Principle**

Where inheritance or abstraction is used, derived implementations should behave in a way that remains compatible with the contracts established by the parent type.

**Interface Segregation Principle**

Interfaces should remain focused and relevant to the code that depends on them. Avoid creating overly broad contracts that force classes to implement behaviour they do not need.

**Dependency Inversion Principle**

Depend on abstractions rather than concrete implementations where appropriate. This helps make the solution easier to test, maintain, and evolve.

By adhering to SOLID principles, your application will have a clearer and more maintainable structure, making it easier to reason about and extend in future.

### Unit Tests

Unit tests play an important role in this challenge, as they do in any software project.

Tests should provide confidence that the core booking behaviour works as expected. A submission does not need exhaustive test coverage, but it should include meaningful tests around the most important business logic.

At a minimum, consider testing:

- Creating a valid booking
- Rejecting overlapping bookings
- Rejecting invalid time ranges
- Editing a booking while preserving business rules
- Cancelling a booking
- Validation and error scenarios

Good unit tests improve code quality, provide a safety net for change, and show that the solution has been built with maintainability in mind.

### Logic and Implementation Guidance

When implementing the challenge, consider the following general practices:

- Use built-in collections and language features appropriately
- Avoid unnecessary duplication
- Keep business rules explicit and easy to locate
- Use descriptive names for variables, methods, classes, and components
- Avoid magic values where constants or enums would improve clarity
- Handle exceptions gracefully
- Return clear errors from the API
- Keep the frontend logic understandable and maintainable
- Ensure the system is simple to run locally

The goal is not to produce the most complex solution. The goal is to produce a solution that is correct, clear, and well designed.

### C# Standards

C# coding standards are important for consistency, readability, maintainability, and code quality.

In this challenge, following good standards will help demonstrate that you can build software that is easy for others to understand and work on. This includes:

- Consistent naming conventions
- Clear class and method structure
- Sensible use of comments
- Appropriate exception handling
- Readable formatting
- Maintainable organisation of code

The backend should reflect modern .NET development practices and should be easy for another developer to navigate.

### Frontend Standards

The frontend should also follow good engineering practices.

This includes:

- Meaningful component and file naming
- Clear separation between UI, state handling, and API interaction
- Use of a state management library appropriate to the chosen framework
- Schema validation on the frontend using a library such as Zod, Yup, or Typebox, kept consistent with the backend validation contract
- Sensible reuse of components where appropriate
- Readable layout and styling
- Clear handling of forms and validation
- A user experience that is functional and easy to understand

The frontend does not need to be visually elaborate, but it should be polished enough to demonstrate care and usability.

### Validation

Validation is a critical part of this challenge.

The application should prevent invalid bookings from being processed and should provide clear messages when validation fails. Validation should protect the integrity of the system and improve the overall user experience.

Examples of scenarios that should be validated include:

- Missing required data
- Invalid room identifiers
- Start time later than end time
- Overlapping room bookings
- Editing a booking into an invalid state

Validation may be implemented using any appropriate approach or library, provided it is consistent and maintainable.

### Error Handling

Error handling is essential for building a robust and user-friendly solution.

The application should handle invalid operations gracefully and avoid crashing or exposing confusing behaviour to the user. When an error occurs, the system should communicate clearly what went wrong.

Good error handling may include:

- Appropriate API status codes
- Structured validation responses
- Meaningful exception handling
- Useful user-facing error messages
- Logging where appropriate

The system should fail gracefully and remain understandable from both a developer and user perspective.

### User Feedback

User interaction and good system feedback are important parts of this challenge.

The user should be able to understand:

- Which rooms exist
- Which bookings already exist
- Whether a booking was created, updated, or cancelled successfully
- Why a request failed if it was invalid
- When data is loading

Even in a small internal business application, clarity matters. A candidate who provides clear, reliable feedback to the user demonstrates strong full stack thinking.

## Technical Expectations

Your solution should include:

- A .NET API
- A Vue frontend
- Local persistence using a simple approach such as SQLite or In Memory store
- Sample seeded room data
- Instructions to run the solution locally
- Meaningful unit tests for core behaviour
- Interactive API documentation via Swagger or OpenAPI, exposed alongside the running API

You may use additional libraries and tools where appropriate, but the final solution should remain easy to run and assess.

## Optional Enhancements

The following are optional and are not required for a successful submission:

- Filtering bookings by room or date
- Improved room availability views
- Integration tests
- Docker support
- CI pipeline
- Improved styling or responsive behaviour
- Additional domain features that do not distract from the core challenge
- An event-driven approach to bookings, for example domain events raised when a booking is created, updated, or cancelled
- Deeper backend work such as scheduling or optimisation algorithms, considered concurrency or threading, or measurable time and space complexity improvements

Optional features should not come at the expense of completing the required functionality well.

## Submission Notes

Please include a README that explains:

- The overall solution structure
- How to run the backend
- How to run the frontend
- Any assumptions made
- Any trade-offs or limitations
- How to run the tests

We value clear thinking and good engineering judgement. A smaller, well-executed solution is better than a larger but incomplete one.

## Summary

This challenge is intended to assess your ability to build a well-structured full stack application using a .NET API and a frontend client. It focuses on API design, business rules, validation, testing, frontend integration, and maintainable code.

A successful submission will demonstrate clear technical thinking, a sensible architecture, and a user experience that makes the booking workflow easy to understand and use.

- Submit your solution in this repository only.
- Do not create a new repository under your own GitHub account.
- Do not split the backend and frontend into separate repositories.
- The .NET API and Vue frontend must both live inside this single provided repository.
- Submissions that place the solution in another repo, or split the solution across multiple repos, may be considered incomplete.
