using System.Security.Claims;
using BookM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BookM.Services;

namespace BookM.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly BookMContext _context;
        private readonly IEmailSender _emailSender;

        public BookingController(BookMContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
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

                var book = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Event)
                    .Include(b => b.TicketType)
                    .FirstOrDefaultAsync(b => b.BookingId == payment.BookingId);

                if(book!= null)
                {
                    string emailSubject = $"Your booking for {book.Event.Title}";

                    string qrCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=150x150&data={payment.TransactionId}";

                    // 2. Update the HTML Body
                    string emailBody = $@"
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <style>
                                    .ticket-container {{ font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px; overflow: hidden; }}
                                    .header {{ background-color: #212529; color: white; padding: 20px; text-align: center; }}
                                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                                    .detail-row {{ margin-bottom: 10px; border-bottom: 1px solid #eee; padding-bottom: 10px; }}
                                    .label {{ font-weight: bold; color: #555; }}
                                    .footer {{ background-color: #eee; padding: 15px; text-align: center; font-size: 12px; color: #777; }}
                                    .qr-section {{ text-align: center; margin: 20px 0; }}
                                </style>
                            </head>
                            <body>
                                <div class='ticket-container'>
                                    <div class='header'>
                                        <h2>Your Event Ticket</h2>
                                        <p>Transaction ID: {payment.TransactionId}</p>
                                    </div>
                                    <div class='content'>
                                        <h3 style='color: #212529;'>{book.Event.Title}</h3>
            
                                        <div class='detail-row'>
                                            <span class='label'>Date:</span> {book.Event.EventDate:MMMM dd, yyyy}
                                        </div>
                                        <div class='detail-row'>
                                            <span class='label'>Ticket Type:</span> {book.TicketType.Name}
                                        </div>
                                        <div class='detail-row'>
                                            <span class='label'>Seats:</span> {book.SeatsBooked}
                                        </div>
            
                                        <div class='qr-section'>
                                            <p style='font-size: 12px; color: #777; margin-bottom: 10px;'>Scan for Entry</p>
                                            <img src='{qrCodeUrl}' alt='Ticket QR Code' width='150' height='150' />
                                        </div>

                                    </div>
                                    <div class='footer'>
                                        <p>Thank you for using BookM!</p>
                                    </div>
                                </div>
                            </body>
                            </html>";
                    try
                    {
                        _emailSender.SendEmailAsync(book.User.Email, emailSubject, emailBody);
                    }
                    catch (Exception ex)
                    {
                        
                        Console.WriteLine($"Failed to send ticket email: {ex.Message}");
                    }

                }
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
