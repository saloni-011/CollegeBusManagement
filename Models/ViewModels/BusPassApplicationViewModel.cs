using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollegeBusManagement.Models.ViewModels
{
    public class BusPassApplicationViewModel
    {
        [Required(ErrorMessage = "Please select a route.")]
        [Display(Name = "Bus Route")]
        public int SelectedRouteId { get; set; }

        [Required(ErrorMessage = "Please select a stop.")]
        [Display(Name = "Boarding Stop")]
        public int SelectedStopId { get; set; }

        [Required(ErrorMessage = "Please select pass type.")]
        [Display(Name = "Pass Type")]
        public string PassType { get; set; }

        public IEnumerable<SelectListItem>? Routes { get; set; }
        public IEnumerable<SelectListItem>? Stops { get; set; }
    }
}
