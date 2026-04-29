namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class ServiceNotificationsViewModel
    {
        public List<ServiceNotificationViewModel> Notifications { get; set; }
            = new List<ServiceNotificationViewModel>();

        public ServiceNotificationViewModel Notification { get; set; }
            = new ServiceNotificationViewModel();

        public string CurrentRole { get; set; } = string.Empty;

        public string CurrentUserEmail { get; set; } = string.Empty;
    }
}