using System.ComponentModel.DataAnnotations;

namespace MiriLanding.Api.Models;

public sealed class ContactRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(120)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; init; } = string.Empty;

    [MaxLength(40)]
    public string? Phone { get; init; }

    [Required]
    [RegularExpression(@"^(images|videos|songs|more)$")]
    public string Service { get; init; } = string.Empty;

    [Required]
    [MinLength(10)]
    [MaxLength(4000)]
    public string Message { get; init; } = string.Empty;
}
