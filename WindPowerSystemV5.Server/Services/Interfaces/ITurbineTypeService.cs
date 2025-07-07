using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface ITurbineTypeService
{
    Task<int> Create(TurbineTypeCreationRequest turbineTypeToCreate, IFormFile infoFile);

    Task<string> NameInfoFile(string fileName);
}
