using EarWorm;
using EarWorm.Code;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Havit.Blazor.Components.Web;         // <------ ADD THIS LINE
using Havit.Blazor.Components.Web.Bootstrap;  // <------ ADD THIS LINE

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddHxServices();
//var settings = new Settings();
//builder.Services.AddSingleton<Settings>();
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
