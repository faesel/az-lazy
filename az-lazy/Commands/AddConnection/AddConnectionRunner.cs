using System;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.AddConnection
{
    public class AddConnectionRunner : ICommandRunner<AddConnectionOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureConnectionManager AzureConnectionManager;

        public AddConnectionRunner(
            ILocalStorageManager localStorageManager,
            IAzureConnectionManager azureConnectionManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureConnectionManager = azureConnectionManager;
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
                    isConnected = await AzureConnectionManager.TestConnection(opts.ConnectionString).ConfigureAwait(false);
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

                LocalStorageManager.AddConnection(opts.ConnectionName, opts.ConnectionString, opts.Select);

                ConsoleHelper.WriteLineSuccessWaiting(storingMessage);
                ConsoleHelper.WriteLineNormal($"Finished adding connection {opts.ConnectionName}");
            }

            return true;
        }
    }
}