using System.Threading.Tasks;

namespace az_lazy.Commands
{
    public interface ICommandExecutor<T> where T : ICommandOptions
    {
        Task Execute(T options);
    }
}