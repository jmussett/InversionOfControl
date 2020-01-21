using System;

namespace InversionOfControl
{
    /// <summary>
    /// Builds a Container Runtime from registerered services and their lifespans.
    /// </summary>
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IRegistrationSource _registrationSource;
        private readonly IContainerBackend _backend;

        public ContainerBuilder()
        {
            _registrationSource = new DefaultRegistrationSource();
            _backend = new DefaultContainerBackend();
        }

        public IContainerBuilder AddScoped<TService>()
            => AddService<TService, TService>(ServiceLifespan.Scoped);

        public IContainerBuilder AddScoped<TService>(Func<IContainerRuntime, TService> factoryMethod)
            => AddService<TService, TService>(ServiceLifespan.Scoped, factoryMethod);

        public IContainerBuilder AddScoped<TService, TConcrete>() where TConcrete : TService
            => AddService<TService, TConcrete>(ServiceLifespan.Scoped);

        public IContainerBuilder AddScoped<TService, TConcrete>(Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService
            => AddService<TService, TConcrete>(ServiceLifespan.Scoped, factoryMethod);

        public IContainerBuilder AddSingleton<TService>()
            => AddService<TService, TService>(ServiceLifespan.Singleton);

        public IContainerBuilder AddSingleton<TService>(TService instance)
            => AddService<TService, TService>(ServiceLifespan.Singleton, instance);

        public IContainerBuilder AddSingleton<TService>(Func<IContainerRuntime, TService> factoryMethod)
            => AddService<TService, TService>(ServiceLifespan.Singleton, factoryMethod);

        public IContainerBuilder AddSingleton<TService, TConcrete>() where TConcrete : TService 
            => AddService<TService, TConcrete>(ServiceLifespan.Singleton);

        public IContainerBuilder AddSingleton<TService, TConcrete>(TConcrete instance) where TConcrete : TService
            => AddService<TService, TConcrete>(ServiceLifespan.Singleton, instance);

        public IContainerBuilder AddSingleton<TService, TConcrete>(Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService
            => AddService<TService, TConcrete>(ServiceLifespan.Singleton, factoryMethod);

        public IContainerBuilder AddTransient<TService>()
            => AddService<TService, TService>(ServiceLifespan.Transient);

        public IContainerBuilder AddTransient<TService>(Func<IContainerRuntime, TService> factoryMethod)
            => AddService<TService, TService>(ServiceLifespan.Transient, factoryMethod);

        public IContainerBuilder AddTransient<TService, TConcrete>() where TConcrete : TService
            => AddService<TService, TConcrete>(ServiceLifespan.Transient);

        public IContainerBuilder AddTransient<TService, TConcrete>(Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService
            => AddService<TService, TConcrete>(ServiceLifespan.Transient, factoryMethod);

        public IContainerBuilder AddTransient(Type serviceType, Type concreteType)
            => AddService(ServiceLifespan.Transient, serviceType, concreteType, null, null);

        private IContainerBuilder AddService<TService, TConcrete>(ServiceLifespan lifespan)
            => AddService(lifespan, typeof(TService), typeof(TConcrete), null, null);

        private IContainerBuilder AddService<TService, TConcrete>(ServiceLifespan lifespan, object instance)
        {
            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            return AddService(lifespan, typeof(TService), typeof(TConcrete), null, instance);
        }

        private IContainerBuilder AddService<TService, TConcrete>(ServiceLifespan lifespan, Func<IContainerRuntime, TConcrete> factoryMethod)
        {
            factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

            return AddService(lifespan, typeof(TService), typeof(TConcrete), r => factoryMethod(r), null);
        }

        private IContainerBuilder AddService(ServiceLifespan lifespan, Type serviceType, Type concreteType, Func<IContainerRuntime, object> factoryMethod, object instance)
        {
            _registrationSource.RegisterService(new ServiceRegistration
            {
                ServiceType = serviceType,
                ConcreteType = concreteType,
                ServiceLifespan = lifespan,
                FactoryMethod = factoryMethod,
                ServiceInstance = instance
            });

            return this;
        }

        public IContainerRuntime BuildRuntime() 
            => new ContainerRuntime(_registrationSource, _backend);
    }
}
