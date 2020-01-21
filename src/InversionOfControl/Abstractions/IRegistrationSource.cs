using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    /// <summary>
    /// A context used for the registration and retrieval of service registrations.
    /// </summary>
    public interface IRegistrationSource
    {
        /// <summary>
        /// Registers a registration to the context for a given service.
        /// </summary>
        void RegisterService(ServiceRegistration registration);

        void RegisterServices(Type serviceType, IEnumerable<ServiceRegistration> registrations);

        /// <summary>
        /// Retrieves the registrations for a service with a given type.
        /// </summary>
        IEnumerable<ServiceRegistration> GetRegistrations(Type type);
    }
}
