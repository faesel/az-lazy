using System;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Manager;
using Azure.Storage.Blobs.Models;
using Spectre.Console;

namespace az_lazy.Commands.AddContainer
{
    public class AddContainerRunner : ICommandRunner<AddContainerOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public AddContainerRunner(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public async Task<bool> Run(AddContainerOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Name))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Creating container {opts.Name}", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            var publicAccessLevel = PublicAccessType.None;

                            if(!string.IsNullOrEmpty(opts.PublicAccess))
                            {
                                Enum.TryParse(opts.PublicAccess, true, out publicAccessLevel);
                            }

                            var uri = await AzureContainerManager.CreateContainer(selectedConnection.ConnectionString, publicAccessLevel, opts.Name);

                            AnsiConsole.MarkupLine($"Creating container {opts.Name} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"[grey]Public Access Level: {publicAccessLevel}[/]");

                            if(!string.IsNullOrEmpty(uri))
                            {
                                AnsiConsole.MarkupLine($"[grey]Uri: {uri}[/]");
                            }

                            AnsiConsole.MarkupLine($"Finished creating container {opts.Name}");
                        }
                        catch(ContainerException ex)
                        {
                            AnsiConsole.MarkupLine($"Creating container {opts.Name} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }

            return true;
        }
    }
}