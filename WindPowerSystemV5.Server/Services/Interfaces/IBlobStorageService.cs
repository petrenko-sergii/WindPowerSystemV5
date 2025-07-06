using Microsoft.AspNetCore.Mvc;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface IBlobStorageService
{
    Task<FileContentResult> DownloadFileAsync(string uri);

    Task<string> UploadFileAsync(IFormFile file);
}