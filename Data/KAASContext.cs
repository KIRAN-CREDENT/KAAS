using KAAS.Models;
using Microsoft.EntityFrameworkCore;

namespace KAAS.Data;

/// <summary>
/// Entity Framework Core DbContext for the KAAS Gaming application.
/// This class manages database connections and entity configurations.
/// Best Practice: DbContext handles ORM responsibilities, abstracting database details.
/// </summary>
public class KAASContext : DbContext
{
    public KAASContext(DbContextOptions<KAASContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet representing the collection of PlayerPreference entities.
    /// Enables LINQ queries against the database table.
    /// </summary>
    public DbSet<PlayerPreference> PlayerPreferences { get; set; } = null!;

    /// <summary>
    /// Configures entity model mappings and database conventions.
    /// Best Practice: Entity configuration is centralized here for maintainability.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure PlayerPreference entity
        modelBuilder.Entity<PlayerPreference>(entity =>
        {
            entity.HasKey(e => e.Id);

            // PlayerName is required and limited to 100 characters
            entity.Property(e => e.PlayerName)
                .IsRequired()
                .HasMaxLength(100);

            // GameGenre is required and limited to 50 characters
            entity.Property(e => e.GameGenre)
                .IsRequired()
                .HasMaxLength(50);

            // PreferenceDetails is optional and limited to 500 characters
            entity.Property(e => e.PreferenceDetails)
                .HasMaxLength(500);

            // Configure timestamp fields with default values
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
