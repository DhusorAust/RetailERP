using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            //for set async:false in globally
        //            webBuilder.ConfigureServices(services =>
        //            {
        //                services.Configure<HttpClientFactoryOptions>(options =>
        //                {
        //                    options.HttpClientActions.Add(client =>
        //                    {
        //                        client.DefaultRequestVersion = new Version(2, 0);
        //                    });
        //                });
        //            });

        //            webBuilder.UseStartup<Startup>();
        //        });
    }
}
