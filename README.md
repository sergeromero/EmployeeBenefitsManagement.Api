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
* Create Users
* Assign Users to Employees

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
* hr3@test.com
* employee1@test.com
* employee2@test.com
* employee3@test.com
* employee4@test.com
* employee5@test.com

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


## 1. Clone the GitHub Repository
 
### Step 1

Select a folder in your file system to clone the HealthBenefitsPortal repository
```bash
cd path-to-your-folder
```

### Step 2. 

Open a PowerShell or Command prompt and clone the HealthBenefitsPortal repository by running the following command:
```bash
git clone https://github.com/sergeromero/EmployeeBenefitsManagement.Api.git
```

## 2. Configure User-Secrets

The project is already configured to use **.NET User Secrets** through its
UserSecretsId. User Secrets keep sensitive development configuration
outside of source control.

On a new development machine, add the required values by following these steps:

### Step 1. Open a Developer Command Prompt or Developer PowerShell

From the Windows Start menu, search for either Developer Command Prompt for Visual Studio or Developer PowerShell for Visual Studio.

### Step 2. Navigate to the API project directory

From the Developer Command Prompt or Developer PowerShell, navigate to the API project directory. The HealthBenefitsPortal.csproj file should be located in this directory.
```bash
	cd EmployeeBenefitsManagement.Api/src/backend/healthbenefitsportal
```

> **Important:** The `dotnet user-secrets` commands must be run from the API project directory.

### Step 3. Add the required User Secrets

Run the following commands. You can use the example values shown below or replace them with your own values.

```bash
dotnet user-secrets set "Jwt:Key" "your-super-secure-key-here-32+chrs" 
```
```bash
dotnet user-secrets set "Identity:DefaultAdministrator:UserName" "admin"
```
```bash
dotnet user-secrets set "Identity:DefaultAdministrator:Password" "your-secure-admin-password"
```
```bash
dotnet user-secrets set "Identity:DefaultAdministrator:Email" "admin@mockdomain.com"
```

The administrator password is used only for local development. Choose a password appropriate for your local environment.

### Step 4. Verify the User Secrets (Optional)

Once you have run the commands, you can verify that the values were created successfully by running:

```bash
dotnet user-secrets list
```

> **Security note:** Do not commit User Secret values to source control. User Secrets are stored locally on the development machine and are not included in the Git repository.

These values are required by the application's authentication and initial administrator provisioning configuration. If the required values are not configured, the application may fail during startup or authentication/administrator provisioning may not work as expected.


## 3. Configure the Database

The application requires a SQL Server database. You can configure the connection string using one of the following approaches.

---

### Option 1 — User Secrets (Recommended)

This approach keeps sensitive configuration out of source control and is the preferred method for local development.
**The project is already configured to use User Secrets. Run these commands from the same API project directory used in the previous section.**

#### Step 1 — Set the Connection String

##### SQL Server (Local Default Instance)

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=HealthBenefitsPortalDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

##### SQL Server Express

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\\SQLEXPRESS;Database=HealthBenefitsPortalDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

---

### Option 2 — appsettings.Development.json (Visual Studio Friendly)

This option is useful for developers who prefer configuring connections through Visual Studio tools.

#### Step 1 — Open Server Explorer

1. In Visual Studio, go to **View → Server Explorer**
2. Right-click **Data Connections → Add Connection**
3. Choose:

   * **Server name**:

     * `localhost` (Full SQL Server)
     * `localhost\\SQLEXPRESS` (SQL Server Express)
   * Authentication: **Windows Authentication**
4. Select or create the database:

   * `HealthBenefitsPortalDb`
5. Click **Test Connection**, then **OK**

#### Step 2 — Copy the Connection String

After creating the connection:

1. Right-click the connection → **Properties**
2. Copy the **Connection String**

#### Step 3 — Update appsettings.Development.json

Add or update the following section:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING_HERE"
  }
}
```

>**Important**: If `ConnectionStrings:DefaultConnection` is configured in both User Secrets and `appsettings.Development.json`, the User Secrets value takes precedence. To use the connection string from appsettings.Development.json, remove the corresponding User Secrets entry.


---

### ⚙️ Notes

* The application uses **Entity Framework Core migrations** to create and update the database schema.
* The database will be created automatically when migrations are applied.
* Ensure that:

  * SQL Server is running
  * The instance name is correct (`localhost` vs `localhost\\SQLEXPRESS`)
  * You have sufficient permissions to create databases

---

### 🧪 Troubleshooting

* **Cannot connect to server**

  * Verify SQL Server service is running
  * Check instance name
* **Login failed**

  * Ensure correct authentication method
* **SSL / certificate errors**

  * Ensure `TrustServerCertificate=True` is present in the connection string

---

## 4. Create the Database

Once the connection string has been configured, you can create the database.

### Step 1. Open the HealthBenefitsPortal solution

From Visual Studio select "Open a project or solution". 
On the Open Project/Solution window, navigate to the solution directory. The HealthBenefitsPortal.slnx file should be located here:
```bash
EmployeeBenefitsManagement.Api/src/backend
```

> **Important:** Before running Entity Framework Core migrations, make sure HealthBenefitsPortal is selected as the solution's Startup Project.

In Visual Studio's Solution Explorer, expand the **20.API** folder, right-click **HealthBenefitsPortal** and select **Set as Startup Project**.

### Step 2. Open the NuGet Package Manager

The NuGet Package Manager Console can be found in the Tools -> NuGet Package Manager -> Package Manager Console menu

### Step 3. Select the Benefits.Infrastructure project

On the Package Manager Console select Benefits.Infrastructure from the 'Default project' dropdown.

### Step 4. Execute the Update-Database command

Run the following command:

```bash
Update-Database
```

## 5. Configure Development Database Seeding

The database seeding process is controlled by configuration settings in `appsettings.json` and environment-specific configuration files.

The default settings in `appsettings.json` intentionally have development seeding disabled. The `appsettings.Development.json` file, where these settings are enabled, is not included in the repository because it is common for developers to store local credentials, connection strings, and secret API keys in this file. Keeping it out of source control helps prevent machine-specific or sensitive configuration from being accidentally committed.

For this demonstration, if you haven't already, add a new file named appsettings.Development.json on the **HealthBenefitsPortal** API project and enable the development seed data by adding this section:

```json
{ ...
  "SeedOptions": {
    "IncludeTestUsers": true,
    "IncludeDemoData": true
  },
  ...
}
```

## 6. Run the Backend

Hit the F5 key or click on the Play button on Visual Studio to run the backend application.

If everything is configured correctly, the backend should start successfully and the configured seed data should be created in the database. The populated tables are:

> IdentityUser, 
> IdentityRole,
> IdentityUserRoles,
> Departments,
> Employees,

At this point, the API is ready to accept requests from the Angular frontend.

## 7. Install Frontend Dependencies

### Step 1. Navigate to the fronend folder

From the folder where you cloned the repository navigate to:

```bash
cd EmployeeBenefitsManagement.Api\src\frontend\health-benefits-portal-ui
```

### Step 2. Run the npm install command

```bash
npm install
```

## 8. Run the Frontend

Open the frontend project in VS Code (or use an existing terminal), then run 

```bash
npm run start -- --open
```

After a brief moment the frontend application will be built and a new browser window will open on the URL http://localhost:4200.

## 9. Explore the Application

You can explore the application's role-based functionality by logging in with one of the following accounts.:

| User | Role | Password |
| ---- | ---- | -------- |
|admin@mockdomain.com | Administrator | The password you configured on user secrets |
| hr1@test.com | HR | Password123! |
| hr2@test.com | HR | Password123! |
| hr3@test.com | HR | Password123! |
| employee1@test.com | Employee | Password123! |
| employee2@test.com | Employee | Password123! |
| employee3@test.com | Employee | Password123! |
| employee4@test.com | Employee | Password123! |
| employee5@test.com | Employee | Password123! |

> **Important:** Each Role has different access permissions and capabilities.




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
* Benefit enrollment workflows
* Unit testing
* Integration testing
* Docker support
* CI/CD pipeline
* Azure deployment

---

## License

This project is intended as a portfolio and learning project demonstrating modern ASP.NET Core development practices.
