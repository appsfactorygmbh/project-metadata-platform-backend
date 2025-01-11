using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.Interfaces;

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
        Assert.Multiple(() =>
        {
            Assert.That(serviceProvider.GetService<ILogConverter>(), Is.Not.Null);
            Assert.That(serviceProvider.GetService<IExceptionHandler<IBasicException>>(), Is.Not.Null);
        });
    }
}
