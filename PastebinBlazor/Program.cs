using Microsoft.EntityFrameworkCore;
using PastebinBlazor.Components;
using PastebinBlazor.MapperProfiles;
using PastebinDatabase.Context;
using PastebinLogic.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<PastebinContext>(options =>
{
    options.UseLazyLoadingProxies()
        .UseSqlServer(builder.Configuration.GetConnectionString("PastebinDatabase"));
});

builder.Services.AddPastebinLogic();

builder.Services.AddAutoMapper(typeof(PasteMapperProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    using var scope = app.Services.CreateScope();

    var mapperConf = scope.ServiceProvider.GetRequiredService<AutoMapper.IConfigurationProvider>();
    mapperConf.AssertConfigurationIsValid();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
