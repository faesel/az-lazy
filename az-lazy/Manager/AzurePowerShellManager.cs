using System.Threading.Tasks;
using System.Management.Automation;
using System;

namespace az_lazy.Manager
{
    public interface IAzurePowerShellManager
    {
        Task GetStorageAccountNames();
    }

    public class AzurePowerShellManager : IAzurePowerShellManager
    {
        public async Task GetStorageAccountNames()
        {
            try
            {
                    

                foreach (PSObject result in ps.Invoke())
                {
                    Console.WriteLine(result);
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}