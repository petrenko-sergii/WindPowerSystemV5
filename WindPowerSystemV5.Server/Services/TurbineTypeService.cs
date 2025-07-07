using AutoMapper;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.Repositories.Interfaces;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Services;

public class TurbineTypeService : ITurbineTypeService
{
    private readonly IMapper _mapper;
    private readonly ITurbineTypeRepository _turbineTypeRepository;
    private readonly IBlobStorageService _blobStorageService;

    public TurbineTypeService(
        IMapper mapper, 
        ITurbineTypeRepository turbineTypeRepository,
        IBlobStorageService blobStorageService)
    {
        _mapper = mapper;
        _turbineTypeRepository = turbineTypeRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<int> Create(
        TurbineTypeCreationRequest turbineTypeToCreate,
        IFormFile infoFile)
    {
        if(infoFile is null || infoFile.Length == 0)
        {
            throw new BadRequestException("File is missing. Upload a file.");
        }

        var turbineType = _mapper.Map<TurbineType>(turbineTypeToCreate);
        var fileName = await _blobStorageService.UploadFileAsync(infoFile);

        turbineType.FileName = fileName;

        return await _turbineTypeRepository.Create(turbineType);
    }

    public async Task<string> NameInfoFile(string fileName)
    {
        var turbineType = await _turbineTypeRepository.GetByFileName(fileName);

        if (turbineType is null)
        {
            throw new NotFoundException($"Turbine type with file name \"{fileName}\" not found.");
        }

        var extension = System.IO.Path.GetExtension(fileName);

        return $"{turbineType.Manufacturer}_{turbineType.Model}{extension}"
            .Replace(" ","_");
    }
}
