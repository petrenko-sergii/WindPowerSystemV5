using AutoMapper;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.Repositories.Interfaces;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Services;

public class TurbineService : ITurbineService
{
    private readonly IMapper _mapper;
    private readonly ITurbineRepository _turbineRepository;

    public TurbineService(IMapper mapper, ITurbineRepository turbineRepository)
    {
        _mapper = mapper;
        _turbineRepository = turbineRepository;
    }

    public async Task<List<TurbineDTO>> Get()
    {
        var turbines = await _turbineRepository.Get();

        var turbineDtos = turbines.Select(t => new TurbineDTO
        {
            Id = t.Id,
            SerialNumber = t.SerialNumber,
            Status = t.Status.ToString(),
            TurbineTypeId = t.TurbineTypeId,
            Manufacturer = t.TurbineType!.Manufacturer,
            Model = t.TurbineType!.Model
        }).ToList();

        return turbineDtos;
    }

    public async Task<TurbineDTO> Get(int id)
    {
        var turbine = await _turbineRepository.Get(id);

        if (turbine == null)
        {
            throw new NotFoundException($"Turbine with ID {id} is not found.");
        }

        return _mapper.Map<TurbineDTO>(turbine);
    }

    public async Task<int> Create(TurbineDTO turbineDto)
    {
        var turbine = _mapper.Map<Turbine>(turbineDto);

        return await _turbineRepository.Create(turbine);
    }

    public async Task Update(TurbineDTO turbineDto)
    {
        var turbineDB = await _turbineRepository.Get(turbineDto.Id);

        if (turbineDB == null)
        {
            throw new NotFoundException($"Turbine with ID {turbineDto.Id} is not found.");
        }

        turbineDB = _mapper.Map<Turbine>(turbineDto);

        await _turbineRepository.Update(turbineDB);
    }
}
