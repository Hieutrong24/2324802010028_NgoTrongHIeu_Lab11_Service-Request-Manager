using ASC.Business.Interfaces;
using ASC.Web.Areas.ServiceRequests.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceRequests.Controllers
{
    [Area("ServiceRequests")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IServiceRequestOperations _serviceRequestOperations;

        public DashboardController(IServiceRequestOperations serviceRequestOperations)
        {
            _serviceRequestOperations = serviceRequestOperations;
        }

        [HttpGet("/ServiceRequests/Dashboard/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var currentUserEmail = User.Identity?.Name ?? string.Empty;
            var currentRole = GetCurrentRole();

            var serviceRequests = await _serviceRequestOperations
                .GetServiceRequestsByRoleAsync(currentRole, currentUserEmail);

            var model = new DashboardViewModel
            {
                ServiceRequests = serviceRequests,
                CurrentRole = currentRole,
                CurrentUserEmail = currentUserEmail,
                TotalRequests = serviceRequests.Count,
                NewRequests = serviceRequests.Count(x =>
                    x.Status != null &&
                    x.Status.Equals("New", StringComparison.OrdinalIgnoreCase)),
                InProgressRequests = serviceRequests.Count(x =>
                    x.Status != null &&
                    (
                        x.Status.Equals("InProgress", StringComparison.OrdinalIgnoreCase) ||
                        x.Status.Equals("In Progress", StringComparison.OrdinalIgnoreCase)
                    )),
                CompletedRequests = serviceRequests.Count(x =>
                    x.Status != null &&
                    x.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            };

            return View(model);
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