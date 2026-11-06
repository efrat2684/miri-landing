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
            .WithOrigins(
                "https://fabulous-yeot-f30b25.netlify.app",
                "http://localhost:4200",
                "http://127.0.0.1:4200")
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

app.UseRouting();
app.UseCors("Frontend");
app.MapControllers();

app.Run();
