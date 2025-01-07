using MachineTracking.Client;
using MachineTracking.Client.Shared.Helpers;
using MachineTracking.Client.Shared.Helpers.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Net.Http.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

using var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var settings = await httpClient.GetFromJsonAsync<AppSettings>("appsettings.json");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(settings.ApiBaseUrl) });

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddSingleton<SignalRService>();
builder.Services.AddScoped<IHttpClientProvider, HttpClientProvider>();

await builder.Build().RunAsync();
