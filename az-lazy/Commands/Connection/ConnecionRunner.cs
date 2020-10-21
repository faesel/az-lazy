using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands
{
    public class ConnectionRunner : IConnectionRunner<ConnectionOptions>
    {
        public readonly ILocalStorageManager LocalStorageManager;

        public ConnectionRunner(
            ILocalStorageManager localStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
        }

        public async Task<bool> Run(ConnectionOptions opts)
        {
            if (opts.List)
            {
                foreach (var connection in LocalStorageManager.GetConnections())
                {
                    var connectionName = $"{connection.ConnectionName} {(connection.IsSelected ? "[*]" : string.Empty)}";
                    var addedOn = $"Added on {connection.DateAdded.ToShortDateString()}";

                    ConsoleHelper.WriteLineAdditionalInfo(connectionName, addedOn);
                }
            }

            if (!string.IsNullOrEmpty(opts.RemoveConnection))
            {
                var removeMessage = $"Removing connection {opts.RemoveConnection}";
                ConsoleHelper.WriteInfoWaiting(removeMessage, true);

                var isSuccessfull = LocalStorageManager.RemoveConnection(opts.RemoveConnection);

                if(isSuccessfull)
                {
                    ConsoleHelper.WriteLineSuccessWaiting(removeMessage);
                    ConsoleHelper.WriteLineNormal($"Finished removing connection {opts.RemoveConnection}");
                }
                else
                {
                    ConsoleHelper.WriteLineFailedWaiting(removeMessage);
                    ConsoleHelper.WriteLineError("Check the connection name exists and try again");
                }

                return isSuccessfull;
            }

            if (!string.IsNullOrEmpty(opts.SelectConnection))
            {
                var selectMessage = $"Selecting connection {opts.SelectConnection}";
                ConsoleHelper.WriteInfoWaiting(selectMessage, true);

                var isSuccessfull = LocalStorageManager.SelectConnection(opts.SelectConnection);

                if(isSuccessfull)
                {
                    ConsoleHelper.WriteLineSuccessWaiting(selectMessage);
                    ConsoleHelper.WriteLineNormal($"Connection {opts.RemoveConnection} is ready to use!");
                }
                else
                {
                    ConsoleHelper.WriteLineFailedWaiting(selectMessage);
                    ConsoleHelper.WriteLineError("Check the connection name exists and try again");
                }

                return isSuccessfull;
            }

            return true;
        }
    }
}