using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Queue.Executor
{
    public class WatchQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public WatchQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.Watch))
            {
                ConsoleHelper.WriteInfoWaiting($"Starting to watch {opts.Watch}", true);

                try
                {
                    ConsoleHelper.WriteLineSuccessWaiting($"Watching queue {opts.Watch}");

                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureStorageManager.WatchQueue(selectedConnection.ConnectionString, opts.Watch).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting($"Failed to watch queue {opts.Watch}");
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }
        }
    }
}