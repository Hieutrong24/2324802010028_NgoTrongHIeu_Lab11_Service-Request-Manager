using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class NewServiceRequestViewModel
    {
        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vehicle name is required")]
        [Display(Name = "Vehicle Name")]
        public string VehicleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vehicle type is required")]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Requested service is required")]
        [Display(Name = "Requested Service")]
        public string RequestedServices { get; set; } = string.Empty;

        [Required(ErrorMessage = "Requested date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; } = DateTime.Now;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Service Engineer")]
        public string? ServiceEngineer { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "New";

        public List<SelectListItem> VehicleTypes { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> RequestedServiceList { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> ServiceEngineers { get; set; }
            = new List<SelectListItem>();
    }
}