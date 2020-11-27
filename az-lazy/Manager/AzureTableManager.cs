using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace az_lazy.Manager
{
    public interface IAzureTableManager
    {
        Task<List<CloudTable>> GetTables(string connectionString);
        Task Sample(string sample, int sampleCount);
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

            await Test(connectionString);

            return cloudTableList;
        }

        public async Task Sample(string sample, int sampleCount)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionstring);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("test");

            TableContinuationToken selectToken = null;
            do
            {
                var results = await table.ExecuteQuerySegmentedAsync(new TableQuery().Take(10), selectToken).ConfigureAwait(false);
                var partitionKey = results.Results.First().PartitionKey;
                var rowKey = results.Results.First().RowKey;
                var propKey = results.Results.First().Properties.First().Key;
                var propValue = results.Results.First().Properties.First().Value;

            }
            while(selectToken != null);
        }
    }
}