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
                var isSuccessfull = LocalStorageManager.RemoveConnection(opts.RemoveConnection);

                var message = isSuccessfull ? $"{opts.RemoveConnection} Connection removed" :
                    $"{opts.RemoveConnection} Connection not found!";

                Console.WriteLine(message);

                return isSuccessfull;
            }

            if (!string.IsNullOrEmpty(opts.SelectConnection))
            {
                var isSuccessfull = LocalStorageManager.SelectConnection(opts.SelectConnection);

                var message = isSuccessfull ? $"{opts.SelectConnection} Connection selected" :
                    $"{opts.SelectConnection} Connection not found!";

                Console.WriteLine(message);

                return isSuccessfull;
            }

            return true;
        }
    }
}