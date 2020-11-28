using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;
using System.Threading.Tasks;

namespace az_lazy.Commands.Blob.Executor
{
    public class RemoveExecutor : ICommandExecutor<BlobOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public RemoveExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public async Task Execute(BlobOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.Container) && !string.IsNullOrEmpty(opts.Remove))
            {
                string message = $"Removing blob {opts.Remove} from container {opts.Container}";
                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureContainerManager.RemoveBlob(selectedConnection.ConnectionString, opts.Container, opts.Remove).ConfigureAwait(false);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineNormal($"Finished removing blob {opts.Remove}");
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
