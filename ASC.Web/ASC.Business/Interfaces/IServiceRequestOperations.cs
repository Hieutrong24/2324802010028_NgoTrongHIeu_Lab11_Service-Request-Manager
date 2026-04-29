using ASC.Model;

namespace ASC.Business.Interfaces
{
    public interface IServiceRequestOperations
    {
        Task<List<ServiceRequest>> GetAllServiceRequestsAsync();

        Task<List<ServiceRequest>> GetServiceRequestsByRoleAsync(string role, string userEmail);

        Task<ServiceRequest?> GetServiceRequestByIdAsync(string id);

        Task<bool> InsertServiceRequestAsync(ServiceRequest serviceRequest);

        Task<bool> UpdateServiceRequestAsync(ServiceRequest serviceRequest);
    }
}