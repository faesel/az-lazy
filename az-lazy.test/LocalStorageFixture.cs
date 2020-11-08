using az_lazy.Commands;
using az_lazy.Commands.AddConnection;
using az_lazy.Commands.AddContainer;
using az_lazy.Commands.AddQueue;
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
        public IAzureStorageManager AzureStorageManager;

        //Runners
        public IConnectionRunner<ConnectionOptions> ConnectionRunner;
        public IConnectionRunner<AddConnectionOptions> AddConnectionRunner;
        public IConnectionRunner<AddQueueOptions> AddQueueRunner;
        public IConnectionRunner<QueueOptions> QueueRunner;
        public IConnectionRunner<ContainerOptions> ContainerRunner;
        public IConnectionRunner<AddContainerOptions> AddContainerRunner;

        public LocalStorageFixture()
        {
            ServiceProvider = new ServiceCollection()
                .AddProjectDependencies()
                .BuildServiceProvider();

            //Add development connection to connect to azure storage emulator
            LocalStorageManager = ServiceProvider.GetService<ILocalStorageManager>();
            LocalStorageManager.AddDevelopmentConnection();

            AzureStorageManager = ServiceProvider.GetService<IAzureStorageManager>();

            AddConnectionRunner = ServiceProvider.GetService<IConnectionRunner<AddConnectionOptions>>();
            ConnectionRunner = ServiceProvider.GetService<IConnectionRunner<ConnectionOptions>>();
            AddQueueRunner = ServiceProvider.GetService<IConnectionRunner<AddQueueOptions>>();
            QueueRunner = ServiceProvider.GetService<IConnectionRunner<QueueOptions>>();
            ContainerRunner = ServiceProvider.GetService<IConnectionRunner<ContainerOptions>>();
            AddContainerRunner = ServiceProvider.GetService<IConnectionRunner<AddContainerOptions>>();
        }
    }
}