using ASC.Business.Helpers;
using ASC.Business.Interfaces;
using ASC.DataAccess.Repository;
using ASC.Model;

namespace ASC.Business.Operations
{
    public class ServiceRequestOperations : IServiceRequestOperations
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceRequestOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<ServiceRequest>> GetAllServiceRequestsAsync()
        {
            var serviceRequests = _unitOfWork.ServiceRequestRepository
                .GetAll()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.RequestedDate)
                .ToList();

            return Task.FromResult(serviceRequests);
        }

        public Task<List<ServiceRequest>> GetServiceRequestsByRoleAsync(string role, string userEmail)
        {
            var predicate = Queries.GetServiceRequestQuery(role, userEmail);

            var serviceRequests = _unitOfWork.ServiceRequestRepository
                .GetAll()
                .AsQueryable()
                .Where(predicate)
                .OrderByDescending(x => x.RequestedDate)
                .ToList();

            return Task.FromResult(serviceRequests);
        }

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(string id)
        {
            return await _unitOfWork.ServiceRequestRepository.GetByIdAsync(id);
        }

        public async Task<bool> InsertServiceRequestAsync(ServiceRequest serviceRequest)
        {
            serviceRequest.Id = Guid.NewGuid().ToString();
            serviceRequest.CreatedDate = DateTime.Now;
            serviceRequest.IsDeleted = false;

            if (string.IsNullOrWhiteSpace(serviceRequest.Status))
            {
                serviceRequest.Status = "New";
            }

            await _unitOfWork.ServiceRequestRepository.AddAsync(serviceRequest);

            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<bool> UpdateServiceRequestAsync(ServiceRequest serviceRequest)
        {
            var existingRequest = await _unitOfWork.ServiceRequestRepository.GetByIdAsync(serviceRequest.Id);

            if (existingRequest == null)
            {
                return false;
            }

            existingRequest.CustomerEmail = serviceRequest.CustomerEmail;
            existingRequest.VehicleName = serviceRequest.VehicleName;
            existingRequest.VehicleType = serviceRequest.VehicleType;
            existingRequest.RequestedServices = serviceRequest.RequestedServices;
            existingRequest.RequestedDate = serviceRequest.RequestedDate;
            existingRequest.Description = serviceRequest.Description;
            existingRequest.ServiceEngineer = serviceRequest.ServiceEngineer;
            existingRequest.Status = serviceRequest.Status;
            existingRequest.ModifiedDate = DateTime.Now;

            existingRequest.RequestedBy = serviceRequest.CustomerEmail;
            existingRequest.ServiceType = serviceRequest.RequestedServices;
            existingRequest.AssignedTo = serviceRequest.ServiceEngineer;

            _unitOfWork.ServiceRequestRepository.Update(existingRequest);

            return await _unitOfWork.SaveAsync() > 0;
        }
    }
}