using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Interfaces;

namespace ProjectMetadataPlatform.Api.Tests;

[TestFixture]
public class DependencyInjectionTests
{
    [Test]
    public void RequestHandlersAreRegistered()
    {
        var serviceCollection = new ServiceCollection() as IServiceCollection;

        serviceCollection.AddApiDependencies();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        Assert.That(serviceProvider.GetService<ILogConverter>(), Is.Not.Null);
    }
}
