using System;

namespace InversionOfControl
{
    /// <summary>
    /// An interface used by the runtime to visit and interact with services.
    /// </summary>
    public interface IServiceVisitor
    {
        /// <summary>
        /// Locates the service registered to the requested type.
        /// </summary>
        object LocateService(DependencyChain chain);

        /// <summary>
        /// Invokes the factory method and returns the service instance for the requested service descriptor.
        /// </summary>
        object InvokeServiceFactory(Func<IContainerRuntime, object> factoryMethod);
    }
}
