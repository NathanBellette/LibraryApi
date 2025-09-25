using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibraryApi.Tests.TestUtilities;

public abstract class TestBase : IDisposable
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IHost TestHost;

    protected TestBase()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices);
        TestHost = builder.Build();
        ServiceProvider = TestHost.Services;
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Override in derived classes to configure test-specific services
    }

    public void Dispose()
    {
        TestHost?.Dispose();
        GC.SuppressFinalize(this);
    }
}
