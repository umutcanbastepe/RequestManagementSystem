using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestManagementSystem.Data;
using RequestManagementSystem.Filters;
using RequestManagementSystem.Helpers;
using RequestManagementSystem.Models.Entities;
using RequestManagementSystem.Models.Enums;
using RequestManagementSystem.Models.ViewModels;
using static RequestManagementSystem.Helpers.Constants;

namespace RequestManagementSystem.Controllers
{
    [RoleAuthorize(Role.Admin)]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.Users
                .OrderBy(u => u.Username)
                .Select(u => new UserListVM
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    RoleName = u.Role.ToString()
                })
                .ToListAsync();

            return View(list);
        }

        public IActionResult Create()
        {
            return View(new UserCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateVM model)
        {
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor.");

            if (ModelState.IsValid)
            {
                _context.Users.Add(new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    FullName = model.FullName,
                    Role = model.Role
                });
                await _context.SaveChangesAsync();
                TempData[TempDataKeys.Message] = "Kullanıcı oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(new UserEditVM
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditVM model)
        {
            if (id != model.Id) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (await _context.Users.AnyAsync(u => u.Username == model.Username && u.Id != id))
                ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor.");

            if (ModelState.IsValid)
            {
                user.Username = model.Username;
                user.FullName = model.FullName;
                user.Role = model.Role;
                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                    user.Password = model.NewPassword;
                await _context.SaveChangesAsync();
                TempData[TempDataKeys.Message] = "Kullanıcı güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = SessionHelper.GetUserId(HttpContext.Session).Value;
            if (id == currentUserId)
            {
                TempData[TempDataKeys.Error] = "Kendinizi silemezsiniz.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var hasRequests = await _context.Requests.AnyAsync(r => r.CreatedByUserId == id);
            if (hasRequests)
            {
                TempData[TempDataKeys.Error] = "Bu kullanıcının talepleri olduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData[TempDataKeys.Message] = "Kullanıcı silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
