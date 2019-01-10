using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OctoGame.DiscordFramework.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AutoAddServices(this IServiceCollection services)
        {
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IService).IsAssignableFrom(x) && !x.IsInterface))
            {
                services.AddSingleton(type);
                // type.GetInterfaces().FirstOrDefault(x => !(x is typeof(IService)))
            }
               
      
            return services;
        }

        public static async Task InitializeServicesAsync(this IServiceProvider services)
        {
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IService).IsAssignableFrom(x) && !x.IsInterface))
            {
                await ((IService) services.GetRequiredService(type)).InitializeAsync();
            }
        }
    }
}