using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["apiUrl"]) });
builder.Services.AddSingleton<IMoviesData, MoviesData>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IDbAccess, DbAccess>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();