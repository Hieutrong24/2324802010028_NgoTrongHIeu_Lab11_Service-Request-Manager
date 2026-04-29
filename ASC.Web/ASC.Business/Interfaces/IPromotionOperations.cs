using ASC.Model;

namespace ASC.Business.Interfaces
{
    public interface IPromotionOperations
    {
        Task<List<Promotion>> GetAllPromotionsAsync();

        Task<List<Promotion>> GetActivePromotionsAsync();

        Task<Promotion?> GetPromotionByIdAsync(string id);

        Task<bool> CreatePromotionAsync(Promotion promotion);

        Task<bool> UpdatePromotionAsync(Promotion promotion);

        Task<bool> DeletePromotionAsync(string id);
    }
}