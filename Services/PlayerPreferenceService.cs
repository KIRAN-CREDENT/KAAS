using KAAS.Data;
using KAAS.DTOs;
using KAAS.Models;
using Microsoft.EntityFrameworkCore;

namespace KAAS.Services;

/// <summary>
/// Service layer for PlayerPreference business logic.
/// Best Practice: Services abstract EF Core operations and provide business logic,
/// enabling easier testing and reusability across controllers/endpoints.
/// </summary>
public interface IPlayerPreferenceService
{
    /// <summary>
    /// Retrieves all player preferences asynchronously.
    /// </summary>
    Task<IEnumerable<PlayerPreferenceDto>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific player preference by ID asynchronously.
    /// </summary>
    Task<PlayerPreferenceDto?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new player preference asynchronously.
    /// </summary>
    Task<PlayerPreferenceDto> CreateAsync(PlayerPreferenceDto dto);

    /// <summary>
    /// Updates an existing player preference asynchronously.
    /// Returns null if the preference with the given ID is not found.
    /// </summary>
    Task<PlayerPreferenceDto?> UpdateAsync(int id, PlayerPreferenceDto dto);

    /// <summary>
    /// Deletes a player preference by ID asynchronously.
    /// Returns true if deletion was successful, false if not found.
    /// </summary>
    Task<bool> DeleteAsync(int id);
}

public class PlayerPreferenceService : IPlayerPreferenceService
{
    private readonly KAASContext _context;

    public PlayerPreferenceService(KAASContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlayerPreferenceDto>> GetAllAsync()
    {
        // Best Practice: Use .AsNoTracking() for read-only queries to improve performance
        return await _context.PlayerPreferences
            .AsNoTracking()
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<PlayerPreferenceDto?> GetByIdAsync(int id)
    {
        // Best Practice: Use Find() for single-key lookups (optimized by EF Core)
        var preference = await _context.PlayerPreferences.FindAsync(id);
        return preference != null ? MapToDto(preference) : null;
    }

    public async Task<PlayerPreferenceDto> CreateAsync(PlayerPreferenceDto dto)
    {
        // Best Practice: Map DTO to entity, add to context, and save asynchronously
        var entity = MapToEntity(dto);
        _context.PlayerPreferences.Add(entity);
        await _context.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<PlayerPreferenceDto?> UpdateAsync(int id, PlayerPreferenceDto dto)
    {
        var entity = await _context.PlayerPreferences.FindAsync(id);
        if (entity == null) return null;

        // Best Practice: Update properties explicitly to avoid accidental overwrites
        entity.PlayerName = dto.PlayerName;
        entity.GameGenre = dto.GameGenre;
        entity.PreferenceDetails = dto.PreferenceDetails;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.PlayerPreferences.Update(entity);
        await _context.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.PlayerPreferences.FindAsync(id);
        if (entity == null) return false;

        _context.PlayerPreferences.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Maps a PlayerPreference entity to a PlayerPreferenceDto.
    /// Best Practice: Centralized mapping logic for maintainability.
    /// </summary>
    private static PlayerPreferenceDto MapToDto(PlayerPreference entity)
    {
        return new PlayerPreferenceDto
        {
            Id = entity.Id,
            PlayerName = entity.PlayerName,
            GameGenre = entity.GameGenre,
            PreferenceDetails = entity.PreferenceDetails
            ,
            CreatedAt = entity.CreatedAt
        };
    }

    /// <summary>
    /// Maps a PlayerPreferenceDto to a PlayerPreference entity.
    /// </summary>
    private static PlayerPreference MapToEntity(PlayerPreferenceDto dto)
    {
        return new PlayerPreference
        {
            PlayerName = dto.PlayerName,
            GameGenre = dto.GameGenre,
            PreferenceDetails = dto.PreferenceDetails
        };
    }
}
