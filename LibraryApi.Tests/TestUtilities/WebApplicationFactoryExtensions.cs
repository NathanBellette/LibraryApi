using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibraryApi.Tests.TestUtilities;

public static class WebApplicationFactoryExtensions
{
    public static WebApplicationFactory<T> WithInMemoryDatabase<T>(this WebApplicationFactory<T> factory) where T : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Add any test-specific service overrides here
                // For example, if we had a database, we could replace it with an in-memory one
            });
        });
    }
}
