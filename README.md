This project is a backend API for managing a doctor’s practice calendar.
It allows events (appointments) to be created, updated, cancelled, and queried, along with attendees and their attendance status.

Architecture Overview
src/
 DoctorCalendar.Api           → HTTP API, controllers, OpenAPI
 DoctorCalendar.Application   → CQRS commands, queries, handlers
 DoctorCalendar.Domain        → Domain entities, value objects, rules
 DoctorCalendar.Infrastructure→ EF Core, SQLite, repositories

Persistence
Database: SQLite
EF Core used for persistence


Running the Project

Run the API
dotnet run --project src/DoctorCalendar.Api

Run tests
dotnet test
