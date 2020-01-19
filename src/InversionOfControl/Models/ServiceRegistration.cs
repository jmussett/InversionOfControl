using System;

namespace InversionOfControl
{
    /// <summary>
    /// Class with registration information for a service.
    /// </summary>
    public partial class ServiceRegistration
    {
        /// <summary>
        /// The type the service is registered for.
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// The concrete implementation type of the service.
        /// </summary>
        public Type ConcreteType { get; set; }

        /// <summary>
        /// The lifespan of the service.
        /// </summary>
        public ServiceLifespan ServiceLifespan { get; set; }

        /// <summary>
        /// The instance of a singleton or scoped service.
        /// </summary>
        public object ServiceInstance { get; set; }

        /// <summary>
        /// The callback method for a factory-created service.
        /// </summary>
        public Func<IContainerRuntime, object> FactoryMethod { get; set; }
    }
}
