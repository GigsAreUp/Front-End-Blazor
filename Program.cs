using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MusicHFE2.Services;

namespace MusicHFE2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            //builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddMudServices();
            builder.Services.AddScoped<StateService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<JwtAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("https://6c2cb30ece9d.ngrok-free.app/")
    };
    client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
    return client;
});
            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IRazorPayService, RazorPayService>();
            await builder.Build().RunAsync();
        }
    }
}
