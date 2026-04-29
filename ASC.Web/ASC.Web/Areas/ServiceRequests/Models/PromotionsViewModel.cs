namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class PromotionsViewModel
    {
        public List<PromotionViewModel> Promotions { get; set; }
            = new List<PromotionViewModel>();

        public PromotionViewModel Promotion { get; set; }
            = new PromotionViewModel();

        public string CurrentRole { get; set; } = string.Empty;

        public string CurrentUserEmail { get; set; } = string.Empty;
    }
}