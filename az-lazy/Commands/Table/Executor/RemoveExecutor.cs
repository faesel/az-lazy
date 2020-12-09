using az_lazy.Helpers;
using az_lazy.Manager;
using System;
using System.Threading.Tasks;

namespace az_lazy.Commands.Table.Executor
{
    public class RemoveExecutor : ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public RemoveExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Remove))
            {
                var infoMessage = $"Removing table {opts.Remove}";
                ConsoleHelper.WriteInfoWaiting(infoMessage, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var result = await AzureTableManager.Remove(selectedConnection.ConnectionString, opts.Remove).ConfigureAwait(false);

                    if(!result) {
                        ConsoleHelper.WriteLineFailedWaiting(infoMessage);
                        ConsoleHelper.WriteLineNormal("Check to ensure the table name is correct");
                    }
                    else {
                        ConsoleHelper.WriteLineSuccessWaiting(infoMessage);
                    }
                }
                catch(Exception ex)
                {
                    ConsoleHelper.WriteLineError(ex.Message);
                    ConsoleHelper.WriteLineFailedWaiting($"Failed to query table {opts.Query}");
                }
            }
        }
    }
}