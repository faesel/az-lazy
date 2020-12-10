using az_lazy.Commands.AddTable;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace az_lazy.Commands.Table
{
    public class AddTableRunner : ICommandRunner<AddTableOptions>
    {
        public AddTableRunner()
        {
        }

        public async Task<bool> Run(AddTableOptions opts)
        {

            return true;
        }
    }
}