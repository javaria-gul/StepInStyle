using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace StepInStyle.Controllers
{
    public class AdminController : Controller
    {
       

        string connectionString = ConfigurationManager.ConnectionStrings["StepInStyleContext"].ConnectionString;

        // GET: Admin/Dashboard
        public ActionResult Dashboard()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            
            int totalProducts = 0;
            int totalCategories = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Products count
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Products", con);
                totalProducts = (int)cmd1.ExecuteScalar();

                // Categories count
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Categories", con);
                totalCategories = (int)cmd2.ExecuteScalar();
            }

            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCategories = totalCategories;

            return View();
        }
    }
}


