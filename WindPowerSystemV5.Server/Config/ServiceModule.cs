using WindPowerSystemV5.Server.Data.Repositories;
using WindPowerSystemV5.Server.Data.Repositories.Interfaces;
using WindPowerSystemV5.Server.Services;
using WindPowerSystemV5.Server.Services.Interfaces;

namespace WindPowerSystemV5.Server.Config;

public static class ServiceModule
{
    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ITurbineRepository, TurbineRepository>();
        services.AddScoped<ITurbineTypeRepository, TurbineTypeRepository>();
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IMaintenanceRecordService, MaintenanceRecordService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<ITurbineConfigSnapshotService, TurbineConfigSnapshotService>();
        services.AddScoped<ITurbineService, TurbineService>();
        services.AddScoped<ITurbineTypeService, TurbineTypeService>();
    }
}
