using AutoMapper;
using WindPowerSystemV5.Server.Data.CosmosDbModels;
using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Mappings;

public class MaintenanceRecordProfile : Profile
{
    public MaintenanceRecordProfile()
    {
        CreateMap<MaintenanceRecordDTO, MaintenanceRecord>();
    }
}
