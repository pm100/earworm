using EarWorm.Code;
using EarWorm;
using Havit.Blazor.Components.Web;   
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddHxServices();
builder.Services.AddSingleton<SavedData>();
builder.Services.AddSingleton<MusicEngine>();
//builder.Services.AddSingleton<SetGenerator>();
//builder.Services.AddSingleton<Application>();


await builder.Build().RunAsync();
