using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.Configuration.Models
{
    public class MasterDataKeyViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Master key name is required")]
        [Display(Name = "Master Key Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}