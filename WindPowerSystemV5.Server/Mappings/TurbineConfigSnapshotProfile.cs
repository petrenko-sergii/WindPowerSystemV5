using AutoMapper;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.CosmosDbModels;

namespace WindPowerSystemV5.Server.Mappings;

public class TurbineConfigSnapshotProfile : Profile
{
    public TurbineConfigSnapshotProfile()
    {
        CreateMap<BladeConfig, BladeConfigDTO>();
        CreateMap<EnvironmentalLimits, EnvironmentalLimitsDTO>();
        CreateMap<SensorEntry, SensorEntryDTO>();
        CreateMap<OperatorInfo, OperatorInfoDTO>();

        CreateMap<TurbineConfigSnapshot, TurbineConfigSnapshotDTO>()
            .ForMember(dest => dest.BladeConfig, opt => opt.MapFrom(src => src.BladeConfig))
            .ForMember(dest => dest.EnvironmentalLimits, opt => opt.MapFrom(src => src.EnvironmentalLimits))
            .ForMember(dest => dest.SensorMap, opt => opt.MapFrom(src => src.SensorMap))
            .ForMember(dest => dest.OperatorInfo, opt => opt.MapFrom(src => src.OperatorInfo));

        CreateMap<BladeConfigDTO, BladeConfig>();
        CreateMap<EnvironmentalLimitsDTO, EnvironmentalLimits>();
        CreateMap<SensorEntryDTO, SensorEntry>();
        CreateMap<OperatorInfoDTO, OperatorInfo>();

        CreateMap<TurbineConfigSnapshotDTO, TurbineConfigSnapshot>()
            .ForMember(dest => dest.BladeConfig, opt => opt.MapFrom(src => src.BladeConfig))
            .ForMember(dest => dest.EnvironmentalLimits, opt => opt.MapFrom(src => src.EnvironmentalLimits))
            .ForMember(dest => dest.SensorMap, opt => opt.MapFrom(src => src.SensorMap))
            .ForMember(dest => dest.OperatorInfo, opt => opt.MapFrom(src => src.OperatorInfo));
    }
}
