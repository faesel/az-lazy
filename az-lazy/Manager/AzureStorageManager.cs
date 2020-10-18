using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;

namespace az_lazy.Manager
{
    public interface IAzureStorageManager
    {
        Task<bool> TestConnection(string connectionString);
    }

    public class AzureStorageManager : IAzureStorageManager
    {
        public async Task<bool> TestConnection(string connectionString)
        {
            var isSuccesfull = true;

            try
            {
                
            }
            catch(Exception ex)
            {

            }

            return true;
        }
    }
}