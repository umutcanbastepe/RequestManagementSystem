using Microsoft.AspNetCore.Mvc;
using RequestManagementSystem.Data;
using RequestManagementSystem.Models.ViewModels;

namespace RequestManagementSystem.Controllers
{

    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardVM
            {
            };

            return View(vm);
        }
    }
}
