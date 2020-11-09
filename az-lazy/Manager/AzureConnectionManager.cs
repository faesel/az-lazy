using System;
using System.Threading.Tasks;
using az_lazy.Exceptions;

namespace az_lazy.Manager
{
    public interface IAzureConnectionManager
    {
        Task<bool> TestConnection(string connectionString);
    }

    public class AzureConnectionManager : IAzureConnectionManager
    {
        private readonly IAzureQueueManager AzureStorageManager;

        public AzureConnectionManager(
            IAzureQueueManager azureStorageManager)
        {
            AzureStorageManager = azureStorageManager;
        }

        public async Task<bool> TestConnection(string connectionString)
        {
            try
            {
                var queues = await AzureStorageManager.GetQueues(connectionString).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                throw new ConnectionException(ex);
            }
        }
    }
}