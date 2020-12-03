using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Table.Executor
{
    public class QueryExecutor: ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public QueryExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Table) && (!string.IsNullOrEmpty(opts.PartitionKey) || !string.IsNullOrEmpty(opts.RowKey)))
            {
                var infoMessage = $"Sampleing table {opts.Table}";
                ConsoleHelper.WriteInfoWaiting(infoMessage, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var sampledEntities = await AzureTableManager.Query(selectedConnection.ConnectionString, opts.Table, opts.PartitionKey, opts.RowKey).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    ConsoleHelper.WriteLineError(ex.Message);
                    ConsoleHelper.WriteLineFailedWaiting($"Failed to sample table {opts.Table}");
                }
            }
        }
    }
}