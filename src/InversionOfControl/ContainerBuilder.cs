using System;
using System.Collections.Generic;

namespace InversionOfControl
{
    /// <summary>
    /// Builds a Container Runtime from registerered services and their lifespans.
    /// </summary>
    public class ContainerBuilder : IContainerBuilder
    {
        private IRegistrationSource _registrationSource;
        private IContainerBackend _backend;

        public ContainerBuilder()
        {
            _registrationSource = new DefaultRegistrationSource();
            _backend = new DefaultContainerBackend();
        }

        public IEnumerable<ServiceRegistration> GetRegistrations(Type type)
            => _registrationSource.GetRegistrations(type);

        public void RegisterService(ServiceRegistration registration)
            => _registrationSource.RegisterService(registration);

        public void RegisterServices(Type serviceType, IEnumerable<ServiceRegistration> registrations)
            => _registrationSource.RegisterServices(serviceType, registrations);

        public IContainerBuilder UseBackend(IContainerBackend backend)
        {
            _backend = backend ?? throw new ArgumentNullException(nameof(backend));

            return this;
        }

        public IContainerBuilder UseRegistrationSource(IRegistrationSource source)
        {
            _registrationSource = source ?? throw new ArgumentNullException(nameof(source));

            return this;
        }

        public IContainerRuntime BuildRuntime()
            => new ContainerRuntime(_registrationSource, _backend);
    }
}
