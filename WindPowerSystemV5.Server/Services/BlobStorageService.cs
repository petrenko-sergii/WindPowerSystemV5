using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WindPowerSystemV5.Server.Config;
using WindPowerSystemV5.Server.Services.Interfaces;
using Path = System.IO.Path;

namespace WindPowerSystemV5.Server.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobStorageOptions _blobStorageOptions;

    public BlobStorageService(IOptions<BlobStorageOptions> blobStorageOptions)
    {
        _blobStorageOptions = blobStorageOptions.Value;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        BlobContainerClient containerClient = new BlobContainerClient(
            _blobStorageOptions.ConnectionString,
            _blobStorageOptions.ContainerName);

        string extension = Path.GetExtension(file.FileName);

        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);

        var fileName = $"{Guid.NewGuid()}{extension}";

        var blobClient = containerClient.GetBlobClient(fileName);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, true);

        return fileName;
    }

    public async Task<FileContentResult> DownloadFileAsync(string fileName)
    {
        var url = GetFileUri(fileName);

        Uri blobUri = new Uri(url);
        StorageSharedKeyCredential credentials = new(
            _blobStorageOptions.StorageAccountName,
            _blobStorageOptions.StorageAccountKey);

        BlobClient blobClient = new(blobUri, credentials);

        var downloadResponse = await blobClient.DownloadStreamingAsync();

        using MemoryStream memoryStream = new();
        await downloadResponse.Value.Content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return new FileContentResult(
            memoryStream.ToArray(),
            downloadResponse.Value.Details.ContentType)
        {
            FileDownloadName = blobUri.Segments.Last()
        };
    }

    private string GetFileUri(string fileName)
    {
        var uri = new UriBuilder
        {
            Scheme = "https",
            Host = $"{_blobStorageOptions.StorageAccountName}.blob.core.windows.net",
            Path = $"{_blobStorageOptions.ContainerName}/{fileName}"
        };
        return uri.ToString();
    }
}
