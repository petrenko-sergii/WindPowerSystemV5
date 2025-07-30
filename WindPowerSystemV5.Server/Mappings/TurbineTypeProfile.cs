using AutoMapper;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Mappings;

public class TurbineTypeProfile : Profile
{
    public TurbineTypeProfile()
    {
        CreateMap<TurbineTypeCreationRequest, TurbineType>();
        CreateMap<TurbineType, TurbineTypeDTO>();
    }
}
