using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Queue.Executor
{
    public class ListExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureStorageManager AzureStorageManager;

        public ListExecutor(
            ILocalStorageManager localStorageManager,
            IAzureStorageManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (opts.List)
            {
                const string message = "Fetching queues";

                ConsoleHelper.WriteInfoWaiting(message, true);

                var selectedConnection = LocalStorageManager.GetSelectedConnection();
                var queueList = await AzureStorageManager.GetQueues(selectedConnection.ConnectionString).ConfigureAwait(false);

                if (queueList.Count > 0)
                {
                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteSepparator();

                    foreach (var queue in queueList)
                    {
                        await queue.FetchAttributesAsync().ConfigureAwait(false);

                        var queueCount = queue.ApproximateMessageCount ?? 0;
                        var isPoisonQueue = queue.Name.EndsWith("poison");
                        var queueInformation = $"{queue.Name} ({queueCount})";

                        if (isPoisonQueue)
                        {
                            ConsoleHelper.WriteLineError(queueInformation);
                        }
                        else
                        {
                            ConsoleHelper.WriteLineNormal(queueInformation);
                        }
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