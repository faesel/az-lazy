using System;
using System.Threading.Tasks;

namespace az_lazy.Commands.Queue
{
    public class QueueRunner : IConnectionRunner<QueueOptions>
    {
        public async Task<bool> Run(QueueOptions opts)
        {
            if(opts.List)
            {
                Console.WriteLine("ConnectionOne [*]");
                Console.WriteLine("ConnectionTwo");
            }

            return true;
        }
    }
}