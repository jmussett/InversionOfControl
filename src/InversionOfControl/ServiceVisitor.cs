using System;

namespace InversionOfControl
{
    // The internal implementation of the IServiceVisitor interface
    internal class ServiceVisitor : IServiceVisitor
    {
        private readonly ContainerRuntime _runtime;
        private readonly ContainerScope _scope;

        public ServiceVisitor(ContainerRuntime runtime, ContainerScope scope)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public object LocateService(DependencyChain chain)
        {
            chain = chain ?? throw new ArgumentNullException(nameof(chain));

            return _runtime.GetService(chain, _scope);
        }

        public object InvokeServiceFactory(Func<IContainerRuntime, object> factoryMethod)
        {
            factoryMethod = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
            
            return factoryMethod.Invoke(_runtime);
        }
    }
}
