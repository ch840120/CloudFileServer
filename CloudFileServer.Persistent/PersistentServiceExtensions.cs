using CloudFileServer.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CloudFileServer.Persistent;

public static class PersistentServiceExtensions
{
    public static IServiceCollection AddPersistentServices(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<INodeTreeRepository, NodeTreeRepository>();

        return services;
    }
}
