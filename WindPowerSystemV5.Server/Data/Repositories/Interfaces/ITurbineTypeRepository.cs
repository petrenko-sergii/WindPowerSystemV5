using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Data.Repositories.Interfaces;

public interface ITurbineTypeRepository
{
    Task<TurbineType?> Get(int id);

    Task<int> Create(TurbineType turbineType);

    Task<TurbineType?> GetByFileName(string fileName);

    Task<List<TurbineTypeDTO>> Search(TurbineTypeSearchCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>Number of turbine types matching the criteria filters (ignores sorting and limit).</summary>
    Task<int> Count(TurbineTypeSearchCriteria criteria, CancellationToken cancellationToken = default);

    Task<List<ManufacturerSummaryDTO>> GetManufacturers(CancellationToken cancellationToken = default);
}