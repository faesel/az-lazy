using System.Threading.Tasks;
using az_lazy.Manager;

namespace az_lazy.Commands.Table.Executor
{
    public class DeleteExecutor : ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public DeleteExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Delete))
            {

            }
        }
    }
}