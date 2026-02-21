using Azure;
using Azure.Storage;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Extensions.Options;
using ServicePlanner.Options;

namespace ServicePlanner.Services;

public sealed class DatabaseSyncService
{
    private const string DataSourcePrefix = "Data Source=";
    private readonly DatabaseSyncOptions _options;
    private readonly ILogger<DatabaseSyncService> _logger;

    public DatabaseSyncService(IOptions<DatabaseSyncOptions> options, ILogger<DatabaseSyncService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SyncIfNeededAsync(string dbPath, IWebHostEnvironment environment, CancellationToken cancellationToken = default)
    {
        if (!environment.IsDevelopment() || !_options.Enabled)
        {
            return;
        }

        if (!TryGetLocalPath(dbPath, out var localPath))
        {
            _logger.LogWarning("Database sync skipped because the database path was empty.");
            return;
        }

        if (!ShouldRefresh(localPath))
        {
            return;
        }

        if (!HasStorageConfig())
        {
            _logger.LogInformation("Database sync skipped because storage credentials were not configured.");
            return;
        }

        try
        {
            await DownloadAsync(localPath, cancellationToken);
            _logger.LogInformation("Downloaded latest database snapshot to {LocalPath}.", localPath);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Database sync failed while downloading from Azure Files.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database sync failed.");
        }
    }

    private bool HasStorageConfig()
    {
        return !string.IsNullOrWhiteSpace(_options.StorageAccountName)
               && !string.IsNullOrWhiteSpace(_options.StorageAccountKey)
               && !string.IsNullOrWhiteSpace(_options.ShareName)
               && !string.IsNullOrWhiteSpace(_options.FilePath);
    }

    private bool ShouldRefresh(string localPath)
    {
        if (_options.RefreshIntervalHours <= 0)
        {
            return true;
        }

        var fileInfo = new FileInfo(localPath);
        if (!fileInfo.Exists)
        {
            return true;
        }

        var cutoff = DateTime.UtcNow.AddHours(-_options.RefreshIntervalHours);
        return fileInfo.LastWriteTimeUtc < cutoff;
    }

    private async Task DownloadAsync(string localPath, CancellationToken cancellationToken)
    {
        var credential = new StorageSharedKeyCredential(
            _options.StorageAccountName!,
            _options.StorageAccountKey!);

        var shareUri = new Uri($"https://{_options.StorageAccountName}.file.core.windows.net/{_options.ShareName}");
        var shareClient = new ShareClient(shareUri, credential);
        var fileClient = shareClient.GetRootDirectoryClient()
            .GetFileClient(_options.FilePath!.Replace('\\', '/'));

        var directory = Path.GetDirectoryName(localPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var download = await fileClient.DownloadAsync(new ShareFileDownloadOptions(), cancellationToken);
        await using var destination = File.Open(localPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await download.Value.Content.CopyToAsync(destination, cancellationToken);
    }

    private static bool TryGetLocalPath(string dbPath, out string localPath)
    {
        localPath = dbPath.StartsWith(DataSourcePrefix, StringComparison.OrdinalIgnoreCase)
            ? dbPath[DataSourcePrefix.Length..]
            : dbPath;

        if (string.IsNullOrWhiteSpace(localPath))
        {
            return false;
        }

        localPath = localPath.Trim();
        return !string.IsNullOrWhiteSpace(localPath);
    }
}
