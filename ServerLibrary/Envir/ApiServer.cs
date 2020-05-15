using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server.Envir
{

    public class ApiServer
    {
        private static Lazy<ApiServer> _lazyInstance = new Lazy<ApiServer>();
        public static ApiServer Instance => _lazyInstance.Value;

        private IWebHost _host;
        private Thread _thread;

        public void Start()
        {
            _thread = new Thread(StartServerCallback);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop()
        {
            _thread?.Abort();
            _thread = null;
            _host = null;
        }

        private void StartServerCallback()
        {
            _host = WebHost.CreateDefaultBuilder()
             .UseStartup<ApiServerStartup>()
             .UseUrls(Config.ApiServerHttpUrl, Config.ApiServerHttpsUrl)
             .ConfigureLogging((context, logging) =>
             {
                 logging.ClearProviders();
                 logging.AddProvider(new ApiServerLoggerProvider());
             })
             .Build();

            _host.Start();
        }
    }
}
