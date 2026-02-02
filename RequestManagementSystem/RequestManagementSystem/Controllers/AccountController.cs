using Microsoft.AspNetCore.Mvc;
using RequestManagementSystem.Data;
using RequestManagementSystem.Helpers;
using RequestManagementSystem.Models.ViewModels;
using static RequestManagementSystem.Helpers.Constants;
namespace RequestManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            var user = _context.Users.FirstOrDefault(x =>
            x.Username == model.Username &&
            x.Password == model.Password);

            if (user == null)
            {
                ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
                return View();
            }

            HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
            HttpContext.Session.SetInt32(SessionKeys.Role, (int)user.Role);
            HttpContext.Session.SetString(SessionKeys.FullName, user.FullName);

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult UnauthorizedAccess()
        {
            return View("~/Views/Shared/Unauthorized.cshtml");
        }
    }
}
