using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Container.Executor
{
    public class RemoveExecutor : ICommandExecutor<ContainerOptions>
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

        public async Task Execute(ContainerOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.RemoveContainer))
            {
                string message = $"Removing container {opts.RemoveContainer}";
                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    await AzureContainerManager.RemoveContainer(selectedConnection.ConnectionString, opts.RemoveContainer);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineNormal($"Finished removing container {opts.RemoveContainer}");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }
        }
    }
}