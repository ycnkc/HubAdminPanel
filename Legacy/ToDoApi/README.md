# Todo API (Layered Architecture)

A robust and scalable To-Do Management API built with **.NET 8**, designed with a layered architecture.

## Technologies
* **.NET 8** (ASP.NET Core Web API)
* **Entity Framework Core** (ORM)
* **MS SQL Server** (Database)
* **Swagger/OpenAPI** (Documentation)

## Architectural Overview
This project follows the **Separation of Concerns** principle to ensure maintainability and testability. The structure is divided into several distinct layers:

* **Models**: Contains the core entities representing the database schema, such as `TodoItem`.
* **Data**: Handles database connectivity and configurations via `AppDbContext`.
* **Repositories**: Abstracts data access logic using the Repository Pattern, implemented in `TodoRepository`.
* **Services**: Manages business logic and validation rules through the `TodoService`.
* **DTOs (Data Transfer Objects)**: Optimizes data transfer between the client and the server using classes like `TodoCreateDto`.

