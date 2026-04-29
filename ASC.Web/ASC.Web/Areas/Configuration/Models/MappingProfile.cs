using ASC.Model;
using AutoMapper;

namespace ASC.Web.Areas.Configuration.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MasterDataKey, MasterDataKeyViewModel>().ReverseMap();

            CreateMap<MasterDataValue, MasterDataValueViewModel>().ReverseMap();
        }
    }
}