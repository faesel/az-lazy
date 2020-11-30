using az_lazy.Commands;
using az_lazy.Commands.AddConnection;
using az_lazy.Commands.AddContainer;
using az_lazy.Commands.AddQueue;
using az_lazy.Commands.Blob;
using az_lazy.Commands.Connection;
using az_lazy.Commands.Container.Executor;
using az_lazy.Commands.Queue;
using az_lazy.Manager;
using az_lazy.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace az_lazy.test
{
    public class LocalStorageFixture
    {
        private readonly ServiceProvider ServiceProvider;
        public ILocalStorageManager LocalStorageManager;
        public IAzureQueueManager AzureQueueManager;
        public IAzureConnectionManager AzureConnectionManager;
        public IAzureContainerManager AzureContainerManager;

        //Runners
        public ICommandRunner<ConnectionOptions> ConnectionRunner;
        public ICommandRunner<AddConnectionOptions> AddConnectionRunner;
        public ICommandRunner<AddQueueOptions> AddQueueRunner;
        public ICommandRunner<QueueOptions> QueueRunner;
        public ICommandRunner<ContainerOptions> ContainerRunner;
        public ICommandRunner<AddContainerOptions> AddContainerRunner;
        public ICommandRunner<BlobOptions> BlobRunner;

        public LocalStorageFixture()
        {
            ServiceProvider = new ServiceCollection()
                .AddProjectDependencies()
                .BuildServiceProvider();

            //Add development connection to connect to azure storage emulator
            LocalStorageManager = ServiceProvider.GetService<ILocalStorageManager>();
            LocalStorageManager.AddDevelopmentConnection();

            AzureQueueManager = ServiceProvider.GetService<IAzureQueueManager>();
            AzureConnectionManager = ServiceProvider.GetService<IAzureConnectionManager>();
            AzureContainerManager = ServiceProvider.GetService<IAzureContainerManager>();

            AddConnectionRunner = ServiceProvider.GetService<ICommandRunner<AddConnectionOptions>>();
            ConnectionRunner = ServiceProvider.GetService<ICommandRunner<ConnectionOptions>>();
            AddQueueRunner = ServiceProvider.GetService<ICommandRunner<AddQueueOptions>>();
            QueueRunner = ServiceProvider.GetService<ICommandRunner<QueueOptions>>();
            ContainerRunner = ServiceProvider.GetService<ICommandRunner<ContainerOptions>>();
            AddContainerRunner = ServiceProvider.GetService<ICommandRunner<AddContainerOptions>>();
            BlobRunner = ServiceProvider.GetService<ICommandRunner<BlobOptions>>();
        }
    }
}