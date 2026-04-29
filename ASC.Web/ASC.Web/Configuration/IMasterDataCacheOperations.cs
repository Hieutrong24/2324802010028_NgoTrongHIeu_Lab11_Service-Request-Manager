namespace ASC.Web.Configuration
{
    public interface IMasterDataCacheOperations
    {
        Task CreateMasterDataCacheAsync();

        Task<MasterDataCache?> GetMasterDataCacheAsync();

        Task RefreshMasterDataCacheAsync();
    }
}