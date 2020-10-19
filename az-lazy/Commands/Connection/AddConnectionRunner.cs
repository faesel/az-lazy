using System;
using System.Threading.Tasks;
using az_lazy.Manager;

namespace az_lazy.Commands.Connection
{
    public class AddConnectionRunner : IConnectionRunner<AddConnectionOptions>
    {
        private readonly ILocalStorageManager localStorageManager;
        private readonly IAzureStorageManager azureStorageManager;

        public AddConnectionRunner(
            ILocalStorageManager localStorageManager,
            IAzureStorageManager azureStorageManager)
        {
            this.localStorageManager = localStorageManager;
            this.azureStorageManager = azureStorageManager;
        }

        public async Task<bool> Run(AddConnectionOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.ConnectionString) && !string.IsNullOrEmpty(opts.ConnectionName))
            {
                Console.Write($"Testing {opts.ConnectionName} connection ...");
                Console.SetCursorPosition(0, Console.CursorTop);
                var result = await azureStorageManager.TestConnection(opts.ConnectionName, opts.ConnectionString);

                if(!result)
                {
                    Console.WriteLine($"Testing {opts.ConnectionName} connection ... Failed!");
                    return false;
                }

                Console.WriteLine($"Testing {opts.ConnectionName} connection ... Succeeded!");

                Console.Write($"Storing {opts.ConnectionName} connection ...");
                Console.SetCursorPosition(0, Console.CursorTop);

                localStorageManager.AddConnection(opts.ConnectionName, opts.ConnectionString);

                Console.WriteLine($"Storing {opts.ConnectionName} connection ... Succeeded!");

                Console.WriteLine($"Finished adding connection {opts.ConnectionName}");
            }

            return true;
        }
    }
}