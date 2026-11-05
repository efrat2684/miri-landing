using MiriLanding.Api.Models;

namespace MiriLanding.Api.Services;

public interface IContactEmailSender
{
    Task SendAsync(ContactRequest request, CancellationToken cancellationToken);
}
