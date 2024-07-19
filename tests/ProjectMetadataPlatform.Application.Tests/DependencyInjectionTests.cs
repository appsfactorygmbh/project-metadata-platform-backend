using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace ProjectMetadataPlatform.Application.Tests;

[TestFixture]
public class DependencyInjectionTests
{
    [Test]
    public void RequestHandlersAreRegistered()
    {
        var serviceCollection = new ServiceCollection() as IServiceCollection;

        serviceCollection.AddApplicationDependencies();

        serviceCollection.BuildServiceProvider();

    }

}
