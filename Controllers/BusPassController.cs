using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CollegeBusManagement.Data;
using CollegeBusManagement.Models;

namespace CollegeBusManagement.Controllers;

public class BusPassController : Controller
{
    private readonly ApplicationDbContext _context;

    public BusPassController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: BusPass/Apply
    public async Task<IActionResult> Apply(int? routeId)
    {
        await LoadRoutesDropDownAsync(routeId);
        var model = new BusPass
        {
            BusRouteId = routeId ?? 0,
            RequestDate = DateTime.Now
        };
        return View(model);
    }

    // POST: BusPass/Apply
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply([Bind("StudentName,RollNumber,Email,ContactNumber,BusRouteId")] BusPass busPass)
    {
        var route = await _context.BusRoutes.FindAsync(busPass.BusRouteId);
        if (route == null)
        {
            ModelState.AddModelError("BusRouteId", "Please select a valid bus route.");
        }
        else if (route.AvailableSeats <= 0)
        {
            ModelState.AddModelError("BusRouteId", $"No seats available on {route.RouteName}. Please choose another route.");
        }

        // Check for active duplicate pending application
        var existingPending = await _context.BusPasses
            .AnyAsync(p => p.RollNumber.ToUpper() == busPass.RollNumber.Trim().ToUpper() && p.Status == "Pending");
        if (existingPending)
        {
            ModelState.AddModelError("RollNumber", "An active pass application for this Roll Number is already pending review.");
        }

        if (ModelState.IsValid)
        {
            busPass.StudentName = busPass.StudentName.Trim();
            busPass.RollNumber = busPass.RollNumber.Trim().ToUpper();
            busPass.Email = busPass.Email.Trim();
            busPass.ContactNumber = busPass.ContactNumber.Trim();
            busPass.Status = "Pending";
            busPass.RequestDate = DateTime.Now;
            busPass.PassExpiryDate = null;

            _context.BusPasses.Add(busPass);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Your bus pass application has been submitted successfully! Use Roll Number '{busPass.RollNumber}' to check your status.";
            return RedirectToAction(nameof(Status), new { rollNumber = busPass.RollNumber });
        }

        await LoadRoutesDropDownAsync(busPass.BusRouteId);
        return View(busPass);
    }

    // GET: BusPass/AdminIndex
    public async Task<IActionResult> AdminIndex(string? statusFilter)
    {
        var query = _context.BusPasses
            .Include(p => p.BusRoute)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All")
        {
            query = query.Where(p => p.Status == statusFilter);
        }

        var applications = await query
            .OrderByDescending(p => p.RequestDate)
            .ToListAsync();

        ViewBag.CurrentFilter = statusFilter ?? "All";
        ViewBag.TotalCount = await _context.BusPasses.CountAsync();
        ViewBag.PendingCount = await _context.BusPasses.CountAsync(p => p.Status == "Pending");
        ViewBag.ApprovedCount = await _context.BusPasses.CountAsync(p => p.Status == "Approved");
        ViewBag.RejectedCount = await _context.BusPasses.CountAsync(p => p.Status == "Rejected");

        return View(applications);
    }

    // POST: BusPass/Approve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var pass = await _context.BusPasses
            .Include(p => p.BusRoute)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pass == null)
        {
            return NotFound();
        }

        if (pass.Status != "Approved")
        {
            if (pass.BusRoute == null)
            {
                TempData["ErrorMessage"] = "Assigned route could not be found.";
                return RedirectToAction(nameof(AdminIndex));
            }

            if (pass.BusRoute.AvailableSeats <= 0)
            {
                TempData["ErrorMessage"] = $"Cannot approve pass. Route '{pass.BusRoute.RouteName}' has no available seats.";
                return RedirectToAction(nameof(AdminIndex));
            }

            // Decrement available seats on the assigned route
            pass.BusRoute.AvailableSeats -= 1;
            pass.Status = "Approved";
            // Set 6-month semester validity
            pass.PassExpiryDate = DateTime.Now.AddMonths(6);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Pass for {pass.StudentName} (Roll: {pass.RollNumber}) has been approved! 1 seat allocated on {pass.BusRoute.RouteName}.";
        }

        return RedirectToAction(nameof(AdminIndex));
    }

    // POST: BusPass/Reject/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var pass = await _context.BusPasses
            .Include(p => p.BusRoute)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pass == null)
        {
            return NotFound();
        }

        // If previously approved, restore the seat
        if (pass.Status == "Approved" && pass.BusRoute != null)
        {
            pass.BusRoute.AvailableSeats = Math.Min(pass.BusRoute.TotalSeats, pass.BusRoute.AvailableSeats + 1);
        }

        pass.Status = "Rejected";
        await _context.SaveChangesAsync();

        TempData["WarningMessage"] = $"Application for {pass.StudentName} (Roll: {pass.RollNumber}) marked as Rejected.";
        return RedirectToAction(nameof(AdminIndex));
    }

    // GET: BusPass/Status
    [HttpGet]
    public async Task<IActionResult> Status(string? rollNumber)
    {
        if (string.IsNullOrWhiteSpace(rollNumber))
        {
            return View();
        }

        var trimmedRoll = rollNumber.Trim().ToUpper();
        var passes = await _context.BusPasses
            .Include(p => p.BusRoute)
            .ThenInclude(r => r!.BusStops)
            .Where(p => p.RollNumber.ToUpper() == trimmedRoll)
            .OrderByDescending(p => p.RequestDate)
            .ToListAsync();

        ViewBag.SearchRollNumber = trimmedRoll;

        if (!passes.Any())
        {
            ViewBag.NotFoundMessage = $"No bus pass application found for Roll Number '{trimmedRoll}'. Please check your input or apply for a new pass.";
            return View();
        }

        return View(passes.First());
    }

    // POST: BusPass/SearchStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SearchStatus(string rollNumber)
    {
        return RedirectToAction(nameof(Status), new { rollNumber });
    }

    private async Task LoadRoutesDropDownAsync(int? selectedRouteId = null)
    {
        var routes = await _context.BusRoutes
            .OrderBy(r => r.RouteName)
            .Select(r => new
            {
                r.Id,
                DisplayText = $"{r.RouteName} - ₹{r.MonthlyFee:N0}/mo ({r.AvailableSeats} seats left)"
            })
            .ToListAsync();

        ViewBag.Routes = new SelectList(routes, "Id", "DisplayText", selectedRouteId);
    }
}
