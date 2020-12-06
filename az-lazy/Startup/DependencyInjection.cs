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
using az_lazy.Commands.Blob.Executor;
using az_lazy.Commands.Table;

namespace az_lazy.Startup
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectDependencies(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAzRunner, AzRunner>();

            //Command Runners
            serviceCollection.AddSingleton<ICommandRunner<ConnectionOptions>, ConnectionRunner>();
            serviceCollection.AddSingleton<ICommandRunner<AddConnectionOptions>, AddConnectionRunner>();
            serviceCollection.AddSingleton<ICommandRunner<QueueOptions>, QueueRunner>();
            serviceCollection.AddSingleton<ICommandRunner<AddQueueOptions>, AddQueueRunner>();
            serviceCollection.AddSingleton<ICommandRunner<ContainerOptions>, ContainerRunner>();
            serviceCollection.AddSingleton<ICommandRunner<AddContainerOptions>, AddContainerRunner>();
            serviceCollection.AddSingleton<ICommandRunner<BlobOptions>, BlobRunner>();
            serviceCollection.AddSingleton<ICommandRunner<TableOptions>, TableRunner>();

            //Executors
            serviceCollection.AddSingleton<ICommandExecutor<ConnectionOptions>, Commands.Connection.Executor.ListExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<ConnectionOptions>, RemoveConnectionExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<ConnectionOptions>, SelectConnectionExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<ConnectionOptions>, WipeExecutor>();

            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, Commands.Queue.Executor.ListExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, AddMessageExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, ClearQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, CureQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, PeekQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, RemoveQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, WatchQueueExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<QueueOptions>, MoveQueueExecutor>();

            serviceCollection.AddSingleton<ICommandExecutor<ContainerOptions>, Commands.Container.Executor.ListExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<ContainerOptions>, Commands.Container.Executor.RemoveExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<ContainerOptions>, TreeExecutor>();

            serviceCollection.AddSingleton<ICommandExecutor<BlobOptions>, Commands.Blob.Executor.RemoveExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<BlobOptions>, UploadExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<BlobOptions>, UploadDirectoryExecutor>();

            serviceCollection.AddSingleton<ICommandExecutor<TableOptions>, Commands.Table.Executor.ListExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<TableOptions>, Commands.Table.Executor.SampleExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<TableOptions>, Commands.Table.Executor.QueryExecutor>();
            serviceCollection.AddSingleton<ICommandExecutor<TableOptions>, Commands.Table.Executor.DeleteExecutor>();

            //Managers
            serviceCollection.AddSingleton<ILocalStorageManager, LocalStorageManager>();
            serviceCollection.AddSingleton<IAzureQueueManager, AzureQueueManager>();
            serviceCollection.AddSingleton<IAzureContainerManager, AzureContainerManager>();
            serviceCollection.AddSingleton<IAzureConnectionManager, AzureConnectionManager>();
            serviceCollection.AddSingleton<IAzureTableManager, AzureTableManager>();

            return serviceCollection;
        }
    }
}