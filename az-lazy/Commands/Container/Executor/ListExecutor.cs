using System.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;

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
                // Synchronous
                await AnsiConsole.Status()
                    .StartAsync("Fetching containers ...", async ctx =>
                    {
                        ctx.Spinner(Spinner.Known.Dots);
                        ctx.SpinnerStyle(Style.Parse("green"));

                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            var containers = await AzureContainerManager.GetContainers(selectedConnection.ConnectionString);

                            if(containers.Count > 0)
                            {
                                AnsiConsole.MarkupLine("Fetching containers ... [bold green]Successful[/]");
                                AnsiConsole.Render(new Rule("Containers").LeftAligned());
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

                                AnsiConsole.MarkupLine($"{container.Name} - [grey]{isPublic} {lastModified}[/]");
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine("Fetching containers ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}