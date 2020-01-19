using System;

namespace InversionOfControl
{
    // The internal implementation of the IContainerRuntime interface
    internal class ContainerRuntime : IContainerRuntime
    {
        private readonly IServiceActivator _activator;
        private readonly IDescriptorContext _descriptorContext;
        private readonly IServiceContextFactory _serviceContextFactory;
        private readonly ContainerScope _runtimeScope;

        internal ContainerRuntime(
            IDescriptorContext descriptors,
            IServiceActivator activator,
            IServiceContextFactory contextFactory)
        {
            _descriptorContext = descriptors ?? throw new ArgumentNullException(nameof(descriptors));
            _activator = activator ?? throw new ArgumentNullException(nameof(activator));
            _serviceContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(activator));

            // We create a runtime scope to handle the storage and retrieval of singleton services.
            _runtimeScope = (ContainerScope) CreateScope();
        }

        public IContainerScope CreateScope()
        {
            // Create new service context for each scope.
            var serviceContext = _serviceContextFactory.CreateContext();

            // Both the runtime and the container scope use the same activator instance.
            return new ContainerScope(serviceContext, _activator, this);
        }

        public TService GetService<TService>()
        {
            var type = typeof(TService);

            var service = GetService(new DependencyChain(type), _runtimeScope);

            // If the service doesn't exist, the type was never registered.
            if (service == null)
                throw new MissingServiceException(type);

            return (TService) service;
        }

        internal object GetService(DependencyChain chain, ContainerScope scope)
        {
            // First, read the descriptor from the runtime to determine the lifespan.
            var descriptor = _descriptorContext.GetDescriptor(chain.Type);

            // We return null here instead of throwing an exception.
            // Different exceptions might want to be thrown depending on the context.
            if (descriptor == null)
                return null;

            switch (descriptor.ServiceLifespan)
            {
                case ServiceLifespan.Transient:

                    // For Transient, activate a new instance each time.
                    return _activator.ActivateInstance(descriptor, chain, new ServiceVisitor(this, scope));

                case ServiceLifespan.Singleton:

                    // For Singleton, rely on the runtime scope to retrieve the instance.
                    return _runtimeScope.GetScopedService(descriptor, chain);

                case ServiceLifespan.Scoped:

                    // For Scoped, rely on the scope to retrieve the instance.
                    return scope.GetScopedService(descriptor, chain);
                default:
                    throw new NotImplementedException($"Lifespan '{descriptor.ServiceLifespan}' is not supported");
            }
        }
    }
}
