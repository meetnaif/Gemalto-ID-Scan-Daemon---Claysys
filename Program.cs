using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Gemalto_ID_Scan_Daemon___Claysys
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
           .WriteTo.File(@"C:\Logs\IDScan Log.log", rollingInterval: RollingInterval.Day)
           .CreateLogger();

            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new SelfHostService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
