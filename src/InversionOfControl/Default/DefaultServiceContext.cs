using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    /// <summary>
    /// The default implementation of the IServiceContext interface
    /// </summary>
    public class DefaultServiceContext : IServiceContext
    {
        private readonly IDictionary<Type, object> _serviceInstances;
        private readonly ICollection<IDisposable> _serviceHandles;

        private bool _disposed = false;

        public DefaultServiceContext()
        {
            _serviceInstances = new Dictionary<Type, object>();
            _serviceHandles = new List<IDisposable>();
        }

        public void AddService(Type serviceType, object instance)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(IServiceContext));

            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            _serviceInstances.Add(serviceType, instance);

            // If the service instance implements IDisposable,
            // add it as a handle so it can be disposed of later.
            if (instance is IDisposable handle)
                _serviceHandles.Add(handle);
        }

        public object GetService(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            if (!_serviceInstances.TryGetValue(type, out var instance))
                return null;

            return instance;
        }

        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(IServiceContext));

            // When disposing of the context, also dispose of all registered service handles.
            foreach (var handle in _serviceHandles)
                handle.Dispose();

            _disposed = true;
        }


    }
}
