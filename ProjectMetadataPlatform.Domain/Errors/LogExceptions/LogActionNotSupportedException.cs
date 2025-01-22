using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Domain.Errors.LogExceptions;

/// <summary>
/// Exception thrown when a specific log action is not supported.
/// </summary>
/// <param name="action">The action that is not supported.</param>
public class LogActionNotSupportedException(Action action) : LogException("The action " + action + " is not supported.");