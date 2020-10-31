using System.Collections.Generic;
using System.Threading.Tasks;

namespace az_lazy.Commands.Queue
{
    public class QueueRunner : IConnectionRunner<QueueOptions>
    {
        public readonly IEnumerable<ICommandExecutor<QueueOptions>> CommandExecutors;

        public QueueRunner(
            IEnumerable<ICommandExecutor<QueueOptions>> commandExecutors)
        {
            this.CommandExecutors = commandExecutors;
        }

        public Task<bool> Run(QueueOptions opts)
        {
            foreach(var executor in CommandExecutors)
            {
                executor.Execute(opts);
            }

            return Task.FromResult(true);
        }
    }
}