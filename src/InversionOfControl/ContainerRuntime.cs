using System;
using System.Collections.Generic;
using System.Linq;

namespace InversionOfControl
{
    // The internal implementation of the IContainerRuntime interface
    internal class ContainerRuntime : IContainerRuntime
    {
        private readonly IContainerBackend _backend;
        private readonly IRegistrationSource _registrationSource;
        private readonly ContainerScope _runtimeScope;

        internal ContainerRuntime(
            IRegistrationSource registrationSource,
            IContainerBackend backend)
        {
            _registrationSource = registrationSource ?? throw new ArgumentNullException(nameof(registrationSource));
            _backend = backend ?? throw
                new ArgumentNullException(nameof(backend));

            // We create a runtime scope to handle the storage and retrieval of singleton services.
            _runtimeScope = (ContainerScope) CreateScope();
        }

        public IContainerScope CreateScope()
        {
            // Create new context for each scope.
            var scopeContext = _backend.CreateScopeContext();

            // Both the runtime and the container scope use the same backend instance.
            return new ContainerScope(scopeContext, _backend, this);
        }

        public object GetService(Type type)
        {
            var services = GetServices(new DependencyChain(type), _runtimeScope);

            return _backend.CreateService(type, services);
        }

        internal IEnumerable<object> GetServices(DependencyChain chain, ContainerScope scope)
        {
            // First, read the registration from the runtime to determine the lifespan.
            var registrations = _registrationSource.GetRegistrations(chain.Type);

            return registrations.Select(registration =>
            {
                switch (registration.ServiceLifespan)
                {
                    case ServiceLifespan.Transient:

                        // For Transient, activate a new instance each time.
                        return _backend.ActivateInstance(registration, chain, new ServiceVisitor(this, scope));

                    case ServiceLifespan.Singleton:

                        // For Singleton, rely on the runtime scope to retrieve the instance.
                        return _runtimeScope.GetScopedService(registration, chain);

                    case ServiceLifespan.Scoped:

                        // For Scoped, rely on the scope to retrieve the instance.
                        return scope.GetScopedService(registration, chain);
                    default:
                        throw new NotImplementedException($"Lifespan '{registration.ServiceLifespan}' is not supported");
                }
            });
        }
    }
}
