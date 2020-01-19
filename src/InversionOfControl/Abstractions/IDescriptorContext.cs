using System;

namespace InversionOfControl
{
    /// <summary>
    /// A context used for the storage and retrieval of service descriptors.
    /// </summary>
    public interface IDescriptorContext
    {
        /// <summary>
        /// Adds a descriptor to the context for a given service.
        /// </summary>
        void AddDescriptor(ServiceDescriptor descriptor);

        /// <summary>
        /// Retrieves the descriptor for a service with a given type.
        /// </summary>
        ServiceDescriptor GetDescriptor(Type type);
    }
}
