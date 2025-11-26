using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SimpleWeb.IntegrationTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Override configuration for testing
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Storage:Type"] = "0", // Local storage
                    ["Storage:FileName"] = "test-file.jpg",
                    ["APPINSIGHTS_CONNECTIONSTRING"] = ""
                });
            });

            builder.ConfigureServices(services =>
            {
                // Additional test-specific service configuration can go here
            });

            builder.UseEnvironment("Development");
        }
    }
}
