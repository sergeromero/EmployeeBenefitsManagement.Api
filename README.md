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

The following users are created if they do not already exist (only in Development mode):

* hr1@test.com
* hr2@test.com
* employee1@test.com
* employee2@test.com

These users all have the same password: Password123!

A default administrator account is also created using credentials stored in User Secrets.

The seeding process is idempotent, allowing the application to start multiple times without creating duplicate roles or users.

---

## 

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

Configure the authentication user Secrets
```bash
dotnet user-secrets set "Jwt:Key" "your-very-long-secret-key-32+chars"
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

On startup the application will seed the required Identity roles, the default administrator account
and, if running in Development mode, some test users.

### 6. Using the API (Authentication Required)

All employee-related endpoints are protected using authentication and require a valid authenticated user.

Authentication Model
The application uses ASP.NET Core Identity for user management and authentication.

At this stage of the project:

Authentication is handled via Identity.
A default administrator user is seeded at startup (see Configure User Secrets)

With the applicatin running you can authenticate and get the token by running:

```bash
curl https://localhost:7129/api/auth/login -H "Content-Type: application/json" -d "{\"email\": \"youradmin@something.com\", \"password\": \"your-password\"}
```

Alternatively you can use the included HealthBenefitsPortal.http file to authenticate and test the endpoints.

Test users are also created during project startup, but they need to be configured before available to use. 
To test the application with these users follow these steps with the received administrator token
(the simplest way of doing it is with the HealthBenefitsPortal.http file included in the project):

1. Run the end point "https://localhost:7129/api/users" to get the users' Ids.
2. Run the end point "https://localhost:7129/api/users/assign-role" with each user Id to 
   assign the roles as follows:

| User | Role |
| ---- | ---- |
| hr1@test.com | HR |
| hr2@test.com | HR |
| employee1@test.com | Employee |
| employee2@test.com | Employee |

Optionally, by creating Employees and assigning them these users you will have additional functionality available in the application 
i.e. seeing the name of the employee displayed in the Header after log in.

1. Create an employee using the post endpoint "https://localhost:7129/api/employees"
2. Assign the newly created employee one of the test users by opening the database with Microsoft SQL Server Management Studio and running
the following SQL statement:

```
  UPDATE [HealthBenefits].[dbo].[Employees]
  SET UserId = (SELECT Id FROM [HealthBenefits].[dbo].[IdentityUsers] AS iu WHERE iu.Email = 'test-user-emai')
  WHERE Id = employee-Id
```

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
