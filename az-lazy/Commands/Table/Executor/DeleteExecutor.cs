using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Table.Executor
{
    public class DeleteExecutor : ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public DeleteExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Delete) && (!string.IsNullOrEmpty(opts.PartitionKey) || !string.IsNullOrEmpty(opts.RowKey)))
            {
                try
                {
                    const string message = "Deleting rows";

                    ConsoleHelper.WriteInfoWaiting(message, true);

                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var deleteCount = await AzureTableManager.DeleteRow(selectedConnection.ConnectionString, opts.Delete, opts.PartitionKey, opts.RowKey).ConfigureAwait(false);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineNormal($"Finished deleting rows, {deleteCount} rows removed");
                }
                catch(Exception ex)
                {
                    ConsoleHelper.WriteLineError(ex.Message);
                    ConsoleHelper.WriteLineFailedWaiting($"Failed to sample table {opts.Sample}");
                }
            }
        }
    }
}