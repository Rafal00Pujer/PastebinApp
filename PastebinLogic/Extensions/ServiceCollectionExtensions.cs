using Microsoft.Extensions.DependencyInjection;
using PastebinLogic.MapperProfiles;

namespace PastebinLogic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPastebinLogic(this IServiceCollection services)
    {
        services.AddScoped<IPasteService, PasteService>()
            .AddScoped<IPasteMetaService, PasteMetaService>()
            .AddScoped<IPastePasswordService, PastePasswordService>()
            .AddScoped<IAddPasteService, AddPasteService>();

        services.AddSingleton<IPasswordService, PasswordService>();

        services.AddAutoMapper(typeof(PasteMapperProfile).Assembly);

        return services;
    }
}