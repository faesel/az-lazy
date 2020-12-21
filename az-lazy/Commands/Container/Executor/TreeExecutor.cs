using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Container.Executor
{
    public class TreeExecutor : ICommandExecutor<ContainerOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public TreeExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public async Task Execute(ContainerOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Tree))
            {
                string message = $"Getting blobs in container {opts.Tree}";
                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var treeNodes = await AzureContainerManager.ContainerTree(
                        selectedConnection.ConnectionString,
                        opts.Tree,
                        opts.Depth,
                        opts.Detailed,
                        opts.Prefix);

                    if(treeNodes.Count > 0)
                    {
                        ConsoleHelper.WriteLineSuccessWaiting(message);
                        ConsoleHelper.WriteSepparator();
                    }

                    new Tree(treeNodes);
                }
                catch(ContainerException ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }
        }
    }
}