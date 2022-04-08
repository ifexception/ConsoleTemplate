namespace ConsoleTemplate.App.Logger
{
    using System;
    using Serilog;
    using SimpleInjector;
    using SimpleInjector.Advanced;

    // https://github.com/simpleinjector/SimpleInjector/issues/295
    public class SerilogContextualLoggerInjectionBehavior : IDependencyInjectionBehavior
    {
        private readonly LoggerConfiguration _loggerConfiguration;
        private readonly IDependencyInjectionBehavior _original;
        private readonly Container _container;

        public SerilogContextualLoggerInjectionBehavior(
            LoggerConfiguration loggerConfiguration,
            ContainerOptions containerOptions
        )
        {
            _loggerConfiguration = loggerConfiguration;
            _original = containerOptions.DependencyInjectionBehavior;
            _container = containerOptions.Container;
        }

        public void Verify(InjectionConsumerInfo consumer)
        {
            _original.Verify(consumer);
        }

        public InstanceProducer? GetInstanceProducer(InjectionConsumerInfo dependency, bool throwOnFailure)
        {
            return dependency.Target.TargetType == typeof(ILogger)
                ? GetLoggerInstanceProducer(dependency.ImplementationType)
                : _original.GetInstanceProducer(dependency, throwOnFailure);
        }

        public bool VerifyDependency(InjectionConsumerInfo dependency, out string? errorMessage)
        {
            return _original.VerifyDependency(dependency, out errorMessage);
        }

        private InstanceProducer<ILogger> GetLoggerInstanceProducer(Type type)
        {
            return Lifestyle.Singleton
                .CreateProducer(() =>
                    _loggerConfiguration.CreateLogger().ForContext(type),
                    _container
                );
            // Lifestyle.Singleton.CreateProducer(() => container.GetInstance<ILogger>()
            // .ForContext(type), container);
        }
    }
}
