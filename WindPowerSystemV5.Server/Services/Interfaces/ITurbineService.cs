using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface ITurbineService
{
    Task<List<TurbineDTO>> Get();

    Task<TurbineDTO> Get(int id);

    Task<int> Create(TurbineDTO turbineDto);

    Task Update(TurbineDTO turbineDto);
}
