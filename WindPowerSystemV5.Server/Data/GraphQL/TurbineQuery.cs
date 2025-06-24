using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Data.GraphQL;

public class TurbineQuery
{
    /// <summary>
    /// Gets all Turbines
    /// </summary>
    [Serial]
    public IQueryable<TurbineDTO> GetTurbines(
        [Service] ApplicationDbContext context)
        => context.Turbines
            .Include(t => t.TurbineType)
            .Select(t => new TurbineDTO
            {
                Id = t.Id,
                SerialNumber = t.SerialNumber,
                Status = t.Status.ToString(),
                TurbineTypeId = t.TurbineTypeId,
                Manufacturer = t.TurbineType!.Manufacturer,
                Model = t.TurbineType!.Model
            });

    /// <summary>
    /// Gets a Turbine by id
    /// </summary>
    [Serial]
    public async Task<TurbineDTO?> GetTurbine(
        [Service] ApplicationDbContext context,
        int id)
    {
        var turbine =  await context.Turbines.FindAsync(id);

        return turbine == null ? null : new TurbineDTO
        {
            Id = turbine.Id,
            SerialNumber = turbine.SerialNumber,
            Status = turbine.Status.ToString(),
            TurbineTypeId = turbine.TurbineTypeId,
        };
    }
}