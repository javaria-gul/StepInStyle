using System.Linq;
using System.Web.Mvc;
using StepInStyle.Models;

namespace StepInStyle.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Register page
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullname, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Message = "Passwords do not match!";
                return View();
            }

            // Check if email already exists
            var existingUser = db.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.Message = "Email already registered.";
                return View();
            }

            var user = new User
            {
                FullName = fullname,
                Email = email,
                Password = password,  
                Role = "User"
            };

            db.Users.Add(user);
            db.SaveChanges();

            TempData["Success"] = "Registration successful. Please login.";
            return RedirectToAction("Login");
        }

        // GET: Login page
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Set session
                Session["UserId"] = user.UserId;
                Session["UserName"] = user.FullName;
                Session["UserRole"] = user.Role;

                // ✅ Check if user is Admin
                if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                // Regular user
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Invalid email or password.";
                return View();
            }
        }

        public ActionResult Dashboard()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
