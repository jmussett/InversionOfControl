using System;

namespace InversionOfControl
{
    // The internal implementation of the IContainerScope interface
    internal class ContainerScope : IContainerScope
    {
        private readonly IServiceContext _services;
        private readonly IServiceActivator _activator;
        private readonly ContainerRuntime _runtime;

        private bool _disposed = false;

        internal ContainerScope(IServiceContext services, IServiceActivator activator, ContainerRuntime runtime)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _activator = activator ?? throw new ArgumentNullException(nameof(activator));
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        }

        public TService GetService<TService>()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(IContainerScope));

            var type = typeof(TService);

            // Services are always resolved at runtime,
            // so we pass the current scope to the runtime method.
            var service = _runtime.GetService(new DependencyChain(type), this);

            // If the service doesn't exist, the type was never registered.
            if (service == null)
                throw new MissingServiceException(type);

            return (TService) service;
        }

        internal object GetScopedService(ServiceDescriptor descriptor, DependencyChain chain)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(IContainerScope));

            // Attempt to retrieve the service instance from the scope.
            var instance = _services.GetService(descriptor.ServiceType);

            // If instance doesn't exist, we need to activate it.
            if (instance == null)
            {
                // Use instance on descriptor, if defined.
                // Otherwise, activate a new instance.
                instance = descriptor.ServiceInstance ??
                    _activator.ActivateInstance(descriptor, chain, new ServiceVisitor(_runtime, this));

                _services.AddService(descriptor.ServiceType, instance);
            }

            return instance;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _services.Dispose();
                _disposed = true;
            }
        }
    }
}
