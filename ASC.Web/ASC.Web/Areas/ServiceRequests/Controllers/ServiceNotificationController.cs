using ASC.Business.Interfaces;
using ASC.Model;
using ASC.Web.Areas.ServiceRequests.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceRequests.Controllers
{
    [Area("ServiceRequests")]
    [Authorize]
    public class ServiceNotificationController : Controller
    {
        private readonly IServiceNotificationOperations _serviceNotificationOperations;

        public ServiceNotificationController(IServiceNotificationOperations serviceNotificationOperations)
        {
            _serviceNotificationOperations = serviceNotificationOperations;
        }

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var currentUserEmail = User.Identity?.Name ?? string.Empty;
            var currentRole = GetCurrentRole();

            List<ServiceNotification> notifications;

            if (currentRole == "Admin")
            {
                notifications = await _serviceNotificationOperations.GetAllNotificationsAsync();
            }
            else
            {
                notifications = await _serviceNotificationOperations.GetNotificationsByUserAsync(currentUserEmail);
            }

            var model = new ServiceNotificationsViewModel
            {
                CurrentRole = currentRole,
                CurrentUserEmail = currentUserEmail,
                Notifications = notifications.Select(x => new ServiceNotificationViewModel
                {
                    Id = x.Id,
                    RecipientEmail = x.RecipientEmail,
                    Title = x.Title,
                    Message = x.Message,
                    NotificationType = x.NotificationType,
                    IsActive = x.IsActive,
                    IsRead = x.IsRead,
                    ReadDate = x.ReadDate,
                    CreatedDate = x.CreatedDate
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Notifications(ServiceNotificationsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await ReloadNotificationsAsync(model);
                return View(model);
            }

            var notification = new ServiceNotification
            {
                Id = model.Notification.Id ?? string.Empty,
                RecipientEmail = model.Notification.RecipientEmail,
                Title = model.Notification.Title,
                Message = model.Notification.Message,
                NotificationType = model.Notification.NotificationType,
                IsActive = model.Notification.IsActive
            };

            if (string.IsNullOrWhiteSpace(model.Notification.Id))
            {
                var result = await _serviceNotificationOperations.CreateNotificationAsync(notification);

                TempData["SuccessMessage"] = result
                    ? "Notification created successfully."
                    : "Cannot create notification.";
            }
            else
            {
                var result = await _serviceNotificationOperations.UpdateNotificationAsync(notification);

                TempData["SuccessMessage"] = result
                    ? "Notification updated successfully."
                    : "Cannot update notification.";
            }

            return RedirectToAction(nameof(Notifications));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                await _serviceNotificationOperations.MarkAsReadAsync(id);
            }

            return RedirectToAction(nameof(Notifications));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                await _serviceNotificationOperations.DeleteNotificationAsync(id);
            }

            TempData["SuccessMessage"] = "Notification deleted successfully.";

            return RedirectToAction(nameof(Notifications));
        }

        private async Task ReloadNotificationsAsync(ServiceNotificationsViewModel model)
        {
            var currentUserEmail = User.Identity?.Name ?? string.Empty;
            var currentRole = GetCurrentRole();

            List<ServiceNotification> notifications;

            if (currentRole == "Admin")
            {
                notifications = await _serviceNotificationOperations.GetAllNotificationsAsync();
            }
            else
            {
                notifications = await _serviceNotificationOperations.GetNotificationsByUserAsync(currentUserEmail);
            }

            model.CurrentRole = currentRole;
            model.CurrentUserEmail = currentUserEmail;

            model.Notifications = notifications.Select(x => new ServiceNotificationViewModel
            {
                Id = x.Id,
                RecipientEmail = x.RecipientEmail,
                Title = x.Title,
                Message = x.Message,
                NotificationType = x.NotificationType,
                IsActive = x.IsActive,
                IsRead = x.IsRead,
                ReadDate = x.ReadDate,
                CreatedDate = x.CreatedDate
            }).ToList();
        }

        private string GetCurrentRole()
        {
            if (User.IsInRole("Admin"))
            {
                return "Admin";
            }

            if (User.IsInRole("Engineer"))
            {
                return "Engineer";
            }

            return "User";
        }
    }
}