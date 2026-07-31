using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkoutApp.Domain.Repositories;
using WorkoutApp.Infrastructure.Options;
using WorkoutApp.Infrastructure.Persistence.Contexts;
using WorkoutApp.Infrastructure.Persistence.Repositories;
using WorkoutApp.Infrastructure.Persistence.UnitOfWork;
using WorkoutApp.Infrastructure.Options;

namespace WorkoutApp.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WorkoutAppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("WorkoutAppDatabase"))
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}