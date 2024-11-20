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
            AuthorId = "42",
            AuthorEmail = "Slartibartfast",
            Author = new User { Email = "Slartibartfast" },
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
            AuthorId = "43",
            AuthorEmail = "Deep Thought",
            Author = new User { Email = "Deep Thought" },
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
            AuthorId = "44",
            AuthorEmail = "Infinite Improbability Drive",
            Author = new User { Email = "Infinite Improbability Drive" },
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
            AuthorId = "45",
            AuthorEmail = "Ground",
            Author = new User { Email = "Ground" },
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
            AuthorId = "46",
            AuthorEmail = "Prostetnic Vogon Jeltz",
            Author = new User { Email = "Prostetnic Vogon Jeltz" },
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
            AuthorId = "47",
            AuthorEmail = "Earth",
            Author = new User { Email = "Earth" },
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
            AuthorId = "48",
            AuthorEmail = "Earth Population",
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

    [Test]
    public async Task ConvertToLogAddedUser_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorUsername = "Infinite Improbability Drive",
            Author = new User { UserName = "Infinite Improbability Drive" },
            Action = Action.ADDED_USER,
            Changes =
            [
                new LogChange { Property = "UserName", OldValue = "", NewValue = "Bowl of Petunias" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Infinite Improbability Drive added a new user with properties: UserName = Bowl of Petunias"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToLogUpdatedUser_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorUsername = "Gandalf",
            Author = new User { UserName = "Gandalf" },
            Action = Action.UPDATED_USER,
            Changes =
            [
                new LogChange { Property = "UserName", OldValue = "Gandalf the Grey", NewValue = "Gandalf the White" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Gandalf updated user properties: set UserName from Gandalf the Grey to Gandalf the White"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToLogRemovedUser_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorUsername = "Ground",
            Author = new User { UserName = "Ground" },
            Action = Action.REMOVED_USER,
            AffectedUserEmail = "whale@air.com",
            Changes =
            [
                new LogChange { Property = "UserName", OldValue = "Whale", NewValue = "" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Ground removed user whale@air.com"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToLogRemovedProject_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorUsername = "Luke Skywalker",
            Author = new User { UserName = "Luke Skywalker" },
            Action = Action.REMOVED_PROJECT,
            ProjectName = "DeathStar",
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Luke Skywalker removed project DeathStar"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToLogAddedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorUsername = "Chancellor Palpatine",
            Author = new User { UserName = "Chancellor Palpatine" },
            Action = Action.ADDED_GLOBAL_PLUGIN,
            Changes =
            [
                new LogChange { Property = "PluginName", OldValue = "", NewValue = "Grand Army of the Republic" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Chancellor Palpatine added a new global plugin with properties: PluginName = Grand Army of the Republic"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToLogUpdatedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorUsername = "Darth Sidious",
            Author = new User { UserName = "Darth Sidious" },
            Action = Action.UPDATED_GLOBAL_PLUGIN,
            Changes =
            [
                new LogChange { Property = "UserName", OldValue = "Republic", NewValue = "First Galactic Empire" }
            ]
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Darth Sidious updated global plugin properties: set UserName from Republic to First Galactic Empire"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToLogArchivedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorUsername = "Zip",
            Author = new User { UserName = "Zip" },
            Action = Action.ARCHIVED_GLOBAL_PLUGIN,
            GlobalPluginName = "Directory",
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Zip archived global plugin Directory"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToLogUnarchivedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorUsername = "Unzip",
            Author = new User { UserName = "Unzip" },
            Action = Action.UNARCHIVED_GLOBAL_PLUGIN,
            GlobalPluginName = "Directory",
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Unzip unarchived global plugin Directory"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }

    [Test]
    public async Task ConvertToLogRemovedGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorUsername = "Recursively",
            Author = new User { UserName = "Recursively" },
            Action = Action.REMOVED_GLOBAL_PLUGIN,
            GlobalPluginName = "root"
        };

        LogResponse logResponse = _logConverter.BuildLogMessage(log);

        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Recursively removed global plugin root"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }
}
