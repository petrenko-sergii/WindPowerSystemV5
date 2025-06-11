using AutoMapper;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.NoSqlModels;

namespace WindPowerSystemV5.Server.Mappings;

public class MaintenanceRecordProfile : Profile
{
    public MaintenanceRecordProfile()
    {
        CreateMap<MaintenanceRecordDTO, MaintenanceRecord>();
    }
}
