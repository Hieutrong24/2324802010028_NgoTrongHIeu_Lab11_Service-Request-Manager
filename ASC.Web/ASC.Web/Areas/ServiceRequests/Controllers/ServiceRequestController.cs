using ASC.Business.Interfaces;
using ASC.Model;
using ASC.Web.Areas.ServiceRequests.Models;
using ASC.Web.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASC.Web.Areas.ServiceRequests.Controllers
{
    [Area("ServiceRequests")]
    [Authorize]
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestOperations _serviceRequestOperations;
        private readonly IMasterDataCacheOperations _masterDataCacheOperations;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public ServiceRequestController(
            IServiceRequestOperations serviceRequestOperations,
            IMasterDataCacheOperations masterDataCacheOperations,
            UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            _serviceRequestOperations = serviceRequestOperations;
            _masterDataCacheOperations = masterDataCacheOperations;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> ServiceRequest()
        {
            var model = new NewServiceRequestViewModel
            {
                CustomerEmail = User.Identity?.Name ?? string.Empty,
                RequestedDate = DateTime.Now,
                Status = "New"
            };

            await LoadDropdownDataAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceRequest(NewServiceRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(model);
                return View(model);
            }

            model.CustomerEmail = User.Identity?.Name ?? model.CustomerEmail;
            model.Status = string.IsNullOrWhiteSpace(model.Status) ? "New" : model.Status;

            var serviceRequest = _mapper.Map<ServiceRequest>(model);

            var result = await _serviceRequestOperations.InsertServiceRequestAsync(serviceRequest);

            if (result)
            {
                TempData["SuccessMessage"] = "Service request created successfully.";
                return RedirectToAction("Dashboard", "Dashboard", new { area = "ServiceRequests" });
            }

            ModelState.AddModelError(string.Empty, "Cannot create service request.");

            await LoadDropdownDataAsync(model);
            return View(model);
        }

        private async Task LoadDropdownDataAsync(NewServiceRequestViewModel model)
        {
            var cache = await _masterDataCacheOperations.GetMasterDataCacheAsync();

            if (cache == null)
            {
                await _masterDataCacheOperations.RefreshMasterDataCacheAsync();
                cache = await _masterDataCacheOperations.GetMasterDataCacheAsync();
            }

            var masterKeys = cache?.MasterDataKeys ?? new List<MasterDataKey>();
            var masterValues = cache?.MasterDataValues ?? new List<MasterDataValue>();

            model.VehicleTypes = GetMasterValuesByKeyName(masterKeys, masterValues, "VehicleType");

            model.RequestedServiceList = GetMasterValuesByKeyName(masterKeys, masterValues, "ServiceType");

            if (!model.RequestedServiceList.Any())
            {
                model.RequestedServiceList = GetMasterValuesByKeyName(masterKeys, masterValues, "Status");
            }

            var engineers = await _userManager.GetUsersInRoleAsync("Engineer");

            model.ServiceEngineers = engineers
                .Where(x => !(x.LockoutEnd.HasValue && x.LockoutEnd.Value > DateTimeOffset.UtcNow))
                .OrderBy(x => x.UserName)
                .Select(x => new SelectListItem
                {
                    Value = x.Email ?? x.UserName ?? string.Empty,
                    Text = x.Email ?? x.UserName ?? string.Empty
                })
                .ToList();
        }

        private List<SelectListItem> GetMasterValuesByKeyName(
            List<MasterDataKey> masterKeys,
            List<MasterDataValue> masterValues,
            string keyName)
        {
            var key = masterKeys.FirstOrDefault(x =>
                x.IsActive &&
                x.Name.Equals(keyName, StringComparison.OrdinalIgnoreCase));

            if (key == null)
            {
                return new List<SelectListItem>();
            }

            return masterValues
                .Where(x => x.IsActive && x.MasterDataKeyId == key.Id)
                .OrderBy(x => x.Value)
                .Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Value
                })
                .ToList();
        }
    }
}