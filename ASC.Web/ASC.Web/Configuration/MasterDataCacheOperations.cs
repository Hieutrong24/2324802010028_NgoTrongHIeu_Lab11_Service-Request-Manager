using ASC.Business.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ASC.Web.Configuration
{
    public class MasterDataCacheOperations : IMasterDataCacheOperations
    {
        private const string MasterDataCacheKey = "MasterDataCache";

        private readonly IDistributedCache _distributedCache;
        private readonly IMasterDataOperations _masterDataOperations;

        public MasterDataCacheOperations(
            IDistributedCache distributedCache,
            IMasterDataOperations masterDataOperations)
        {
            _distributedCache = distributedCache;
            _masterDataOperations = masterDataOperations;
        }

        public async Task CreateMasterDataCacheAsync()
        {
            var existingCache = await GetMasterDataCacheAsync();

            if (existingCache != null &&
                existingCache.MasterDataKeys.Any() &&
                existingCache.MasterDataValues.Any())
            {
                return;
            }

            await RefreshMasterDataCacheAsync();
        }

        public async Task<MasterDataCache?> GetMasterDataCacheAsync()
        {
            var cachedData = await _distributedCache.GetStringAsync(MasterDataCacheKey);

            if (string.IsNullOrWhiteSpace(cachedData))
            {
                return null;
            }

            return JsonSerializer.Deserialize<MasterDataCache>(cachedData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task RefreshMasterDataCacheAsync()
        {
            var masterKeys = await _masterDataOperations.GetAllMasterKeysAsync();
            var masterValues = await _masterDataOperations.GetAllMasterValuesAsync();

            var cacheModel = new MasterDataCache
            {
                MasterDataKeys = masterKeys,
                MasterDataValues = masterValues
            };

            var jsonData = JsonSerializer.Serialize(cacheModel);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
                SlidingExpiration = TimeSpan.FromHours(2)
            };

            await _distributedCache.SetStringAsync(
                MasterDataCacheKey,
                jsonData,
                cacheOptions);
        }
    }
}