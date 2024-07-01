using Klayman.Application.KeyboardLayoutSetManagement;

namespace Klayman.Service;

public class Worker(
    ILogger<Worker> logger,
    IKeyboardLayoutSetExporter layoutSetExporter) : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting service...");
        
        var importResult = layoutSetExporter.ImportLayoutSetCacheFromJson();
        if (importResult.IsFailed)
        {
            logger.LogError("Unable to import keyboard layout sets. {error}", importResult.ErrorMessage);
        }

        return Task.CompletedTask;
    }
    
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        var exportResult = layoutSetExporter.ExportLayoutSetCacheToJson();
        if (exportResult.IsFailed)
        {
            logger.LogError("Unable to export keyboard layout sets. {error}", exportResult.ErrorMessage);
        }

        return Task.CompletedTask;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Stopping service...");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}