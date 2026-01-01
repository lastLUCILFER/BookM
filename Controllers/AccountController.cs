using System.Security.Claims;
using System.Security.Cryptography; 
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using BookM.Models;

namespace BookM.Controllers
{
    public class AccountController : Controller
    {

        private readonly BookMContext _context;
        private readonly BookM.Services.Neo4jService _neo4jService;

        public AccountController(BookMContext context, BookM.Services.Neo4jService neo4jService)
        {
            _context = context;
            _neo4jService = neo4jService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && user.PasswordHash == HashPassword(password))
            {
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

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email already registered.";
                return View();
            }

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            try 
            {
                await _neo4jService.CreateUserAsync(user);
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Failed to sync user to Neo4j: {ex.Message}");
            }

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




