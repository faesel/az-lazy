using System.Threading.Tasks;
using az_lazy.Manager;

namespace az_lazy.Commands.Connection.Executor
{
    public class ListRemoteExecutor : ICommandExecutor<ConnectionOptions>
    {
        private readonly IAzurePowerShellManager AzurePowerShellManager;

        public ListRemoteExecutor(
            IAzurePowerShellManager azurePowerShellManager)
        {
            this.AzurePowerShellManager = azurePowerShellManager;
        }

        public Task Execute(ConnectionOptions opts)
        {
            AzurePowerShellManager.GetStorageAccountNames();

            return Task.CompletedTask;
        }
    }
}