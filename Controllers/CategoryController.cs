using StepInStyle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;


namespace StepInStyle.Controllers
{
    public class CategoryController : Controller
    {
         private readonly ApplicationDbContext db = new ApplicationDbContext();
       

        // GET: Category
        public ActionResult Index()
        {
            var categories = db.Categories.ToList(); // assuming db is your DB context
            return View(categories);
        }
        [HttpPost]
        public ActionResult Add(string CategoryName, string Description)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StepInStyleContext"].ConnectionString))
            {
                if (!string.IsNullOrEmpty(CategoryName))
                {
                    string query = "INSERT INTO Categories (CategoryName, Description) VALUES (@CategoryName, @Description)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@CategoryName", CategoryName);
                    cmd.Parameters.AddWithValue("@Description", Description ?? "");
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                return RedirectToAction("Index"); // or whatever your view method is
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StepInStyleContext"].ConnectionString))
            {
                string query = "DELETE FROM Categories WHERE CategoryId = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Index");
            }
        }




    }
}