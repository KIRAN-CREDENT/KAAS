using KAAS.Data;
using KAAS.DTOs;
using KAAS.Services;
using Microsoft.EntityFrameworkCore;
using KAAS.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

// ============================================================================
// KAAS Gaming - ASP.NET Core 8 Minimal API with Razor Pages
// Demonstrates: Dependency Injection, EF Core, Async/Await, API Best Practices
// ============================================================================

// Best Practice: Use WebApplication.CreateBuilder (not WebApplicationBuilder)
var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// SERVICE CONFIGURATION (Dependency Injection)
// ============================================================================

// Best Practice: Register DbContext with connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Use SQLite for this example (simplifies deployment for school projects)
builder.Services.AddDbContext<KAASContext>(options =>
    options.UseSqlite(connectionString));

// Best Practice: Register service abstraction for testability and loose coupling
builder.Services.AddScoped<IPlayerPreferenceService, PlayerPreferenceService>();

// Add Razor Pages support
builder.Services.AddRazorPages();

// Add Swagger/OpenAPI support for interactive API testing
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "KAAS Gaming API",
        Version = "v1",
        Description = "A minimal ASP.NET Core 8 API demonstrating best practices for player preference management."
    });
});

// Build the application
var app = builder.Build();

// ============================================================================
// MIDDLEWARE CONFIGURATION
// ============================================================================

// Best Practice: Enable Swagger UI for development documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "KAAS Gaming API v1");
        options.RoutePrefix = "api-docs"; // Swagger UI at /api-docs
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Map Razor Pages
app.MapRazorPages();

// ============================================================================
// MINIMAL API ENDPOINTS - PLAYER PREFERENCES CRUD
// ============================================================================

var api = app.MapGroup("/api/preferences")
    .WithName("PlayerPreferences");
    // Enable OpenAPI documentation for this group

/// <summary>
/// GET /api/preferences
/// Retrieves all player preferences.
/// Best Practice: Async/await for non-blocking I/O operations.
/// </summary>
api.MapGet("/", GetAllPreferences)
    .WithName("GetAllPreferences")
    .WithSummary("Get all player preferences")
    .WithDescription("Retrieves a list of all player preferences from the database.")
    .Produces<IEnumerable<PlayerPreferenceDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status500InternalServerError);

/// <summary>
/// GET /api/preferences/{id}
/// Retrieves a specific player preference by ID.
/// Best Practice: Proper HTTP status codes (404 for not found).
/// </summary>
api.MapGet("/{id}", GetPreferenceById)
    .WithName("GetPreferenceById")
    .WithSummary("Get a player preference by ID")
    .WithDescription("Retrieves a specific player preference record.")
    .Produces<PlayerPreferenceDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

/// <summary>
/// POST /api/preferences
/// Creates a new player preference.
/// Best Practice: Validation and error handling with meaningful responses.
/// </summary>
api.MapPost("/", CreatePreference)
    .WithName("CreatePreference")
    .WithSummary("Create a new player preference")
    .WithDescription("Creates a new player preference record.")
    .Accepts<PlayerPreferenceDto>("application/json")
    .Produces<PlayerPreferenceDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

/// <summary>
/// PUT /api/preferences/{id}
/// Updates an existing player preference.
/// Best Practice: Idempotent operation with proper validation.
/// </summary>
api.MapPut("/{id}", UpdatePreference)
    .WithName("UpdatePreference")
    .WithSummary("Update a player preference")
    .WithDescription("Updates an existing player preference record.")
    .Accepts<PlayerPreferenceDto>("application/json")
    .Produces<PlayerPreferenceDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

/// <summary>
/// DELETE /api/preferences/{id}
/// Deletes a player preference by ID.
/// Best Practice: Proper status codes (204 for success with no content).
/// </summary>
api.MapDelete("/{id}", DeletePreference)
    .WithName("DeletePreference")
    .WithSummary("Delete a player preference")
    .WithDescription("Deletes a player preference record by ID.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

// ============================================================================
// ENDPOINT HANDLER IMPLEMENTATIONS
// ============================================================================

/// <summary>
/// Retrieves all player preferences.
/// </summary>
async Task<IResult> GetAllPreferences(IPlayerPreferenceService service)
{
    try
    {
        var preferences = await service.GetAllAsync();
        return Results.Ok(preferences);
    }
    catch (Exception ex)
    {
        // Best Practice: Log exceptions and return appropriate error response
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An error occurred while retrieving preferences.");
    }
}

/// <summary>
/// Retrieves a specific player preference by ID.
/// </summary>
async Task<IResult> GetPreferenceById(int id, IPlayerPreferenceService service)
{
    try
    {
        // Best Practice: Validate input parameter
        if (id <= 0)
            return Results.BadRequest("ID must be greater than zero.");

        var preference = await service.GetByIdAsync(id);
        if (preference == null)
            return Results.NotFound($"Player preference with ID {id} not found.");

        return Results.Ok(preference);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An error occurred while retrieving the preference.");
    }
}

/// <summary>
/// Creates a new player preference.
/// </summary>
async Task<IResult> CreatePreference(PlayerPreferenceDto dto, IPlayerPreferenceService service)
{
    try
    {
        // Best Practice: Input validation before business logic
        if (string.IsNullOrWhiteSpace(dto.PlayerName))
            return Results.BadRequest("PlayerName is required.");

        if (string.IsNullOrWhiteSpace(dto.GameGenre))
            return Results.BadRequest("GameGenre is required.");

        var createdPreference = await service.CreateAsync(dto);
        // Best Practice: Return 201 Created with Location header pointing to the new resource
        return Results.Created($"/api/preferences/{createdPreference.Id}", createdPreference);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An error occurred while creating the preference.");
    }
}

/// <summary>
/// Updates an existing player preference.
/// </summary>
async Task<IResult> UpdatePreference(int id, PlayerPreferenceDto dto, IPlayerPreferenceService service)
{
    try
    {
        // Best Practice: Validate input parameters
        if (id <= 0)
            return Results.BadRequest("ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(dto.PlayerName))
            return Results.BadRequest("PlayerName is required.");

        if (string.IsNullOrWhiteSpace(dto.GameGenre))
            return Results.BadRequest("GameGenre is required.");

        var updatedPreference = await service.UpdateAsync(id, dto);
        if (updatedPreference == null)
            return Results.NotFound($"Player preference with ID {id} not found.");

        return Results.Ok(updatedPreference);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An error occurred while updating the preference.");
    }
}

/// <summary>
/// Deletes a player preference by ID.
/// </summary>
async Task<IResult> DeletePreference(int id, IPlayerPreferenceService service)
{
    try
    {
        // Best Practice: Validate input parameter
        if (id <= 0)
            return Results.BadRequest("ID must be greater than zero.");

        var deleted = await service.DeleteAsync(id);
        if (!deleted)
            return Results.NotFound($"Player preference with ID {id} not found.");

        // Best Practice: Return 204 No Content for successful deletion
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "An error occurred while deleting the preference.");
    }
}

app.Run();

// ============================================================================
// Seed demo data (useful for presentation / interviews)
// Best Practice: Seed data in development only and idempotently.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<KAASContext>();
    // Ensure database is created. For production, use Migrations instead.
    await db.Database.EnsureCreatedAsync();

    if (!await db.PlayerPreferences.AnyAsync())
    {
        db.PlayerPreferences.AddRange(
            new PlayerPreference { PlayerName = "Alex", GameGenre = "RPG", PreferenceDetails = "Difficulty: Hard, Multiplayer: Yes" },
            new PlayerPreference { PlayerName = "Casey", GameGenre = "FPS", PreferenceDetails = "Multiplayer: Yes, Sensitivity: Medium" },
            new PlayerPreference { PlayerName = "Taylor", GameGenre = "Strategy", PreferenceDetails = "Prefers turn-based, AI difficulty: Normal" }
        );

        await db.SaveChangesAsync();
    }
}
