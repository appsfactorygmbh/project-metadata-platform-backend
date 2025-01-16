namespace ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

/// <summary>
/// Represents an abstract base class for project-related exceptions in the Project Metadata Platform used to mark exceptions that are related to projects so that they are handled by the ProjectsExceptionHandler.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public abstract class ProjectException(string message): PmpException(message);