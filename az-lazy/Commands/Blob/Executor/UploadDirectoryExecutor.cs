using az_lazy.Helpers;
using az_lazy.Manager;
using System;
using System.Threading.Tasks;

namespace az_lazy.Commands.Blob.Executor
{
    public class UploadDirectoryExecutor : ICommandExecutor<BlobOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public UploadDirectoryExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public async Task Execute(BlobOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Directory))
            {
                var message = $"Syncing directory {opts.Directory}";
                ConsoleHelper.WriteLineInfo(message);
                ConsoleHelper.WriteSepparator();

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureContainerManager.UploadBlobFromFolder(selectedConnection.ConnectionString, opts.Container, opts.Directory, opts.UploadPath);

                    Console.WriteLine(string.Empty);
                    ConsoleHelper.WriteLineSuccessWaiting(message);
                }
                catch(Exception ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting("Unable to sync directory");
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }
        }
    }
}