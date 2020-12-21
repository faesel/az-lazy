using System;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Queue.Executor
{
    public class ListExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public ListExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
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
                var queueList = await AzureStorageManager.GetQueues(selectedConnection.ConnectionString);

                if (queueList.Count > 0)
                {
                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteSepparator();

                    if(!string.IsNullOrEmpty(opts.Contains))
                    {
                        queueList = queueList.Where(x => x.Name.Contains(opts.Contains)).ToList();
                    }

                    foreach (var queue in queueList)
                    {
                        await queue.FetchAttributesAsync();

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