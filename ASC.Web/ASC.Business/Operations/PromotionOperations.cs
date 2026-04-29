using ASC.Business.Interfaces;
using ASC.DataAccess.Repository;
using ASC.Model;

namespace ASC.Business.Operations
{
    public class PromotionOperations : IPromotionOperations
    {
        private readonly IUnitOfWork _unitOfWork;

        public PromotionOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<Promotion>> GetAllPromotionsAsync()
        {
            var promotions = _unitOfWork.PromotionRepository
                .GetAll()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            return Task.FromResult(promotions);
        }

        public Task<List<Promotion>> GetActivePromotionsAsync()
        {
            var now = DateTime.Now;

            var promotions = _unitOfWork.PromotionRepository
                .GetAll()
                .Where(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.StartDate <= now &&
                    x.EndDate >= now)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            return Task.FromResult(promotions);
        }

        public async Task<Promotion?> GetPromotionByIdAsync(string id)
        {
            return await _unitOfWork.PromotionRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreatePromotionAsync(Promotion promotion)
        {
            promotion.Id = Guid.NewGuid().ToString();
            promotion.CreatedDate = DateTime.Now;
            promotion.IsDeleted = false;

            await _unitOfWork.PromotionRepository.AddAsync(promotion);

            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> UpdatePromotionAsync(Promotion promotion)
        {
            var existingPromotion = await _unitOfWork.PromotionRepository
                .GetByIdAsync(promotion.Id);

            if (existingPromotion == null)
            {
                return false;
            }

            existingPromotion.Title = promotion.Title;
            existingPromotion.Code = promotion.Code;
            existingPromotion.Description = promotion.Description;
            existingPromotion.DiscountPercent = promotion.DiscountPercent;
            existingPromotion.ApplicableService = promotion.ApplicableService;
            existingPromotion.StartDate = promotion.StartDate;
            existingPromotion.EndDate = promotion.EndDate;
            existingPromotion.TermsAndConditions = promotion.TermsAndConditions;
            existingPromotion.IsActive = promotion.IsActive;
            existingPromotion.ModifiedDate = DateTime.Now;

            _unitOfWork.PromotionRepository.Update(existingPromotion);

            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> DeletePromotionAsync(string id)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);

            if (promotion == null)
            {
                return false;
            }

            promotion.IsDeleted = true;
            promotion.ModifiedDate = DateTime.Now;

            _unitOfWork.PromotionRepository.Update(promotion);

            return await _unitOfWork.SaveAsync() > 0;
        }
    }
}