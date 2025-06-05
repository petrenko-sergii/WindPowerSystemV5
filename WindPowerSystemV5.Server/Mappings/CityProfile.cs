using AutoMapper;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Mappings;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<City, CityDTO>();
        CreateMap<Country, CountryDTO>();
    }
}
