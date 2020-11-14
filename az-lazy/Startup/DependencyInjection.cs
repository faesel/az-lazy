using az_lazy.Commands;
using az_lazy.Commands.Connection;
using az_lazy.Commands.Queue;
using az_lazy.Commands.AddQueue;
using az_lazy.Manager;
using Microsoft.Extensions.DependencyInjection;
using az_lazy.Commands.AddConnection;
using az_lazy.Commands.Connection.Executor;
using az_lazy.Commands.Queue.Executor;
using az_lazy.Commands.Container.Executor;
using az_lazy.Commands.Blob;
using az_lazy.Commands.AddContainer;

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
            serviceCollection.AddSingleton<IConnectionRunner<ContainerOptions>, ContainerRunner>();
            serviceCollection.AddSingleton<IConnectionRunner<AddContainerOptions>, AddContainerRunner>();
            serviceCollection.AddSingleton<IConnectionRunner<BlobOptions>, BlobRunner>();

            //Executors
            serviceCollection.AddSingleton<ICommandExecutor<ConnectionOptions>, Commands.Connection.Executor.ListExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<ConnectionOptions>, RemoveConnectionExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<ConnectionOptions>, SelectConnectionExecutor>();

            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, Commands.Queue.Executor.ListExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, AddMessageExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, ClearQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, CureQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, PeekQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, RemoveQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, WatchQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, MoveQueueExecutor>();

            serviceCollection.AddSingleton<ICommandExecutor<ContainerOptions>, Commands.Container.Executor.ListExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<ContainerOptions>, RemoveExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<ContainerOptions>, TreeExecutor>();

            serviceCollection.AddSingleton<ICommandExecutor<BlobOptions>, Commands.Blob.Executor.RemoveExecutor>();

            //Managers
            serviceCollection.AddSingleton<ILocalStorageManager, LocalStorageManager>();
            serviceCollection.AddSingleton<IAzureQueueManager, AzureQueueManager>();
            serviceCollection.AddSingleton<IAzureContainerManager, AzureContainerManager>();
            serviceCollection.AddSingleton<IAzureConnectionManager, AzureConnectionManager>();

            return serviceCollection;
        }
    }
}