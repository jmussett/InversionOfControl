using System;

namespace InversionOfControl
{
    /// <summary>
    /// Builds a container runtime with registered services and their lifespans.
    /// </summary>
    public interface IContainerBuilder
    {
        /// <summary>
        /// Registers a transient service to the container.
        /// </summary>
        IContainerBuilder AddTransient<TService>();

        /// <summary>
        /// Registers a transient service with a factory method to the container.
        /// </summary>
        IContainerBuilder AddTransient<TService>(Func<IContainerRuntime, TService> factoryMethod);

        /// <summary>
        /// Registers a transient service with a concrete implementation to the container.
        /// </summary>
        IContainerBuilder AddTransient<TService, TConcrete>() where TConcrete : TService;

        /// <summary>
        /// Registers a transient service with a concrete implementation and a factory method to the container.
        /// </summary>
        IContainerBuilder AddTransient<TService, TConcrete>(Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService;

        /// <summary>
        /// Registers a transient unbound service type with a unbound concrete implementation type. To be used when registering generic type definitions.
        /// </summary>
        IContainerBuilder AddTransient(Type serviceType, Type concreteType);



        /// <summary>
        /// Registers a singleton service to the container.
        /// </summary>
        IContainerBuilder AddSingleton<TService>();

        /// <summary>
        /// Registers a singleton service with an instance object to the container.
        /// </summary>
        IContainerBuilder AddSingleton<TService>(TService instance);

        /// <summary>
        /// Registers a singleton service with a factory method to the container.
        /// </summary>
        IContainerBuilder AddSingleton<TService>(Func<IContainerRuntime, TService> factoryMethod);

        /// <summary>
        /// Registers a singleton service with a concrete implementation to the container.
        /// </summary>
        IContainerBuilder AddSingleton<TService, TConcrete>() where TConcrete : TService;

        /// <summary>
        /// Registers a singleton service with a concrete implementation and an instance object to the container.
        /// </summary>
        IContainerBuilder AddSingleton<TService, TConcrete>(TConcrete instance) where TConcrete : TService;

        /// <summary>
        /// Registers a singleton service with a concrete implementation and a factory method to the container.
        /// </summary>
        IContainerBuilder AddSingleton<TService, TConcrete>(Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService;



        /// <summary>
        /// Registers a scoped service to the container.
        /// </summary>
        IContainerBuilder AddScoped<TService>();

        /// <summary>
        /// Registers a scoped service with a factory method to the container.
        /// </summary>
        IContainerBuilder AddScoped<TService>(Func<IContainerRuntime, TService> factoryMethod);

        /// <summary>
        /// Registers a scoped service with a concrete implementation to the container.
        /// </summary>
        IContainerBuilder AddScoped<TService, TConcrete>() where TConcrete : TService;

        /// <summary>
        /// Registers a scoped service with a concrete implementation and a factory method to the container.
        /// </summary>
        IContainerBuilder AddScoped<TService, TConcrete>(Func<IContainerRuntime, TConcrete> factoryMethod) where TConcrete : TService;



        /// <summary>
        /// Builds a container runtime from the registered services.
        /// </summary>
        IContainerRuntime BuildRuntime();
    }
}
