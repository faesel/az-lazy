using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Queue
{
    public class AddQueueRunner : IConnectionRunner<AddQueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureStorageManager AzureStorageManager;

        public AddQueueRunner(
            ILocalStorageManager localStorageManager,
            IAzureStorageManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task<bool> Run(AddQueueOptions options)
        {
            if(!string.IsNullOrEmpty(options.Name))
            {
                var message = $"Creating new queue {options.Name}";

                ConsoleHelper.WriteInfoWaiting(message, true);

                var connection = LocalStorageManager.GetSelectedConnection();

                try
                {
                    await AzureStorageManager.CreateQueue(connection.ConnectionString, options.Name).ConfigureAwait(false);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineNormal($"Finished creating queue {options.Name}");
                }
                catch(QueueException ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }

                return true;
            }

            return false;
        }
    }
}