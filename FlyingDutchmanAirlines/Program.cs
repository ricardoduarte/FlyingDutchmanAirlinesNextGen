using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FlyingDutchmanAirlines;

class Program
{
    static void Main(string[] args)
    {
        InitializeHost();
    }

    private static void InitializeHost()
    {
        Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder => {
            builder.UseStartup<Startup>();
            builder.UseUrls("http://localhost:8080");
        }).Build().Run();
    }
}
