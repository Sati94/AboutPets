using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopApiTest.IntegrationTest
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<Program> where TProgram : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var appDb = scopedServices.GetRequiredService<WebShopContext>();
                    var userManager = scopedServices.GetRequiredService<UserManager<IdentityUser>>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

                    try
                    {
                        var options = new DbContextOptionsBuilder<WebShopContext>()
                           .UseInMemoryDatabase("InMemoryWebShopContext")
                           .Options;

                        services.Remove(services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<WebShopContext>)));
                        
                        services.TryAddSingleton(options);
                        if(appDb != null)
                        {
                                var seedData = new SeedData(appDb, userManager);

                                seedData.PopulateTestData(appDb, userManager);
                        }
                        else
                        {
                            Console.WriteLine($"Error:{appDb.Products}");
                        }

                       
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}
       