using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    /// <summary>
    /// An interface used to visit and interact with services within a container.
    /// </summary>
    public interface IContainerVisitor
    {
        /// <summary>
        /// Locates the service for the current node in the dependancy chain.
        /// </summary>
        IEnumerable<object> LocateServices(DependencyChain chain);

        /// <summary>
        /// Invokes the factory method and returns the instance for the requested service.
        /// </summary>
        object InvokeServiceFactory(Func<IContainerRuntime, object> factoryMethod);
    }
}
