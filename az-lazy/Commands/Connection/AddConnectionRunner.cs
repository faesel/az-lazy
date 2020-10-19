using System;
using System.Threading.Tasks;
using az_lazy.Exceptions;
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
            if (!string.IsNullOrEmpty(opts.AccessKey) && !string.IsNullOrEmpty(opts.ConnectionName))
            {
                Console.Write($"Testing {opts.ConnectionName} connection ...");
                Console.SetCursorPosition(0, Console.CursorTop);

                var isConnected = false;

                try
                {
                    isConnected = await AzureStorageManager.TestConnection(opts.ConnectionName, opts.AccessKey).ConfigureAwait(false);
                }
                catch(ConnectionException connectionException)
                {
                    Console.WriteLine(connectionException.Message);
                }

                if(!isConnected)
                {
                    Console.WriteLine($"Testing {opts.ConnectionName} connection ... Failed!");
                    return false;
                }

                Console.WriteLine($"Testing {opts.ConnectionName} connection ... Succeeded!");

                Console.Write($"Storing {opts.ConnectionName} connection ...");
                Console.SetCursorPosition(0, Console.CursorTop);

                LocalStorageManager.AddConnection(opts.ConnectionName, opts.AccessKey);

                Console.WriteLine($"Storing {opts.ConnectionName} connection ... Succeeded!");

                Console.WriteLine($"Finished adding connection {opts.ConnectionName}");
            }

            return true;
        }
    }
}