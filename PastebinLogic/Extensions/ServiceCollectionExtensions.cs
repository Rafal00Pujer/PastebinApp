using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PastebinLogic.Services.Implementations;
using PastebinLogic.Services.Interfaces;

namespace PastebinLogic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPastebinLogic(this IServiceCollection services)
    {
        services.AddScoped<IPasteService, PasteService>()
            .AddScoped<IPasteMetaService, PasteMetaService>()
            .AddScoped<IPastePasswordService, PastePasswordService>();

        services.AddAutoMapper(Assembly.GetCallingAssembly());

        return services;
    }
}