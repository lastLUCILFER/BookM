using BookM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookM.Models;

namespace BookM.Controllers
{
    public class Neo4jController : Controller
    {
        private readonly Neo4jService _neo4jService;
        private readonly BookMContext _sqlcontext;

        public Neo4jController(Neo4jService neo4jService, BookMContext context)
        {
            _neo4jService = neo4jService;
            _sqlcontext = context;
        }

        [HttpGet("neo4jtest/check")]
        public async Task<IActionResult> CheckConnection()
        {
            try
            {
                var version = await _neo4jService.GetServerVersionAsync();
                return Content($"Connected to Neo4j successfully! Server Version: {version}");
            }
            catch (Exception ex)
            {
                return Content($"Failed to connect to Neo4j. Error: {ex.Message}");
            }
        }
        
        [HttpPost("migrate-users")]
        public async Task<IActionResult> MigrateUsers()
        {
            var sqlUsers = await _sqlcontext.Users.ToListAsync();
            
            var usersData = sqlUsers.Select(u => new Dictionary<string, object>
            {
                { "UserId", u.UserId },
                { "Email", u.Email },
                { "Username", u.Name },
                { "PasswordHash", u.PasswordHash },
                { "IsAdmin", u.IsAdmin }
            }).ToList();

            if (usersData.Any())
            {
                await _neo4jService.BatchCreateUsersAsync(usersData);
            }

            return Content($"Migrated {usersData.Count} users to Neo4j successfully (Batch Mode).");
        }

        [HttpGet("migrate-events")]
        public async Task<IActionResult> MigrateEvents()
        {
            var events = await _sqlcontext.Events
                .Include(e => e.Category)
                .ToListAsync();

            var eventsData = events.Select(e => new Dictionary<string, object>
            {
                { "EventId", e.EventId },
                { "Title", e.Title },
                { "Description", e.Description ?? "" },
                { "EventDate", e.EventDate.ToString("yyyy-MM-ddTHH:mm:ss") },
                { "Location", e.Location ?? "" },
                { "Capacity", e.Capacity },
                { "ImageUrl", e.ImageUrl ?? "" },
                { "CategoryId", e.CategoryId },
                { "CategoryName", e.Category.Name }
            }).ToList();

            if (eventsData.Any())
            {
                await _neo4jService.BatchCreateEventsAsync(eventsData);
            }

            return Content($"Migrated {eventsData.Count} events and their categories to Neo4j successfully.");
        }

        [HttpGet("migrate-categories")]
        public async Task<IActionResult> MigrateCategories()
        {
            var categories = await _sqlcontext.Category
                .Include(c => c.Events)
                .ToListAsync();

            var categoriesData = categories.Select(c => new Dictionary<string, object>
            {
                { "CategoryId", c.CategoryId },
                { "Name", c.Name },
                { "EventIds", c.Events.Select(e => e.EventId).ToList() }
            }).ToList();

            if (categoriesData.Any())
            {
                await _neo4jService.BatchCreateCategoriesAsync(categoriesData);
            }

            return Content($"Migrated {categoriesData.Count} categories and their event relationships to Neo4j successfully.");
        }
    }
}
