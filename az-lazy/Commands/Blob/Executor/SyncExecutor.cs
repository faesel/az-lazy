using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;
using System.Threading.Tasks;

namespace az_lazy.Commands.Blob.Executor
{
    public class SyncExecutor : ICommandExecutor<BlobOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public SyncExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public Task Execute(BlobOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}