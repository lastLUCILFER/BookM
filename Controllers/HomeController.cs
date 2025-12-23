using System.Diagnostics;
using BookM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookM.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookMContext _context;

        public HomeController(ILogger<HomeController> logger, BookMContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            ViewBag.SliderEvents = await _context.Events
                                           .Include(e => e.Category)
                                           .OrderByDescending(e => e.EventId)
                                           .Take(4)
                                           .ToListAsync();

            // 2. Get 4 "Important" upcoming events (happening soon)
            // We filter for future dates and order by date
            ViewBag.UpcomingEvents = await _context.Events
                                             .Include(e => e.Category)
                                             .Where(e => e.EventDate >= DateTime.Now)
                                             .OrderBy(e => e.EventDate)
                                             .Take(4)
                                             .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> AllEvents(string category)
        {
            var query = _context.Events
                                .Include(e => e.Category)
                                .AsQueryable();

           
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category.Name == category);
                ViewData["Title"] = $"{category} Events";
            }
            
            else
            {
                ViewData["Title"] = "All Events";
            }

            var events = await query.OrderBy(e => e.EventDate).ToListAsync();
            return View(events);
        }

        public async Task<IActionResult> EventDetails(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var eventDetails = await _context.Events
                                             .Include(e => e.Category)
                                             .Include(e => e.TicketTypes)
                                             .FirstOrDefaultAsync(e => e.EventId == id);
            if(eventDetails == null)
            {
                return NotFound();
            }
            return View(eventDetails);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
