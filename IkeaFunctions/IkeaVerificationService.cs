using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;

namespace IkeaFunctions;

public static class IkeaVerificationService
{
    internal const string PageAddress = @"https://www.ikea.com/co/es/";

    public static async Task Run(string response, ILogger log)
    {
        try
        {
            // var cs = configuration.GetConnectionString("EMAIL_SERVICE");
            var cs = Environment.GetEnvironmentVariable("EMAIL_SERVICE_CONNECTION_STRING");

            log.LogInformation("Retrieving EMAIL COMMUNICATION SERVICE connection string");

            // if (string.IsNullOrWhiteSpace(cs))
            // {
            //     log.LogError("EMAIL COMMUNICATION SERVICE connection string not found");
            //     return;
            // }

            // var sender = configuration.GetConnectionStringOrSetting("SENDER");
            // var recipient = configuration.GetConnectionStringOrSetting("RECIPIENT");
            var sender = Environment.GetEnvironmentVariable("SENDER");
            var recipient = Environment.GetEnvironmentVariable("RECIPIENT");

            log.LogInformation("SENDER: {sender}", sender);
            log.LogInformation("RECIPIENT: {recipient}", recipient);

            log.LogInformation("Sending email");


            var email = new EmailClient(cs);

            EmailSendOperation emailSendOperation;

            if (response.Contains("¡Nos vemos este año en Bogot!", StringComparison.OrdinalIgnoreCase))
            {
                log.LogInformation("Found string in webpage.");
                // send success email
                emailSendOperation = await email.SendAsync(
                    wait: Azure.WaitUntil.Completed,
                    senderAddress: sender,
                    recipientAddress: recipient,
                    subject: "IKEA Status Update - CHANGED",
                    htmlContent: null,
                    plainTextContent: $"""IKEA page changed. "Nos vemos este año" not found. Go here {PageAddress}""");
            }
            else
            {
                emailSendOperation = await email.SendAsync(
                    wait: Azure.WaitUntil.Completed,
                    senderAddress: sender,
                    recipientAddress: recipient,
                    subject: "IKEA Status Update - No Change",
                    htmlContent: null,
                    plainTextContent:
                    $"""No change detected on the web page of IKEA. You can check by yourself here {PageAddress}""");
            }


            var emailSendResult = emailSendOperation.Value;
            log.LogInformation($"Email send. Status = {emailSendResult.Status}");
        }
        catch (RequestFailedException ex)
        {
            log.LogError("Email send operation failed with error code: {code}, message:{message}",
                ex.ErrorCode, ex.Message);
        }
    }
}