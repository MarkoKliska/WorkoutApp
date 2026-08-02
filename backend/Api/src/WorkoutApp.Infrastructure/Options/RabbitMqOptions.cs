using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Infrastructure.Options;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    [Required]
    public string Host { get; init; } = string.Empty;

    public string VirtualHost { get; init; } = "/";

    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}