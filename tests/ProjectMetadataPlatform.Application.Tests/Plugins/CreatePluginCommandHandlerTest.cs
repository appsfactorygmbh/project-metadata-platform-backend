using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class CreatePluginCommandHandlerTest
{
    private CreatePluginCommandHandler _handler;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockPluginRepo = new Mock<IPluginRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreatePluginCommandHandler(_mockPluginRepo.Object, _mockLogRepo.Object, _mockUnitOfWork.Object);
    }

    [Test]
    public async Task CreatePlugin_Test()
    {
        var examplePlugin = new Plugin { PluginName = "Airlock", Id = 13, ProjectPlugins = [] };
        _mockPluginRepo.Setup(m => m.StorePlugin(It.IsAny<Plugin>())).Callback<Plugin>(p => p.Id = 13);

        int result = await _handler.Handle(new CreatePluginCommand("Airlock", true, []), It.IsAny<CancellationToken>());
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(13));
            Assert.That(examplePlugin.PluginName, Is.EqualTo("Airlock"));
        });
    }
}
