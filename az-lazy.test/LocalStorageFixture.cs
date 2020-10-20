using az_lazy.Commands;
using az_lazy.Commands.Connection;
using az_lazy.Commands.Queue;
using az_lazy.Manager;
using az_lazy.Startup;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace az_lazy.test
{
    public class LocalStorageFixture
    {
        private ServiceProvider ServiceProvider;
        public ILocalStorageManager LocalStorageManager;

        //Runners
        public IConnectionRunner<ConnectionOptions> ConnectionRunner;
        public IConnectionRunner<AddConnectionOptions> AddConnectionRunner;

        public LocalStorageFixture()
        {
            ServiceProvider = new ServiceCollection()
                .AddProjectDependencies()
                .BuildServiceProvider();

            //Add development connection to connect to azure storage emulator
            LocalStorageManager = ServiceProvider.GetService<ILocalStorageManager>();
            LocalStorageManager.AddDevelopmentConnection(null);

            AddConnectionRunner = ServiceProvider.GetService<IConnectionRunner<AddConnectionOptions>>();
            ConnectionRunner = ServiceProvider.GetService<IConnectionRunner<ConnectionOptions>>();
        }
    }
}