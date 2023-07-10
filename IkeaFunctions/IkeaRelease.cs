using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IkeaFunctions;

public class IkeaRelease
{
    private readonly ILogger _logger;

    public IkeaRelease(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<IkeaRelease>();
    }

    [Function("IkeaRelease")]
    public async Task RunAsync([TimerTrigger("0 */5 * * * *")] MyInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        _logger.LogInformation("This is a key {key}", Environment.GetEnvironmentVariable("MY_VARIABLE_KEY"));
        try
        {
            var client = new HttpClient {BaseAddress = new Uri(IkeaVerificationService.PageAddress)};
            var response = await client.GetStringAsync("");

            await IkeaVerificationService.Run(response, _logger);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                "The request failed due to an underlying issue such as network connectivity, " +
                "DNS failure, server certificate validation (or timeout for .NET Framework only).");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError("The request failed due to timeout.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during function execution.");
        }
    }
}

public class MyInfo
{
    public MyScheduleStatus ScheduleStatus { get; set; }

    public bool IsPastDue { get; set; }
}

public class MyScheduleStatus
{
    public DateTime Last { get; set; }

    public DateTime Next { get; set; }

    public DateTime LastUpdated { get; set; }
}