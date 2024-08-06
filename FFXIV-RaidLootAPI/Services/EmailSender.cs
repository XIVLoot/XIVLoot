using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FFXIV_RaidLootAPI.Services;

public class AuthMessageSenderOptions
{
    public string? SendGridKey { get; set; }
    public string? SmtpServer { get; set; } // Added
    public int SmtpPort { get; set; } // Added
    public string? SmtpUsername { get; set; } // Added
    public string? SmtpPassword { get; set; } // Added
}

public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                       ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {   
        Console.WriteLine("Sending email");
        if (string.IsNullOrEmpty(Options.SendGridKey))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(Options.SendGridKey, subject, message, toEmail);
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SmtpClient(Options.SmtpServer)
        {
            Port = Options.SmtpPort,
            Credentials = new NetworkCredential(Options.SmtpUsername, Options.SmtpPassword),
            EnableSsl = true,
        };
        var mailMessage = new MailMessage
        {
            From = new MailAddress("antoto001@gmail.com", "Test"),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}