using Azure.Storage.Blobs;
using Data.Contexts;
using FileProvider_G.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddDbContext<DataContext>(x => x.UseSqlServer(Environment.GetEnvironmentVariable("SQLSERVER")));
        services.AddScoped<BlobServiceClient>(x => new BlobServiceClient(Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT")));
        services.AddScoped<FileService>();

    })
    .Build();

host.Run();
