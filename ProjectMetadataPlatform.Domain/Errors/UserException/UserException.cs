namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Represents an abstract base class for user-related exceptions.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public abstract class UserException(string message) : PmpException(message);
