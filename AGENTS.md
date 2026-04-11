# AGENTS.md

## Project Overview

**demomvcdata** is an ASP.NET Core MVC web application built with .NET 9.0. It manages "Zonas Inseguras" (Unsafe Zones) with full CRUD operations and includes ASP.NET Core Identity for user authentication.

## Tech Stack

- **Framework**: ASP.NET Core MVC (.NET 9.0)
- **Language**: C#
- **Database**: SQLite (via `app.db`)
- **ORM**: Entity Framework Core 9.0 (SQLite provider)
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Razor Views, Bootstrap, jQuery

## Project Structure

```
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs          # Home and Privacy pages
│   └── ZonasInsegurasController.cs # CRUD for Zonas Inseguras
├── Models/                # Domain and view models
│   ├── ZonaInsegura.cs            # Main entity model
│   └── ErrorViewModel.cs          # Error view model
├── Views/                 # Razor Views
│   ├── Home/                      # Home views (Index, Privacy)
│   ├── ZonasInseguras/            # CRUD views (Index, Create, Edit, Delete, Details)
│   └── Shared/                    # Layout and partial views
├── Data/                  # Data access layer
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── Migrations/                # EF Core migrations
├── Areas/
│   └── Identity/                  # ASP.NET Core Identity area
├── wwwroot/               # Static files (CSS, JS, libraries)
├── Program.cs             # Application entry point and configuration
├── appsettings.json       # Application settings and connection string
├── demomvcdata.csproj     # Project file
└── demomvcdata.sln        # Solution file
```

## Build and Run

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run

# Run with hot reload (development)
dotnet watch run
```

## Database and Migrations

The application uses SQLite with the database file `app.db` at the project root. Connection string is configured in `appsettings.json`.

```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations to the database
dotnet ef database update
```

## Coding Conventions

- **Language**: The codebase uses C# with file-scoped namespaces (e.g., `namespace demomvcdata.Models;`).
- **Nullable reference types** are enabled (`<Nullable>enable</Nullable>`).
- **Implicit usings** are enabled (`<ImplicitUsings>enable</ImplicitUsings>`).
- **Data annotations** are used for model validation (e.g., `[Required]`, `[StringLength]`, `[Range]`).
- **Display attributes** include Spanish labels (e.g., `[Display(Name = "Nombre de la Zona")]`).
- **Error messages** are written in Spanish.
- Controllers use constructor injection for `ApplicationDbContext`.
- Async/await pattern is used for all database operations.
- Standard MVC CRUD pattern: `Index`, `Details`, `Create`, `Edit`, `Delete` actions.

## Key Models

### ZonaInsegura

| Property       | Type       | Constraints                          |
| -------------- | ---------- | ------------------------------------ |
| Id             | int        | Primary key, auto-generated          |
| Nombre         | string     | Required, max 100 chars              |
| Direccion      | string     | Required, max 200 chars              |
| NivelPeligro   | int        | Required, range 1–5                  |
| Descripcion    | string?    | Optional, max 500 chars              |
| FechaRegistro  | DateTime   | Defaults to `DateTime.Now`           |
| Activa         | bool       | Defaults to `true`                   |

## Testing

There is currently no test project in this solution. If adding tests, follow the standard .NET convention:

```bash
# Create a test project (xUnit recommended)
dotnet new xunit -n demomvcdata.Tests
dotnet sln add demomvcdata.Tests/demomvcdata.Tests.csproj

# Run tests
dotnet test
```

## Tips for Agents

- The application language (UI labels, validation messages) is **Spanish**.
- When adding new entities, follow the same pattern as `ZonaInsegura`: model with data annotations → DbSet in `ApplicationDbContext` → controller with CRUD → Razor views.
- The database file `app.db` is committed to the repository; be careful not to overwrite it with incompatible schema changes without a proper migration.
- Use `dotnet ef` commands for any database schema changes—never modify migration files manually.
