using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace IkeaFunctions;

public static class IkeaRelease
{
    // private readonly IConfiguration _configuration;
    //
    // public IkeaRelease(IConfiguration configuration)
    // {
    //     _configuration = configuration;
    // }

    [FunctionName("IkeaRelease")]
    public static async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
        log.LogInformation("This is a key {key}", Environment.GetEnvironmentVariable("MY_VARIABLE_KEY"));
        try
        {
            var client = new HttpClient {BaseAddress = new Uri(IkeaVerificationService.PageAddress)};
            var response = await client.GetStringAsync("");
        
            await IkeaVerificationService.Run(response, log);
        }
        catch (HttpRequestException ex)
        {
            log.LogError(
                "The request failed due to an underlying issue such as network connectivity, " +
                "DNS failure, server certificate validation (or timeout for .NET Framework only).");
        }
        catch (TaskCanceledException ex)
        {
            log.LogError("The request failed due to timeout.");
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Unexpected error during function execution.");
        }
    }
}