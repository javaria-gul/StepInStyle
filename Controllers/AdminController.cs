using System.Web.Mvc;

namespace StepInStyle.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Dashboard()
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }

}
