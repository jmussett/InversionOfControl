using System;

namespace InversionOfControl
{
    // The internal implementation of the IContainerScope interface
    internal class ContainerScope : IContainerScope
    {
        private readonly IScopeContext _scopeContext;
        private readonly IContainerBackend _backend;
        private readonly ContainerRuntime _runtime;

        private bool _disposed = false;

        internal ContainerScope(IScopeContext scopeContext, IContainerBackend backend, ContainerRuntime runtime)
        {
            _scopeContext = scopeContext ?? throw new ArgumentNullException(nameof(scopeContext));
            _backend = backend ?? throw new ArgumentNullException(nameof(backend));
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        }

        public TService GetService<TService>()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(IContainerScope));

            var type = typeof(TService);

            // Services are always resolved at runtime,
            // so we pass the current scope to the runtime method.
            var services = _runtime.GetServices(new DependencyChain(type), this);

            return _backend.CreateService<TService>(services);
        }

        internal object GetScopedService(ServiceRegistration registration, DependencyChain chain)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(IContainerScope));

            // Attempt to retrieve the instance from the service context.
            var instance = _scopeContext.GetService(registration.ConcreteType);

            // If instance doesn't exist, we need to activate it.
            if (instance == null)
            {
                // Use instance on registration, if defined.
                // Otherwise, activate a new instance.
                instance = registration.ServiceInstance ??
                    _backend.ActivateInstance(registration, chain, new ServiceVisitor(_runtime, this));

                _scopeContext.AddService(registration.ConcreteType, instance);
            }

            return instance;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _scopeContext.Dispose();
                _disposed = true;
            }
        }
    }
}
