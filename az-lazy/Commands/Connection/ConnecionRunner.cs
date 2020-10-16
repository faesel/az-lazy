using System;
using System.Threading.Tasks;
using az_lazy.Manager;

namespace az_lazy.Commands
{
    public class ConnectionRunner : IConnectionRunner<ConnectionOptions>
    {
        public readonly ILocalStorageManager localStorageManager;

        public ConnectionRunner(
            ILocalStorageManager localStorageManager)
        {
            this.localStorageManager = localStorageManager;
        }

        public async Task<bool> Run(ConnectionOptions opts)
        {
            if (opts.List)
            {
                foreach (var connection in localStorageManager.GetConnections())
                {
                    Console.WriteLine($"{connection.ConnectionName} {(connection.IsSelected ? "[*]" : string.Empty)} - Added on {connection.DateAdded.ToShortDateString()}");
                }
            }

            if (!string.IsNullOrEmpty(opts.ConnectionString) && !string.IsNullOrEmpty(opts.ConnectionName))
            {
                localStorageManager.AddConnection(opts.ConnectionName, opts.ConnectionString);
                Console.WriteLine($"{opts.ConnectionName} Connection Added");
            }

            if (!string.IsNullOrEmpty(opts.RemoveConnection))
            {
                localStorageManager.RemoveConnection(opts.RemoveConnection);
                Console.WriteLine($"{opts.RemoveConnection} Connection removed");
            }

            if (!string.IsNullOrEmpty(opts.SelectConnection))
            {
                localStorageManager.SelectConnection(opts.SelectConnection);
                Console.WriteLine($"{opts.SelectConnection} Connection selected");
            }

            return true;
        }
    }
}