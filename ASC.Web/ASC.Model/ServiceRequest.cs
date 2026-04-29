using System.ComponentModel.DataAnnotations;

namespace ASC.Model
{
    public class ServiceRequest : BaseEntity
    {
        [Required]
        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vehicle Name")]
        public string VehicleName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Requested Services")]
        public string RequestedServices { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; } = DateTime.Now;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Service Engineer")]
        public string? ServiceEngineer { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "New";

        
        public string? RequestedBy { get; set; }

        public string? ServiceType { get; set; }

        public string? AssignedTo { get; set; }
    }
}