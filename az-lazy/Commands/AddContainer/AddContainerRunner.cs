using System;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;
using Azure.Storage.Blobs.Models;

namespace az_lazy.Commands.AddContainer
{
    public class AddContainerRunner : IConnectionRunner<AddContainerOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureStorageManager AzureStorageManager;

        public AddContainerRunner(
            ILocalStorageManager localStorageManager,
            IAzureStorageManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task<bool> Run(AddContainerOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Name))
            {
                var message = $"Creating container {opts.Name}";
                ConsoleHelper.WriteInfoWaiting(message, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var publicAccessLevel = PublicAccessType.None;

                    if(!string.IsNullOrEmpty(opts.PublicAccess))
                    {
                        Enum.TryParse(opts.PublicAccess, true, out publicAccessLevel);
                    }

                    var uri = await AzureStorageManager.CreateContainer(selectedConnection, publicAccessLevel, opts.Name).ConfigureAwait(false);

                    ConsoleHelper.WriteLineSuccessWaiting(message);
                    ConsoleHelper.WriteLineInfo($"Public Access Level: {publicAccessLevel}");
                    if(!string.IsNullOrEmpty(uri))
                    {
                        ConsoleHelper.WriteLineInfo($"Uri: {uri}");
                    }
                    ConsoleHelper.WriteLineNormal($"Finished creating container {opts.Name}");

                    return true;
                }
                catch(ContainerException ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting(message);
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }

            return false;
        }
    }
}