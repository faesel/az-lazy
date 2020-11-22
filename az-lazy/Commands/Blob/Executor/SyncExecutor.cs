using System.IO;
using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;
using System;
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
                    await AzureContainerManager.UploadBlobFromFolder(selectedConnection.ConnectionString, opts.Container, opts.Directory, opts.UploadPath).ConfigureAwait(false);

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