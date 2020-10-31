using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Queue.Executor
{
    public class CureQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureStorageManager AzureStorageManager;

        public CureQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureStorageManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.CureQueue))
            {
                string message = $"Clearing poison queue {opts.CureQueue}-poison";
                ConsoleHelper.WriteInfoWaiting(message);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureStorageManager.MovePoisonQueues(selectedConnection.ConnectionString, opts.CureQueue).ConfigureAwait(false);

                    ConsoleHelper.WriteLineNormal("Finished moving poison queue messages");
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