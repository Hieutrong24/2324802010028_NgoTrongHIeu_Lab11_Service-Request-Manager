using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASC.Web.Areas.Configuration.Models
{
    public class MasterValuesViewModel
    {
        public List<MasterDataValueViewModel> MasterValues { get; set; }
            = new List<MasterDataValueViewModel>();

        public MasterDataValueViewModel MasterValue { get; set; }
            = new MasterDataValueViewModel();

        public List<SelectListItem> MasterKeys { get; set; }
            = new List<SelectListItem>();
    }
}