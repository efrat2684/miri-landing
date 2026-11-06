using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MiriLanding.Api.Models;
using MiriLanding.Api.Services;

namespace MiriLanding.Api.Controllers;

[ApiController]
[EnableCors("Frontend")]
[Route("api/[controller]")]
public sealed class ContactController : ControllerBase
{
    private readonly IContactEmailSender _emailSender;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IContactEmailSender emailSender, ILogger<ContactController> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    [HttpOptions]
    public IActionResult Preflight() => NoContent();

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ContactRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _emailSender.SendAsync(request, cancellationToken);
            return Ok(new { message = "הפנייה נשלחה בהצלחה." });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Email is not configured.");
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new { message = "שליחת המייל לא הוגדרה בשרת. יש להגדיר פרטי SMTP." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact email: {Reason}", ex.Message);

            var message = ex.Message.Contains("BadCredentials", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("Username and Password not accepted", StringComparison.OrdinalIgnoreCase)
                ? "Gmail דחה את סיסמת האפליקציה. צרו App Password חדשה ב-Google, עדכנו את משתני הסביבה ב-Render והפעילו מחדש."
                : IsLikelySmtpTimeoutOrBlock(ex)
                    ? "חיבור ל-Gmail SMTP נכשל או נחסם מהענן (יציאה לפורט 587). בדקו שסיסמת האפליקציה נכונה; אם עדיין נכשל — שקלו שירות מייל עם HTTPS (SendGrid/Mailgun) או שרת עם יציאה פתוחה."
                    : "שליחת המייל נכשלה. בדקו את הגדרות Gmail ב-Render והפעילו מחדש את השירות.";

            return StatusCode(StatusCodes.Status502BadGateway, new { message });
        }
    }

    private static bool IsLikelySmtpTimeoutOrBlock(Exception ex)
    {
        var msg = ex.ToString();
        return msg.Contains("timed out", StringComparison.OrdinalIgnoreCase)
            || msg.Contains("Timeout", StringComparison.OrdinalIgnoreCase)
            || msg.Contains("reset", StringComparison.OrdinalIgnoreCase)
            || msg.Contains("Could not connect", StringComparison.OrdinalIgnoreCase);
    }
}
