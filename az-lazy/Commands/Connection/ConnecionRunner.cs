using System;
using System.Threading.Tasks;

namespace az_lazy.Commands
{
    public class ConnectionRunner : IConnectionRunner<ConnectionOptions>
    {
        public async Task<bool> Run(ConnectionOptions opts)
        {
            if(opts.List)
            {
                Console.WriteLine("ConnectionOne [*]");
                Console.WriteLine("ConnectionTwo");
            }

            if(!string.IsNullOrEmpty(opts.ConnectionString))
            {
                Console.WriteLine("Connection added");
            }

            if(!string.IsNullOrEmpty(opts.RemoveConnection))
            {
                Console.WriteLine("Connection removed");
            }

            if(!string.IsNullOrEmpty(opts.SelectConnection))
            {
                Console.WriteLine("Connection selected");
            }

            return true;
        }
    }
}