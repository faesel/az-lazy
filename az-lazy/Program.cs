using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using az_lazy.Startup;
using System;
using az_lazy.Manager;

namespace az_lazy
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
               .AddProjectDependencies()
               .BuildServiceProvider();

            //Ensure there is always a development connection available
            var localStorageManager = serviceProvider.GetService<ILocalStorageManager>();
            localStorageManager.AddDevelopmentConnection();

            var azRunner = serviceProvider.GetService<IAzRunner>();
            await azRunner.Startup(args).ConfigureAwait(false);
        }
    }
}
