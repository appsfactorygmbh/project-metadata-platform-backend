using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Exception thrown when a user already exists in the system.
/// </summary>
public class UserAlreadyExistsException : UserException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="userId">The ID of the user that already exists.</param>
    public UserAlreadyExistsException(string userId)
        : base("The user with email " + userId + " already exists.")
    {
    }
}