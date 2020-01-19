using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    /// <summary>
    /// The default implementation of the IRegistrationContext interface
    /// </summary>
    public class DefaultRegistrationContext : IRegistrationContext
    {
        private readonly Dictionary<Type, ServiceRegistration> _registrations;

        public DefaultRegistrationContext()
            => _registrations = new Dictionary<Type, ServiceRegistration>();

        public void RegisterService(ServiceRegistration registration)
        {
            registration = registration ?? throw new ArgumentNullException(nameof(registration));

            _registrations.Add(registration.ServiceType, registration);
        }

        public ServiceRegistration GetRegistration(Type type)
        {
            // If the type is generic, see if it's generic type definition is registered first.
            if (type.IsGenericType)
            {
                if (_registrations.TryGetValue(type.GetGenericTypeDefinition(), out var definitionRegistration))
                    return definitionRegistration;
            }

            if (_registrations.TryGetValue(type, out var registration))
                return registration;

            // No service registration could be found
            return null;
        }
    }
}
