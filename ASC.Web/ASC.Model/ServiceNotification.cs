using System.ComponentModel.DataAnnotations;

namespace ASC.Model
{
    public class ServiceNotification : BaseEntity
    {
        [Required]
        [Display(Name = "Recipient Email")]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Notification Type")]
        public string NotificationType { get; set; } = "General";

        [Display(Name = "Related Entity Id")]
        public string? RelatedEntityId { get; set; }

        [Display(Name = "Related Entity Type")]
        public string? RelatedEntityType { get; set; }

        [Display(Name = "Is Read")]
        public bool IsRead { get; set; } = false;

        [Display(Name = "Read Date")]
        public DateTime? ReadDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}