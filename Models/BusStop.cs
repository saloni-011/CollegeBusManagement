using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeBusManagement.Models;

public class BusStop
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Stop name is required")]
    [StringLength(100, ErrorMessage = "Stop name cannot exceed 100 characters")]
    [Display(Name = "Stop Name")]
    public string StopName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pickup time is required")]
    [StringLength(50, ErrorMessage = "Pickup time cannot exceed 50 characters")]
    [Display(Name = "Pickup Time")]
    public string PickupTime { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Bus Route")]
    public int BusRouteId { get; set; }

    [ForeignKey(nameof(BusRouteId))]
    public BusRoute? BusRoute { get; set; }
}
