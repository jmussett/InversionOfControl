namespace InversionOfControl
{
    /// <summary>
    /// An interface used by the runtime for activating new services. 
    /// </summary>
    public interface IServiceActivator
    {
        /// <summary>
        /// Activates a new instance of a registered instance in a given location in a dependency chain.
        /// </summary>
        object ActivateInstance(ServiceRegistration registration, DependencyChain chain, IServiceVisitor visitor);
    }
}
