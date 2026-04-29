using ASC.Model;

namespace ASC.Business.Interfaces
{
    public interface IMasterDataOperations
    {
        Task<List<MasterDataKey>> GetAllMasterKeysAsync();

        Task<MasterDataKey?> GetMasterKeyByIdAsync(string id);

        Task<bool> InsertMasterKeyAsync(MasterDataKey masterDataKey);

        Task<bool> UpdateMasterKeyAsync(MasterDataKey masterDataKey);

        Task<List<MasterDataValue>> GetAllMasterValuesAsync();

        Task<List<MasterDataValue>> GetMasterValuesByKeyIdAsync(string masterDataKeyId);

        Task<MasterDataValue?> GetMasterValueByIdAsync(string id);

        Task<bool> InsertMasterValueAsync(MasterDataValue masterDataValue);

        Task<bool> UpdateMasterValueAsync(MasterDataValue masterDataValue);

        Task<bool> UploadMasterDataAsync(List<MasterDataValue> masterDataValues);
    }
}