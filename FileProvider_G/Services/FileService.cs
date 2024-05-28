
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Data.Contexts;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FileProvider_G.Services;

public class FileService(ILogger<FileService> logger, DataContext context, BlobServiceClient client)
{
    private readonly ILogger<FileService> _logger = logger;
    private readonly DataContext _context = context;
    private readonly BlobServiceClient _client = client;
    private BlobContainerClient? _container;

    public async Task SetBlobContainer(string containerName)
    {
        _container = _client.GetBlobContainerClient(containerName);
        await _container.CreateIfNotExistsAsync();
        _client.GetBlobContainerClient(containerName).SetAccessPolicy(PublicAccessType.BlobContainer);
    }

    public string SetFileName(IFormFile file)
    {
        var newFileName = $"{Guid.NewGuid()}_{file.FileName}";
        return newFileName;
    }

    public async Task<string> UploadFileAsync(IFormFile file, FileEntity fileEntity)
    {
        BlobHttpHeaders headers = new()
        {
            ContentType = file.ContentType,

        };
        var blobClient = _container!.GetBlobClient(fileEntity.FileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, headers);
        return blobClient.Uri.ToString();
    }

    public async Task SaveToDatabaseAsync(FileEntity fileEntity)
    {
        _context.Files.Add(fileEntity);
        await _context.SaveChangesAsync();
    }

}
