# KAAS Gaming - Player Preferences (Minimal ASP.NET Core 8 Demo)

This repository contains a minimal, well-structured ASP.NET Core 8 application demonstrating core web development concepts with Entity Framework Core and Razor Pages.

Purpose
- Demonstrate a small, interview-friendly web app managing player preferences for a gaming initiative.
- Showcase dependency injection, EF Core, minimal APIs, DTO mapping, asynchronous programming, and a small Razor Pages UI.

Included Files
- `Program.cs` - Application startup, dependency injection, minimal API endpoints, Swagger, and demo seeding.
- `appsettings.json` - Minimal configuration with a `DefaultConnection` (SQLite by default).
- `KAAS.csproj` - Project file with required NuGet packages.
- `Models/PlayerPreference.cs` - EF Core entity.
- `DTOs/PlayerPreferenceDto.cs` - API DTO used for data transfer.
- `Data/KAASContext.cs` - EF Core DbContext and entity configuration.
- `Services/PlayerPreferenceService.cs` - Service layer abstracting EF Core operations.
- `Pages/Index.cshtml` and `Pages/Index.cshtml.cs` - Minimal Razor Pages UI for listing and creating preferences.

Build and run

Prerequisites
- .NET 8 SDK installed (the project targets `net8.0`).

Commands

```bash
# from repository root
dotnet build
dotnet run
```

By default the app uses SQLite (file `KAAS.db`). When the app starts in development, it seeds a few example preferences if none exist.

Open in your browser
- Razor UI: `https://localhost:5001/` (check console output for actual URLs/ports)
- Swagger UI (development): `https://localhost:5001/api-docs`

Notes and best practices
- For production use, prefer EF Core Migrations instead of `EnsureCreated` and remove demo seeding from startup.
- DTOs are used to separate API contracts from domain models.
- All database operations use async methods to keep the app responsive.
- The service layer enables easier testing and separation of concerns.

Testing
- No automated tests are included in this minimal example. Adding unit tests for the service layer and integration tests for the API is recommended.

Privacy and sensitive data
- This project avoids hardcoded credentials and timestamps in source files. The `appsettings.json` contains a local SQLite connection by default.

License
- This is a demo/example project. Add a license file if you plan to distribute it.
