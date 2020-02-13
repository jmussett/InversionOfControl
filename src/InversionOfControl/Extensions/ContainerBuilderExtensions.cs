using System;

namespace InversionOfControl
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddScoped<TService>(this ContainerBuilder builder)
            => AddService<TService, TService>(builder, ServiceLifespan.Scoped);

        public static ContainerBuilder AddScoped<TService>(this ContainerBuilder builder,
            Func<IContainerRuntime, TService> factoryMethod)
            => AddService<TService, TService>(builder, ServiceLifespan.Scoped, factoryMethod);

        public static ContainerBuilder AddScoped<TService, TConcrete>(this ContainerBuilder builder)
            where TConcrete : TService
            => AddService<TService, TConcrete>(builder, ServiceLifespan.Scoped);

        public static ContainerBuilder AddScoped<TService, TConcrete>(this ContainerBuilder builder,
            Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService
            => AddService<TService, TConcrete>(builder, ServiceLifespan.Scoped, factoryMethod);

        public static ContainerBuilder AddSingleton<TService>(this ContainerBuilder builder)
            => AddService<TService, TService>(builder, ServiceLifespan.Singleton);

        public static ContainerBuilder AddSingleton<TService>(this ContainerBuilder builder,
            TService instance)
            => AddService<TService, TService>(builder, ServiceLifespan.Singleton, instance);

        public static ContainerBuilder AddSingleton<TService>(this ContainerBuilder builder,
            Func<IContainerRuntime, TService> factoryMethod)
            => AddService<TService, TService>(builder, ServiceLifespan.Singleton, factoryMethod);

        public static ContainerBuilder AddSingleton<TService, TConcrete>(this ContainerBuilder builder)
            where TConcrete : TService
            => AddService<TService, TConcrete>(builder, ServiceLifespan.Singleton);

        public static ContainerBuilder AddSingleton<TService, TConcrete>(this ContainerBuilder builder,
            TConcrete instance) where TConcrete : TService
            => AddService<TService, TConcrete>(builder, ServiceLifespan.Singleton, instance);

        public static ContainerBuilder AddSingleton<TService, TConcrete>(this ContainerBuilder builder,
            Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService
            => AddService<TService, TConcrete>(builder, ServiceLifespan.Singleton, factoryMethod);

        public static ContainerBuilder AddTransient<TService>(this ContainerBuilder builder)
            => AddService<TService, TService>(builder, ServiceLifespan.Transient);

        public static ContainerBuilder AddTransient<TService>(this ContainerBuilder builder,
            Func<IContainerRuntime, TService> factoryMethod)
            => AddService<TService, TService>(builder, ServiceLifespan.Transient, factoryMethod);

        public static ContainerBuilder AddTransient<TService, TConcrete>(this ContainerBuilder builder)
            where TConcrete : TService
            => AddService<TService, TConcrete>(builder, ServiceLifespan.Transient);

        public static ContainerBuilder AddTransient<TService, TConcrete>(this ContainerBuilder builder,
            Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService
            => AddService<TService, TConcrete>(builder, ServiceLifespan.Transient, factoryMethod);

        public static ContainerBuilder AddTransient(this ContainerBuilder builder, 
            Type serviceType, Type concreteType)
            => AddService(builder, ServiceLifespan.Transient, serviceType, concreteType, null, null);

        public static ContainerBuilder AddService<TService, TConcrete>(this ContainerBuilder builder,
            ServiceLifespan lifespan)
            => AddService(builder, lifespan, typeof(TService), typeof(TConcrete), null, null);

        public static ContainerBuilder AddService<TService, TConcrete>(this ContainerBuilder builder,
            ServiceLifespan lifespan, object instance)
        {
            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            return AddService(builder, lifespan, typeof(TService), typeof(TConcrete), null, instance);
        }

        private static ContainerBuilder AddService<TService, TConcrete>(this ContainerBuilder builder,
            ServiceLifespan lifespan, Func<IContainerRuntime, TConcrete> factoryMethod)
        {
            factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));

            return AddService(builder, lifespan, typeof(TService), typeof(TConcrete), r => factoryMethod(r), null);
        }

        public static ContainerBuilder AddService(this ContainerBuilder builder,
            ServiceLifespan lifespan, Type serviceType, Type concreteType)
            => AddService(builder, lifespan, serviceType, concreteType, null, null);

        public static ContainerBuilder AddService(this ContainerBuilder builder,
            ServiceLifespan lifespan, Type serviceType, Type concreteType, Func<IContainerRuntime, object> factoryMethod, object instance)
        {
            builder.RegisterService(new ServiceRegistration
            {
                ServiceType = serviceType,
                ConcreteType = concreteType,
                ServiceLifespan = lifespan,
                FactoryMethod = factoryMethod,
                ServiceInstance = instance
            });

            return builder;
        }
    }
}
