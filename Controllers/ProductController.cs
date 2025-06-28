using StepInStyle.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StepInStyle.Controllers
{
    public class ProductController : Controller
    {
        string connectionString = ConfigurationManager.ConnectionStrings["StepInStyleContext"].ConnectionString;


        // GET: Product/Index
        public ActionResult Index()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Product product = new Product
                    {
                        ProductId = Convert.ToInt32(rdr["ProductId"]),
                        ProductName = rdr["ProductName"].ToString(),
                        Description = rdr["Description"].ToString(),
                        Price = Convert.ToDecimal(rdr["Price"]),
                        Quantity = Convert.ToInt32(rdr["Quantity"]),
                        ImagePath = rdr["ImagePath"].ToString(),
                        CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
                    };
                    products.Add(product);
                }
            }

            return View(products);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            // Categories ki list bhi view ko bhejni hai dropdown ke liye
            List<Category> categories = new List<Category>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Categories";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryId = Convert.ToInt32(rdr["CategoryId"]),
                        CategoryName = rdr["CategoryName"].ToString()
                    });
                }
            }
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                string imagePath = "";

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Content/Images/Products"), fileName);
                    imageFile.SaveAs(path);
                    imagePath = "/Content/Images/Products/" + fileName;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Products (ProductName, CategoryId, Description, Price, Quantity, ImagePath, CreatedAt) " +
                                   "VALUES (@ProductName, @CategoryId, @Description, @Price, @Quantity, @ImagePath, GETDATE())";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                    cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                    cmd.Parameters.AddWithValue("@ImagePath", imagePath);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            // agar model validation fail ho jaye to categories dobara bhej do view me
            List<Category> categories = new List<Category>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Categories";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryId = Convert.ToInt32(rdr["CategoryId"]),
                        CategoryName = rdr["CategoryName"].ToString()
                    });
                }
            }
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View(product);
        }
        // GET: Product/Edit/5
        // GET: Edit Product
        public ActionResult Edit(int id)
        {
            Product product = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products WHERE ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductId", id);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    product = new Product
                    {
                        ProductId = Convert.ToInt32(rdr["ProductId"]),
                        ProductName = rdr["ProductName"].ToString(),
                        Description = rdr["Description"].ToString(),
                        Price = Convert.ToDecimal(rdr["Price"]),
                        Quantity = Convert.ToInt32(rdr["Quantity"]),
                        ImagePath = rdr["ImagePath"].ToString(),
                        CreatedAt = Convert.ToDateTime(rdr["CreatedAt"])
                    };
                }
            }

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: Edit Product
        [HttpPost]
        public ActionResult Edit(Product model, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                string imagePath = model.ImagePath; // Use existing path by default

                // Agar new image aayi hai toh usko save karo
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Server.MapPath("~/Content/Images/Products/");

                    // Agar folder nahi hai toh bana lo
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullPath = Path.Combine(path, fileName);
                    ImageFile.SaveAs(fullPath);

                    imagePath = "/Content/Images/Products/" + fileName;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Products SET 
                            ProductName = @ProductName,
                            Description = @Description,
                            Price = @Price,
                            Quantity = @Quantity,
                            ImagePath = @ImagePath
                            WHERE ProductId = @ProductId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProductName", model.ProductName);
                    cmd.Parameters.AddWithValue("@Description", model.Description);
                    cmd.Parameters.AddWithValue("@Price", model.Price);
                    cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                    cmd.Parameters.AddWithValue("@ImagePath", imagePath); // Safely assigned
                    cmd.Parameters.AddWithValue("@ProductId", model.ProductId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            // Agar ModelState invalid ho to same view wapas bhejna
            return View(model);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            Product product = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Products WHERE ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductId", id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    product = new Product
                    {
                        ProductId = Convert.ToInt32(rdr["ProductId"]),
                        ProductName = rdr["ProductName"].ToString(),
                        Description = rdr["Description"].ToString(),
                        Price = Convert.ToDecimal(rdr["Price"]),
                        Quantity = Convert.ToInt32(rdr["Quantity"]),
                        ImagePath = rdr["ImagePath"].ToString()
                    };
                }
            }

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
        [HttpPost]
        public ActionResult Delete(Product model)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Products WHERE ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProductId", model.ProductId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        //public ActionResult Index()
        //{
        //    var products = db.Products.Include("Category").ToList();
        //    return View(products);
        //}





    }



}