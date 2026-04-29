using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class ServiceNotificationViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Recipient email is required")]
        [Display(Name = "Recipient Email")]
        public string RecipientEmail { get; set; } = "All";

        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Notification Type")]
        public string NotificationType { get; set; } = "General";

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}