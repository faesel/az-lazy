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

        public async Task Execute(BlobOptions options)
        {
            if(!string.IsNullOrEmpty(options.Sync))
            {
                Console.WriteLine("Files:");
                var files = Directory.GetFiles(options.Sync, "*", SearchOption.AllDirectories);

                foreach(var file in files)
                {
                    Console.WriteLine(file.Replace(options.Sync + @"\", string.Empty));
                }
            }
        }
    }
}