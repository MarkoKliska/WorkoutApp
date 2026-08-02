using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Mail.Service.Options;

public sealed class BrevoSmtpOptions
{
    public const string SectionName = "Brevo";

    [Required]
    public string Host { get; init; } = "smtp-relay.brevo.com";

    [Range(1, 65535)]
    public int Port { get; init; } = 587;

    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string SenderEmail { get; init; } = string.Empty;

    public string SenderName { get; init; } = "WorkoutApp";
}