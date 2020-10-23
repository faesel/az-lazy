using az_lazy.Commands;
using az_lazy.Commands.Connection;
using az_lazy.Commands.Queue;
using az_lazy.Manager;
using Microsoft.Extensions.DependencyInjection;

namespace az_lazy.Startup
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectDependencies(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAzRunner, AzRunner>();

            //Command Runners
            serviceCollection.AddSingleton<IConnectionRunner<ConnectionOptions>, ConnectionRunner>();
            serviceCollection.AddSingleton<IConnectionRunner<AddConnectionOptions>, AddConnectionRunner>();
            serviceCollection.AddSingleton<IConnectionRunner<QueueOptions>, QueueRunner>();
            serviceCollection.AddSingleton<IConnectionRunner<AddQueueOptions>, AddQueueRunner>();

            //Managers
            serviceCollection.AddSingleton<ILocalStorageManager, LocalStorageManager>();
            serviceCollection.AddSingleton<IAzureStorageManager, AzureStorageManager>();

            return serviceCollection;
        }
    }
}