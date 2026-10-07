using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeBusManagement.Models;

public class BusRoute
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Route name is required")]
    [StringLength(100, ErrorMessage = "Route name cannot exceed 100 characters")]
    [Display(Name = "Route Name")]
    public string RouteName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start point is required")]
    [StringLength(100, ErrorMessage = "Start point cannot exceed 100 characters")]
    [Display(Name = "Start Point")]
    public string StartPoint { get; set; } = string.Empty;

    [Required(ErrorMessage = "End point is required")]
    [StringLength(100, ErrorMessage = "End point cannot exceed 100 characters")]
    [Display(Name = "End Point")]
    public string EndPoint { get; set; } = string.Empty;

    [Required(ErrorMessage = "Monthly fee is required")]
    [Range(0, 100000, ErrorMessage = "Monthly fee must be non-negative")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Monthly Fee")]
    public decimal MonthlyFee { get; set; }

    [Required(ErrorMessage = "Total seats is required")]
    [Range(1, 1000, ErrorMessage = "Total seats must be at least 1")]
    [Display(Name = "Total Seats")]
    public int TotalSeats { get; set; }

    [Required(ErrorMessage = "Available seats is required")]
    [Range(0, 1000, ErrorMessage = "Available seats cannot be negative")]
    [Display(Name = "Available Seats")]
    public int AvailableSeats { get; set; }

    // Navigation properties
    public ICollection<BusStop> BusStops { get; set; } = new List<BusStop>();
    public ICollection<BusPass> BusPasses { get; set; } = new List<BusPass>();
}
