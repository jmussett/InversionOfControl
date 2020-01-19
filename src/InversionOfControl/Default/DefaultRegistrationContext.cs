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
        private readonly Dictionary<Type, ServiceRegistration> _definitionRegistrations;

        public DefaultRegistrationContext()
        {
            _registrations = new Dictionary<Type, ServiceRegistration>();
            _definitionRegistrations = new Dictionary<Type, ServiceRegistration>();
        }

        public void RegisterService(ServiceRegistration registration)
        {
            registration = registration ?? throw new ArgumentNullException(nameof(registration));

            // If we are registering a service that is a generic type definition,
            // we need to add it to a seperate dictionary.
            if (registration.ServiceType.IsGenericTypeDefinition)
                _definitionRegistrations.Add(registration.ServiceType, registration);
            else 
                _registrations.Add(registration.ServiceType, registration);
        }

        public ServiceRegistration GetRegistration(Type type)
        {
            // If the type is generic, see if it's generic type definition is registered first.
            if (type.IsGenericType)
            {
                if (_definitionRegistrations.TryGetValue(type.GetGenericTypeDefinition(), out var definitionRegistration))
                    return definitionRegistration;
            }

            // If it's not generic or doesn't have a type definition registered,
            // attempt to retrieve it from the normal dictionary.
            if (_registrations.TryGetValue(type, out var registration))
                return registration;

            // No service registration could be found
            return null;
        }
    }
}
