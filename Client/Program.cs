
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;
using Client.ServicesClient;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IMoviesData, MoviesData>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services
    .AddScoped<IAccountService, AccountService>()
    .AddScoped<IAlertService, AlertService>()
    .AddScoped<IHttpService, HttpService>()
    .AddScoped<ILocalStorageService, LocalStorageService>();

builder.Services.AddAuthorizationCore();


await builder.Build().RunAsync();