using System.Threading.Tasks;
using az_lazy.Manager;

namespace az_lazy.Commands.Table.Executor
{
    public class SampleExecutor : ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public SampleExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Sample))
            {
                await AzureTableManager.Sample(opts.Sample, opts.SampleCount);
            }
        }
    }
}