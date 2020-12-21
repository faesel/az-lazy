using az_lazy.Exceptions;
using az_lazy.Helpers;
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
                string message = $"Uploading {opts.UploadFile}";
                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureContainerManager.UploadBlob(selectedConnection.ConnectionString, opts.Container, opts.UploadFile, opts.UploadPath);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineNormal($"Finished uploading {opts.UploadFile}");
                }
                catch (ContainerException ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }
        }
    }
}
