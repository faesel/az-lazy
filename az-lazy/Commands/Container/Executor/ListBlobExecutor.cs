using System;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Container.Executor
{
    public class ListExecutor: ICommandExecutor<ContainerOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureStorageManager AzureStorageManager;

        public ListExecutor(
            ILocalStorageManager localStorageManager,
            IAzureStorageManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(ContainerOptions opts)
        {
            if(opts.List)
            {
                const string message = "Fetching containers";

                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var containers = await AzureStorageManager.GetContainers(selectedConnection).ConfigureAwait(false);

                    if(containers.Count > 0)
                    {
                        ConsoleHelper.WriteLineSuccessWaiting(message);
                        ConsoleHelper.WriteSepparator();
                    }

                    foreach (var container in containers)
                    {
                        var isPublic = container.Properties.PublicAccess.HasValue ? "(public)" : "(private)";
                        var lastModified = container.Properties.LastModified.DateTime.ToShortDateString();

                        ConsoleHelper.WriteLineNormal(container.Name, $"{isPublic} {lastModified}");
                    }
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