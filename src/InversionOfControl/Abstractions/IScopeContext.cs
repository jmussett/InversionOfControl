using System;

namespace InversionOfControl
{
    /// <summary>
    /// A context used for the storage and retrieval of service instances.
    /// </summary>
    public interface IScopeContext : IDisposable
    {
        /// <summary>
        /// Adds a service with a given type and instance.
        /// </summary>
        void AddService(Type serviceType, object instance);

        /// <summary>
        /// Retrieves a service instance for a given type.
        /// </summary>
        object GetService(Type type);
    }
}