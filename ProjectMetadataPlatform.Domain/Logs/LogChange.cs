namespace ProjectMetadataPlatform.Domain.Logs;

public class LogChange
{
    /// <summary>
    ///     The id of the Log change
    /// </summary>
    public int Id { get; set; }

    public Log? Log { get; set; }

    public int LogId { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }

    public string Property { get; set; }
}
