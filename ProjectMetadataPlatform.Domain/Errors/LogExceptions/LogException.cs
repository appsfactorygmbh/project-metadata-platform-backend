namespace ProjectMetadataPlatform.Domain.Errors.LogExceptions;

public abstract class LogException(string message): PmpException(message);