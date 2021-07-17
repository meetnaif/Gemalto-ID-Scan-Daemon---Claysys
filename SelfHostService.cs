using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.SelfHost;

namespace Gemalto_ID_Scan_Daemon___Claysys
{
    partial class SelfHostService : ServiceBase
    {
        public SelfHostService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var GemaltoServiceURL = ConfigurationManager.AppSettings.Get("GemaltoService_URL");
            var config = new HttpSelfHostConfiguration(GemaltoServiceURL);
            config.EnableCors(new EnableCorsAttribute("*", headers: "*", methods: "*"));
            config.Routes.MapHttpRoute(
               name: "API",
               routeTemplate: "{controller}/{action}/",
               defaults: new { id = RouteParameter.Optional }
           );

            HttpSelfHostServer server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
