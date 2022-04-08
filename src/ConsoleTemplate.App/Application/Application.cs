namespace ConsoleTemplate.App
{
    using System.Threading.Tasks;
    using Serilog;

    public class Application : IApplication
    {
        private readonly ILogger _logger;

        public Application(ILogger logger)
        {
            _logger = logger;
        }

        public Task<string> RunAsync()
        {
            _logger.Information("Starting up main task...");
            return Task.FromResult("ConsoleTemplate.App");
        }
    }
}
