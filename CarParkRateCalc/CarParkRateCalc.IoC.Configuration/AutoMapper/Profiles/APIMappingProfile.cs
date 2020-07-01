using AutoMapper;
using DC = CarParkRateCalc.API.DataContracts;
using S = CarParkRateCalc.Services.Model;

namespace CarParkRateCalc.IoC.Configuration.AutoMapper.Profiles
{
    public class APIMappingProfile : Profile
    {
        public APIMappingProfile()
        {
            CreateMap<DC.Charge, S.Charge>().ReverseMap();
            
        }
    }
}
