using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Data;

namespace WebShopApiTest.IntegrationTest
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<Program> where TProgram : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
           base.ConfigureWebHost(builder);

           builder.ConfigureTestServices(services =>
           {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<WebShopContext>));

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

             
                services.AddDbContext<WebShopContext>((container, options) =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
                builder.UseEnvironment("Development");

           });
        }
    }
}
       