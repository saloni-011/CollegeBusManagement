using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeBusManagement.Data;
using CollegeBusManagement.Models;

namespace CollegeBusManagement.Controllers;

public class RouteController : Controller
{
    private readonly ApplicationDbContext _context;

    public RouteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Route
    public async Task<IActionResult> Index()
    {
        var routes = await _context.BusRoutes
            .Include(r => r.BusStops)
            .OrderBy(r => r.RouteName)
            .ToListAsync();

        return View(routes);
    }

    // GET: Route/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var route = await _context.BusRoutes
            .Include(r => r.BusStops)
            .Include(r => r.BusPasses)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (route == null)
        {
            return NotFound();
        }

        return View(route);
    }

    // GET: Route/Create
    public IActionResult Create()
    {
        var model = new BusRoute
        {
            TotalSeats = 40,
            AvailableSeats = 40,
            MonthlyFee = 1200.00m
        };
        return View(model);
    }

    // POST: Route/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RouteName,StartPoint,EndPoint,MonthlyFee,TotalSeats,AvailableSeats")] BusRoute busRoute, string? initialStops)
    {
        if (ModelState.IsValid)
        {
            if (busRoute.AvailableSeats <= 0)
            {
                busRoute.AvailableSeats = busRoute.TotalSeats;
            }

            // Parse initial intermediate stops if supplied
            if (!string.IsNullOrWhiteSpace(initialStops))
            {
                var stopNames = initialStops.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in stopNames)
                {
                    var trimmed = name.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        busRoute.BusStops.Add(new BusStop
                        {
                            StopName = trimmed,
                            PickupTime = "07:30 AM"
                        });
                    }
                }
            }

            _context.Add(busRoute);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Route '{busRoute.RouteName}' was created successfully!";
            return RedirectToAction(nameof(Index));
        }

        return View(busRoute);
    }

    // GET: Route/AddStop/5
    public async Task<IActionResult> AddStop(int? id)
    {
        if (id == null) return NotFound();

        var route = await _context.BusRoutes.FindAsync(id);
        if (route == null) return NotFound();

        ViewBag.Route = route;
        return View(new BusStop { BusRouteId = route.Id, PickupTime = "07:30 AM" });
    }

    // POST: Route/AddStop
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStop([Bind("StopName,PickupTime,BusRouteId")] BusStop busStop)
    {
        if (ModelState.IsValid)
        {
            _context.BusStops.Add(busStop);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Stop '{busStop.StopName}' was added to the route!";
            return RedirectToAction(nameof(Details), new { id = busStop.BusRouteId });
        }

        ViewBag.Route = await _context.BusRoutes.FindAsync(busStop.BusRouteId);
        return View(busStop);
    }
}
