using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using az_lazy.Model;

namespace az_lazy.Manager
{
    public interface IAzureTableManager
    {
        Task<List<CloudTable>> GetTables(string connectionString);
        Task<List<DynamicTableEntity>> Sample(string connectionString, string sample, int sampleCount);
    }

    public class AzureTableManager : IAzureTableManager
    {
        private const int DefaultSampleCount = 10;

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

        public async Task<List<DynamicTableEntity>> Sample(string connectionString, string sample, int sampleCount)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(sample);

            sampleCount = sampleCount == 0 ? DefaultSampleCount : sampleCount;

            TableContinuationToken selectToken = null;
            List<DynamicTableEntity> tableEntities = new List<DynamicTableEntity>();

            int takeCount = 0;

            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(new TableQuery().Take(sampleCount), selectToken).ConfigureAwait(false);
                tableEntities.AddRange(segment.Results);

                takeCount += sampleCount;
            }
            while(selectToken != null && takeCount <= sampleCount);

            return tableEntities;
        }
    }
}