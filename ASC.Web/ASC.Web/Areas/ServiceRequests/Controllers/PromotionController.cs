using ASC.Business.Interfaces;
using ASC.Model;
using ASC.Web.Areas.ServiceRequests.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Areas.ServiceRequests.Controllers
{
    [Area("ServiceRequests")]
    [Authorize]
    public class PromotionController : Controller
    {
        private readonly IPromotionOperations _promotionOperations;

        public PromotionController(IPromotionOperations promotionOperations)
        {
            _promotionOperations = promotionOperations;
        }

        [HttpGet]
        public async Task<IActionResult> Promotions()
        {
            var currentUserEmail = User.Identity?.Name ?? string.Empty;
            var currentRole = GetCurrentRole();

            List<Promotion> promotions;

            if (currentRole == "Admin")
            {
                promotions = await _promotionOperations.GetAllPromotionsAsync();
            }
            else
            {
                promotions = await _promotionOperations.GetActivePromotionsAsync();
            }

            var model = new PromotionsViewModel
            {
                CurrentRole = currentRole,
                CurrentUserEmail = currentUserEmail,
                Promotions = promotions.Select(x => new PromotionViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Code = x.Code,
                    Description = x.Description,
                    DiscountPercent = x.DiscountPercent,
                    ApplicableService = x.ApplicableService,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    TermsAndConditions = x.TermsAndConditions,
                    IsActive = x.IsActive
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promotions(PromotionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await ReloadPromotionsAsync(model);
                return View(model);
            }

            if (model.Promotion.EndDate < model.Promotion.StartDate)
            {
                ModelState.AddModelError("Promotion.EndDate", "End date must be greater than or equal to start date.");
                await ReloadPromotionsAsync(model);
                return View(model);
            }

            var promotion = new Promotion
            {
                Id = model.Promotion.Id ?? string.Empty,
                Title = model.Promotion.Title,
                Code = model.Promotion.Code,
                Description = model.Promotion.Description,
                DiscountPercent = model.Promotion.DiscountPercent,
                ApplicableService = model.Promotion.ApplicableService,
                StartDate = model.Promotion.StartDate,
                EndDate = model.Promotion.EndDate,
                TermsAndConditions = model.Promotion.TermsAndConditions,
                IsActive = model.Promotion.IsActive
            };

            if (string.IsNullOrWhiteSpace(model.Promotion.Id))
            {
                var result = await _promotionOperations.CreatePromotionAsync(promotion);

                TempData["SuccessMessage"] = result
                    ? "Promotion created successfully."
                    : "Cannot create promotion.";
            }
            else
            {
                var result = await _promotionOperations.UpdatePromotionAsync(promotion);

                TempData["SuccessMessage"] = result
                    ? "Promotion updated successfully."
                    : "Cannot update promotion.";
            }

            return RedirectToAction(nameof(Promotions));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                await _promotionOperations.DeletePromotionAsync(id);
            }

            TempData["SuccessMessage"] = "Promotion deleted successfully.";

            return RedirectToAction(nameof(Promotions));
        }

        private async Task ReloadPromotionsAsync(PromotionsViewModel model)
        {
            var currentUserEmail = User.Identity?.Name ?? string.Empty;
            var currentRole = GetCurrentRole();

            List<Promotion> promotions;

            if (currentRole == "Admin")
            {
                promotions = await _promotionOperations.GetAllPromotionsAsync();
            }
            else
            {
                promotions = await _promotionOperations.GetActivePromotionsAsync();
            }

            model.CurrentRole = currentRole;
            model.CurrentUserEmail = currentUserEmail;

            model.Promotions = promotions.Select(x => new PromotionViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Code = x.Code,
                Description = x.Description,
                DiscountPercent = x.DiscountPercent,
                ApplicableService = x.ApplicableService,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TermsAndConditions = x.TermsAndConditions,
                IsActive = x.IsActive
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