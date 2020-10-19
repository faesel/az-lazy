using System;
using System.Threading.Tasks;
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
                    Console.WriteLine($"{connection.ConnectionName} {(connection.IsSelected ? "[*]" : string.Empty)} - Added on {connection.DateAdded.ToShortDateString()}");
                }
            }

            if (!string.IsNullOrEmpty(opts.RemoveConnection))
            {
                LocalStorageManager.RemoveConnection(opts.RemoveConnection);
                Console.WriteLine($"{opts.RemoveConnection} Connection removed");
            }

            if (!string.IsNullOrEmpty(opts.SelectConnection))
            {
                LocalStorageManager.SelectConnection(opts.SelectConnection);
                Console.WriteLine($"{opts.SelectConnection} Connection selected");
            }

            return true;
        }
    }
}