using AutoMapper;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.Repositories.Interfaces;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Services;

public class CityService : ICityService
{
    private readonly IMapper _mapper;
    private readonly ICityRepository _cityRepository;

    public CityService(IMapper mapper, ICityRepository cityRepository)
    {
        _mapper = mapper;
        _cityRepository = cityRepository;
    }

    public async Task<List<CityDTO>> Get()
    {
        var cities = await _cityRepository.Get();
        return _mapper.Map<List<CityDTO>>(cities);
    }

    public async Task<List<CityDTO>> GetWithCountry()
    {
        var cities = await _cityRepository.GetWithCountry();
        return _mapper.Map<List<CityDTO>>(cities);
    }

    public async Task<CityDTO> GetCached(int id)
    {
        var city = await _cityRepository.GetCached(id);

        if (city == null)
        {
            throw new NotFoundException($"City with ID {id} is not found.");
        }

        return _mapper.Map<CityDTO>(city);
    }
}
