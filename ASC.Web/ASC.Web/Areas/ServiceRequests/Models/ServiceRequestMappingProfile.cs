using ASC.Model;
using AutoMapper;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class ServiceRequestMappingProfile : Profile
    {
        public ServiceRequestMappingProfile()
        {
            CreateMap<NewServiceRequestViewModel, ServiceRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedBy, opt => opt.MapFrom(src => src.CustomerEmail))
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.RequestedServices))
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.ServiceEngineer));

            CreateMap<ServiceRequest, NewServiceRequestViewModel>();
        }
    }
}