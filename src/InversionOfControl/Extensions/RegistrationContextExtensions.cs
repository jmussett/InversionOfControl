using System;

namespace InversionOfControl
{
    public static class RegistrationContextExtensions
    {
        public static IRegistrationSource AddScoped<TService>(this IRegistrationSource source)
            => AddService<TService, TService>(source, ServiceLifespan.Scoped);

        public static IRegistrationSource AddScoped<TService>(this IRegistrationSource source,
            Func<IContainerRuntime, TService> factoryMethod)
            => AddService<TService, TService>(source, ServiceLifespan.Scoped, factoryMethod);

        public static IRegistrationSource AddScoped<TService, TConcrete>(this IRegistrationSource source)
            where TConcrete : TService
            => AddService<TService, TConcrete>(source, ServiceLifespan.Scoped);

        public static IRegistrationSource AddScoped<TService, TConcrete>(this IRegistrationSource source,
            Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService
            => AddService<TService, TConcrete>(source, ServiceLifespan.Scoped, factoryMethod);

        public static IRegistrationSource AddSingleton<TService>(this IRegistrationSource source)
            => AddService<TService, TService>(source, ServiceLifespan.Singleton);

        public static IRegistrationSource AddSingleton<TService>(this IRegistrationSource source,
            TService instance)
            => AddService<TService, TService>(source, ServiceLifespan.Singleton, instance);

        public static IRegistrationSource AddSingleton<TService>(this IRegistrationSource source,
            Func<IContainerRuntime, TService> factoryMethod)
            => AddService<TService, TService>(source, ServiceLifespan.Singleton, factoryMethod);

        public static IRegistrationSource AddSingleton<TService, TConcrete>(this IRegistrationSource source)
            where TConcrete : TService
            => AddService<TService, TConcrete>(source, ServiceLifespan.Singleton);

        public static IRegistrationSource AddSingleton<TService, TConcrete>(this IRegistrationSource source,
            TConcrete instance) where TConcrete : TService
            => AddService<TService, TConcrete>(source, ServiceLifespan.Singleton, instance);

        public static IRegistrationSource AddSingleton<TService, TConcrete>(this IRegistrationSource source,
            Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService
            => AddService<TService, TConcrete>(source, ServiceLifespan.Singleton, factoryMethod);

        public static IRegistrationSource AddTransient<TService>(this IRegistrationSource source)
            => AddService<TService, TService>(source, ServiceLifespan.Transient);

        public static IRegistrationSource AddTransient<TService>(this IRegistrationSource source,
            Func<IContainerRuntime, TService> factoryMethod)
            => AddService<TService, TService>(source, ServiceLifespan.Transient, factoryMethod);

        public static IRegistrationSource AddTransient<TService, TConcrete>(this IRegistrationSource source)
            where TConcrete : TService
            => AddService<TService, TConcrete>(source, ServiceLifespan.Transient);

        public static IRegistrationSource AddTransient<TService, TConcrete>(this IRegistrationSource source,
            Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService
            => AddService<TService, TConcrete>(source, ServiceLifespan.Transient, factoryMethod);

        public static IRegistrationSource AddTransient(this IRegistrationSource source, 
            Type serviceType, Type concreteType)
            => AddService(source, ServiceLifespan.Transient, serviceType, concreteType, null, null);

        public static IRegistrationSource AddService<TService, TConcrete>(this IRegistrationSource source,
            ServiceLifespan lifespan)
            => AddService(source, lifespan, typeof(TService), typeof(TConcrete), null, null);

        public static IRegistrationSource AddService<TService, TConcrete>(this IRegistrationSource source,
            ServiceLifespan lifespan, object instance)
        {
            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            return AddService(source, lifespan, typeof(TService), typeof(TConcrete), null, instance);
        }

        private static IRegistrationSource AddService<TService, TConcrete>(this IRegistrationSource source,
            ServiceLifespan lifespan, Func<IContainerRuntime, TConcrete> factoryMethod)
        {
            factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

            return AddService(source, lifespan, typeof(TService), typeof(TConcrete), r => factoryMethod(r), null);
        }

        public static IRegistrationSource AddService(this IRegistrationSource source,
            ServiceLifespan lifespan, Type serviceType, Type concreteType)
            => AddService(source, lifespan, serviceType, concreteType, null, null);

        public static IRegistrationSource AddService(this IRegistrationSource source,
            ServiceLifespan lifespan, Type serviceType, Type concreteType, Func<IContainerRuntime, object> factoryMethod, object instance)
        {
            source.RegisterService(new ServiceRegistration
            {
                ServiceType = serviceType,
                ConcreteType = concreteType,
                ServiceLifespan = lifespan,
                FactoryMethod = factoryMethod,
                ServiceInstance = instance
            });

            return source;
        }

        public static IContainerRuntime BuildRuntime(this IRegistrationSource registration)
        {
            var builder = registration as ContainerBuilder;
            if (builder == null)
                throw new InvalidOperationException("This registration source is not a valid container builder.");

            return builder.BuildRuntime();
        }
    }
}
