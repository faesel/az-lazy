using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace az_lazy.Manager
{
    public interface IAzureTableManager
    {
        Task<List<CloudTable>> GetTables(string connectionString);
    }

    public class AzureTableManager : IAzureTableManager
    {
        public async Task<List<CloudTable>> GetTables(string connectionString)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = cloudStorageAccount.CreateCloudTableClient();

            TableContinuationToken token = null;
            List<CloudTable> cloudTableList = new List<CloudTable>();

            do
            {
                TableResultSegment segment = await tableClient.ListTablesSegmentedAsync(token).ConfigureAwait(false);
                token = segment.ContinuationToken;
                cloudTableList.AddRange(segment.Results);
            }
            while (token != null);

            return cloudTableList;
        }
    }
}