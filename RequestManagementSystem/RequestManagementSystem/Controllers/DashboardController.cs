using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestManagementSystem.Data;
using RequestManagementSystem.Filters;
using RequestManagementSystem.Helpers;
using RequestManagementSystem.Models.Enums;
using RequestManagementSystem.Models.ViewModels;

namespace RequestManagementSystem.Controllers
{

    [RoleAuthorize(Role.Admin, Role.Manager, Role.User)]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session).Value;
            var role = SessionHelper.GetRole(HttpContext.Session);
            var isManagerOrAdmin = SessionHelper.IsInRole(HttpContext.Session, Role.Manager, Role.Admin);

            var vm = new DashboardVM
            {
                IsManagerOrAdmin = isManagerOrAdmin
            };

            if (isManagerOrAdmin)
            {
                vm.TotalRequestCount = await _context.Requests.CountAsync();
                vm.PendingApprovalCount = await _context.Requests.CountAsync(r => r.Status == RequestStatus.PendingApproval);
                vm.LastRequests = await _context.Requests
                    .Include(r => r.CreatedByUser)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new RequestListVM
                    {
                        Id = r.Id,
                        RequestNo = r.RequestNo,
                        Title = r.Title,
                        Type = r.Type,
                        Priority = r.Priority,
                        Status = r.Status,
                        CreatedByFullName = r.CreatedByUser.FullName,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();
            }
            else
            {
                vm.MyRequestCount = await _context.Requests.CountAsync(r => r.CreatedByUserId == userId);
                vm.MyPendingCount = await _context.Requests.CountAsync(r => r.CreatedByUserId == userId && r.Status == RequestStatus.PendingApproval);
                vm.LastRequests = await _context.Requests
                    .Include(r => r.CreatedByUser)
                    .Where(r => r.CreatedByUserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new RequestListVM
                    {
                        Id = r.Id,
                        RequestNo = r.RequestNo,
                        Title = r.Title,
                        Type = r.Type,
                        Priority = r.Priority,
                        Status = r.Status,
                        CreatedByFullName = r.CreatedByUser.FullName,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();
            }

            return View(vm);
        }
    }
}
