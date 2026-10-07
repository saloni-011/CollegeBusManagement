using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeBusManagement.Models;

public class BusPass
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Student name is required")]
    [StringLength(100, ErrorMessage = "Student name cannot exceed 100 characters")]
    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Roll number is required")]
    [StringLength(50, ErrorMessage = "Roll number cannot exceed 50 characters")]
    [Display(Name = "Roll Number")]
    public string RollNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Contact number cannot exceed 20 characters")]
    [Display(Name = "Contact Number")]
    public string ContactNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Bus Route")]
    public int BusRouteId { get; set; }

    [ForeignKey(nameof(BusRouteId))]
    public BusRoute? BusRoute { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"

    [Display(Name = "Request Date")]
    [DataType(DataType.DateTime)]
    public DateTime RequestDate { get; set; } = DateTime.Now;

    [Display(Name = "Pass Expiry Date")]
    [DataType(DataType.Date)]
    public DateTime? PassExpiryDate { get; set; }
}
