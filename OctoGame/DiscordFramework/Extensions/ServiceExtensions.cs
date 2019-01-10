using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OctoGame.DiscordFramework.Language;

namespace OctoGame.DiscordFramework.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AutoAddServices(this IServiceCollection services)
        {
            var singleCont = 0;
            var trasCont = 0;

            var watchSingl = Stopwatch.StartNew();     
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IServiceSingleton).IsAssignableFrom(x) && !x.IsInterface))
            {
                singleCont++;
                services.AddSingleton(type);
  

                // type.GetInterfaces().FirstOrDefault(x => !(x is typeof(IServiceSingleton)))
            }
            watchSingl.Stop();
            
          
            var watchTrans = Stopwatch.StartNew();
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IServiceTransient).IsAssignableFrom(x) && !x.IsInterface))
            {
                trasCont++;
                services.AddTransient(type);
                

            }
            watchTrans.Stop();

            Console.WriteLine($"Singleton added count: {singleCont} ({watchSingl.Elapsed:m\\:ss\\.ffff}s.)\n" +
                              $"Transient added count: {trasCont} ({watchTrans.Elapsed:m\\:ss\\.ffff}s.)");
            return services;
        }

        public static async Task InitializeServicesAsync(this IServiceProvider services)
        {
            var singleCont = 0;
            var trasCont = 0;

            var watchSingl = Stopwatch.StartNew();
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IServiceSingleton).IsAssignableFrom(x) && !x.IsInterface))
            {
                singleCont++;
                await ((IServiceSingleton) services.GetRequiredService(type)).InitializeAsync();
            }
            watchSingl.Stop();


            var watchTrans = Stopwatch.StartNew();
            foreach (var type in Assembly.GetEntryAssembly().GetTypes()
                .Where(x => typeof(IServiceTransient).IsAssignableFrom(x) && !x.IsInterface))
            {
                trasCont++;
                await ((IServiceTransient)services.GetRequiredService(type)).InitializeAsync();
            }
            watchTrans.Stop();

            Console.WriteLine($"\nSingleton Initialized count: {singleCont} ({watchSingl.Elapsed:m\\:ss\\.ffff}s.)\n" +
                              $"Transient Initialized count: {trasCont} ({watchTrans.Elapsed:m\\:ss\\.ffff}s.)\n");
        }
    }
}