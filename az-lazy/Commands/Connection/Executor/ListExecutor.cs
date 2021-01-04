using System;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Connection.Executor
{
    public class ListExecutor : ICommandExecutor<ConnectionOptions>
    {
        public readonly ILocalStorageManager LocalStorageManager;

        public ListExecutor(
            ILocalStorageManager localStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
        }

        public Task Execute(ConnectionOptions opts)
        {
            if(opts.List)
            {
                AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync("Getting connections ... ", _ =>
                    {
                        try
                        {
                            var connections =  LocalStorageManager.GetConnections();

                            if(connections.Count > 0)
                            {
                                AnsiConsole.MarkupLine("Getting connections ... [bold green]Successful[/]");

                                foreach (var connection in connections)
                                {
                                    var connectionName = connection.ConnectionName + (connection.IsSelected ? " [[*]]" : string.Empty);
                                    var addedOn = $"Added on {connection.DateAdded.ToShortDateString()}";

                                    AnsiConsole.MarkupLine($"{connectionName} - [grey]{addedOn.EscapeMarkup()}[/]");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine("Getting connections ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }

                        return Task.CompletedTask;
                    });
            }

            return Task.CompletedTask;
        }
    }
}