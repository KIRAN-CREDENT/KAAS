namespace KAAS.Models;

/// <summary>
/// Entity class representing a player's gaming preferences.
/// This is the domain model used for database persistence.
/// </summary>
public class PlayerPreference
{
    /// <summary>
    /// Unique identifier for the player preference record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// The primary game genre the player prefers (e.g., RPG, FPS, Strategy).
    /// </summary>
    public string GameGenre { get; set; } = string.Empty;

    /// <summary>
    /// Additional preference details (e.g., difficulty level, multiplayer preference).
    /// </summary>
    public string PreferenceDetails { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp indicating when this preference record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp indicating when this preference record was last updated.
    /// This value is managed by the database by default.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
