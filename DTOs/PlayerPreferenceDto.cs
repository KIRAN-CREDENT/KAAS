namespace KAAS.DTOs;

/// <summary>
/// Data Transfer Object for PlayerPreference.
/// This DTO separates API contracts from domain models, enabling:
/// - Clear API contracts that won't change when domain models evolve
/// - Selective exposure of entity properties
/// - Validation at the API boundary
/// </summary>
public class PlayerPreferenceDto
{
    /// <summary>
    /// Unique identifier for the player preference (read-only on creation).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the player (required, must be non-empty).
    /// </summary>
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// The primary game genre the player prefers.
    /// Examples: RPG, FPS, Strategy, Adventure, Sports.
    /// </summary>
    public string GameGenre { get; set; } = string.Empty;

    /// <summary>
    /// Additional preference details as free-form text.
    /// Examples: "Difficulty: Hard, Multiplayer: Yes, Graphics: Ultra".
    /// </summary>
    public string PreferenceDetails { get; set; } = string.Empty;
    
    /// <summary>
    /// Created timestamp for display purposes in the UI.
    /// Including timestamps in DTOs is useful for read scenarios.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
