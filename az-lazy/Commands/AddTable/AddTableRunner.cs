using az_lazy.Helpers;
using az_lazy.Manager;
using System;
using System.Threading.Tasks;

namespace az_lazy.Commands.AddTable
{
    public class AddTableRunner : ICommandRunner<AddTableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public AddTableRunner(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task<bool> Run(AddTableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Name))
            {
                string message = $"Creating table {opts.Name}";
                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureTableManager.Create(selectedConnection.ConnectionString, opts.Name);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineNormal($"Finished creating table {opts.Name}");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }

            return true;
        }
    }
}