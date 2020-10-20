using System;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Connection
{
    public class AddConnectionRunner : IConnectionRunner<AddConnectionOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureStorageManager AzureStorageManager;

        public AddConnectionRunner(
            ILocalStorageManager localStorageManager,
            IAzureStorageManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task<bool> Run(AddConnectionOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.ConnectionString) && !string.IsNullOrEmpty(opts.ConnectionName))
            {
                var testingMessage = $"Testing {opts.ConnectionName} connection";

                ConsoleHelper.WriteInfoWaiting(testingMessage, true);

                var errorMessage = string.Empty;
                bool isConnected;

                try
                {
                    isConnected = await AzureStorageManager.TestConnection(opts.ConnectionString).ConfigureAwait(false);
                }
                catch (ConnectionException connectionException)
                {
                    errorMessage = connectionException.Message;
                    isConnected = false;
                }

                if (!isConnected)
                {
                    ConsoleHelper.WriteLineFailedWaiting(testingMessage);
                    ConsoleHelper.WriteLineError(errorMessage);

                    return false;
                }

                ConsoleHelper.WriteLineSuccessWaiting(testingMessage);

                var storingMessage = $"Storing {opts.ConnectionName} connection";
                ConsoleHelper.WriteInfoWaiting(storingMessage, true);

                LocalStorageManager.AddConnection(opts.ConnectionName, opts.ConnectionString);

                ConsoleHelper.WriteLineSuccessWaiting(storingMessage);
                ConsoleHelper.WriteLineEnd($"Finished adding connection {opts.ConnectionName}");
            }

            return true;
        }
    }
}