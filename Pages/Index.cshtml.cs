using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KAAS.DTOs;
using KAAS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KAAS.Pages
{
    /// <summary>
    /// Index page model for displaying and managing player preferences.
    /// Demonstrates: Razor Pages, dependency injection, async operations.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly IPlayerPreferenceService _service;

        /// <summary>
        /// Collection of player preferences to display on the page.
        /// </summary>
        public List<PlayerPreferenceDto> Preferences { get; set; } = new List<PlayerPreferenceDto>();

        public IndexModel(IPlayerPreferenceService service)
        {
            _service = service;
        }

        /// <summary>
        /// Best Practice: OnGetAsync is called when the page loads via GET request.
        /// This loads all player preferences from the database.
        /// </summary>
        public async Task OnGetAsync()
        {
            try
            {
                var prefs = await _service.GetAllAsync();
                Preferences = prefs.ToList();
            }
            catch (Exception ex)
            {
                // Best Practice: Log error and continue with empty list
                Console.WriteLine($"Error loading preferences: {ex.Message}");
                Preferences = new List<PlayerPreferenceDto>();
            }
        }

        /// <summary>
        /// Best Practice: OnPostCreateAsync handles form submission for creating new preferences.
        /// Demonstrates form binding and validation.
        /// </summary>
        public async Task<IActionResult> OnPostCreateAsync(string playerName, string gameGenre, string preferenceDetails)
        {
            try
            {
                // Best Practice: Validate input on server side
                if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(gameGenre))
                {
                    TempData["Error"] = "Player Name and Game Genre are required.";
                    return RedirectToPage();
                }

                var dto = new PlayerPreferenceDto
                {
                    PlayerName = playerName.Trim(),
                    GameGenre = gameGenre.Trim(),
                    PreferenceDetails = preferenceDetails?.Trim() ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                await _service.CreateAsync(dto);
                TempData["Success"] = $"Successfully added {playerName}'s preferences!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating preference: {ex.Message}";
            }

            // Redirect back to GET to prevent form resubmission
            return RedirectToPage();
        }

        /// <summary>
        /// Best Practice: OnPostDeleteAsync handles deletion of player preferences.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (success)
                {
                    TempData["Success"] = "Preference deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Preference not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting preference: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}
