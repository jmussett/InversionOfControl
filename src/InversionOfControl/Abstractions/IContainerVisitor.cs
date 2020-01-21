using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    /// <summary>
    /// An interface used by the runtime to visit and interact with services.
    /// </summary>
    public interface IContainerVisitor
    {
        /// <summary>
        /// Locates the service registered to the requested type.
        /// </summary>
        IEnumerable<object> LocateServices(DependencyChain chain);

        /// <summary>
        /// Invokes the factory method and returns the instance for the requested service.
        /// </summary>
        object InvokeServiceFactory(Func<IContainerRuntime, object> factoryMethod);
    }
}
