using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookM.Services; 

namespace BookM.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly BookMContext _context;
        private readonly TicketmasterService _tmService;
        public AdminController(BookMContext context, TicketmasterService tmService)
        {
            _context = context;
            _tmService = tmService;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch the data and put it in the "ViewBag" backpack
            ViewBag.Categories = await _context.Category.ToListAsync();
            ViewBag.Events = await _context.Events.Include(e => e.Category).ToListAsync();
            ViewBag.Users = await _context.Users.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromTicketmaster(string location)
        {
            // Update the fallback logic variable names
            string searchLocation = string.IsNullOrEmpty(location) ? "New York" : location;

            // Pass this to your service
            // Note: You might need to update the method signature inside _tmService too!
            int count = await _tmService.ImportEventsAsync(searchLocation);

            TempData["Message"] = $"Success! Imported {count} events from {searchLocation}.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ManageCategories(int? editId)
        {
            ViewBag.EditId = editId; 
            return View(await _context.Category.ToListAsync());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var category = new Category { Name = name };
                _context.Category.Add(category);
                await _context.SaveChangesAsync();
            }
           
            return RedirectToAction(nameof(ManageCategories));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, string name)
        {
            var category = await _context.Category.FindAsync(id);
            if (category != null && !string.IsNullOrWhiteSpace(name))
            {
                category.Name = name;
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
           
            return RedirectToAction(nameof(ManageCategories));
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageCategories));
        }


        public async Task<IActionResult> ManageEvents()
        {
            
            var events = await _context.Events.Include(e => e.Category).ToListAsync();
            return View(events);
        }

        public IActionResult CreateEvent()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name");
            return View();
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(Event @event)
        {
            ModelState.Remove("Category");
            ModelState.Remove("TicketTypes");
            ModelState.Remove("Bookings");

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageEvents));
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", @event.CategoryId);
            return View(@event);
        }

        public async Task<IActionResult> EditEvent(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var @event =  await _context.Events.FindAsync(id);

            if(@event == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name");
            return View(@event);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, Event @event)
        {
            if(id != @event.EventId)
            {
                return NotFound();
            }
            ModelState.Remove("Category");
            ModelState.Remove("TicketTypes");
            ModelState.Remove("Bookings");

            if (ModelState.IsValid) {

                try {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }catch (DbUpdateConcurrencyException) {

                    if (_context.Events.Any(e => e.EventId==id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }

                return RedirectToAction(nameof(ManageEvents));
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", @event.CategoryId);
            return View(@event);

        }

        public async Task<IActionResult> DeleteEvent(int id)
        {
            var @event = await _context.Events.FindAsync(id);
           if(@event != null)
            {
                _context.Remove(@event);
                await _context.SaveChangesAsync();
            }
           return RedirectToAction(nameof(ManageEvents));
        }
    } 
}