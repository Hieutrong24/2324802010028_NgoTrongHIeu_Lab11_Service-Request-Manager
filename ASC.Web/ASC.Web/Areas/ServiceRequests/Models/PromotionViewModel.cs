using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class PromotionViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Promotion title is required")]
        [Display(Name = "Promotion Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Promotion code is required")]
        [Display(Name = "Promotion Code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100")]
        [Display(Name = "Discount Percent")]
        public decimal DiscountPercent { get; set; }

        [Display(Name = "Applicable Service")]
        public string? ApplicableService { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);

        [Display(Name = "Terms And Conditions")]
        public string? TermsAndConditions { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}