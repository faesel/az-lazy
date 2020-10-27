using System;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Queue
{
    public class QueueRunner : IConnectionRunner<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureStorageManager AzureStorageManager;

        public QueueRunner(
            ILocalStorageManager localStorageManager,
            IAzureStorageManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task<bool> Run(QueueOptions opts)
        {
            if(opts.List)
            {
                const string message = "Fetching queues";

                ConsoleHelper.WriteInfoWaiting(message, true);

                var selectedConnection = LocalStorageManager.GetSelectedConnection();
                var queueList = await AzureStorageManager.GetQueues(selectedConnection.ConnectionString).ConfigureAwait(false);

                if(queueList.Count > 0)
                {
                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteSepparator();

                    foreach(var queue in queueList)
                    {
                        await queue.FetchAttributesAsync().ConfigureAwait(false);

                        var queueCount = queue.ApproximateMessageCount ?? 0;
                        var isPoisonQueue = queue.Name.EndsWith("poison");
                        var queueInformation = $"{queue.Name} ({queueCount})";

                        if(isPoisonQueue)
                        {
                            ConsoleHelper.WriteLineError(queueInformation);
                        }
                        else
                        {
                            ConsoleHelper.WriteLineNormal(queueInformation);
                        }
                    }

                    return true;
                }
                else
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError("No queues found");
                }
            }

            if(!string.IsNullOrEmpty(opts.RemoveQueue))
            {
                string message = $"Removing queue {opts.RemoveQueue}";
                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureStorageManager.RemoveQueue(selectedConnection.ConnectionString, opts.RemoveQueue).ConfigureAwait(false);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineNormal($"Finished removing queue {opts.RemoveQueue}");

                    return true;
                }
                catch(Exception ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }

            if(!string.IsNullOrEmpty(opts.CureQueue))
            {
                string message = $"Clearing poison queue {opts.CureQueue}-poison ...";
                ConsoleHelper.WriteLineInfo(message);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureStorageManager.MovePoisonQueues(selectedConnection.ConnectionString, opts.CureQueue).ConfigureAwait(false);

                    ConsoleHelper.WriteLineNormal("Finished moving poison queue messages");

                    return true;
                }
                catch(QueueException ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }

            if(!string.IsNullOrEmpty(opts.ClearQueue))
            {
                string message = $"Clearing queue {opts.ClearQueue}";
                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureStorageManager.ClearQueue(selectedConnection.ConnectionString, opts.ClearQueue).ConfigureAwait(false);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineNormal($"Finished clearing queue {opts.ClearQueue}");

                    return true;
                }
                catch(Exception ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }

            return false;
        }
    }
}