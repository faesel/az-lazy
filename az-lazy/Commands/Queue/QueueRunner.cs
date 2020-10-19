using System;
using System.Threading.Tasks;
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
                var selectedConnection = LocalStorageManager.GetSelectedConnection();
                var queueList = await AzureStorageManager.GetQueues(selectedConnection.ConnectionName, selectedConnection.ConnectionString).ConfigureAwait(false);

                if(queueList.Count > 0)
                {
                    foreach(var queue in queueList)
                    {
                        Console.WriteLine(queue.Name);
                    }
                }
                else
                {
                    Console.WriteLine("No queues found");
                }
            }

            return true;
        }
    }
}