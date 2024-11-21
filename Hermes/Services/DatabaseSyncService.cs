using System;
using Dotmim.Sync.MariaDB;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync;
using Hermes.Repositories;
using System.Threading.Tasks;
using Hermes.Common;

namespace Hermes.Services;

public class DataBaseSyncService(
    HermesRemoteContext hermesRemoteContext,
    HermesLocalContext hermesLocalContext,
    ILogger logger)
{
    private readonly string[] _tablesToSync = ["users", "feature_permissions"];

    public async Task Sync(Action<string>? onProgress = null)
    {
        var serverProvider = new MariaDBSyncProvider(hermesRemoteContext.ConnectionString);
        var clientProvider = new SqliteSyncProvider(hermesLocalContext.FilePath);
        var setup = new SyncSetup(_tablesToSync);
        var agent = new SyncAgent(clientProvider, serverProvider);
        var progress = new SynchronousProgress<ProgressArgs>(s =>
            onProgress?.Invoke($@"{Language.Resources.txt_syncing_database} - {s.ProgressPercentage:p}"));
        var syncResult = await agent.SynchronizeAsync(setup, progress);
        logger.Debug(syncResult.ToString());
    }
}