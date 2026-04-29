using ASC.Business.Interfaces;
using ASC.DataAccess.Repository;
using ASC.Model;

namespace ASC.Business.Operations
{
    public class MasterDataOperations : IMasterDataOperations
    {
        private readonly IUnitOfWork _unitOfWork;

        public MasterDataOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<MasterDataKey>> GetAllMasterKeysAsync()
        {
            var masterKeys = _unitOfWork.MasterDataKeyRepository
                .GetAll()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ToList();

            return Task.FromResult(masterKeys);
        }

        public async Task<MasterDataKey?> GetMasterKeyByIdAsync(string id)
        {
            return await _unitOfWork.MasterDataKeyRepository.GetByIdAsync(id);
        }

        public async Task<bool> InsertMasterKeyAsync(MasterDataKey masterDataKey)
        {
            masterDataKey.Id = Guid.NewGuid().ToString();
            masterDataKey.CreatedDate = DateTime.Now;
            masterDataKey.IsDeleted = false;

            await _unitOfWork.MasterDataKeyRepository.AddAsync(masterDataKey);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> UpdateMasterKeyAsync(MasterDataKey masterDataKey)
        {
            var existingKey = await _unitOfWork.MasterDataKeyRepository.GetByIdAsync(masterDataKey.Id);

            if (existingKey == null)
            {
                return false;
            }

            existingKey.Name = masterDataKey.Name;
            existingKey.IsActive = masterDataKey.IsActive;
            existingKey.ModifiedDate = DateTime.Now;

            _unitOfWork.MasterDataKeyRepository.Update(existingKey);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public Task<List<MasterDataValue>> GetAllMasterValuesAsync()
        {
            var masterValues = _unitOfWork.MasterDataValueRepository
                .GetAll()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Value)
                .ToList();

            return Task.FromResult(masterValues);
        }

        public Task<List<MasterDataValue>> GetMasterValuesByKeyIdAsync(string masterDataKeyId)
        {
            var masterValues = _unitOfWork.MasterDataValueRepository
                .Find(x => !x.IsDeleted && x.MasterDataKeyId == masterDataKeyId)
                .OrderBy(x => x.Value)
                .ToList();

            return Task.FromResult(masterValues);
        }

        public async Task<MasterDataValue?> GetMasterValueByIdAsync(string id)
        {
            return await _unitOfWork.MasterDataValueRepository.GetByIdAsync(id);
        }

        public async Task<bool> InsertMasterValueAsync(MasterDataValue masterDataValue)
        {
            masterDataValue.Id = Guid.NewGuid().ToString();
            masterDataValue.CreatedDate = DateTime.Now;
            masterDataValue.IsDeleted = false;

            await _unitOfWork.MasterDataValueRepository.AddAsync(masterDataValue);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> UpdateMasterValueAsync(MasterDataValue masterDataValue)
        {
            var existingValue = await _unitOfWork.MasterDataValueRepository.GetByIdAsync(masterDataValue.Id);

            if (existingValue == null)
            {
                return false;
            }

            existingValue.MasterDataKeyId = masterDataValue.MasterDataKeyId;
            existingValue.Value = masterDataValue.Value;
            existingValue.IsActive = masterDataValue.IsActive;
            existingValue.ModifiedDate = DateTime.Now;

            _unitOfWork.MasterDataValueRepository.Update(existingValue);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> UploadMasterDataAsync(List<MasterDataValue> masterDataValues)
        {
            if (masterDataValues == null || !masterDataValues.Any())
            {
                return false;
            }

            foreach (var value in masterDataValues)
            {
                value.Id = Guid.NewGuid().ToString();
                value.CreatedDate = DateTime.Now;
                value.IsDeleted = false;

                await _unitOfWork.MasterDataValueRepository.AddAsync(value);
            }

            return await _unitOfWork.SaveAsync() > 0;
        }
    }
}