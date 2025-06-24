using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.Enums;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Utils.Exceptions;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Data.GraphQL;

public class TurbineMutation
{
    /// <summary>
    /// Update an existing Turbine
    /// </summary>
    [Serial]
    [Authorize(Roles = ["RegisteredUser"])]
    public async Task<TurbineDTO> UpdateTurbine(
        [Service] ApplicationDbContext context, TurbineUpdateRequest turbine)
    {
        var turbineToUpdate = await context.Turbines
            .Where(t => t.Id == turbine.Id)
            .FirstOrDefaultAsync();

        if (turbineToUpdate == null)
        {
            throw new NotFoundException($"Turbine with ID {turbine.Id} is not found.");
        }

        if (!Enum.TryParse<TurbineStatus>(turbine.Status, true, out var status))
        {
            throw new BadRequestException($"Invalid TurbineStatus value: {turbine.Status}");
        }

        turbineToUpdate.SerialNumber = turbine.SerialNumber;
        turbineToUpdate.Status = status;
        turbineToUpdate.TurbineTypeId = turbine.TurbineTypeId;

        context.Turbines.Update(turbineToUpdate);
        await context.SaveChangesAsync();

        return new TurbineDTO
        {
            Id = turbineToUpdate.Id,
            SerialNumber = turbineToUpdate.SerialNumber,
            Status = turbineToUpdate.Status.ToString(),
            TurbineTypeId = turbineToUpdate.TurbineTypeId,
        };
    }

    /// <summary>
    /// Add a new Turbine
    /// </summary>
    [Serial]
    [Authorize(Roles = ["RegisteredUser"])]
    public async Task<TurbineDTO> AddTurbine(
        [Service] ApplicationDbContext context, TurbineCreationRequest turbine)
    {
        if (!Enum.TryParse<TurbineStatus>(turbine.Status, true, out var status))
        {
            throw new BadRequestException($"Invalid TurbineStatus value: {turbine.Status}");
        }

        var newTurbine = new Turbine
        {
            SerialNumber = turbine.SerialNumber,
            Status = status,
            TurbineTypeId = turbine.TurbineTypeId
        };

        context.Turbines.Add(newTurbine);
        await context.SaveChangesAsync();

        return new TurbineDTO
        {
            Id = newTurbine.Id,
            SerialNumber = newTurbine.SerialNumber,
            Status = newTurbine.Status.ToString(),
            TurbineTypeId = newTurbine.TurbineTypeId
        };
    }
}