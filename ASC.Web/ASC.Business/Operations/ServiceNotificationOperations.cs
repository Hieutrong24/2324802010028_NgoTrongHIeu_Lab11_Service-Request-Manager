using ASC.Business.Interfaces;
using ASC.DataAccess.Repository;
using ASC.Model;

namespace ASC.Business.Operations
{
    public class ServiceNotificationOperations : IServiceNotificationOperations
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceNotificationOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<ServiceNotification>> GetAllNotificationsAsync()
        {
            var notifications = _unitOfWork.ServiceNotificationRepository
                .GetAll()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            return Task.FromResult(notifications);
        }

        public Task<List<ServiceNotification>> GetNotificationsByUserAsync(string email)
        {
            var notifications = _unitOfWork.ServiceNotificationRepository
                .GetAll()
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    (
                        x.RecipientEmail == email ||
                        x.RecipientEmail == "All"
                    ))
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            return Task.FromResult(notifications);
        }

        public async Task<ServiceNotification?> GetNotificationByIdAsync(string id)
        {
            return await _unitOfWork.ServiceNotificationRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateNotificationAsync(ServiceNotification notification)
        {
            notification.Id = Guid.NewGuid().ToString();
            notification.CreatedDate = DateTime.Now;
            notification.IsDeleted = false;
            notification.IsRead = false;

            await _unitOfWork.ServiceNotificationRepository.AddAsync(notification);

            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> UpdateNotificationAsync(ServiceNotification notification)
        {
            var existingNotification = await _unitOfWork.ServiceNotificationRepository
                .GetByIdAsync(notification.Id);

            if (existingNotification == null)
            {
                return false;
            }

            existingNotification.RecipientEmail = notification.RecipientEmail;
            existingNotification.Title = notification.Title;
            existingNotification.Message = notification.Message;
            existingNotification.NotificationType = notification.NotificationType;
            existingNotification.RelatedEntityId = notification.RelatedEntityId;
            existingNotification.RelatedEntityType = notification.RelatedEntityType;
            existingNotification.IsActive = notification.IsActive;
            existingNotification.ModifiedDate = DateTime.Now;

            _unitOfWork.ServiceNotificationRepository.Update(existingNotification);

            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> MarkAsReadAsync(string id)
        {
            var notification = await _unitOfWork.ServiceNotificationRepository.GetByIdAsync(id);

            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            notification.ReadDate = DateTime.Now;
            notification.ModifiedDate = DateTime.Now;

            _unitOfWork.ServiceNotificationRepository.Update(notification);

            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> DeleteNotificationAsync(string id)
        {
            var notification = await _unitOfWork.ServiceNotificationRepository.GetByIdAsync(id);

            if (notification == null)
            {
                return false;
            }

            notification.IsDeleted = true;
            notification.ModifiedDate = DateTime.Now;

            _unitOfWork.ServiceNotificationRepository.Update(notification);

            return await _unitOfWork.SaveAsync() > 0;
        }
    }
}