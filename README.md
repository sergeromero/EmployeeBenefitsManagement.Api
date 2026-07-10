# Employee Benefits API

A production-oriented ASP.NET Core Web API that demonstrates enterprise application architecture and development practices using modern .NET technologies.

The project is designed as a portfolio application that emphasizes maintainability, clean architecture, security, and extensibility rather than serving as a simple CRUD sample.

---

## Technology Stack

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* MediatR
* FluentValidation
* Clean Architecture
* CQRS

---

## Solution Architecture

The solution follows Clean Architecture principles and is organized into the following layers:

### API

Responsible for:

* HTTP endpoints
* Request pipeline
* Global exception handling
* Dependency composition
* Infrastructure initialization

### Application

Contains:

* Use cases
* CQRS commands and queries
* Validation
* Behaviors
* Application contracts

### Domain

Contains the business model:

* Entities
* Value objects
* Domain rules
* Domain exceptions

The Domain layer has no dependency on any infrastructure technology.

### Infrastructure

Contains all technical implementations, including:

* Entity Framework Core
* SQL Server persistence
* ASP.NET Core Identity
* Repository implementations
* Configuration
* Startup initialization

---

## Current Features

### Employee Management

* Create Employee
* Retrieve Employee by Id

### Benefits Domain

* Departments
* Employees
* Benefit Types
* Benefit Plans
* Enrollment Categories
* Employee Enrollments

### Validation

* FluentValidation
* MediatR validation pipeline
* Centralized validation handling

### Exception Handling

* Global exception handler
* RFC 7807 Problem Details responses

### Authentication & Authorization

* ASP.NET Core Identity
* Secure password policy
* Account lockout policy
* Unique email enforcement
* Default administrator account seeding
* Role-based authorization foundation

---

## Identity

During application startup the system automatically verifies the existence of the required Identity data.

The following roles are created if they do not already exist:

* Administrator
* HR
* Employee

A default administrator account is also created using credentials stored in User Secrets.

The seeding process is idempotent, allowing the application to start multiple times without creating duplicate roles or users.

---

## Getting Started

### Prerequisites

* .NET 10 SDK
* SQL Server
* Visual Studio 2022 (or later) or Visual Studio Code

### 1. Clone the repository

```bash
git clone <repository-url>
```

### 2. Configure User Secrets

Initialize User Secrets for the API project:

```bash
dotnet user-secrets init --project <ApiProject>
```

Configure the default administrator credentials:

```text
Identity:DefaultAdministrator:UserName
Identity:DefaultAdministrator:Email
Identity:DefaultAdministrator:Password
```

### 3. Configure the database connection

Update the `DefaultConnection` connection string in `appsettings.Development.json`.

### 4. Apply the database migrations

```powershell
Update-Database
```

or

```bash
dotnet ef database update
```

### 5. Run the application

Start the API using Visual Studio or:

```bash
dotnet run
```

On startup the application will seed the required Identity roles and the default administrator account.

---

## Project Goals

This project demonstrates:

* Enterprise application architecture
* Separation of concerns
* Clean Architecture
* CQRS
* Dependency Injection
* Secure authentication foundation
* Production-oriented coding practices
* Maintainable and testable design

---

## Roadmap

Planned enhancements include:

* Employee update and deletion
* Authentication endpoints
* JWT bearer authentication
* Role-based authorization
* Benefit enrollment workflows
* Unit testing
* Integration testing
* Docker support
* CI/CD pipeline
* Azure deployment

---

## License

This project is intended as a portfolio and learning project demonstrating modern ASP.NET Core development practices.
