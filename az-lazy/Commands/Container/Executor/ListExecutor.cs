using System;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Container.Executor
{
    public class ListExecutor : ICommandExecutor<ContainerOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public ListExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
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
                    var containers = await AzureContainerManager.GetContainers(selectedConnection.ConnectionString);

                    if(containers.Count > 0)
                    {
                        ConsoleHelper.WriteLineSuccessWaiting(message);
                        ConsoleHelper.WriteSepparator();
                    }

                    if(!string.IsNullOrEmpty(opts.Contains))
                    {
                        containers = containers.Where(x => x.Name.Contains(opts.Contains)).ToList();
                    }

                    foreach (var container in containers)
                    {
                        var containerProperties = container.Properties;

                        var isPublic = containerProperties.PublicAccess.HasValue ? "(public)" : "(private)";
                        var lastModified = containerProperties.LastModified.DateTime.ToShortDateString();

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