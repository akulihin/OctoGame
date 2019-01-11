using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OctoGame.DiscordFramework.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AutoAddSingleton(this IServiceCollection services)
        {
            var singletonServicesCount = 0;


            var watchSingleton = Stopwatch.StartNew();     
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IServiceSingleton).IsAssignableFrom(x) && !x.IsInterface))
            {
                singletonServicesCount++;
                services.AddSingleton(type);
  

                // type.GetInterfaces().FirstOrDefault(x => !(x is typeof(IServiceSingleton)))
                //x != typeof(IService)
            }
            watchSingleton.Stop();

            var log = new LoginFromConsole();
            log.Info($"Singleton added count: {singletonServicesCount} ({watchSingleton.Elapsed:m\\:ss\\.ffff}s.)");
            return services;
        }

        public static IServiceCollection AutoAddTransient(this IServiceCollection services)
        {
            var transientServicesCount = 0;
          
            var watchTransient = Stopwatch.StartNew();
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IServiceTransient).IsAssignableFrom(x) && !x.IsInterface))
            {
                transientServicesCount++;
                services.AddTransient(type);
                

            }
            watchTransient.Stop();

            var log = new LoginFromConsole();     
            log.Info($"Transient added count: {transientServicesCount} ({watchTransient.Elapsed:m\\:ss\\.ffff}s.)");
            return services;

        }


        public static async Task InitializeServicesAsync(this IServiceProvider services)
        {
            var singletonServicesCount = 0;
            var transientServicesCount = 0;

            var watchSingleton = Stopwatch.StartNew();
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IServiceSingleton).IsAssignableFrom(x) && !x.IsInterface))
            {
                singletonServicesCount++;
                await ((IServiceSingleton) services.GetRequiredService(type)).InitializeAsync();
            }
            watchSingleton.Stop();


            var watchTransient = Stopwatch.StartNew();
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IServiceTransient).IsAssignableFrom(x) && !x.IsInterface))
            {
                transientServicesCount++;
                await ((IServiceTransient)services.GetRequiredService(type)).InitializeAsync();
            }
            watchTransient.Stop();

            var log = new LoginFromConsole();

            log.Info($"\nSingleton Initialized count: {singletonServicesCount} ({watchSingleton.Elapsed:m\\:ss\\.ffff}s.)");
            log.Info($"Transient Initialized count: {transientServicesCount} ({watchTransient.Elapsed:m\\:ss\\.ffff}s.)\n");
        }
    }
}