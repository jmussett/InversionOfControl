namespace InversionOfControl
{
    /// <summary>
    /// The lifespan of a service.
    /// </summary>
    public enum ServiceLifespan
    {
        /// <summary>
        /// A new instance is created every time a service is requested
        /// </summary>
        Transient,

        /// <summary>
        /// Only a single instance of the type is created.
        /// This instance is returned for all requests
        /// </summary>
        Singleton,

        /// <summary>
        /// Only a single instance of the type is created per scope.
        /// When the scope is disposed then all instances of created scoped services are also disposed (if they implement IDisposable)
        /// </summary>
        Scoped
    }
}
