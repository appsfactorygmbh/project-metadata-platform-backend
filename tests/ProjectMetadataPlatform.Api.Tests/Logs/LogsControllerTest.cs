using MediatR;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Logs;

namespace ProjectMetadataPlatform.Api.Tests.Logs;

public class LogsControllerTest
{
    private LogsController _controller;
    private Mock<IMediator> _mediator;
    private Mock<LogConverter> _logConverter;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _logConverter = new Mock<LogConverter>();
        _controller = new LogsController(_mediator.Object, _logConverter.Object);
    }


}
