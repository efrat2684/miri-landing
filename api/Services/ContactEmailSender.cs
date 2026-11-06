using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MiriLanding.Api.Models;
using MiriLanding.Api.Options;

namespace MiriLanding.Api.Services;

public sealed class ContactEmailSender : IContactEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<ContactEmailSender> _logger;

    public ContactEmailSender(IOptions<EmailOptions> options, ILogger<ContactEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(ContactRequest request, CancellationToken cancellationToken)
    {
        var smtpUser = _options.Smtp.User.Trim();
        var smtpPassword = _options.Smtp.Password.Replace(" ", string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPassword))
        {
            throw new InvalidOperationException("Email SMTP credentials are not configured.");
        }

        var fromAddress = string.IsNullOrWhiteSpace(_options.FromAddress)
            ? smtpUser
            : _options.FromAddress.Trim();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, fromAddress));
        message.To.Add(MailboxAddress.Parse(_options.Recipient));
        message.ReplyTo.Add(MailboxAddress.Parse(request.Email));
        message.Subject = $"פנייה חדשה מהאתר — {request.Name}";

        var serviceLabel = request.Service switch
        {
            "images" => "תמונות",
            "videos" => "סרטונים",
            "songs" => "שירים",
            "more" => "ועוד",
            _ => request.Service,
        };

        var phone = string.IsNullOrWhiteSpace(request.Phone) ? "לא צוין" : request.Phone.Trim();

        message.Body = new TextPart("plain")
        {
            Text =
                $"פנייה חדשה מדף הנחיתה של מירי קולר{Environment.NewLine}{Environment.NewLine}" +
                $"שם: {request.Name.Trim()}{Environment.NewLine}" +
                $"אימייל: {request.Email.Trim()}{Environment.NewLine}" +
                $"טלפון: {phone}{Environment.NewLine}" +
                $"סוג העבודה: {serviceLabel}{Environment.NewLine}{Environment.NewLine}" +
                $"הודעה:{Environment.NewLine}{request.Message.Trim()}",
        };

        using var client = new SmtpClient();
        var timeoutMs = Math.Clamp(_options.Smtp.TimeoutMilliseconds, 5_000, 60_000);
        client.Timeout = timeoutMs;

        await client.ConnectAsync(_options.Smtp.Host, _options.Smtp.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(smtpUser, smtpPassword, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        _logger.LogInformation("Contact email sent for {Name} to {Recipient}", request.Name, _options.Recipient);
    }
}
