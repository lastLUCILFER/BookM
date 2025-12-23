using System.Security.Claims;
using BookM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BookM.Controllers
{
    public class BookingController : Controller
    {
        private readonly BookMContext _context;

        public BookingController(BookMContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Book(int eventID)
        {
            var @event = await _context.Events
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.EventId == eventID);

            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(int eventId,int TicketTypeId, int seatsBooked)
        {
            var TicketType = await _context.TicketType.FindAsync(TicketTypeId);

            if (TicketType == null)  return NotFound("Ticket Type not found");

            if(TicketType.QuantityAvailable < seatsBooked)
            {
                return BadRequest("Not enough tickets available");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var booking = new Booking
            {
                UserId = userId,
                EventId = eventId,
                SeatsBooked = seatsBooked,
                TicketTypeId = TicketTypeId,
                BookingDate = DateTime.Now
            };

            TicketType.QuantityAvailable -= seatsBooked;


            _context.Bookings.Add(booking);
            _context.TicketType.Update(TicketType);
            await _context.SaveChangesAsync();
            return RedirectToAction("Payment", new { bookingId = booking.BookingId });
        }

        public async Task<IActionResult> Payment(int bookingId) {

            var booking = await _context.Bookings
                .Include(e => e.Event)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(B => B.BookingId == bookingId);

            if (booking == null) return NotFound("Booking not found");

            decimal totalAmount = booking.TicketType.Price * booking.SeatsBooked;
            var payment = new Payment
            {
                BookingId = bookingId,
                Booking = booking,
                PaymentDate = DateTime.Now,
                Amount = totalAmount

            };
            return View(payment);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(Payment payment)
        {
            ModelState.Remove("Booking");
            ModelState.Remove("TransactionId");
            ModelState.Remove("Status");

            if(ModelState.IsValid)
            {
                payment.Status = "Completed";
                payment.PaymentDate = DateTime.Now;
                payment.TransactionId = Guid.NewGuid().ToString();
                _context.Payment.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Confirmation");
            }

            return View("Payment", payment);

        }
      
   
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
