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
    [RoleAuthorize(Role.Admin, Role.Manager, Role.User)]
    public class RequestController : Controller
    {
        private readonly AppDbContext _context;

        public RequestController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? searchTitle = null, RequestStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session).Value;
            var role = SessionHelper.GetRole(HttpContext.Session);

            var query = _context.Requests
                .Include(r => r.CreatedByUser)
                .AsQueryable();

            if (role == Role.User)
                query = query.Where(r => r.CreatedByUserId == userId);

            if (!string.IsNullOrWhiteSpace(searchTitle))
                query = query.Where(r => r.Title.Contains(searchTitle));
            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);
            if (dateFrom.HasValue)
                query = query.Where(r => r.CreatedAt.Date >= dateFrom.Value.Date);
            if (dateTo.HasValue)
                query = query.Where(r => r.CreatedAt.Date <= dateTo.Value.Date);

            var totalCount = await query.CountAsync();
            var list = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            var vm = new RequestListIndexVM
            {
                Requests = list,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                SearchTitle = searchTitle,
                FilterStatus = status,
                FilterDateFrom = dateFrom,
                FilterDateTo = dateTo
            };
            return View(vm);
        }

        public IActionResult Create()
        {
            return View(new RequestCreateVM
            {
                Status = RequestStatus.Draft,
                Priority = RequestPriority.Medium,
                Type = RequestType.IT
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session).Value;
                var requestNo = await GenerateRequestNoAsync();

                var entity = new Request
                {
                    RequestNo = requestNo,
                    Title = model.Title,
                    Description = model.Description,
                    Type = model.Type,
                    Priority = model.Priority,
                    Status = RequestStatus.Draft,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.Now
                };
                _context.Requests.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session).Value;
            var role = SessionHelper.GetRole(HttpContext.Session);

            var entity = await _context.Requests.FindAsync(id);
            if (entity == null) return NotFound();

            if (entity.Status == RequestStatus.Approved)
                return RedirectToAction(nameof(Detail), new { id });

            if (role == Role.User && entity.CreatedByUserId != userId)
                return RedirectToAction("UnauthorizedAccess", "Account");

            var vm = new RequestCreateVM
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Type = entity.Type,
                Priority = entity.Priority,
                Status = entity.Status
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RequestCreateVM model)
        {
            if (id != model.Id) return NotFound();

            var entity = await _context.Requests.FindAsync(id);
            if (entity == null) return NotFound();

            if (entity.Status == RequestStatus.Approved)
            {
                ModelState.AddModelError("", "Onaylanmış talep düzenlenemez.");
                return View(model);
            }

            var userId = SessionHelper.GetUserId(HttpContext.Session).Value;
            var role = SessionHelper.GetRole(HttpContext.Session);
            if (role == Role.User && entity.CreatedByUserId != userId)
                return RedirectToAction("UnauthorizedAccess", "Account");

            if (ModelState.IsValid)
            {
                entity.Title = model.Title;
                entity.Description = model.Description;
                entity.Type = model.Type;
                entity.Priority = model.Priority;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id)
        {
            var entity = await _context.Requests.FindAsync(id);
            if (entity == null) return NotFound();
            if (entity.Status != RequestStatus.Draft) return RedirectToAction(nameof(Detail), new { id });

            var userId = SessionHelper.GetUserId(HttpContext.Session).Value;
            if (entity.CreatedByUserId != userId)
                return RedirectToAction("UnauthorizedAccess", "Account");

            entity.Status = RequestStatus.PendingApproval;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id });
        }

        public async Task<IActionResult> Detail(int id)
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session).Value;
            var role = SessionHelper.GetRole(HttpContext.Session);

            var entity = await _context.Requests
                .Include(r => r.CreatedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (entity == null) return NotFound();

            if (role == Role.User && entity.CreatedByUserId != userId)
                return RedirectToAction("UnauthorizedAccess", "Account");

            var history = await _context.RequestHistories
                .Include(h => h.ActionByUser)
                .Where(h => h.RequestId == id)
                .OrderByDescending(h => h.ActionDate)
                .Select(h => new RequestHistoryItemVM
                {
                    Status = h.Status,
                    Description = h.Description ?? "",
                    ActionByFullName = h.ActionByUser.FullName,
                    ActionDate = h.ActionDate
                })
                .ToListAsync();

            var vm = new RequestDetailVM
            {
                Id = entity.Id,
                RequestNo = entity.RequestNo,
                Title = entity.Title,
                Description = entity.Description,
                Type = entity.Type,
                Priority = entity.Priority,
                Status = entity.Status,
                CreatedByFullName = entity.CreatedByUser.FullName,
                CreatedAt = entity.CreatedAt,
                CanEdit = (entity.Status != RequestStatus.Approved && entity.Status != RequestStatus.Rejected) && (role == Role.Admin || entity.CreatedByUserId == userId),
                CanApproveReject = (role == Role.Manager || role == Role.Admin) && entity.Status == RequestStatus.PendingApproval,
                History = history
            };
            return View(vm);
        }

        private async Task<string> GenerateRequestNoAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _context.Requests.CountAsync(r => r.CreatedAt.Year == year);
            return $"TAL-{year}-{(count + 1):D4}";
        }
    }
}
