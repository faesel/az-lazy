using az_lazy.Manager;
using System.Threading.Tasks;

namespace az_lazy.Commands.Blob.Executor
{
    public class UploadExecutor : ICommandExecutor<BlobOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public UploadExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public async Task Execute(BlobOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.Container) && !string.IsNullOrEmpty(opts.UploadFile))
            {
                var selectedConnection = LocalStorageManager.GetSelectedConnection();
                await AzureContainerManager.UploadBlob(selectedConnection.ConnectionString, opts.Container, opts.UploadFile, opts.UploadPath);
            }
        }
    }
}
