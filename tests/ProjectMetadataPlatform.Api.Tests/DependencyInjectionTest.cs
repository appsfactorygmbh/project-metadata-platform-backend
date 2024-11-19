using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectMetadataPlatform.Application;

namespace ProjectMetadataPlatform.Api.Tests;

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
