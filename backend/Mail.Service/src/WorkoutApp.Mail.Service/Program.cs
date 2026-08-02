using FluentValidation;
using MassTransit;
using WorkoutApp.Mail.Service.Consumers;
using WorkoutApp.Mail.Service.Options;
using WorkoutApp.Mail.Service.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<RabbitMqOptions>()
    .Bind(builder.Configuration.GetSection(RabbitMqOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<BrevoSmtpOptions>()
    .Bind(builder.Configuration.GetSection(BrevoSmtpOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddSingleton<IEmailSender, BrevoEmailSender>();

var rabbitMqOptions = builder.Configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>()
    ?? throw new InvalidOperationException("RabbitMq configuration section is missing.");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, h =>
        {
            h.Username(rabbitMqOptions.Username);
            h.Password(rabbitMqOptions.Password);
        });

        cfg.ReceiveEndpoint("user-registered-email", e =>
        {
            e.UseMessageRetry(r => r
                .Interval(3, TimeSpan.FromSeconds(10))
                .Ignore<ValidationException>());

            e.ConfigureConsumer<UserRegisteredConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();