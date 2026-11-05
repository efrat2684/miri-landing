namespace MiriLanding.Api.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string Recipient { get; init; } = "hanicohen9889@gmail.com";

    public string FromAddress { get; init; } = string.Empty;

    public string FromName { get; init; } = "מירי קולר - אתר";

    public SmtpOptions Smtp { get; init; } = new();
}

public sealed class SmtpOptions
{
    public string Host { get; init; } = "smtp.gmail.com";

    public int Port { get; init; } = 587;

    public string User { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
