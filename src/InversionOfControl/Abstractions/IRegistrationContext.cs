using System;

namespace InversionOfControl
{
    /// <summary>
    /// A context used for the registration and retrieval of service registrations.
    /// </summary>
    public interface IRegistrationContext
    {
        /// <summary>
        /// Registers a registration to the context for a given service.
        /// </summary>
        void RegisterService(ServiceRegistration registration);

        /// <summary>
        /// Retrieves the registration for a service with a given type.
        /// </summary>
        ServiceRegistration GetRegistration(Type type);
    }
}
