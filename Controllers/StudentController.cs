using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CollegeBusManagement.Models;
using CollegeBusManagement.Models.ViewModels;
using CollegeBusManagement.Data;

namespace CollegeBusManagement.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ApplyPass()
        {
            var model = new BusPassApplicationViewModel
            {
                Routes = _context.BusRoutes.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.RouteName
                }),
                Stops = _context.BusStops.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.StopName
                })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyPass(BusPassApplicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var passApplication = new BusPass
                {
                    BusRouteId = model.SelectedRouteId,
                    Status = "Pending"
                };

                _context.BusPasses.Add(passApplication);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyPass));
            }

            model.Routes = _context.BusRoutes.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.RouteName
            });
            model.Stops = _context.BusStops.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.StopName
            });

            return View(model);
        }

        public IActionResult MyPass()
        {
            var applications = _context.BusPasses.ToList();
            return View(applications);
        }
    }
}
