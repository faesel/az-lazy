using System;
using Azure.Storage.Queues;

namespace az_lazy.Manager
{
    public interface IAzureStorageManager
    {
        bool TestConnection(string connectionString);
    }

    public class AzureStorageManager : IAzureStorageManager
    {
        public bool TestConnection(string connectionString)
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