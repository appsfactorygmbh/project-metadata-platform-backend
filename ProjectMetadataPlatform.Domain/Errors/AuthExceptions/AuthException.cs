namespace ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

public abstract class AuthException(string message) : PmpException(message);