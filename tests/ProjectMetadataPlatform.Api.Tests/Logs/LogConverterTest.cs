using System;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Logs;
using ProjectMetadataPlatform.Api.Logs.Models;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.User;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Api.Tests.Logs;

public class LogConverterTest
{
    private LogConverter _logConverter;

    [SetUp]
    public void Setup()
    {
        _logConverter = new LogConverter();
    }

    [Test]
    public async Task ConvertToUpdatedProjectLog_Test()
    {
        var log = new Log
        {
            Id = 41,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "42",
            Email = "Slartibartfast",
            User = new User { Email = "Slartibartfast" },
            ProjectId = 43,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange { Property = "Fjords", OldValue = "None", NewValue = "Many" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage,
                Is.EqualTo("Slartibartfast updated project properties:  set Fjords from None to Many"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToArchivedProjectLog_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "43",
            Email = "Deep Thought",
            User = new User { Email = "Deep Thought" },
            ProjectId = 44,
            Project = new Project
            {
                ProjectName = "Ultimate Question of Life, the Universe and Everything",
                ClientName = "Mice",
                BusinessUnit = "",
                TeamNumber = 1,
                Department = ""
            },
            Action = Action.ARCHIVED_PROJECT
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Deep Thought archived project Ultimate Question of Life, the Universe and Everything"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToAddedProjectLog_Test()
    {
        var log = new Log
        {
            Id = 43,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "44",
            Email = "Infinite Improbability Drive",
            User = new User { Email = "Infinite Improbability Drive" },
            ProjectId = 45,
            Project = new Project
            {
                ProjectName = "Atmosphere",
                ClientName = "",
                BusinessUnit = "",
                TeamNumber = 1,
                Department = ""
            },
            Action = Action.ADDED_PROJECT_PLUGIN,
            Changes =
            [
                new LogChange { Property = "flyingObjects", OldValue = "", NewValue = "Wale" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Infinite Improbability Drive added a new plugin to project Atmosphere with properties: flyingObjects = Wale"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToUpdatedProjectPluginLog_Test()
    {
        var log = new Log
        {
            Id = 44,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "45",
            Email = "Ground",
            User = new User { Email = "Ground" },
            ProjectId = 46,
            Project = new Project
            {
                ProjectName = "Wale",
                ClientName = "",
                BusinessUnit = "",
                TeamNumber = 1,
                Department = ""
            },
            Action = Action.UPDATED_PROJECT_PLUGIN,
            Changes =
            [
                new LogChange { Property = "alive", OldValue = "yes", NewValue = "no" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Ground updated plugin properties in project Wale:  set alive from yes to no"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToRemovedProjectPluginLog_Test()
    {
        var log = new Log
        {
            Id = 45,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "46",
            Email = "Prostetnic Vogon Jeltz",
            User = new User { Email = "Prostetnic Vogon Jeltz" },
            ProjectId = 47,
            Project = new Project
            {
                ProjectName = "Solarsystem",
                ClientName = "Mice",
                BusinessUnit = "",
                TeamNumber = 1,
                Department = ""
            },
            Action = Action.REMOVED_PROJECT_PLUGIN,
            Changes =
            [
                new LogChange { Property = "Earth", OldValue = "intact", NewValue = "destroyed" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Prostetnic Vogon Jeltz removed a plugin from project Solarsystem with properties: Earth = destroyed"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToUnarchivedProjectLog_Test()
    {
        var log = new Log
        {
            Id = 46,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "47",
            Email = "Earth",
            User = new User { Email = "Earth" },
            ProjectId = 48,
            Project = new Project
            {
                ProjectName = "Ultimate Question of Life, the Universe and Everything",
                ClientName = "Mice",
                BusinessUnit = "",
                TeamNumber = 1,
                Department = ""
            },
            Action = Action.UNARCHIVED_PROJECT
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Earth unarchived project Ultimate Question of Life, the Universe and Everything"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToProjectLogDeletedUser_Test()
    {
        var log = new Log
        {
            Id = 47,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "48",
            Email = "Earth Population",
            ProjectId = 49,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange { Property = "Home Planet", OldValue = "Earth", NewValue = "None" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage,
                Is.EqualTo("Earth Population (deleted user) updated project properties:  set Home Planet from Earth to None"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }
}
