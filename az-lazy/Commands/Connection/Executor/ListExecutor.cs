using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

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

        public Task Execute(ConnectionOptions options)
        {
            foreach (var connection in LocalStorageManager.GetConnections())
            {
                var connectionName = $"{connection.ConnectionName} {(connection.IsSelected ? "[*]" : string.Empty)}";
                var addedOn = $"Added on {connection.DateAdded.ToShortDateString()}";

                ConsoleHelper.WriteLineAdditionalInfo(connectionName, addedOn);
            }

            return Task.CompletedTask;
        }
    }
}