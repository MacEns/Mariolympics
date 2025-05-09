using Mariolympics.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddRazorComponents()
    .AddInteractiveServerComponents();



if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: true);
}
else
{
    builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile(builder.Environment.ContentRootPath + "\\..\\appsettings.json", optional: true);
}

services.AddOptions();

services.AddDbContextFactory<MariolympicsContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("MariolympicsContext"));
});

services.AddTransient<TournamentRepository, TournamentRepository>();
services.AddTransient<PersonRepository, PersonRepository>();

services.AddBlazorBootstrap();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
