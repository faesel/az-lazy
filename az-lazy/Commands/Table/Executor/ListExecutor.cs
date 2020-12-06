using System.Linq;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Table.Executor
{
    public class ListExecutor : ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public ListExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if (opts.List)
            {
                const string message = "Fetching tables";

                ConsoleHelper.WriteInfoWaiting(message, true);

                var selectedConnection = LocalStorageManager.GetSelectedConnection();
                var tables = await AzureTableManager.GetTables(selectedConnection.ConnectionString).ConfigureAwait(false);

                if (tables.Count > 0)
                {
                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteSepparator();

                    if(!string.IsNullOrEmpty(opts.Contains))
                    {
                        tables = tables.Where(x => x.Name.Contains(opts.Contains)).ToList();
                    }

                    foreach (var table in tables)
                    {
                        ConsoleHelper.WriteLineNormal(table.Name);
                    }
                }
                else
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError("No queues found");
                }
            }
        }
    }
}