namespace ASC.Web.Areas.Configuration.Models
{
    public class MasterKeysViewModel
    {
        public List<MasterDataKeyViewModel> MasterKeys { get; set; }
            = new List<MasterDataKeyViewModel>();

        public MasterDataKeyViewModel MasterKey { get; set; }
            = new MasterDataKeyViewModel();
    }
}