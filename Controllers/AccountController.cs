using System;
using System.Linq;
using System.Web.Mvc;
using StepInStyle.Models;  

namespace StepInStyle.Controllers
{
    public class AccountController : Controller
    {
        StepInStyleContext db = new StepInStyleContext();  // DbContext instance

        // GET: Register
        public ActionResult Register()
        {
            return View();  // Show registration form
        }

        // POST: Register
        [HttpPost]
        public ActionResult Register(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    user.Role = "User";  // Default role for all new registrations
                    db.Users.Add(user);
                    db.SaveChanges();

                    TempData["Success"] = "Registration successful. Please login.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Something went wrong during registration. Please try again.";
                // Optionally log: ex.Message
            }

            return View(user);  // Return form with any validation messages
        }

        // GET: Login
        public ActionResult Login()
        {
            return View();  // Show login form
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                {
                    ViewBag.Message = "Email and Password are required.";
                    return View();
                }

                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);

                if (existingUser == null)
                {
                    ViewBag.Message = "No account found with this email.";
                    return View();
                }

                if (existingUser.Password != user.Password)
                {
                    ViewBag.Message = "Incorrect password.";
                    return View();
                }

                // Set session
                Session["UserID"] = existingUser.UserId;
                Session["UserName"] = existingUser.FullName;
                Session["UserRole"] = existingUser.Role;

                // Redirect to proper dashboard
                if (existingUser.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");

                }
                else
                {
                    return RedirectToAction("Dashboard", "User");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "An error occurred during login. Please try again.";
                // Optionally log: ex.Message
            }

            return View();
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear();  // Clear session data
            return RedirectToAction("Login");
        }
    }
}
