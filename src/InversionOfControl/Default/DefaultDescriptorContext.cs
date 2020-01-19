using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    /// <summary>
    /// The default implementation of the IDescriptorContext interface
    /// </summary>
    public class DefaultDescriptorContext : IDescriptorContext
    {
        private readonly Dictionary<Type, ServiceDescriptor> _descriptors;
        private readonly Dictionary<Type, ServiceDescriptor> _definitionDescriptors;

        public DefaultDescriptorContext()
        {
            _descriptors = new Dictionary<Type, ServiceDescriptor>();
            _definitionDescriptors = new Dictionary<Type, ServiceDescriptor>();
        }

        public void AddDescriptor(ServiceDescriptor descriptor)
        {
            descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));

            // If we are adding a descriptor that is a generic type definition,
            // we need to add it to a seperate dictionary.
            if (descriptor.ServiceType.IsGenericTypeDefinition)
                _definitionDescriptors.Add(descriptor.ServiceType, descriptor);
            else 
                _descriptors.Add(descriptor.ServiceType, descriptor);
        }

        public ServiceDescriptor GetDescriptor(Type type)
        {
            // If the type is generic, see if it's generic type definition is registered first.
            if (type.IsGenericType)
            {
                if (_definitionDescriptors.TryGetValue(type.GetGenericTypeDefinition(), out var definitionDescriptor))
                    return definitionDescriptor;
            }

            // If it's not generic or doesn't have a type definition descriptor registered,
            // attempt to retrieve it from the normal dictionary.
            if (_descriptors.TryGetValue(type, out var descriptor))
                return descriptor;

            // No service descriptor could be found
            return null;
        }
    }
}
