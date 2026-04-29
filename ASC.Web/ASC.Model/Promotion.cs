using System.ComponentModel.DataAnnotations;

namespace ASC.Model
{
    public class Promotion : BaseEntity
    {
        [Required]
        [Display(Name = "Promotion Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Promotion Code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 100)]
        [Display(Name = "Discount Percent")]
        public decimal DiscountPercent { get; set; }

        [Display(Name = "Applicable Service")]
        public string? ApplicableService { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);

        [Display(Name = "Terms And Conditions")]
        public string? TermsAndConditions { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}