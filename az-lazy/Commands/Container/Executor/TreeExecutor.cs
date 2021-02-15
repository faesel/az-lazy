using System;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

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
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Getting blobs in container {opts.Tree} ...", async _ =>
                    {
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
                                AnsiConsole.MarkupLine($"Getting blobs in container {opts.Tree} ... [bold green]Successful[/]");
                                
                            }


                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Getting blobs in container {opts.Tree} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });



                var tree = new Tree("Root")
                    .Style("white on red");

                var foo = tree.AddNode("[yellow]Foo[/]");
                var table = foo.AddNode("[yellow]GREjhR[/]");





                AnsiConsole.Render(tree);

                // string message = $"Getting blobs in container {opts.Tree}";
                // ConsoleHelper.WriteInfoWaiting(message, true);

                // try
                // {
                //     var selectedConnection = LocalStorageManager.GetSelectedConnection();
                //     var treeNodes = await AzureContainerManager.ContainerTree(
                //         selectedConnection.ConnectionString,
                //         opts.Tree,
                //         opts.Depth,
                //         opts.Detailed,
                //         opts.Prefix);

                //     if(treeNodes.Count > 0)
                //     {
                //         ConsoleHelper.WriteLineSuccessWaiting(message);
                //         ConsoleHelper.WriteSepparator();
                //     }

                //     new Tree(treeNodes);
                // }
                // catch(ContainerException ex)
                // {
                //     ConsoleHelper.WriteLineFailedWaiting(message);
                //     ConsoleHelper.WriteLineError(ex.Message);
                // }
            }
        }
    }
}