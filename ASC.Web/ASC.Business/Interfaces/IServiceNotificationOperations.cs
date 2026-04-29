using ASC.Model;

namespace ASC.Business.Interfaces
{
    public interface IServiceNotificationOperations
    {
        Task<List<ServiceNotification>> GetAllNotificationsAsync();

        Task<List<ServiceNotification>> GetNotificationsByUserAsync(string email);

        Task<ServiceNotification?> GetNotificationByIdAsync(string id);

        Task<bool> CreateNotificationAsync(ServiceNotification notification);

        Task<bool> UpdateNotificationAsync(ServiceNotification notification);

        Task<bool> MarkAsReadAsync(string id);

        Task<bool> DeleteNotificationAsync(string id);
    }
}