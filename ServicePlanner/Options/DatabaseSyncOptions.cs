namespace ServicePlanner.Options;

public sealed class DatabaseSyncOptions
{
    public const string SectionName = "DatabaseSync";

    public bool Enabled { get; set; } = true;
    public string? StorageAccountName { get; set; }
    public string? StorageAccountKey { get; set; }
    public string? ShareName { get; set; }
    public string? FilePath { get; set; }
    public int RefreshIntervalHours { get; set; } = 24;
}
