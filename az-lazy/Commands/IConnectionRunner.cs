using System.Threading.Tasks;

namespace az_lazy.Commands
{
    public interface ICommandRunner<T> where T : ICommandOptions
    {
        Task<bool> Run(T options);
    }
}