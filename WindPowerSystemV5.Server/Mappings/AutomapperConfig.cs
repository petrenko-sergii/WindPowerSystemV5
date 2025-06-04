using AutoMapper;
using System.Reflection;

namespace WindPowerSystemV5.Server.Mappings;

public static class AutomapperConfig
{
    public static IMapper CreateMapper()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps([assembly]);
        });

        return config.CreateMapper();
    }
}
