using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using Microsoft.Azure.Cosmos.Table;

namespace az_lazy.Manager
{
    public interface IAzureTableManager
    {
        Task<List<CloudTable>> GetTables(string connectionString);
        Task<List<DynamicTableEntity>> Sample(string connectionString, string sample, int sampleCount);
        Task<List<DynamicTableEntity>> Query(string connectionString, string tableName, string partitionKey, string rowKey, int take);
        Task<int> DeleteRow(string connectionString, string tableName, string partitionKey, string rowKey);
        Task<bool> Remove(string connectionString, string tableToRemote);
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

        public async Task<List<DynamicTableEntity>> Query(string connectionString, string tableName, string partitionKey, string rowKey, int take)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);

            List<string> tableQueries = new List<string>();

            string query = string.Empty;
            if(!string.IsNullOrEmpty(partitionKey) || !string.IsNullOrEmpty(rowKey))
            {
                if(!string.IsNullOrEmpty(partitionKey))
                {
                    tableQueries.Add(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
                }

                if(!string.IsNullOrEmpty(rowKey))
                {
                    tableQueries.Add(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));
                }

                query = tableQueries.Count == 1 ?
                    tableQueries[0] : TableQuery.CombineFilters(tableQueries[0], TableOperators.And, tableQueries[1]);
            }

            int? takeCount = null;
            if(take > 0)
            {
                takeCount = take;
            }

            TableContinuationToken selectToken = null;
            List<DynamicTableEntity> tableEntities = new List<DynamicTableEntity>();

            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(new TableQuery { FilterString = query }.Take(takeCount), selectToken).ConfigureAwait(false);
                tableEntities.AddRange(segment.Results);
            }
            while(selectToken != null);

            return tableEntities;
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

        public async Task<int> DeleteRow(string connectionString, string tableName, string partitionKey, string rowKey)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);

            List<string> tableQueries = new List<string>();
            if(!string.IsNullOrEmpty(partitionKey))
            {
                tableQueries.Add(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            }

            if(!string.IsNullOrEmpty(rowKey))
            {
                tableQueries.Add(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));
            }

            string query = tableQueries.Count == 1 ?
                tableQueries[0] : TableQuery.CombineFilters(tableQueries[0], TableOperators.And, tableQueries[1]);

            int deleteCount = 0;
            foreach (var item in table.ExecuteQuery(new TableQuery { FilterString = query }))
            {
                var deleteOperation = TableOperation.Delete(item);
                var result = await table.ExecuteAsync(deleteOperation).ConfigureAwait(false);

                if(result.HttpStatusCode == (int)HttpStatusCode.NoContent)
                {
                    deleteCount++;
                }
            }

            return deleteCount;
        }

        public async Task<bool> Remove(string connectionString, string tableToRemote)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableToRemote);

            return await table.DeleteIfExistsAsync().ConfigureAwait(false);
        }
    }
}