using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using BuffteksWebsite.Models;

namespace BuffteksWebsite
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //the seeding routine is from here: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app-xplat/working-with-sql?view=aspnetcore-2.0
            //this should change in 2.1
            var host = BuildWebHost(args);

            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    DataSeeder.Initialize(services);
                }
                catch(Exception exp)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exp, "An error occurred seeding the Database");

                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
