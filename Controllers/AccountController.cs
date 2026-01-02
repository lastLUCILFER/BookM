using System.Security.Claims;
using System.Security.Cryptography; 
using System.Text;
using BookM.Models;
using BookM.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookM.Controllers
{
    public class AccountController : Controller
    {

        private readonly BookMContext _context;
        private readonly BookM.Services.Neo4jService _neo4jService;
        private readonly IEmailSender _emailSender;

        public AccountController(BookMContext context, BookM.Services.Neo4jService neo4jService, IEmailSender emailSender)
        {
            _context = context;
            _neo4jService = neo4jService;
            _emailSender = emailSender;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && user.PasswordHash == HashPassword(password))
            {
                if (!user.IsEmailVerified)
                {
                    ViewBag.Error = "Please verify your email address before logging in.";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var Principal = new ClaimsPrincipal(identity);
                

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Principal);

                if (user.IsAdmin)
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Home");

            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }


        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            // Password Validation
            if (password.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters long.";
                return View();
            }
            if (!password.Any(char.IsUpper))
            {
                ViewBag.Error = "Password must contain at least one uppercase letter.";
                return View();
            }
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                ViewBag.Error = "Password must contain at least one special character.";
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ViewBag.Error = "Email already registered.";
                return View();
            }

            var token = Guid.NewGuid().ToString();

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = HashPassword(password),
                VerificationToken = token,
                IsEmailVerified = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
           

            var verificationLink = Url.Action("VerifyEmail", "Account",
                new { token = token, email = email }, Request.Scheme);

            await _emailSender.SendEmailAsync(email, "Verify your BookM Account",
                $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.");

            return RedirectToAction("CheckEmail");
        }
        [HttpGet]
        public IActionResult CheckEmail()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Check if user exists and token matches
            if (user != null && user.VerificationToken == token)
            {
                user.IsEmailVerified = true;
                user.VerificationToken = null; // Clear token so it can't be used twice
                await _context.SaveChangesAsync();

                // Sync to Neo4j ONLY after they are verified (Optional, but cleaner)
                try { await _neo4jService.CreateUserAsync(user); } catch { }

                TempData["Message"] = "Email verified successfully! You can now login.";
                return RedirectToAction("Login"); // Send them to Login page with success message
            }

            TempData["Error"] = "Invalid verification link or already verified.";
            return RedirectToAction("Login"); 
        }



        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }

}




