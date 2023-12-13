using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopApiTest.IntegrationTest
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
               .ConfigureWebHostDefaults(webBuilder =>
               {
                    webBuilder.UseStartup<TProgram>();
               })
               .ConfigureServices(services =>
               {
               foreach (var service in services)
               {
                     Console.WriteLine($"Registered service: {service.ServiceType}");
               }

               var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<WebShopContext>));
                    if (descriptor != null)
                    {
                       services.Remove(descriptor);
                    }

               var existingOrderService = services.SingleOrDefault(d => d.ServiceType == typeof(IOrderService));
                    if (existingOrderService == null)
                    {
                        services.AddOrderService(); 
                    }
               var existingOrderItemService = services.SingleOrDefault(d => d.ServiceType == typeof(IOrderItemService));
                   if (existingOrderItemService == null)
                   {
                       services.AddOrderItemService();
                   }       
               var existingUserService = services.SingleOrDefault(d => d.ServiceType == typeof(IUserService));
                   if (existingUserService == null)
                   {
                       services.AddUserService();
                   }
               var existingProductService = services.SingleOrDefault(d => d.ServiceType == typeof(IProductService));
                   if (existingProductService == null)
                   {
                       services.AddProductService();
                   }        
               var existingUserProfileService = services.SingleOrDefault(d => d.ServiceType == typeof(IUserProfileService));
                   if (existingUserProfileService == null)
                   {
                       services.AddUserProfileService();
                   }        
               });

            return builder;
        }
    }

        public static class ServiceCollectionExtensions
        {
            public static void AddOrderService(this IServiceCollection services)
            {
                // Itt add hozzá a SomeServiceType-t a szolgáltatásokhoz
                services.AddSingleton<IOrderService, OrderService>();
            }
            public static void AddOrderItemService(this IServiceCollection services)
            {
            // Itt add hozzá a SomeServiceType-t a szolgáltatásokhoz
                services.AddSingleton<IOrderItemService, OrderItemService>();
            }
            public static void AddUserService(this IServiceCollection services)
            {
            // Itt add hozzá a SomeServiceType-t a szolgáltatásokhoz
                services.AddSingleton<IUserService, UserService>();
            }
            public static void AddProductService(this IServiceCollection services)
            {
            // Itt add hozzá a SomeServiceType-t a szolgáltatásokhoz
               services.AddSingleton<IProductService, ProductService>();
            }
            public static void AddUserProfileService(this IServiceCollection services)
            {
            // Itt add hozzá a SomeServiceType-t a szolgáltatásokhoz
               services.AddSingleton<IUserProfileService, UserProfileService>();
            }
    }
}
