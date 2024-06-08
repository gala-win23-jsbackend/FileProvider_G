using Data.Entity;
using FileProvider_G.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileProvider_G.Functions;

public class Upload(ILogger<Upload> logger, FileService fileService)
{
    private readonly ILogger<Upload> _logger = logger;
    private readonly FileService _fileService = fileService;


    [Function("Upload")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            if (req.Form.Files["file"] is IFormFile file)
            {
                var containerName = !string.IsNullOrEmpty(req.Query["containerName"]) ? req.Query["containerName"].ToString() : "uploads";

                var fileEntity = new FileEntity
                {
                    FileName = _fileService.SetFileName(file),
                    ContentType = file.ContentType,
                    ContainerName = containerName
                };

                await _fileService.SetBlobContainer(fileEntity.ContainerName);
                var filePath = await _fileService.UploadFileAsync(file, fileEntity);
                fileEntity.FilePath = filePath;

                await _fileService.SaveToDatabaseAsync(fileEntity);
                return new OkObjectResult(fileEntity.FilePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : Upload.Run :: {ex.Message}");
        }
        return new BadRequestResult();
    }
}
