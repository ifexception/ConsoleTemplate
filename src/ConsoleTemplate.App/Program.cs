namespace ConsoleTemplate.App
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using ConsoleTemplate.App.Config;
    using ConsoleTemplate.App.Logger;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;

    public static class Program
    {
        private static readonly Container _container;
        private static readonly IConfiguration _configuration;

        static Program()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: false)
                .Build();

            var serilogConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext();
            ;

            var mySettings = new MySettings();
            _configuration.GetSection("MySettings").Bind(mySettings);

            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            _container.Options.DependencyInjectionBehavior = new SerilogContextualLoggerInjectionBehavior(
                serilogConfiguration,
                _container.Options
            );

            _container.RegisterSingleton<MySettings>();

            _container.Register<IApplication, Application>(Lifestyle.Singleton);

            _container.Verify();
        }

        public static Task<int> Main(string[] args)
        {
            try
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var app = _container.GetInstance<IApplication>();
                    app.RunAsync();
                    Console.ReadLine();
                }
                return Task.FromResult(0);
            }
            catch (Exception)
            {
                return Task.FromResult(1);
            }
        }
    }
}
