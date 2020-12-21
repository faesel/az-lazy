using System.Collections.Generic;
using System.Threading.Tasks;

namespace az_lazy.Commands.Table
{
    public class TableRunner : ICommandRunner<TableOptions>
    {
        public readonly IEnumerable<ICommandExecutor<TableOptions>> CommandExecutors;

        public TableRunner(
            IEnumerable<ICommandExecutor<TableOptions>> commandExecutors)
        {
            this.CommandExecutors = commandExecutors;
        }

        public async Task<bool> Run(TableOptions opts)
        {
            foreach(var executor in CommandExecutors)
            {
                await executor.Execute(opts);
            }

            return true;
        }
    }
}