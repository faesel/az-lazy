using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using az_lazy.Startup;
using System;

namespace az_lazy
{
    class Program
    {
        private static async Task Main(string[] args)
        {
             var serviceProvider = new ServiceCollection()
                .AddProjectDependencies()
                .BuildServiceProvider();

             var azRunner = serviceProvider.GetService<IAzRunner>();

             await azRunner.Startup(args).ConfigureAwait(false);
        }
    }
}
