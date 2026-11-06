using MiriLanding.Api.Options;
using MiriLanding.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));
builder.Services.AddControllers();
builder.Services.AddScoped<IContactEmailSender, ContactEmailSender>();
builder.Services.AddCors(options =>
{
  options.AddPolicy("Frontend", policy =>
      policy
          .SetIsOriginAllowed(origin =>
          {
            if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            {
              return uri.Host is "localhost" or "127.0.0.1" || uri.Host.EndsWith("netlify.app");
            }
            return false;
          })
          .AllowAnyHeader()
          .AllowAnyMethod());
});
var app = builder.Build();

var emailConfigured = !string.IsNullOrWhiteSpace(app.Configuration["Email:Smtp:User"])
    && !string.IsNullOrWhiteSpace(app.Configuration["Email:Smtp:Password"]);
if (emailConfigured)
{
    app.Logger.LogInformation("Email SMTP is configured for {Recipient}", app.Configuration["Email:Recipient"]);
}
else
{
    app.Logger.LogWarning("Email SMTP is not configured. Set User Secrets or environment variables, then restart the API.");
}

app.UseCors("Frontend");
app.MapControllers();

app.Run();
