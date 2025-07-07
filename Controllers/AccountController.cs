using System.Web.Mvc;
using TaskManagementApp.DAL;
using TaskManagementApp.Models;

namespace TaskManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbHelper db = new DbHelper();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (db.IsUserNameExists(user.Username))
                {
                    ModelState.AddModelError("UserName", "This username is already taken.");
                    return View(user);
                }
                user.Role = "User"; // default role
                bool success = db.RegisterUser(user);
                if (success)
                    return RedirectToAction("Login");
                else
                    ViewBag.Error = "Registration failed.";
            }
            return View(user);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both fields.";
                return View();
            }

            var user = db.Login(username, password);
            if (user != null)
            {
                Session["UserId"] = user.Id;
                Session["Username"] = user.Username;
                Session["Role"] = user.Role;
                return RedirectToAction("Index", "Task");
            }

            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}