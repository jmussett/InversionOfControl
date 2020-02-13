using System;

namespace InversionOfControl
{
    /// <summary>
    /// Builds a container runtime with registered services and their lifespans.
    /// </summary>
    public interface IContainerBuilder
    {
        /// <summary>
        /// Allows the resulting container to use a custom backend implementation.
        /// </summary>
        IContainerBuilder UseBackend(IContainerBackend backend);

        /// <summary>
        /// Allows the resulting container to use a custom registration source implementation.
        /// </summary>
        IContainerBuilder UseRegistrationSource(IRegistrationSource source);

        /// <summary>
        /// Builds a container runtime from the registered services.
        /// </summary>
        IContainerRuntime BuildRuntime();
    }
}
