using ASC.Model;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class DashboardViewModel
    {
        public List<ServiceRequest> ServiceRequests { get; set; }
            = new List<ServiceRequest>();

        public int TotalRequests { get; set; }

        public int NewRequests { get; set; }

        public int InProgressRequests { get; set; }

        public int CompletedRequests { get; set; }

        public string CurrentRole { get; set; } = string.Empty;

        public string CurrentUserEmail { get; set; } = string.Empty;
    }
}