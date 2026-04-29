using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.Configuration.Models
{
    public class MasterDataValueViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Master key is required")]
        [Display(Name = "Master Key")]
        public string MasterDataKeyId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Master value is required")]
        [Display(Name = "Master Value")]
        public string Value { get; set; } = string.Empty;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> MasterKeys { get; set; }
            = new List<SelectListItem>();
    }
}   