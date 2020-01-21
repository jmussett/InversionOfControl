using System;
using System.Collections.Generic;
using System.Linq;

namespace InversionOfControl
{
    /// <summary>
    /// The default implementation of the IRegistrationContext interface
    /// </summary>
    public class DefaultRegistrationSource : IRegistrationSource
    {
        private readonly Dictionary<Type, List<ServiceRegistration>> _registrations;

        public DefaultRegistrationSource()
            => _registrations = new Dictionary<Type, List<ServiceRegistration>>();

        public void RegisterService(ServiceRegistration registration)
            => RegisterServices(registration.ServiceType, new List<ServiceRegistration> { registration });

        public void RegisterServices(Type serviceType, IEnumerable<ServiceRegistration> registrations)
        {
            serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            registrations = registrations ?? throw new ArgumentNullException(nameof(registrations));

            if (_registrations.ContainsKey(serviceType))
                _registrations[serviceType].AddRange(registrations);
            else
                _registrations.Add(serviceType, registrations.ToList());
        }

        public IEnumerable<ServiceRegistration> GetRegistrations(Type type)
        {            
            if (type.IsGenericType)
            {
                var typeDefinition = type.GetGenericTypeDefinition();

                // If the type is generic, see if it's generic type definition is registered first.
                if (_registrations.TryGetValue(typeDefinition, out var definitionRegistrations))
                    return definitionRegistrations;

                var genericArgument = type.GetGenericArguments().First();

                // If the type is generic, and a list can be assigned to it, attempt to retrieve the registrations for the generic argument type.
                if (type.IsAssignableFrom(typeof(List<>).MakeGenericType(genericArgument)))
                {
                    if (_registrations.TryGetValue(genericArgument, out var enumerableRegistrations))
                        return enumerableRegistrations;
                }
            }

            if (_registrations.TryGetValue(type, out var registrations))
                return registrations;

            // No service registration could be found
            return new List<ServiceRegistration>();
        }
    }
}
