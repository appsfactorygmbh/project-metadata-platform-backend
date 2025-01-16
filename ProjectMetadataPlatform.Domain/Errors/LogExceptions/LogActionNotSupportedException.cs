using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Domain.Errors.LogExceptions;

public class LogActionNotSupportedException(Action action) : LogException("The action " + action + " is not supported.");