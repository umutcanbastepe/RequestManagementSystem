using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequestManagementSystem.Data;
using RequestManagementSystem.Filters;
using RequestManagementSystem.Helpers;
using RequestManagementSystem.Models.Entities;
using RequestManagementSystem.Models.Enums;
using RequestManagementSystem.Models.ViewModels;

namespace RequestManagementSystem.Controllers
{
    [RoleAuthorize(Role.Manager, Role.Admin)]
    public class ApprovalController : Controller
    {
        private readonly AppDbContext _context;

        public ApprovalController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? requestId = null)
        {
            var query = _context.Requests
                .Include(r => r.CreatedByUser)
                .Where(r => r.Status == RequestStatus.PendingApproval);

            if (requestId.HasValue)
            {
                var single = await query.FirstOrDefaultAsync(r => r.Id == requestId.Value);
                if (single != null)
                {
                    var list = new List<ApprovalItemVM>
                    {
                        MapToItem(single)
                    };
                    return View(list);
                }
            }

            var items = await query
                .OrderBy(r => r.CreatedAt)
                .Select(r => new ApprovalItemVM
                {
                    Id = r.Id,
                    RequestNo = r.RequestNo,
                    Title = r.Title,
                    Type = r.Type,
                    Priority = r.Priority,
                    CreatedByFullName = r.CreatedByUser.FullName,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var entity = await _context.Requests.FindAsync(id);
            if (entity == null) return NotFound();
            if (entity.Status != RequestStatus.PendingApproval)
                return RedirectToAction(nameof(Index));

            var userId = SessionHelper.GetUserId(HttpContext.Session).Value;

            entity.Status = RequestStatus.Approved;
            _context.RequestHistories.Add(new RequestHistory
            {
                RequestId = entity.Id,
                Status = RequestStatus.Approved,
                Description = "Talep onaylandı.",
                ActionByUserId = userId,
                ActionDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            TempData["Message"] = "Talep onaylandı.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int id)
        {
            var entity = await _context.Requests
                .Include(r => r.CreatedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (entity == null) return NotFound();
            if (entity.Status != RequestStatus.PendingApproval)
                return RedirectToAction(nameof(Index));

            var vm = new RejectVM
            {
                RequestId = entity.Id,
                RequestNo = entity.RequestNo
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(RejectVM model)
        {
            var entity = await _context.Requests.FindAsync(model.RequestId);
            if (entity == null) return NotFound();
            if (entity.Status != RequestStatus.PendingApproval)
                return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session).Value;
                entity.Status = RequestStatus.Rejected;
                _context.RequestHistories.Add(new RequestHistory
                {
                    RequestId = entity.Id,
                    Status = RequestStatus.Rejected,
                    Description = model.Reason,
                    ActionByUserId = userId,
                    ActionDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                TempData["Message"] = "Talep reddedildi.";
                return RedirectToAction(nameof(Index));
            }
            model.RequestNo = entity.RequestNo;
            return View(model);
        }

        private static ApprovalItemVM MapToItem(Request r)
        {
            return new ApprovalItemVM
            {
                Id = r.Id,
                RequestNo = r.RequestNo,
                Title = r.Title,
                Type = r.Type,
                Priority = r.Priority,
                CreatedByFullName = r.CreatedByUser?.FullName ?? "",
                CreatedAt = r.CreatedAt
            };
        }
    }
}
