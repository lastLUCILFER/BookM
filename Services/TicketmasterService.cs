using System.Text.Json;
using BookM.Models;
using Microsoft.EntityFrameworkCore;

namespace BookM.Services
{
    public class TicketmasterService
    {
        private readonly HttpClient _httpClient;
        private readonly BookMContext _context;
        // Keep your API Key safe!
        private readonly string _apiKey = "vSdOYVUqBEVOkQyzSNaG48nLbmZYiPrZ";

        public TicketmasterService(HttpClient httpClient, BookMContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

      
        public async Task<int> ImportEventsAsync(string locationName)
        {
            // 2. UPDATE: Use 'locationName' in the URL. 
            // Note: We still use "&city=" for the API parameter because Ticketmaster expects a city name.
            string url = $"https://app.ticketmaster.com/discovery/v2/events.json?apikey={_apiKey}&city={locationName}&size=20&sort=date,asc";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return 0;

            var jsonString = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TicketmasterResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (data?._embedded?.events == null) return 0;

            int count = 0;

            foreach (var tmEvent in data._embedded.events)
            {
                if (!_context.Events.Any(e => e.Title == tmEvent.name))
                {
                    int catId = MapCategory(tmEvent.classifications?.FirstOrDefault()?.segment?.name);

                    var newEvent = new Event
                    {
                        Title = tmEvent.name.Length > 100 ? tmEvent.name.Substring(0, 97) + "..." : tmEvent.name,

                        // 3. UPDATE: Safe mapping for Description
                        Description = $"Experience {tmEvent.name} live at {tmEvent._embedded?.venues?.FirstOrDefault()?.name ?? locationName}!",

                        EventDate = tmEvent.dates?.start?.dateTime ?? DateTime.Now.AddDays(10),

                        // 4. UPDATE: Ensure this maps to your new 'Location' property
                        // We prefer the Venue Name (e.g., "O2 Arena"), but fallback to the City name if missing.
                        Location = tmEvent._embedded?.venues?.FirstOrDefault()?.name ?? locationName,

                        Capacity = new Random().Next(1000, 50000),
                        Ltype = "Indoor",
                        CategoryId = catId,
                        ImageUrl = tmEvent.images?.FirstOrDefault()?.url
                    };

                    _context.Events.Add(newEvent);

                    // Create tickets...
                    _context.TicketType.AddRange(
                        new TicketType { Name = "Standard", Price = 50, QuantityAvailable = 500, Event = newEvent },
                        new TicketType { Name = "VIP", Price = 150, QuantityAvailable = 100, Event = newEvent },
                        new TicketType { Name = "Golden Circle", Price = 1200, QuantityAvailable = 10, Event = newEvent }
                    );

                    count++;
                }
            }

            await _context.SaveChangesAsync();
            return count;
        }

        private int MapCategory(string tmCategory)
        {
            if (string.IsNullOrEmpty(tmCategory)) return 1;
            tmCategory = tmCategory.ToLower();

            if (tmCategory.Contains("music")) return 2;
            if (tmCategory.Contains("sports")) return 3;
            if (tmCategory.Contains("arts") || tmCategory.Contains("theatre")) return 4;
            return 1;
        }
    }
}