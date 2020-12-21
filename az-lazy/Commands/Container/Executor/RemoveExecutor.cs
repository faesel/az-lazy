using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;

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
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Removing container {opts.RemoveContainer}", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureContainerManager.RemoveContainer(selectedConnection.ConnectionString, opts.RemoveContainer);

                            AnsiConsole.MarkupLine($"Removing container {opts.RemoveContainer} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished removing container {opts.RemoveContainer}");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Removing container {opts.RemoveContainer} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}