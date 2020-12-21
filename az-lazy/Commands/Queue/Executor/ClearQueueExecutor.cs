using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Queue.Executor
{
    public class ClearQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public ClearQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.ClearQueue))
            {
                string message = $"Clearing queue {opts.ClearQueue}";
                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureStorageManager.ClearQueue(selectedConnection.ConnectionString, opts.ClearQueue);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineNormal($"Finished clearing queue {opts.ClearQueue}");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }
        }
    }
}