using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace HACC.Models.Canvas.Test.ClientSide;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args: args);
        builder.RootComponents.Add<App>(selector: "app");
        await builder.Build().RunAsync();
    }
}